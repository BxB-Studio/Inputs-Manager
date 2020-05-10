#region Namespaces

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.InputSystem;
#else
using UnityEngine.Experimental.Input;
#endif

#endregion

namespace Utilities.Inputs
{
	public class InputsManagerEditor : EditorWindow
	{
		#region Enumerators

		private enum BindTarget { None, Positive, Negative }

		#endregion

		#region  Variables

		private static InputsManagerEditor editorInstance;
		private static InputsManager.InputAxis bindingAxis;
		private static InputsManager.Input input;
		private static BindTarget bindingTarget;
		private static Event bindingEvent;
		private static string inittializerKey = "InputsManager_Init";
		private static string inputName;
		private static bool addingInput;
		private static bool sortingInputs;
		private static bool hasBind;
		private static bool hasShiftBind;
		private static bool settings;
		private static bool export;
		private static bool exportAll = true;
		private static bool[] exportInputs;
		private static int bindingKey;
		private static bool Import
		{
			get
			{
				return !string.IsNullOrEmpty(importJson) && importInputs != null;
			}
		}
		private static bool importAll = true;
		private static bool[] importInputsSelection;
		private static string importJson;
		private static InputsManager.Input[] importInputs;
		private static bool importAdditive;
		private static bool importOverride;

		private Vector2 scroll;

		#endregion

		#region  Methods

		#region  Utilities

