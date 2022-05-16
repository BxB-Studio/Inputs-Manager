#region Namespaces

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
#if ENABLE_INPUT_SYSTEM
#if UNITY_2019_1_OR_NEWER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#else
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.LowLevel;
#endif
#endif
using Utilities.Editor;

#endregion

namespace Utilities.Inputs
{
	public class InputsManagerEditor : EditorWindow
	{
		#region Enumerators

		private enum BindTarget { None, Positive, Negative, GamepadPositive, GamepadNegative }
		private static string[] Keys
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				if (keys == null || keys.Length < 1)
				{
					keys = new string[Enum.GetValues(typeof(Key)).Length];

					int firstIntValue = keys.Length;

					for (int i = 0; i < keys.Length; i++)
					{
						string key = ((Key)i).ToString();

						if (!int.TryParse(key, out int _))
							keys[i] = key;
						else
							firstIntValue = Mathf.Min(firstIntValue, i);
					}

					if (firstIntValue != keys.Length)
						Array.Resize(ref keys, firstIntValue);
				}
#endif

				return keys;
			}
		}
		private static string[] keys;
		private static string[] GamepadBindings
		{
			get
			{
				if (gamepadBindings == null || gamepadBindings.Length < 1)
				{
					gamepadBindings = new string[Enum.GetValues(typeof(GamepadBinding)).Length];

					int firstIntValue = gamepadBindings.Length;

					for (int i = 0; i < gamepadBindings.Length; i++)
					{
						string binding = ((GamepadBinding)i).ToString();

						if (!int.TryParse(binding, out int _))
							gamepadBindings[i] = binding;
						else
							firstIntValue = Mathf.Min(firstIntValue, i);
					}

					if (firstIntValue != gamepadBindings.Length)
						Array.Resize(ref gamepadBindings, firstIntValue);
				}

				return gamepadBindings;
			}
		}
		private static string[] gamepadBindings;

		#endregion

		#region Variables

		private static InputsManagerEditor editorInstance;
		private static InputsManager.Input.Axis bindingAxis;
		private static InputsManager.Input input;
		private static BindTarget bindingTarget;
		private static Event bindingEvent;
		private static GamepadBinding bindedGamepadBind;
		private readonly static string inittializerKey = "InputsManager_Init";
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
				return !string.IsNullOrEmpty(importJson) && !string.IsNullOrWhiteSpace(importJson) && importInputs != null;
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

		#region Methods

		#region Menu Items

		[MenuItem("Tools/Utilities/Inputs Manager/Edit Settings...", false, 1)]
		public static void OpenInputsManager()
		{
			if (!EditorPrefs.HasKey(inittializerKey))
			{
				EditorUtility.DisplayDialog("Inputs Manager: Welcome!", "Hey! Thank you for using the Beta version of the Inputs Manager, we are looking forward to improve it based on your honorable reviews and reports in case of any problems!", "Okay!");
				CreateInputsManager();

				return;
			}

#if UNITY_2019_3_OR_NEWER
			float minWindowWidth = 360f;
#else
			float minWindowWidth = 370f;
#endif

			editorInstance = GetWindow<InputsManagerEditor>();
			editorInstance.titleContent = new GUIContent("InputsManager");
			editorInstance.minSize = new Vector2(minWindowWidth, 512f);

			editorInstance.Show();
		}
		public static void OpenInputsManager(InputsManager.Input input)
		{
			OpenInputsManager();
			EditInput(input);
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

			string dataPath = Path.Combine(Application.dataPath, "Resources", $"{InputsManager.DataAssetPath}.bytes");
			DataSerializationUtility<InputsManager.DataSheet> data = new DataSerializationUtility<InputsManager.DataSheet>(dataPath, false);

			if (!data.Delete())
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Internal Error", "Unable to delete the current `InputsManager` asset in order to create a new one!", "Report Error...", "Cancel"))
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
			else if (EditorUtility.DisplayDialog("Inputs Manager: Info", "A new Inputs Manager asset has been created! Do you want to load a Json preset file?", "Yes", "No"))
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
			OpenExternalURL("https://github.com/yboumaiza7/Inputs-Manager/issues/new");
		}
		[MenuItem("Tools/Utilities/Inputs Manager/About...", false, 5)]
		public static void About()
		{
			OpenExternalURL("https://github.com/yboumaiza7/Inputs-Manager");
		}

		private static void OpenExternalURL(string url)
		{
			if (EditorUtility.DisplayDialog("Inputs Manager: Info", "You're about to visit an external link. Do you want to proceed?", "Yes", "No"))
				Application.OpenURL(url);
		}

		#endregion

		#region Utilities

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

			while (!string.IsNullOrEmpty(line = stream.ReadLine()))
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
		private static void BindAxis(InputsManager.Input.Axis axis, BindTarget target)
		{
			bindingAxis = axis;
			bindingTarget = target;
		}
		private static void EndBindAxis(bool saveChanges)
		{
			bool bindingForGamepad = bindingTarget == BindTarget.GamepadPositive || bindingTarget == BindTarget.GamepadNegative;

#if ENABLE_INPUT_SYSTEM
			switch (bindingTarget)
			{
				case BindTarget.Positive:
					bindingAxis.Positive = (Key)bindingKey;

					break;

				case BindTarget.Negative:
					bindingAxis.Negative = (Key)bindingKey;

					break;

				case BindTarget.GamepadPositive:
					bindingAxis.GamepadPositive = (GamepadBinding)bindingKey;

					break;

				case BindTarget.GamepadNegative:
					bindingAxis.GamepadNegative = (GamepadBinding)bindingKey;

					break;
			}

			bindingAxis = null;
			bindingKey = bindingForGamepad ? (int)GamepadBinding.None : (int)Key.None;
			bindingTarget = BindTarget.None;

			if (bindingForGamepad)
				InputSystem.onEvent -= GamepadBindEvent;
#endif
		}