		[MenuItem("Tools/Utilities/Inputs Manager/Debug/Available Joysticks", false, 0)]
		public static void DebugJoysticks()
		{
			string[] availableJoysticks = Gamepad.all.Select(gamepad => $"{gamepad.name} ({gamepad.displayName}: {gamepad.shortDisplayName}): {gamepad.description}").ToArray();
			string message = $"Available Joysticks ({availableJoysticks.Length})\r\n";

			for (int i = 0; i < availableJoysticks.Length; i++)
				message += $"{i}. {availableJoysticks[i]}\r\n";

			Debug.Log(message);
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Edit Settings...", false, 1)]
		public static void OpenInputsManager()
		{
			if (!EditorPrefs.HasKey(inittializerKey))
			{
				EditorUtility.DisplayDialog("Inputs Manager: Welcome!", "Hey! Thank you for using the Beta version of the Inputs Manager, we're looking forward to improve it based on your honorable reviews and reports in case of any problems!", "Okay!");
				CreateInputsManager();

				return;
			}

#if UNITY_2019_3_OR_NEWER
			float minWindowWidth = 350f;
#else
			float minWindowWidth = 360f;
#endif

			editorInstance = GetWindow<InputsManagerEditor>();
			editorInstance.titleContent = new GUIContent("InputsManager");
			editorInstance.minSize = new Vector2(minWindowWidth, 512f);

			editorInstance.Show();
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Reset Settings", true)]
		protected static bool CheckResetInputsManager()
		{
			return InputsManager.DataAssetExists;
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Reset Settings", false, 2)]
		public static void ResetInputsManager()
		{
			if (!EditorUtility.DisplayDialog("Inputs Manager: Warning", "Are you sure of reseting the Inputs Manager to it's original state?", "Yes I'm sure", "No"))
				return;

			if (!AssetDatabase.DeleteAsset($"Assets/{InputsManager.DataAssetPath}"))
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Internal Error", "Unable to delete the current `InputsManager` asset in order to create a new one!", "Report error...", "Cancel"))
					ReportError();

				return;
			}

			InputsManager.RemoveAll();
			CreateInputsManager();
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Create Data Asset", true)]
		protected static bool CheckCreateInputsManager()
		{
			return !CheckResetInputsManager();
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Create Data Asset", false, 3)]
		public static void CreateInputsManager()
		{
			EditorPrefs.SetInt(inittializerKey, 0);

			if (!RecreateDataFile())
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Internal Error", "We were unable to create a new Inputs Manager asset!", "Report Error...", "Cancel"))
					ReportError();
			}
			else if (EditorUtility.DisplayDialog("Inputs Manager: Info", "A new Inputs Manager asset has been created! Do you want to load the a preset from a Json file?", "Yes", "No"))
			{
				LoadPreset();
				OpenInputsManager();
			}
			else
			{
				OpenInputsManager();
				settings = false;
			}
		}
		[MenuItem("Tools/Utilities/Inputs Manager/Report Error...", false, 4)]
		public static void ReportError()
		{
			Application.OpenURL("https://github.com/mediamax07/Inputs-Manager/issues/new");
		}
		[MenuItem("Tools/Utilities/Inputs Manager/About...", false, 4)]
		public static void About()
		{
			Application.OpenURL("https://github.com/mediamax07/Inputs-Manager");
		}

		#endregion

		#region  Editor

		private static bool RecreateDataFile()
		{
			bool process = InputsManager.SaveData();

			AssetDatabase.Refresh();

			return process;
		}
		private static void LoadPreset()
		{
			if (!ImportJsonFromPath())
			{
				EditorUtility.DisplayDialog("Inputs Manager: Error", "Sorry! But we failed to load the selected file!", "Okay");
					
				return;
			}

			if (!InputsFromJson())
				EditorUtility.DisplayDialog("Inputs Manager: Error", "The loaded file has to be a valid Json Preset file!", "Okay");
			else
				settings = true;
		}
		private static bool ImportJsonFromPath()
		{
			string path = EditorUtility.OpenFilePanel("Choose a preset file", "", "json");

			importJson = "";

			if (string.IsNullOrEmpty(path) || !File.Exists(path))
				return false;
			
			StreamReader stream = File.OpenText(path);
			string line;
			float progress;

			while(!string.IsNullOrEmpty(line = stream.ReadLine()))
			{
				progress = stream.BaseStream.Position / stream.BaseStream.Length;

				EditorUtility.DisplayProgressBar("Inputs Manager", $"Reading file...", progress);

				importJson += $"{line}\r\n";
			}
					
			EditorUtility.DisplayProgressBar("Inputs Manager", $"Finishing...", 1f);
			stream.Close();
			EditorUtility.ClearProgressBar();

			return true;
		}
		private static bool InputsFromJson()
		{
			importInputs = JsonUtility.FromJson<Utility.JsonArray<InputsManager.Input>>(importJson).ToArray();

			if (importInputs == null)
				return false;

			importInputsSelection = new bool[importInputs.Length];
			
			for (int i = 0; i < importInputsSelection.Length; i++)
				importInputsSelection[i] = true;

			return true;
		}
		private static void MoveInput(int oldIndex, int newIndex)
		{
			InputsManager.Input input = InputsManager.GetInput(oldIndex);

			InputsManager.RemoveInput(oldIndex);
			InputsManager.InsertInput(newIndex, input);
		}
		private static void EditInput(InputsManager.Input input)
		{
			InputsManagerEditor.input = input;
			inputName = input.Name;
			input.Name = "";
		}
		private static void SaveInput()
		{
			input.Name = inputName;
			input = null;
		}
		private static void SwitchNewInput(bool state)
		{
			addingInput = state;

			if (state)
				EditInput(new InputsManager.Input("New input"));
			else
				SaveInput();

			editorInstance.Repaint();
		}
		private static void DuplicateInput(InputsManager.Input input)
		{
			EditInput(new InputsManager.Input(input));
			addingInput = true;
		}
		private static void BindAxis(InputsManager.InputAxis axis, BindTarget target)
		{
			bindingAxis = axis;
			bindingTarget = target;
		}
		private static void EndBindAxis(bool saveChanges)
		{
			if (bindingKey != (int)Key.None && saveChanges)
				switch (bindingTarget)
				{
					case BindTarget.Positive:
						bindingAxis.Positive = (Key)bindingKey;
						break;

					case BindTarget.Negative:
						bindingAxis.Negative = (Key)bindingKey;
						break;
				}

			bindingAxis = null;
			bindingKey = (int)Key.None;
			bindingTarget = BindTarget.None;
		}
		private static void InputAxisEditor(InputsManager.InputAxis axis, InputsManager.InputType type, InputsManager.InputAxis mainAxis = null)
		{
			#region Editor Styles

			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = 28f,
				stretchHeight = false,
				fixedHeight = 15f
			};

			#endregion

			if (type == InputsManager.InputType.Axis)
			{
				bool enumDisabled = axis.Positive == Key.None || axis.Negative == Key.None;

				if (enumDisabled)
					axis.StrongSide = InputsManager.InputAxisStrongSide.None;

				EditorGUI.BeginDisabledGroup(enumDisabled);

				axis.StrongSide = (InputsManager.InputAxisStrongSide)EditorGUILayout.EnumPopup(new GUIContent("Strong Side", "The Strong Side indicates which pressed side wins at runtime"), axis.StrongSide);
			
				EditorGUI.EndDisabledGroup();
			}

			bool positiveDisabled = mainAxis && mainAxis.Positive == Key.None;

			if (positiveDisabled)
				axis.Positive = Key.None;

			EditorGUI.BeginDisabledGroup(positiveDisabled);
			EditorGUILayout.BeginHorizontal();

			axis.Positive = (Key)EditorGUILayout.EnumPopup(type == InputsManager.InputType.Button ? "Button" : "Positive", axis.Positive);

			if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Eye, "Bind"), unstretchableMiniButtonWide))
				BindAxis(axis, BindTarget.Positive);

			EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
			
			if (type == InputsManager.InputType.Axis)
			{
				bool negativeDisabled = mainAxis && mainAxis.Negative == Key.None;

				if (negativeDisabled)
					axis.Negative = Key.None;

				EditorGUI.BeginDisabledGroup(negativeDisabled);
				EditorGUILayout.BeginHorizontal();

				axis.Negative = (Key)EditorGUILayout.EnumPopup("Negative", axis.Negative);

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Eye, "Bind"), unstretchableMiniButtonWide))
					BindAxis(axis, BindTarget.Negative);

				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
			}

			InputsManager.Input keyUsedPositive = Array.Find(InputsManager.KeyUsed(axis.Positive), query => query != input);

			if (keyUsedPositive)
				EditorGUILayout.HelpBox($"The `{axis.Positive}` key seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.None);

			InputsManager.Input keyUsedNegative = Array.Find(InputsManager.KeyUsed(axis.Negative), query => query != input);

			if (keyUsedNegative)
				EditorGUILayout.HelpBox($"The `{axis.Negative}` key seems to be selected by another input{(keyUsedPositive ? " as well" : ". It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame")}.", MessageType.None);
		}
		private static void InputEditor(InputsManager.Input input)
		{
			if (bindingAxis && bindingTarget != BindTarget.None)
			{
				EditorGUILayout.LabelField($"Binding key for the `{inputName}` input", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox(bindingKey == (int)Key.None ? "Waiting for key press..." : $"Current Key: {(bindingKey != 999 ? ((Key)bindingKey).ToString() : "Other")}", MessageType.None);
					
				if (hasShiftBind)
				{
					if (EditorUtility.DisplayDialog("Inputs Manager: Info", "Shift key detected! Which one would you choose?", "Left Shift", "Right Shift"))
						bindingKey = (int)Key.LeftShift;
					else
						bindingKey = (int)Key.RightShift;

					hasShiftBind = false;
					hasBind = true;

					EditorGUILayout.EndScrollView();
					editorInstance.Repaint();

					return;
				}

				if (bindingEvent.type == EventType.KeyUp || bindingEvent.shift)
				{
					if (bindingEvent.shift)
						hasShiftBind = true;
					else
					{
						Key currentKey = InputsManager.KeyCodeToKey(bindingEvent.keyCode);

						bindingKey = currentKey == Key.None ? 999 : (int)currentKey;
					}
					
					hasBind = true;

					EditorGUILayout.EndScrollView();
					editorInstance.Repaint();

					return;
				}

				if (bindingKey != (int)Key.None && bindingKey != 999)
				{
					InputsManager.Input keyUsed = Array.Find(InputsManager.KeyUsed((Key)bindingKey), query => query != input);

					if (keyUsed)
						EditorGUILayout.HelpBox("The current key seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.Info);

					if (GUILayout.Button("Save"))
						EndBindAxis(true);
				}

				EditorGUILayout.EndScrollView();

				return;
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.LabelField("Properties", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			inputName = EditorGUILayout.TextField("Name", inputName);
			input.Type = (InputsManager.InputType)EditorGUILayout.EnumPopup("Type", input.Type);
			input.Interpolation = (InputsManager.InputAxisInterpolation)EditorGUILayout.EnumPopup(new GUIContent("Interpolation", "The interpolation method specifies how the current value of the axis moves towards the target within the interval.\r\n\r\nSmooth: Linear interpolation over time\r\nJump: Same as Smooth with the exception that if an opposite direction is triggered, the value goes instantly to neutral and continue from there. This method won't work if the input type is set to button\r\nInstant: No interpolation"), input.Interpolation);
				
			EditorGUILayout.LabelField("Value Interval");

			EditorGUI.indentLevel++;

			Vector2 valueInterval = input.ValueInterval;

			switch (input.Type)
			{
				case InputsManager.InputType.Axis:
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Negative == Key.None || input.Invert && input.Main.Positive == Key.None);

					valueInterval.x = Mathf.Clamp(EditorGUILayout.FloatField("Minimum", valueInterval.x), Mathf.NegativeInfinity, valueInterval.y);
					
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Positive == Key.None || input.Invert && input.Main.Negative == Key.None);

					valueInterval.y = Mathf.Clamp(EditorGUILayout.FloatField("Maximum", valueInterval.y), valueInterval.x, Mathf.Infinity);

					EditorGUI.EndDisabledGroup();

					input.ValueInterval = valueInterval;

					break;

				case InputsManager.InputType.Button:

					if (input.Invert)
					{
						EditorGUI.BeginDisabledGroup(input.Main.Positive == Key.None);

						valueInterval.y = -Mathf.Clamp(EditorGUILayout.FloatField("Minimum", -valueInterval.y), Mathf.NegativeInfinity, 0f);

						EditorGUI.EndDisabledGroup();
					}

					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.FloatField(input.Invert ? "Maximum" : "Minimum", valueInterval.x);
					EditorGUI.EndDisabledGroup();

					if (!input.Invert)
					{
						EditorGUI.BeginDisabledGroup(input.Main.Positive == Key.None);

						valueInterval.y = Mathf.Clamp(EditorGUILayout.FloatField("Maximum", valueInterval.y), 0f, Mathf.Infinity);

						EditorGUI.EndDisabledGroup();
					}

					input.ValueInterval = valueInterval;

					break;
			}
						
			EditorGUI.indentLevel--;

			input.Invert = EditorGUILayout.Toggle("Invert", input.Invert);

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.LabelField("Keyboard", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			EditorGUILayout.LabelField("Main Bindings", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			InputAxisEditor(input.Main, input.Type);

			EditorGUI.indentLevel--;

			EditorGUI.BeginDisabledGroup(input.Main.Positive == Key.None && input.Main.Negative == Key.None);
			EditorGUILayout.LabelField("Alternative Bindings", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			InputAxisEditor(input.Alt, input.Type, input.Main);
			
			EditorGUI.indentLevel--;

			EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.LabelField("Joystick", EditorStyles.miniBoldLabel);
			EditorGUILayout.HelpBox("Not available at the moment. Stay tuned for new updates!", MessageType.None);
			EditorGUILayout.EndVertical();
		}
		private void OnGUI()
		{
			if (EditorApplication.isPlaying)
			{
				input = null;
				addingInput = false;
				sortingInputs = false;
				settings = false;
				export = false;
				importJson = string.Empty;
			}

			if (!InputsManager.DataAssetExists)
			{
				EditorGUILayout.HelpBox($"We couldn't find the data asset file at the following path \"{InputsManager.DataAssetPath}\". You can create a new one from `Tools > Utilities > Inputs Manager > Create data asset`", MessageType.Error);

				return;
			}

			if (!InputsManager.DataLoaded)
				InputsManager.LoadData();

			scroll = EditorGUILayout.BeginScrollView(scroll);

			EditorGUILayout.Space();

			#region Editor Styles

			float miniButtonSmallWidth = 16f;
			float miniButtonWidth = 20f;
			float miniButtonWideWidth = 25f;
			float miniButtonHeight = 15f;

			GUIStyle unstretchableMiniButtonSmall = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButton = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonNormal = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonLeftSmall = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonLeftWide = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonMiddleSmall = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonMiddle = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonMiddleWide = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonRightSmall = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonRight = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};
			GUIStyle unstretchableMiniButtonRightWide = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
			};

			#endregion

			#region Settings Editor

			if (settings)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.ChevronLeft), unstretchableMiniButtonWide))
				{
					if (export || Import)
					{
						export = false;
						exportInputs = null;
						importJson = "";
					}
					else
						settings = false;

					return;
				}

				GUILayout.Space(5f);
				EditorGUILayout.LabelField(export ? "Save Preset" : (Import ? "Load Preset" : "Settings"), EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (export)
				{
					string exportPathKey = "InputsManager_JsonExportPath";
					string exportPrettyKey = "InputsManager_JsonExportPretty";
					string exportPath = "";
					bool exportPretty = true;

					if (!EditorPrefs.HasKey(exportPathKey))
						EditorPrefs.SetString(exportPathKey, "");

					if (!EditorPrefs.HasKey(exportPrettyKey))
						EditorPrefs.SetBool(exportPrettyKey, true);
					
					exportPath = EditorPrefs.GetString(exportPathKey);
					exportPretty = EditorPrefs.GetBool(exportPrettyKey);

					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.LabelField("Export Properties", EditorStyles.miniBoldLabel);
					EditorGUILayout.Space();

					EditorGUI.indentLevel++;

					EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.TextField("Save Path", exportPath);
					EditorGUI.EndDisabledGroup();

					if (GUILayout.Button("...", unstretchableMiniButtonNormal))
					{
						string path = EditorUtility.SaveFilePanel("Save preset file", "", string.IsNullOrEmpty(exportPath) ? "" : Path.GetFileNameWithoutExtension(exportPath), "json");

						if (!string.IsNullOrEmpty(path))
							exportPath = path;
					}

					EditorGUILayout.EndHorizontal();
					
					exportPretty = EditorGUILayout.Toggle(new GUIContent("Pretty Print", "Pretty Print allows you to export a readable Json file"), exportPretty);
					
					EditorGUI.indentLevel--;

					EditorGUILayout.Space();
					EditorPrefs.SetString(exportPathKey, exportPath);
					EditorPrefs.SetBool(exportPrettyKey, exportPretty);
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.LabelField("Inputs Selection", EditorStyles.miniBoldLabel);
					EditorGUILayout.Space();

					EditorGUI.indentLevel++;

					bool newExportAll = EditorGUILayout.BeginToggleGroup("Select All", exportAll);
					bool exportingAll = true;

					EditorGUILayout.EndToggleGroup();
					EditorGUILayout.Space();

					for (int i = 0; i < exportInputs.Length; i++)
					{
						exportInputs[i] = EditorGUILayout.BeginToggleGroup(InputsManager.GetInput(i).Name, newExportAll == exportAll ? exportInputs[i] : newExportAll);
						exportingAll = exportingAll && exportInputs[i];

						EditorGUILayout.EndToggleGroup();
					}

					if (newExportAll != exportAll)
						exportAll = newExportAll;

					exportAll = exportingAll;

					EditorGUI.indentLevel--;

					EditorGUILayout.Space();
					EditorGUILayout.EndVertical();
					EditorGUI.BeginDisabledGroup(exportInputs.Where(boolean => boolean == true).Count() == 0);
					
					if (GUILayout.Button("Export"))
					{
						InputsManager.Input[] inputs = new InputsManager.Input[exportInputs.Where(boolean => boolean == true).Count()];
						int j = 0;

						for (int i = 0; i < InputsManager.Count; i++)
						{
							EditorUtility.DisplayProgressBar("Inputs Manager", "Processing inputs...", i / InputsManager.Count);

							if (exportInputs[i])
							{
								inputs[j] = InputsManager.GetInput(i);

								j++;
							}
						}

						EditorUtility.DisplayProgressBar("Inputs Manager", "Generating Json...", (InputsManager.Count - 1) / InputsManager.Count);

						Utility.JsonArray<InputsManager.Input> jsonArray = new Utility.JsonArray<InputsManager.Input>(inputs);
						string json = JsonUtility.ToJson(jsonArray, exportPretty);

						if (File.Exists(exportPath))
							File.Delete(exportPath);

						StreamWriter stream = File.CreateText(exportPath);

						EditorUtility.DisplayProgressBar("Inputs Manager", "Writing file...", 1f);

						for (int i = 0; i < json.Length; i++)
							stream.Write(json[i]);

						EditorUtility.DisplayProgressBar("Inputs Manager", "Finishing...", 1f);
						stream.Close();

						EditorUtility.ClearProgressBar();
						
						if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
						{
							if (EditorUtility.DisplayDialog("Inputs Manager: Info", $"Json preset file saved successfully on the following path:\r\n\"{exportPath}\"", "Open folder", "Proceed"))
							{
								string processName = "";

								switch (Application.platform)
								{
									case RuntimePlatform.LinuxEditor:
										processName = "xdg-open";
										exportPath = $"{Path.GetDirectoryName(exportPath)}";
										break;

									case RuntimePlatform.OSXEditor:
										processName = "open";
										break;

									case RuntimePlatform.WindowsEditor:
										processName = "explorer.exe";
										exportPath = $"{Path.GetDirectoryName(exportPath)}";
										break;
								}

								if (!string.IsNullOrEmpty(processName))
									System.Diagnostics.Process.Start(processName, exportPath);
							}
						}
						else
							EditorUtility.DisplayDialog("Inputs Manager: Info", $"Json preset file has been exported successfully!", "Okay");

						inputs = null;
						export = false;
					}

					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndScrollView();

					return;
				}

				if (Import)
				{
					string jsonPreviewFoldoutKey = "InputsManager_PresetJsonPreviewFoldout";

					if (!EditorPrefs.HasKey(jsonPreviewFoldoutKey))
						EditorPrefs.SetBool(jsonPreviewFoldoutKey, false);

					bool jsonPreviewFoldout = EditorPrefs.GetBool(jsonPreviewFoldoutKey);

					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.LabelField("Import Properties", EditorStyles.miniBoldLabel);
					EditorGUILayout.Space();

					EditorGUI.indentLevel++;

					string[] importModes = new string[]
					{
						"Clear",
						"Additive"
					};
					string[] importOverrideModes = new string[]
					{
						"Keep Existing",
						"Ignore Existing"
					};
					bool forceAdditive = InputsManager.Count == 0;

					EditorGUI.BeginDisabledGroup(forceAdditive);

					importAdditive = Utility.IntToBool(EditorGUILayout.Popup(new GUIContent("Import Mode", "Clear: Clears all the existing inputs and add the selected inputs\r\nAdditive: Add the selected inputs without clearing the existing inputs"), Utility.BoolToInt(importAdditive || forceAdditive), importModes));

					EditorGUI.BeginDisabledGroup(!importAdditive);

					importOverride = Utility.IntToBool(EditorGUILayout.Popup(new GUIContent("Override Mode", "Ignore Existing: The selected inputs will override the existing inputs if their names match\r\nKeep Existing: Some imported inputs are gonna be ignored if their names match the existing inputs"), Utility.BoolToInt(importOverride || forceAdditive), importOverrideModes));

					EditorGUI.EndDisabledGroup();
					EditorGUI.EndDisabledGroup();

					EditorGUI.indentLevel--;
					
					EditorGUILayout.Space();
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.LabelField("Inputs Selection", EditorStyles.miniBoldLabel);

					EditorGUI.indentLevel++;

					EditorGUILayout.Space();

					bool newImportAll = EditorGUILayout.BeginToggleGroup("Select All", importAll);
					bool importingAll = true;

					EditorGUILayout.EndToggleGroup();
					EditorGUILayout.Space();

					for (int i = 0; i < importInputsSelection.Length; i++)
					{
						bool inputExists = importAdditive && InputsManager.IndexOf(importInputs[i].Name) != -1;
						bool inputDisabled = inputExists && !importOverride;

						EditorGUI.BeginDisabledGroup(inputDisabled);

						importInputsSelection[i] = EditorGUILayout.BeginToggleGroup(importInputs[i].Name, newImportAll == importAll ? importInputsSelection[i] : newImportAll) && !inputDisabled;
						importingAll = importingAll && importInputsSelection[i];

						EditorGUILayout.EndToggleGroup();
						EditorGUI.EndDisabledGroup();
					}

					if (newImportAll != importAll)
						importAll = newImportAll;

					importAll = importingAll;

					EditorGUI.indentLevel--;

					EditorGUILayout.Space();
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Json File Preview", EditorStyles.miniBoldLabel);

					if (GUILayout.Button(jsonPreviewFoldout ? "Hide" : "Show", unstretchableMiniButtonNormal))
						jsonPreviewFoldout = !jsonPreviewFoldout;

					EditorGUILayout.EndHorizontal();
					EditorPrefs.SetBool(jsonPreviewFoldoutKey, jsonPreviewFoldout);

					if (jsonPreviewFoldout)
					{
						EditorGUILayout.Space();
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.TextArea(importJson);
						EditorGUI.EndDisabledGroup();
					}

					EditorGUILayout.EndVertical();
					EditorGUI.BeginDisabledGroup(importInputsSelection.Where(boolean => boolean == true).Count() == 0);
					
					if (GUILayout.Button("Import"))
					{
						if (!importAdditive)
							InputsManager.RemoveAll();

						for (int i = 0; i < importInputs.Length; i++)
							if (importInputsSelection[i])
							{
								int existingInputIndex = InputsManager.IndexOf(importInputs[i].Name);

								if (existingInputIndex != -1)
								{
									InputsManager.RemoveInput(importInputs[i].Name);
									InputsManager.InsertInput(existingInputIndex, importInputs[i]);
								}
								else
									InputsManager.AddInput(importInputs[i]);
							}

						EditorUtility.DisplayDialog("Inputs Manager: Info", $"The selected inputs have been imported successfully!", "Okay");

						importJson = "";
						settings = false;
					}
					
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndScrollView();

					return;
				}

				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Data Management", EditorStyles.miniBoldLabel);
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Reset", EditorStyles.miniButtonLeft))
				{
					ResetInputsManager();

					return;
				}

				if (GUILayout.Button("Load Preset", EditorStyles.miniButtonMid))
				{
					LoadPreset();

					return;
				}

				EditorGUI.BeginDisabledGroup(InputsManager.Count == 0);

				if (GUILayout.Button("Save Preset", EditorStyles.miniButtonRight))
				{
					export = true;
					exportInputs = new bool[InputsManager.Count];

					for (int i = 0; i < exportInputs.Length; i++)
						exportInputs[i] = true;

					return;
				}

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Timing", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				InputsManager.InterpolationTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Interpolation Time", $"How much time does it take an input to reach it's target. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.InterpolationTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) / 1000f);
				InputsManager.HoldTriggerTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Trigger", $"How much time does it take an input to be triggered as held. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldTriggerTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) / 1000f);
				InputsManager.HoldWaitTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Wait", $"How much time does it take an input to be triggered as held once more. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldWaitTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) / 1000f);
				InputsManager.DoublePressTimeout = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Double Press Timeout", $"Double press check time range. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.DoublePressTimeout * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) / 1000f);

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Help", EditorStyles.miniBoldLabel);
				EditorGUILayout.Space();

				EditorGUILayout.HelpBox("The open-source Inputs Manager has been created by MediaMax.", MessageType.None);

				if (GUILayout.Button("Report Error/Issue"))
					ReportError();

				if (GUILayout.Button("More about Inputs Manager"))
					About();

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndScrollView();

				return;
			}

			#endregion

			#region Input Editor

			if (input)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.ChevronLeft), unstretchableMiniButtonWide))
				{
					if (bindingAxis && bindingTarget != BindTarget.None)
						EndBindAxis(false);
					else if (addingInput)
						SwitchNewInput(false);
					else if (string.IsNullOrEmpty(inputName))
						EditorUtility.DisplayDialog("Inputs Manager: Error", "The input name cannot be empty.", "Okay");
					else if (InputsManager.IndexOf(inputName) != -1)
						EditorUtility.DisplayDialog("Inputs Manager: Info", "We didn't save the input name because it matches another one.", "Okay");
					else 
						SaveInput();

					return;
				}

				bindingEvent = Event.current;

				GUILayout.Space(5f);
				EditorGUILayout.LabelField(addingInput ? "Create New Input" : $"{inputName} Configurations", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				InputEditor(input);

				if (hasBind)
				{
					hasBind = false;

					return;
				}

				if (bindingAxis && bindingTarget != BindTarget.None)
					return;

				EditorGUILayout.Space();

				if (addingInput)
					if (GUILayout.Button($"Add {(string.IsNullOrEmpty(input.Name) ? "New Input" : inputName)}"))
					{
						if (string.IsNullOrEmpty(inputName))
							EditorUtility.DisplayDialog("Inputs Manager: Error", "The new input name cannot be empty.", "Okay");
						else if (InputsManager.IndexOf(inputName) != -1)
							EditorUtility.DisplayDialog("Inputs Manager: Error", "The new input name matches an older input. Please use a different name or modify the existing input.", "Okay");
						else
						{
							input.Name = inputName;
							InputsManager.AddInput(input);
							SwitchNewInput(false);
						}
					}

				EditorGUILayout.EndScrollView();

				return;
			}
			else
				bindingEvent = null;

			#endregion

			#region Inputs Editor

			if (sortingInputs)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.ChevronLeft), unstretchableMiniButtonWide))
				{
					sortingInputs = false;

					return;
				}

				GUILayout.Space(5f);
				EditorGUILayout.LabelField("Sort", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Inputs Manager", EditorStyles.boldLabel);
				
				if (!EditorApplication.isPlaying)
				{
					EditorGUI.BeginDisabledGroup(!InputsManager.DataChanged);

					if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Save), unstretchableMiniButtonWide))
						InputsManager.SaveData();

					EditorGUI.EndDisabledGroup();

					if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Add), unstretchableMiniButtonWide))
					{
						SwitchNewInput(true);
						sortingInputs = false;
						settings = false;

						return;
					}

					EditorGUI.BeginDisabledGroup(InputsManager.Count < 2);

					if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Sort), unstretchableMiniButtonWide))
					{
						sortingInputs = !sortingInputs;
						settings = false;
					}

					if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Trash), unstretchableMiniButtonWide))
						if (EditorUtility.DisplayDialog("Inputs Manager: Warning", "Are you sure of removing all of the existing inputs?", "Yes", "No"))
						{
							InputsManager.RemoveAll();

							return;
						}

					EditorGUI.EndDisabledGroup();

					if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Settings), unstretchableMiniButtonWide))
					{
						settings = true;
						sortingInputs = false;

						return;
					}
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			if (InputsManager.Count > 0)
			{
				for (int i = 0; i < InputsManager.Count; i++)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.BeginHorizontal();

					if (sortingInputs)
					{
						EditorGUI.BeginDisabledGroup(i == 0);

						if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.CaretUp), unstretchableMiniButtonLeft))
						{
							MoveInput(i, i - 1);

							return;
						}
							
						EditorGUI.EndDisabledGroup();
						EditorGUI.BeginDisabledGroup(i == InputsManager.Count - 1);

						if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.CaretDown), unstretchableMiniButtonRight))
						{
							MoveInput(i, i + 1);

							return;
						}
							
						EditorGUI.EndDisabledGroup();
					}

					EditorGUILayout.LabelField(InputsManager.GetInput(i).Name, EditorStyles.miniBoldLabel);
					
					if (!EditorApplication.isPlaying && !sortingInputs)
					{
						if (GUILayout.Button(EditorUtilities.Icons.Pencil, unstretchableMiniButtonWide))
							EditInput(InputsManager.GetInput(i));

						if (GUILayout.Button(EditorUtilities.Icons.Clone, unstretchableMiniButtonWide))
							DuplicateInput(InputsManager.GetInput(i));

						if (GUILayout.Button(EditorUtilities.Icons.Trash, unstretchableMiniButtonWide))
							if (EditorUtility.DisplayDialog("Inputs Manager: Warning", $"Are you sure of removing the '{InputsManager.GetInput(i).Name}' input?", "Yes", "Not really"))
								InputsManager.RemoveInput(i);
					}

					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}

				if (EditorApplication.isPlaying)
					EditorGUILayout.HelpBox("Hey! Unfortunately, you can't change or modify anything because you're on play mode.", MessageType.Info);
			}
			else if (!EditorApplication.isPlaying)
				EditorGUILayout.HelpBox("The inputs list is empty for the moment, press the \"+\" button to create a new one. You can also import some presets from the \"Settings\" menu.", MessageType.Info);
			else
				EditorGUILayout.HelpBox("The inputs list is empty for the moment!", MessageType.Info);

			#endregion

			EditorGUILayout.EndScrollView();
		}

		#endregion

		#region  Destroy & Enable

		private void OnEnable()
		{
			editorInstance = this;

			if (input)
				input = null;

			if (bindingAxis)
				bindingAxis = null;
		}
		private void OnDestroy()
		{
			editorInstance = null;

			if (input && !string.IsNullOrEmpty(inputName))
				SaveInput();
			else if (input)
				input = null;

			if (bindingAxis && bindingTarget != BindTarget.None)
				EndBindAxis(false);
			else if (bindingAxis)
				bindingAxis = null;

			if (InputsManager.DataChanged)
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Warning", "You've some unsaved data that you might lose! Do you want to save it?", "Save", "Discard"))
					InputsManager.SaveData();
			}

			InputsManager.UnloadData();
		}

		#endregion
		
		#endregion
	}
}