#if ENABLE_INPUT_SYSTEM
#if UNITY_2019_1_OR_NEWER
		private static void GamepadBindEvent(InputEventPtr eventPtr, InputDevice device)
#else
		private static void GamepadBindEvent(InputEventPtr eventPtr)
#endif
		{
			if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
				return;

#if UNITY_2019_1_OR_NEWER
			if (!(device is Gamepad gamepad))
				return;
#else
			Gamepad gamepad = InputSystem.GetDeviceById(eventPtr.deviceId) as Gamepad;
#endif

			if (gamepad == null)
				return;

			if (gamepad.dpad.ReadValueFromEvent(eventPtr, out Vector2 dpad) && dpad != Vector2.zero)
			{
				if (dpad.y > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.DpadUp;
				else if (dpad.x > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.DpadRight;
				else if (dpad.y < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.DpadDown;
				else if (dpad.x < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.DpadLeft;
			}
			else if (gamepad.buttonNorth.ReadValueFromEvent(eventPtr, out float buttonNorth) && buttonNorth > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.ButtonNorth;
			else if (gamepad.buttonEast.ReadValueFromEvent(eventPtr, out float buttonEast) && buttonEast > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.ButtonEast;
			else if (gamepad.buttonSouth.ReadValueFromEvent(eventPtr, out float buttonSouth) && buttonSouth > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.ButtonSouth;
			else if (gamepad.buttonWest.ReadValueFromEvent(eventPtr, out float buttonWest) && buttonWest > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.ButtonWest;
			else if (gamepad.leftStickButton.ReadValueFromEvent(eventPtr, out float leftStickButton) && leftStickButton > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.LeftStickButton;
			else if (gamepad.leftStick.ReadValueFromEvent(eventPtr, out Vector2 leftStick) && leftStick != Vector2.zero)
			{
				if (leftStick.y > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.LeftStickUp;
				else if (leftStick.x > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.LeftStickRight;
				else if (leftStick.y < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.LeftStickDown;
				else if (leftStick.x < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.LeftStickLeft;
			}
			else if (gamepad.rightStickButton.ReadValueFromEvent(eventPtr, out float rightStickButton) && rightStickButton > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.RightStickButton;
			else if (gamepad.rightStick.ReadValueFromEvent(eventPtr, out Vector2 rightStick) && rightStick != Vector2.zero)
			{
				if (rightStick.y > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.RightStickUp;
				else if (rightStick.x > InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.RightStickRight;
				else if (rightStick.y < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.RightStickDown;
				else if (rightStick.x < -InputsManager.GamepadThreshold)
					bindedGamepadBind = GamepadBinding.RightStickLeft;
			}
			else if (gamepad.leftShoulder.ReadValueFromEvent(eventPtr, out float leftShoulder) && leftShoulder > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.LeftShoulder;
			else if (gamepad.rightShoulder.ReadValueFromEvent(eventPtr, out float rightShoulder) && rightShoulder > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.RightShoulder;
			else if (gamepad.leftTrigger.ReadValueFromEvent(eventPtr, out float leftTrigger) && leftTrigger > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.LeftTrigger;
			else if (gamepad.rightTrigger.ReadValueFromEvent(eventPtr, out float rightTrigger) && rightTrigger > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.RightTrigger;
			else if (gamepad.startButton.ReadValueFromEvent(eventPtr, out float startButton) && startButton > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.StartButton;
			else if (gamepad.selectButton.ReadValueFromEvent(eventPtr, out float selectButton) && selectButton > InputsManager.GamepadThreshold)
				bindedGamepadBind = GamepadBinding.SelectButton;
		}
#endif
		#endregion

		#region Editor

		private static void InputAxisEditor(InputsManager.Input.Axis axis, InputsManager.Input.InputType type, InputsManager.Input.Axis mainAxis = null, bool isGamepadEditor = false)
		{
			#region Editor Styles

			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = 28f
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = 15f
#endif
			};

			#endregion

#if ENABLE_INPUT_SYSTEM
			if (type == InputsManager.Input.InputType.Axis)
			{
				bool enumDisabled;

				if (isGamepadEditor)
					enumDisabled = axis.GamepadPositive == GamepadBinding.None || axis.GamepadNegative == GamepadBinding.None;
				else
					enumDisabled = axis.Positive == Key.None || axis.Negative == Key.None;

				InputsManager.Input.Axis.Side newStrongSide = isGamepadEditor ? axis.GamepadStrongSide : axis.StrongSide;

				if (enumDisabled)
					newStrongSide = InputsManager.Input.Axis.Side.None;

				EditorGUI.BeginDisabledGroup(enumDisabled);

				newStrongSide = (InputsManager.Input.Axis.Side)EditorGUILayout.EnumPopup(new GUIContent("Strong Side", "The Strong Side indicates which pressed side wins at runtime"), newStrongSide);

				if (!isGamepadEditor && axis.StrongSide != newStrongSide)
					axis.StrongSide = newStrongSide;

				if (isGamepadEditor && axis.GamepadStrongSide != newStrongSide)
					axis.GamepadStrongSide = newStrongSide;

				EditorGUI.EndDisabledGroup();
			}

			bool positiveDisabled;

			if (isGamepadEditor)
				positiveDisabled = mainAxis && mainAxis.GamepadPositive == GamepadBinding.None;
			else
				positiveDisabled = mainAxis && mainAxis.Positive == Key.None;

			int newPositive = isGamepadEditor ? (int)axis.GamepadPositive : (int)axis.Positive;

			if (positiveDisabled)
				newPositive = isGamepadEditor ? (int)GamepadBinding.None : (int)Key.None;

			EditorGUI.BeginDisabledGroup(positiveDisabled);
			EditorGUILayout.BeginHorizontal();

			newPositive = EditorGUILayout.Popup(type == InputsManager.Input.InputType.Button ? "Button" : "Positive", newPositive, isGamepadEditor ? GamepadBindings : Keys);

			if (isGamepadEditor)
			{
				GamepadBinding newGamepadPositive = (GamepadBinding)newPositive;

				if (axis.GamepadPositive != newGamepadPositive)
					axis.GamepadPositive = newGamepadPositive;
			}
			else
			{
				Key newKeyboardPositive = (Key)newPositive;

				if (axis.Positive != newKeyboardPositive)
					axis.Positive = newKeyboardPositive;
			}

			if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Eye, "Bind"), unstretchableMiniButtonWide))
			{
				if (isGamepadEditor)
					InputSystem.onEvent += GamepadBindEvent;

				BindAxis(axis, isGamepadEditor ? BindTarget.GamepadPositive : BindTarget.Positive);
			}

			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

			if (type == InputsManager.Input.InputType.Axis)
			{
				bool negativeDisabled;

				if (isGamepadEditor)
					negativeDisabled = mainAxis && mainAxis.GamepadNegative == GamepadBinding.None;
				else
					negativeDisabled = mainAxis && mainAxis.Negative == Key.None;

				EditorGUI.BeginDisabledGroup(negativeDisabled);
				EditorGUILayout.BeginHorizontal();

				int newNegative = EditorGUILayout.Popup("Negative", isGamepadEditor ? (int)axis.GamepadNegative : (int)axis.Negative, isGamepadEditor ? GamepadBindings : Keys);

				if (isGamepadEditor)
				{
					GamepadBinding newGamepadNegative = (GamepadBinding)newNegative;

					if (axis.GamepadNegative != newGamepadNegative)
						axis.GamepadNegative = newGamepadNegative;
				}
				else
				{
					Key newKeyboardNegative = (Key)newNegative;

					if (axis.Negative != newKeyboardNegative)
						axis.Negative = newKeyboardNegative;
				}

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Eye, "Bind"), unstretchableMiniButtonWide))
				{
					if (isGamepadEditor)
						InputSystem.onEvent += GamepadBindEvent;

					BindAxis(axis, isGamepadEditor ? BindTarget.GamepadNegative : BindTarget.Negative);
				}

				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
			}

			InputsManager.Input positiveBindingUsed = Array.Find(isGamepadEditor ? InputsManager.GamepadBindingUsed(axis.GamepadPositive) : InputsManager.KeyUsed(axis.Positive), @in => @in != input);

			if (positiveBindingUsed)
				EditorGUILayout.HelpBox($"The `{(isGamepadEditor ? axis.GamepadPositive.ToString() : axis.Positive.ToString())}` binding seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.None);

			InputsManager.Input negativeBindingUsed = Array.Find(isGamepadEditor ? InputsManager.GamepadBindingUsed(axis.GamepadNegative) : InputsManager.KeyUsed(axis.Negative), @in => @in != input);

			if (negativeBindingUsed)
				EditorGUILayout.HelpBox($"The `{(isGamepadEditor ? axis.GamepadNegative.ToString() : axis.Negative.ToString())}` binding seems to be selected by another input{(positiveBindingUsed ? " as well" : ". It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame")}.", MessageType.None);
#endif
		}
		private static void InputEditor(InputsManager.Input input)
		{
#if ENABLE_INPUT_SYSTEM
			if (bindingAxis && bindingTarget != BindTarget.None)
			{
				bool bindingForGamepad = bindingTarget == BindTarget.GamepadPositive || bindingTarget == BindTarget.GamepadNegative;

				EditorGUILayout.LabelField($"Binding key for the `{inputName}` input", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox(bindingKey == (bindingForGamepad ? (int)GamepadBinding.None : (int)Key.None) ? "Waiting for key press..." : $"Current Key: {(bindingKey != 999 ? (bindingForGamepad ? ((GamepadBinding)bindingKey).ToString() : ((Key)bindingKey).ToString()) : "Unknown")}", MessageType.None);

				if (hasShiftBind)
				{
					if (EditorUtility.DisplayDialog("Inputs Manager: Info", "Shift key detected! Which one would you choose?", "Left Shift", "Right Shift"))
						bindingKey = (int)Key.LeftShift;
					else
						bindingKey = (int)Key.RightShift;

					hasShiftBind = false;
					hasBind = true;

					editorInstance.Repaint();
				}

				if (bindingForGamepad)
				{
					InputSystem.Update();
					editorInstance.Repaint();

					if (bindedGamepadBind != GamepadBinding.None)
					{
						bindingKey = (int)bindedGamepadBind;
						bindedGamepadBind = (int)GamepadBinding.None;
						hasBind = true;
					}
				}
				else if (bindingEvent.type == EventType.KeyUp || bindingEvent.shift)
				{
					if (bindingEvent.shift)
						hasShiftBind = true;
					else
					{
						Key currentKey = InputsManager.KeyCodeToKey(bindingEvent.keyCode);

						bindingKey = currentKey == Key.None ? 999 : (int)currentKey;
					}

					hasBind = true;

					editorInstance.Repaint();
				}

				if ((!bindingForGamepad && bindingKey != (int)Key.None || bindingForGamepad && bindingKey != (int)GamepadBinding.None) && bindingKey != 999)
				{
					InputsManager.Input keyUsed = Array.Find(bindingForGamepad ? InputsManager.GamepadBindingUsed((GamepadBinding)bindingKey) : InputsManager.KeyUsed((Key)bindingKey), query => query != input);

					if (keyUsed)
						EditorGUILayout.HelpBox("The current binding seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.Info);

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

			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

			inputName = EditorGUILayout.TextField("Name", inputName);
			input.Type = (InputsManager.Input.InputType)EditorGUILayout.EnumPopup("Type", input.Type);

			EditorGUI.EndDisabledGroup();

			input.Interpolation = (InputsManager.Input.InputInterpolation)EditorGUILayout.EnumPopup(new GUIContent("Interpolation", "The interpolation method specifies how the current value of the axis moves towards the target within the interval.\r\n\r\nSmooth: Linear interpolation over time\r\nJump: Same as Smooth with the exception that if an opposite direction is triggered, the value goes instantly to neutral and continue from there. This method won't work if the input type is set to button\r\nInstant: No interpolation"), input.Interpolation);

			EditorGUILayout.LabelField("Value Interval");

			EditorGUI.indentLevel++;

			Vector2 valueInterval = input.ValueInterval;

			switch (input.Type)
			{
				case InputsManager.Input.InputType.Axis:
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Negative == Key.None || input.Invert && input.Main.Positive == Key.None);

					valueInterval.x = Mathf.Clamp(EditorGUILayout.FloatField("Minimum", valueInterval.x), Mathf.NegativeInfinity, valueInterval.y);

					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Positive == Key.None || input.Invert && input.Main.Negative == Key.None);

					valueInterval.y = Mathf.Clamp(EditorGUILayout.FloatField("Maximum", valueInterval.y), valueInterval.x, Mathf.Infinity);

					EditorGUI.EndDisabledGroup();

					input.ValueInterval = valueInterval;

					break;

				case InputsManager.Input.InputType.Button:

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
			EditorGUILayout.LabelField("Gamepad", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			EditorGUILayout.LabelField("Main Bindings", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			InputAxisEditor(input.Main, input.Type, null, true);

			EditorGUI.indentLevel--;

			EditorGUI.BeginDisabledGroup(input.Main.GamepadPositive == GamepadBinding.None && input.Main.GamepadNegative == GamepadBinding.None);
			EditorGUILayout.LabelField("Alternative Bindings", EditorStyles.miniBoldLabel);

			EditorGUI.indentLevel++;

			InputAxisEditor(input.Alt, input.Type, input.Main, true);

			EditorGUI.indentLevel--;

			EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
#endif
		}
		private void OnGUI()
		{
			if (EditorApplication.isPlaying)
			{
				//input = null;
				addingInput = false;
				sortingInputs = false;
				//settings = false;
				export = false;
				importJson = string.Empty;
			}

			bool newInputSystemInstalled = true;

#if !ENABLE_INPUT_SYSTEM
			newInputSystemInstalled = false;
#endif

			if (!newInputSystemInstalled)
			{
				EditorGUILayout.HelpBox("It seems like the the Unity New Input System is either not installed or disabled on this project. Please check the Package Manager and install the requested package! Otherwise you need to enable it from the Player Settings window. For more information check the Unity help forums on how to install packages from the Package Manager.", MessageType.Error);

				return;
			}
			else if (!InputsManager.DataAssetExists)
			{
				EditorGUILayout.HelpBox($"We couldn't find the data asset file at the following path \"Resources/{InputsManager.DataAssetPath}\". You can create a new one from `Tools > Utilities > Inputs Manager > Create data asset`", MessageType.Error);

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
#if !UNITY_2019_3_OR_NEWER
			float miniButtonHeight = 15f;
#endif

			GUIStyle unstretchableMiniButtonSmall = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButton = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonNormal = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonLeftSmall = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonLeftWide = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonMiddleSmall = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonMiddle = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonMiddleWide = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonRightSmall = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonRight = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
			};
			GUIStyle unstretchableMiniButtonRightWide = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
#if !UNITY_2019_3_OR_NEWER
				,
				stretchHeight = false,
				fixedHeight = miniButtonHeight
#endif
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
				EditorGUILayout.LabelField(export ? "Save Preset" : Import ? "Load Preset" : "Settings", EditorStyles.boldLabel);
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
							if (EditorUtility.DisplayDialog("Inputs Manager: Info", $"Json preset file saved successfully on the following path:\r\n\"{exportPath}\"", "Open folder", "Continue"))
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
					bool forceAdditive = InputsManager.Count < 1;

					EditorGUI.BeginDisabledGroup(forceAdditive);

					importAdditive = Utility.NumberToBool(EditorGUILayout.Popup(new GUIContent("Import Mode", "Clear: Clears all the existing inputs and add the selected inputs\r\nAdditive: Add the selected inputs without clearing the existing inputs"), Utility.BoolToNumber(importAdditive || forceAdditive), importModes));

					EditorGUI.BeginDisabledGroup(!importAdditive);

					importOverride = Utility.NumberToBool(EditorGUILayout.Popup(new GUIContent("Override Mode", "Ignore Existing: The selected inputs will override the existing inputs if their names match\r\nKeep Existing: Some imported inputs are gonna be ignored if their names match the existing inputs"), Utility.BoolToNumber(importOverride || forceAdditive), importOverrideModes));

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
						bool inputExists = importAdditive && InputsManager.IndexOf(importInputs[i].Name) > -1;
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

								if (existingInputIndex > -1)
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

				EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
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

				EditorGUI.BeginDisabledGroup(InputsManager.Count < 1);

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
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Behaviour", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				InputsManager.InputSource newInputSourcePriority = (InputsManager.InputSource)EditorGUILayout.EnumPopup(new GUIContent("Source Priority", "This indicates which input source has priority over the others."), InputsManager.InputSourcePriority);

				if (InputsManager.InputSourcePriority != newInputSourcePriority)
					InputsManager.InputSourcePriority = newInputSourcePriority;

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Timing", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				float newInterpolationTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Interpolation Time", $"How much time does it take an input to reach it's target. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.InterpolationTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f);

				if (InputsManager.InterpolationTime != newInterpolationTime)
					InputsManager.InterpolationTime = newInterpolationTime;

				float newHoldTriggerTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Trigger", $"How much time does it take an input to be triggered as held. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldTriggerTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f);

				if (InputsManager.HoldTriggerTime != newHoldTriggerTime)
					InputsManager.HoldTriggerTime = newHoldTriggerTime;

				float newHoldWaitTime = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Wait", $"How much time does it take an input to be triggered as held once more. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldWaitTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f);

				if (InputsManager.HoldWaitTime != newHoldWaitTime)
					InputsManager.HoldWaitTime = newHoldWaitTime;

				float newDoublePressTimeout = Utility.ClampInfinity(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Double Press Timeout", $"Double press check time range. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.DoublePressTimeout * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f);

				if (InputsManager.DoublePressTimeout != newDoublePressTimeout)
					InputsManager.DoublePressTimeout = newDoublePressTimeout;

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Thresholds", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

				float newGamepadThreshold = Mathf.Clamp01(EditorGUILayout.Slider(new GUIContent("Gamepad Threshold", "The gamepad threshold is the minimum value used to detect gamepad actions. This value doesn't apply to sticks or triggers in Play mode."), InputsManager.GamepadThreshold, 0f, 1f));

				if (InputsManager.GamepadThreshold != newGamepadThreshold)
					InputsManager.GamepadThreshold = newGamepadThreshold;

				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Help", EditorStyles.miniBoldLabel);
				EditorGUILayout.Space();

				EditorGUILayout.HelpBox("The Inputs Manager is an open-source asset available on GitHub and is created by MediaMax.", MessageType.None);

				if (GUILayout.Button("Report Error/Issue"))
					ReportError();

				if (GUILayout.Button("Visit the Github Repository"))
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
					else if (string.IsNullOrEmpty(inputName) || string.IsNullOrWhiteSpace(inputName))
						EditorUtility.DisplayDialog("Inputs Manager: Error", "The input name cannot be empty.", "Okay");
					else if (!EditorApplication.isPlaying && InputsManager.IndexOf(inputName) > -1)
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
					if (GUILayout.Button($"Add {(string.IsNullOrEmpty(input.Name) || string.IsNullOrWhiteSpace(input.Name) ? "New Input" : inputName)}"))
					{
						if (string.IsNullOrEmpty(inputName) || string.IsNullOrWhiteSpace(inputName))
							EditorUtility.DisplayDialog("Inputs Manager: Error", "The new input name cannot be empty.", "Okay");
						else if (InputsManager.IndexOf(inputName) > -1)
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
				EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
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
					sortingInputs = true;
					settings = false;
				}

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Trash), unstretchableMiniButtonWide))
					if (EditorUtility.DisplayDialog("Inputs Manager: Warning", "Are you sure of removing all of the existing inputs?", "Yes", "No"))
						{
							InputsManager.RemoveAll();

							return;
						}

				EditorGUI.EndDisabledGroup();
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Settings), unstretchableMiniButtonWide))
				{
					settings = true;
					sortingInputs = false;

					return;
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			if (EditorApplication.isPlaying)
				EditorGUILayout.HelpBox("You can't change or modify some settings in play mode. Keep in mind thay any changes on the Inputs Manager editor won't be saved unless using a custom script to override this behaviour!", MessageType.Info);

			if (InputsManager.Count > 0)
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

					if (!sortingInputs)
					{
						if (GUILayout.Button(EditorUtilities.Icons.Pencil, unstretchableMiniButtonWide))
							EditInput(InputsManager.GetInput(i));

						EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

						if (GUILayout.Button(EditorUtilities.Icons.Clone, unstretchableMiniButtonWide))
							DuplicateInput(InputsManager.GetInput(i));

						if (GUILayout.Button(EditorUtilities.Icons.Trash, unstretchableMiniButtonWide))
							if (EditorUtility.DisplayDialog("Inputs Manager: Warning", $"Are you sure of removing the '{InputsManager.GetInput(i).Name}' input?", "Yes", "Not really"))
								InputsManager.RemoveInput(i);

						EditorGUI.EndDisabledGroup();
					}

					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			else if (!EditorApplication.isPlaying)
				EditorGUILayout.HelpBox("The inputs list is empty for the moment, press the \"+\" button to create a new one. You can also import some presets from the \"Settings\" menu.", MessageType.Info);
			else
				EditorGUILayout.HelpBox("The inputs list is empty for the moment!", MessageType.Info);

			#endregion

			EditorGUILayout.EndScrollView();
		}

		#endregion

		#region Destroy & Enable

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

			if (input && !string.IsNullOrEmpty(inputName) && !string.IsNullOrWhiteSpace(inputName))
				SaveInput();
			else if (input)
				input = null;

			if (bindingAxis && bindingTarget != BindTarget.None)
				EndBindAxis(false);
			else if (bindingAxis)
				bindingAxis = null;

			if (InputsManager.DataChanged)
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Warning", "You have some unsaved data that you might lose! Do you want to save it?", "Save", "Discard"))
					InputsManager.SaveData();
			}

			InputsManager.ForceDataChange();
		}

		#endregion

		#endregion
	}
}
