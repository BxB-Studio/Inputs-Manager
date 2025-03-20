#region Namespaces

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Utilities.Editor;

using Object = UnityEngine.Object;

#endregion

namespace Utilities.Inputs.Editor
{
	/// <summary>
	/// A class that contains the editor for the Inputs Manager.
	/// </summary>
	public class InputsManagerEditor : EditorWindow
	{
		#region Enumerators

		/// <summary>
		/// The target of the binding operation, indicating which input binding is being configured.
		/// </summary>
		private enum BindTarget { None, Positive, Negative, GamepadPositive, GamepadNegative }
		
		/// <summary>
		/// Gets all available keyboard keys for input binding, filtering out numeric values.
		/// </summary>
		private static string[] Keys
		{
			get
			{
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

				return keys;
			}
		}
		
		/// <summary>
		/// Cached array of keyboard key names for the input binding UI.
		/// </summary>
		private static string[] keys;
		
		/// <summary>
		/// Gets all available gamepad bindings for input configuration, filtering out numeric values.
		/// </summary>
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
		
		/// <summary>
		/// Cached array of gamepad binding names for the input binding UI.
		/// </summary>
		private static string[] gamepadBindings;

		#endregion

		#region Variables

		#region Properties

		/// <summary>
		/// Indicates whether an input is currently being edited in the editor window.
		/// Used to control UI state and prevent conflicting operations.
		/// </summary>
		public static bool EditingInput { get; private set; }
		
		/// <summary>
		/// Gets the full file path of the old data asset if it exists.
		/// Returns an empty string if the asset doesn't exist.
		/// </summary>
		private static string OldDataAssetFullPath => OldDataAssetExists(out Object dataAsset) ? AssetDatabase.GetAssetPath(dataAsset) : string.Empty;

		#endregion

		#region Fields

		/// <summary>
		/// Reference to the current instance of the InputsManagerEditor window.
		/// </summary>
		private static InputsManagerEditor editorInstance;
		
		/// <summary>
		/// The input axis currently being configured in the binding process.
		/// </summary>
		private static InputAxis bindingAxis;
		
		/// <summary>
		/// The input currently being edited in the editor.
		/// </summary>
		private static Input input;
		
		/// <summary>
		/// The current binding target (positive/negative, keyboard/gamepad) being configured.
		/// </summary>
		private static BindTarget bindingTarget;
		
		/// <summary>
		/// The event captured during key binding to detect keyboard input.
		/// </summary>
		private static Event bindingEvent;
		
		/// <summary>
		/// The gamepad binding detected during the binding process.
		/// </summary>
		private static GamepadBinding boundGamepadBind;
		
		/// <summary>
		/// EditorPrefs key used to track if the InputsManager has been initialized.
		/// </summary>
		private readonly static string initializerKey = "InputsManager_Init";
		
		/// <summary>
		/// The name of the input currently being edited.
		/// </summary>
		private static string inputName;
		
		/// <summary>
		/// Flag indicating if a new input is being added.
		/// </summary>
		private static bool addingInput;
		
		/// <summary>
		/// Flag indicating if inputs are being sorted in the editor.
		/// </summary>
		private static bool sortingInputs;
		
		/// <summary>
		/// Flag indicating if a binding has been detected during the binding process.
		/// </summary>
		private static bool hasBind;
		
		/// <summary>
		/// Flag indicating if a shift key binding has been detected.
		/// </summary>
		private static bool hasShiftBind;
		
		/// <summary>
		/// Flag indicating if the settings panel is currently open.
		/// </summary>
		private static bool settings;
		
		/// <summary>
		/// Flag indicating if the export panel is currently open.
		/// </summary>
		private static bool export;
		
		/// <summary>
		/// Flag indicating if all inputs should be exported.
		/// </summary>
		private static bool exportAll = true;
		
		/// <summary>
		/// Array of flags indicating which inputs should be exported.
		/// </summary>
		private static bool[] exportInputs;
		
		/// <summary>
		/// The key code of the binding currently being configured.
		/// </summary>
		private static int bindingKey;
		
		/// <summary>
		/// Indicates whether import data is available and valid for processing.
		/// </summary>
		private static bool Import
		{
			get
			{
				return !importJson.IsNullOrEmpty() && !importJson.IsNullOrWhiteSpace() && importInputs != null;
			}
		}
		
		/// <summary>
		/// Flag indicating if all inputs should be imported.
		/// </summary>
		private static bool importAll = true;
		
		/// <summary>
		/// Array of flags indicating which inputs should be imported.
		/// </summary>
		private static bool[] importInputsSelection;
		
		/// <summary>
		/// The JSON string containing input data to be imported.
		/// </summary>
		private static string importJson;
		
		/// <summary>
		/// Array of inputs parsed from the import JSON.
		/// </summary>
		private static Input[] importInputs;
		
		/// <summary>
		/// Flag indicating if imported inputs should be added to existing inputs rather than replacing them.
		/// </summary>
		private static bool importAdditive;
		
		/// <summary>
		/// Flag indicating if imported inputs should override existing inputs with the same name.
		/// </summary>
		private static bool importOverride;

		/// <summary>
		/// The scroll position of the editor window's scroll view.
		/// </summary>
		private Vector2 scroll;

		#endregion

		#endregion

		#region Methods

		#region Menu Items

		/// <summary>
		/// Opens the Inputs Manager editor window.
		/// Creates a new Inputs Manager if it doesn't exist yet.
		/// </summary>
		[MenuItem("Tools/Utilities/Inputs Manager/Edit Settings...", false, 1)]
		public static void OpenWindow()
		{
			if (!PlayerPrefs.HasKey(initializerKey))
			{
				EditorUtility.DisplayDialog("Inputs Manager: Welcome!", "Hey! Thank you for using the Inputs Manager, we are looking forward to improve it based on your honourable reviews and reports in case of any problems.", "Okay");
				CreateInputsManager();

				return;
			}

			float minWindowWidth = 360f;

			editorInstance = GetWindow<InputsManagerEditor>(false, "Inputs Manager", true);
			editorInstance.minSize = new Vector2(minWindowWidth, 512f);
		}
		
		/// <summary>
		/// Opens the Inputs Manager window and immediately begins editing the specified input.
		/// </summary>
		/// <param name="input">The input to edit in the window.</param>
		public static void OpenWindow(Input input)
		{
			OpenWindow();
			EditInput(input);
		}
		
		/// <summary>
		/// Opens the Inputs Manager window.
		/// </summary>
		[Obsolete("Use `OpenWindow` instead.")]
		public static void OpenInputsManager()
		{
			if (!PlayerPrefs.HasKey(initializerKey))
			{
				EditorUtility.DisplayDialog("Inputs Manager: Welcome!", "Hey! Thank you for using the Inputs Manager, we are looking forward to improve it based on your honourable reviews and reports in case of any problems!", "Okay");
				CreateInputsManager();

				return;
			}

			float minWindowWidth = 360f;

			editorInstance = GetWindow<InputsManagerEditor>(false, "Inputs Manager", true);
			editorInstance.minSize = new Vector2(minWindowWidth, 512f);
		}
		
		/// <summary>
		/// Opens the Inputs Manager window and immediately begins editing the specified input.
		/// </summary>
		/// <param name="input">The input to edit in the window.</param>
		[Obsolete("Use `OpenWindow(Input)` instead.")]
		public static void OpenInputsManager(Input input)
		{
			OpenWindow(input);
		}
		
		/// <summary>
		/// Resets the Inputs Manager to its original state after confirmation.
		/// Deletes the current data asset and creates a new one.
		/// </summary>
		[MenuItem("Tools/Utilities/Inputs Manager/Reset Settings", false, 2)]
		public static void ResetInputsManager()
		{
			if (!EditorUtility.DisplayDialog("Inputs Manager: Warning", "Are you sure of resetting the Inputs Manager to it's original state?", "Yes I'm sure", "No"))
				return;

			string dataPath = Path.Combine(Application.dataPath, "Resources", $"{InputsManager.DataAssetPath}.bytes");
			DataSerializationUtility<InputsManagerData> data = new DataSerializationUtility<InputsManagerData>(dataPath, false);

			if (!data.Delete())
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Internal Error", "Unable to delete the current `InputsManager` asset in order to create a new one!", "Report Error...", "Cancel"))
					ReportError();

				return;
			}

			InputsManager.RemoveAll();
			CreateInputsManager();
		}
		
		/// <summary>
		/// Creates a new Inputs Manager data asset and optionally loads a preset.
		/// Opens the editor window after creation.
		/// </summary>
		[MenuItem("Tools/Utilities/Inputs Manager/Create Data Asset", false, 3)]
		public static void CreateInputsManager()
		{
			PlayerPrefs.SetInt(initializerKey, 0);

			if (!RecreateDataFile())
			{
				if (EditorUtility.DisplayDialog("Inputs Manager: Internal Error", "We were unable to create a new Inputs Manager asset!", "Report Error...", "Cancel"))
					ReportError();
			}
			else if (EditorUtility.DisplayDialog("Inputs Manager: Info", "A new Inputs Manager asset has been created! Do you want to load a Json preset file?", "Yes", "No"))
			{
				LoadPreset();
				OpenWindow();
			}
			else
			{
				OpenWindow();

				settings = false;
			}
		}
		
		/// <summary>
		/// Opens the GitHub issues page to report an error with the Inputs Manager.
		/// </summary>
		[MenuItem("Tools/Utilities/Inputs Manager/Report Error...", false, 4)]
		public static void ReportError()
		{
			OpenExternalURL(@"https://github.com/BxB-Studio/Inputs-Manager/issues/new");
		}
		
		/// <summary>
		/// Opens the GitHub repository page for the Inputs Manager.
		/// </summary>
		[MenuItem("Tools/Utilities/Inputs Manager/About...", false, 5)]
		public static void About()
		{
			OpenExternalURL(@"https://github.com/BxB-Studio/Inputs-Manager");
		}
		
		/// <summary>
		/// Validates the "Create Data Asset" menu item.
		/// Returns true if the Inputs Manager data asset doesn't exist yet.
		/// </summary>
		/// <returns>True if the Inputs Manager can be created, false otherwise.</returns>
		[MenuItem("Tools/Utilities/Inputs Manager/Create Data Asset", true)]
		protected static bool CheckCreateInputsManager()
		{
			return !CheckResetInputsManager();
		}
		
		/// <summary>
		/// Validates the "Reset Settings" menu item.
		/// Returns true if the Inputs Manager data asset exists and can be reset.
		/// </summary>
		/// <returns>True if the Inputs Manager can be reset, false otherwise.</returns>
		[MenuItem("Tools/Utilities/Inputs Manager/Reset Settings", true)]
		protected static bool CheckResetInputsManager()
		{
			return InputsManager.DataAssetExists;
		}
		
		/// <summary>
		/// Opens an external URL after displaying a confirmation dialog.
		/// </summary>
		/// <param name="url">The URL to open in the default web browser.</param>
		private static void OpenExternalURL(string url)
		{
			if (EditorUtility.DisplayDialog("Inputs Manager: Info", "You're about to visit an external link. Do you want to proceed?", "Yes", "No"))
				Application.OpenURL(url);
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Closes the Inputs Manager editor window if it's open.
		/// Finds any existing instances and closes them.
		/// </summary>
		public static void CloseWindow()
		{
			var instance = Resources.FindObjectsOfTypeAll<InputsManagerEditor>().FirstOrDefault();

			if (instance == null)
				return;

			instance.Close();
		}
		
		/// <summary>
		/// Checks if the old data asset exists at the legacy location.
		/// </summary>
		/// <param name="dataAsset">Output parameter that will contain the data asset if found.</param>
		/// <returns>True if the old data asset exists, false otherwise.</returns>
		private static bool OldDataAssetExists(out Object dataAsset)
		{
			dataAsset = Resources.Load($"Assets{Path.DirectorySeparatorChar}InputsManager_Data");

			return dataAsset;
		}
		
		/// <summary>
		/// Recreates the Inputs Manager data file.
		/// Saves the current data and refreshes the AssetDatabase.
		/// </summary>
		/// <returns>True if the data file was successfully recreated, false otherwise.</returns>
		private static bool RecreateDataFile()
		{
			bool process = InputsManager.EditorSaveData();

			AssetDatabase.Refresh();

			return process;
		}
		
		/// <summary>
		/// Loads a preset from a JSON file.
		/// Prompts the user to select a file and processes it.
		/// </summary>
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
		
		/// <summary>
		/// Imports a JSON file from a user-selected path.
		/// Shows a progress bar while reading the file.
		/// </summary>
		/// <returns>True if the file was successfully imported, false otherwise.</returns>
		private static bool ImportJsonFromPath()
		{
			string path = EditorUtility.OpenFilePanel("Choose a preset file", string.Empty, "json");

			importJson = "";

			if (path.IsNullOrEmpty() || !File.Exists(path))
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
		
		/// <summary>
		/// Parses the imported JSON string into an array of Input objects.
		/// Initializes the import selection array with all items selected.
		/// </summary>
		/// <returns>True if the JSON was successfully parsed into inputs, false otherwise.</returns>
		private static bool InputsFromJson()
		{
			importInputs = JsonUtility.FromJson<Utility.JsonArray<Input>>(importJson).ToArray();

			if (importInputs == null)
				return false;

			importInputsSelection = new bool[importInputs.Length];

			for (int i = 0; i < importInputsSelection.Length; i++)
				importInputsSelection[i] = true;

			return true;
		}
		
		/// <summary>
		/// Moves an input from one index to another in the InputsManager.
		/// Used for reordering inputs in the editor.
		/// </summary>
		/// <param name="oldIndex">The current index of the input.</param>
		/// <param name="newIndex">The target index to move the input to.</param>
		private static void MoveInput(int oldIndex, int newIndex)
		{
			Input input = InputsManager.GetInput(oldIndex);

			InputsManager.RemoveInput(oldIndex);
			InputsManager.InsertInput(newIndex, input);
		}
		
		/// <summary>
		/// Begins editing an existing input.
		/// Stores the input name and sets the editing state.
		/// </summary>
		/// <param name="input">The input to edit.</param>
		private static void EditInput(Input input)
		{
			InputsManagerEditor.input = input;
			inputName = input.Name;
			EditingInput = true;
			input.Name = "";
		}
		
		/// <summary>
		/// Saves the changes to the currently edited input.
		/// Restores the input name and exits editing mode.
		/// </summary>
		private static void SaveInput()
		{
			input.Name = inputName;
			EditingInput = false;
			input = null;
		}
		
		/// <summary>
		/// Toggles the new input creation state.
		/// Creates a new input object when entering creation mode.
		/// </summary>
		/// <param name="state">True to enter new input mode, false to exit.</param>
		private static void SwitchNewInput(bool state)
		{
			addingInput = state;

			if (state)
				EditInput(new Input("New input"));
			else
				SaveInput();

			editorInstance.Repaint();
		}
		
		/// <summary>
		/// Creates a duplicate of an existing input and begins editing it.
		/// </summary>
		/// <param name="input">The input to duplicate.</param>
		private static void DuplicateInput(Input input)
		{
			EditInput(new Input(input));
			addingInput = true;
		}
		
		/// <summary>
		/// Begins the binding process for an input axis.
		/// Sets up the binding state to capture user input.
		/// </summary>
		/// <param name="axis">The axis to bind.</param>
		/// <param name="target">The specific binding target (positive/negative, keyboard/gamepad).</param>
		private static void BindAxis(InputAxis axis, BindTarget target)
		{
			bindingAxis = axis;
			bindingTarget = target;
		}
		
		/// <summary>
		/// Completes the binding process for an input axis.
		/// Applies the captured binding to the appropriate axis property.
		/// </summary>
		private static void EndBindAxis()
		{
			bool bindingForGamepad = bindingTarget == BindTarget.GamepadPositive || bindingTarget == BindTarget.GamepadNegative;

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
		}
		
		/// <summary>
		/// Handles input events from gamepads during the binding process.
		/// Detects button presses, stick movements, and trigger activations.
		/// </summary>
		/// <param name="eventPtr">Pointer to the input event data.</param>
		/// <param name="device">The input device that generated the event.</param>
		private static void GamepadBindEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>() || !(device is Gamepad gamepad))
				return;

			if (gamepad.dpad.ReadValueFromEvent(eventPtr, out Vector2 dpad) && dpad != Vector2.zero)
			{
				if (dpad.y > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.DpadUp;
				else if (dpad.x > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.DpadRight;
				else if (dpad.y < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.DpadDown;
				else if (dpad.x < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.DpadLeft;
			}
			else if (gamepad.buttonNorth.ReadValueFromEvent(eventPtr, out float buttonNorth) && buttonNorth > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.ButtonNorth;
			else if (gamepad.buttonEast.ReadValueFromEvent(eventPtr, out float buttonEast) && buttonEast > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.ButtonEast;
			else if (gamepad.buttonSouth.ReadValueFromEvent(eventPtr, out float buttonSouth) && buttonSouth > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.ButtonSouth;
			else if (gamepad.buttonWest.ReadValueFromEvent(eventPtr, out float buttonWest) && buttonWest > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.ButtonWest;
			else if (gamepad.leftStickButton.ReadValueFromEvent(eventPtr, out float leftStickButton) && leftStickButton > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.LeftStickButton;
			else if (gamepad.leftStick.ReadValueFromEvent(eventPtr, out Vector2 leftStick) && leftStick != Vector2.zero)
			{
				if (leftStick.y > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.LeftStickUp;
				else if (leftStick.x > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.LeftStickRight;
				else if (leftStick.y < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.LeftStickDown;
				else if (leftStick.x < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.LeftStickLeft;
			}
			else if (gamepad.rightStickButton.ReadValueFromEvent(eventPtr, out float rightStickButton) && rightStickButton > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.RightStickButton;
			else if (gamepad.rightStick.ReadValueFromEvent(eventPtr, out Vector2 rightStick) && rightStick != Vector2.zero)
			{
				if (rightStick.y > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.RightStickUp;
				else if (rightStick.x > InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.RightStickRight;
				else if (rightStick.y < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.RightStickDown;
				else if (rightStick.x < -InputsManager.GamepadThreshold)
					boundGamepadBind = GamepadBinding.RightStickLeft;
			}
			else if (gamepad.leftShoulder.ReadValueFromEvent(eventPtr, out float leftShoulder) && leftShoulder > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.LeftShoulder;
			else if (gamepad.rightShoulder.ReadValueFromEvent(eventPtr, out float rightShoulder) && rightShoulder > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.RightShoulder;
			else if (gamepad.leftTrigger.ReadValueFromEvent(eventPtr, out float leftTrigger) && leftTrigger > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.LeftTrigger;
			else if (gamepad.rightTrigger.ReadValueFromEvent(eventPtr, out float rightTrigger) && rightTrigger > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.RightTrigger;
			else if (gamepad.startButton.ReadValueFromEvent(eventPtr, out float startButton) && startButton > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.StartButton;
			else if (gamepad.selectButton.ReadValueFromEvent(eventPtr, out float selectButton) && selectButton > InputsManager.GamepadThreshold)
				boundGamepadBind = GamepadBinding.SelectButton;
		}

		#endregion

		#region Editor

		/// <summary>
		/// Renders and handles the editor UI for an input axis.
		/// Allows configuring positive and negative bindings for keyboard or gamepad.
		/// </summary>
		/// <param name="axis">The input axis to edit.</param>
		/// <param name="type">The type of input (Button or Axis).</param>
		/// <param name="mainAxis">The main axis reference for alternative bindings.</param>
		/// <param name="isGamepadEditor">Whether this is editing gamepad bindings or keyboard bindings.</param>
		private static void InputAxisEditor(InputAxis axis, InputType type, InputAxis mainAxis = null, bool isGamepadEditor = false)
		{
			#region Editor Styles

			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = 28f
			};

			#endregion

			if (type == InputType.Axis)
			{
				bool enumDisabled;

				if (isGamepadEditor)
					enumDisabled = axis.GamepadPositive == GamepadBinding.None || axis.GamepadNegative == GamepadBinding.None;
				else
					enumDisabled = axis.Positive == Key.None || axis.Negative == Key.None;

				InputAxisStrongSide newStrongSide = isGamepadEditor ? axis.GamepadStrongSide : axis.StrongSide;

				if (enumDisabled)
					newStrongSide = InputAxisStrongSide.None;

				EditorGUI.BeginDisabledGroup(enumDisabled);

				newStrongSide = (InputAxisStrongSide)EditorGUILayout.EnumPopup(new GUIContent("Strong Side", "The Strong Side indicates which pressed side wins at runtime"), newStrongSide);

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

			newPositive = EditorGUILayout.Popup(type == InputType.Button ? "Button" : "Positive", newPositive, isGamepadEditor ? GamepadBindings : Keys);

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

			if (type == InputType.Axis)
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

			Input positiveBindingUsed = Array.Find(isGamepadEditor ? InputsManager.GamepadBindingUsed(axis.GamepadPositive) : InputsManager.KeyUsed(axis.Positive), @in => @in != input);

			if (positiveBindingUsed)
				EditorGUILayout.HelpBox($"The `{(isGamepadEditor ? axis.GamepadPositive.ToString() : axis.Positive.ToString())}` binding seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.None);

			Input negativeBindingUsed = Array.Find(isGamepadEditor ? InputsManager.GamepadBindingUsed(axis.GamepadNegative) : InputsManager.KeyUsed(axis.Negative), @in => @in != input);

			if (negativeBindingUsed)
				EditorGUILayout.HelpBox($"The `{(isGamepadEditor ? axis.GamepadNegative.ToString() : axis.Negative.ToString())}` binding seems to be selected by another input{(positiveBindingUsed ? " as well" : ". It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame")}.", MessageType.None);
		}
		/// <summary>
		/// Edits an input configuration in the editor.
		/// Displays and handles the UI for configuring all aspects of an input including name, type, 
		/// interpolation, value intervals, keyboard bindings, and gamepad bindings.
		/// Manages the binding process when a user wants to assign a key or gamepad button.
		/// </summary>
		/// <param name="input">The input to edit. If null, the method returns immediately.</param>
		private static void InputEditor(Input input)
		{
			if (!input)
				return;

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

					if (boundGamepadBind != GamepadBinding.None)
					{
						bindingKey = (int)boundGamepadBind;
						boundGamepadBind = (int)GamepadBinding.None;
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
					Input keyUsed = Array.Find(bindingForGamepad ? InputsManager.GamepadBindingUsed((GamepadBinding)bindingKey) : InputsManager.KeyUsed((Key)bindingKey), query => query != input);

					if (keyUsed)
						EditorGUILayout.HelpBox("The current binding seems to be selected by another input. It's alright to use it that way, but it might cause some issues if two inputs are triggered at the same frame.", MessageType.Info);

					if (GUILayout.Button("Save"))
						EndBindAxis();
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
			input.Type = (InputType)EditorGUILayout.EnumPopup("Type", input.Type);

			EditorGUI.EndDisabledGroup();

			input.Interpolation = (InputInterpolation)EditorGUILayout.EnumPopup(new GUIContent("Interpolation", "The interpolation method specifies how the current value of the axis moves towards the target within the interval.\r\n\r\nSmooth: Linear interpolation over time\r\nJump: Same as Smooth with the exception that if an opposite direction is triggered, the value goes instantly to neutral and continue from there. This method won't work if the input type is set to button\r\nInstant: No interpolation"), input.Interpolation);

			EditorGUILayout.LabelField("Value Interval");

			EditorGUI.indentLevel++;

			Vector2 valueInterval = input.ValueInterval;

			switch (input.Type)
			{
				case InputType.Axis:
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Negative == Key.None || input.Invert && input.Main.Positive == Key.None);

					valueInterval.x = Mathf.Clamp(EditorGUILayout.FloatField("Minimum", valueInterval.x), Mathf.NegativeInfinity, valueInterval.y);

					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(!input.Invert && input.Main.Positive == Key.None || input.Invert && input.Main.Negative == Key.None);

					valueInterval.y = Mathf.Clamp(EditorGUILayout.FloatField("Maximum", valueInterval.y), valueInterval.x, Mathf.Infinity);

					EditorGUI.EndDisabledGroup();

					input.ValueInterval = valueInterval;

					break;

				case InputType.Button:

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

			if (!InputsManager.DataAssetExists)
			{
				if (!OldDataAssetExists(out _))
				{
					EditorGUILayout.HelpBox($"We couldn't find the data asset file at the following path \"Resources{Path.DirectorySeparatorChar}{InputsManager.DataAssetPath}\". You can create a new one from `Tools > Utilities > Inputs Manager > Create data asset`", MessageType.Error);

					return;
				}
				else
				{
					string newAssetPath = OldDataAssetFullPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

					newAssetPath = newAssetPath.Replace($"{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}Settings{Path.DirectorySeparatorChar}");

					string newAssetDirectoryPath = Path.GetDirectoryName(newAssetPath);

					if (!Directory.Exists(newAssetDirectoryPath))
						Directory.CreateDirectory(newAssetDirectoryPath);

					AssetDatabase.MoveAsset(OldDataAssetFullPath, newAssetPath);
				}
			}

			if (!InputsManager.DataLoaded)
			{
				AssetDatabase.Refresh();
				InputsManager.LoadData();
			}

			scroll = EditorGUILayout.BeginScrollView(scroll);

			EditorGUILayout.Space();

			#region Editor Styles

			float miniButtonSmallWidth = 16f;
			float miniButtonWidth = 20f;
			float miniButtonWideWidth = 25f;

			GUIStyle unstretchableMiniButtonSmall = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
			};
			GUIStyle unstretchableMiniButton = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
			};
			GUIStyle unstretchableMiniButtonNormal = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false
			};
			GUIStyle unstretchableMiniButtonWide = new GUIStyle(EditorStyles.miniButton)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
			};
			GUIStyle unstretchableMiniButtonLeftSmall = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
			};
			GUIStyle unstretchableMiniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
			};
			GUIStyle unstretchableMiniButtonLeftWide = new GUIStyle(EditorStyles.miniButtonLeft)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
			};
			GUIStyle unstretchableMiniButtonMiddleSmall = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
			};
			GUIStyle unstretchableMiniButtonMiddle = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
			};
			GUIStyle unstretchableMiniButtonMiddleWide = new GUIStyle(EditorStyles.miniButtonMid)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
			};
			GUIStyle unstretchableMiniButtonRightSmall = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonSmallWidth
			};
			GUIStyle unstretchableMiniButtonRight = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWidth
			};
			GUIStyle unstretchableMiniButtonRightWide = new GUIStyle(EditorStyles.miniButtonRight)
			{
				stretchWidth = false,
				fixedWidth = miniButtonWideWidth
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
						string path = EditorUtility.SaveFilePanel("Save preset file", "", exportPath.IsNullOrEmpty() ? "" : Path.GetFileNameWithoutExtension(exportPath), "json");

						if (!path.IsNullOrEmpty())
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

					int exportInputsCount = exportInputs.Where(value => value == true).Count();

					EditorGUI.BeginDisabledGroup(exportPath.IsNullOrEmpty() || exportInputsCount == 0);

					if (GUILayout.Button("Export"))
					{
						Input[] inputs = new Input[exportInputsCount];
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

						Utility.JsonArray<Input> jsonArray = new Utility.JsonArray<Input>(inputs);
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

								if (!processName.IsNullOrEmpty())
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

					importOverride = Utility.NumberToBool(EditorGUILayout.Popup(new GUIContent("Override Mode", "Ignore Existing: The selected inputs will override the existing inputs if their names match\r\nKeep Existing: Some imported inputs are going to be ignored if their names match the existing inputs"), Utility.BoolToNumber(importOverride || forceAdditive), importOverrideModes));

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
					ResetInputsManager();

				if (GUILayout.Button("Load Preset", EditorStyles.miniButtonMid))
					LoadPreset();

				EditorGUI.BeginDisabledGroup(InputsManager.Count < 1);

				if (GUILayout.Button("Save Preset", EditorStyles.miniButtonRight))
				{
					export = true;
					exportInputs = new bool[InputsManager.Count];

					for (int i = 0; i < exportInputs.Length; i++)
						exportInputs[i] = true;
				}

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Behaviour", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				InputSource newInputSourcePriority = (InputSource)EditorGUILayout.EnumPopup(new GUIContent("Source Priority", "This indicates which input source has priority over the others."), InputsManager.InputSourcePriority);

				if (InputsManager.InputSourcePriority != newInputSourcePriority)
					InputsManager.InputSourcePriority = newInputSourcePriority;

				sbyte newDefaultGamepadIndex = (sbyte)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Default Gamepad Index", "The index of the default gamepad used by the player that acts as a keyboard alternative"), InputsManager.DefaultGamepadIndex), 0, 127);

				if (InputsManager.DefaultGamepadIndex != newDefaultGamepadIndex)
					InputsManager.DefaultGamepadIndex = newDefaultGamepadIndex;

				if (newDefaultGamepadIndex >= InputsManager.GamepadsCount)
					EditorGUILayout.HelpBox("The current gamepad index doesn't refer to any existing connected device. This won't cause any errors, but the value will fallback to 0 at runtime when no device is found.", MessageType.Info);
				else
					EditorGUILayout.HelpBox($"Default Gamepad: {InputsManager.GamepadNames[newDefaultGamepadIndex]}", MessageType.None);

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Timing", EditorStyles.miniBoldLabel);

				EditorGUI.indentLevel++;

				float newInterpolationTime = Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Interpolation Time", $"How much time does it take an input to reach it's target. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.InterpolationTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f;

				if (InputsManager.InterpolationTime != newInterpolationTime)
					InputsManager.InterpolationTime = newInterpolationTime;

				float newHoldTriggerTime = Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Trigger", $"How much time does it take an input to be triggered as held. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldTriggerTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f;

				if (InputsManager.HoldTriggerTime != newHoldTriggerTime)
					InputsManager.HoldTriggerTime = newHoldTriggerTime;

				float newHoldWaitTime = Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Hold Wait", $"How much time does it take an input to be triggered as held once more. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.HoldWaitTime * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f;

				if (InputsManager.HoldWaitTime != newHoldWaitTime)
					InputsManager.HoldWaitTime = newHoldWaitTime;

				float newDoublePressTimeout = Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(new GUIContent("Double Press Timeout", $"Double press check time range. Measured in {Utility.FullUnit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)}s ({Utility.Unit(Utility.Units.TimeAccurate, Utility.UnitType.Metric)})"), Utility.NumberToValueWithUnit(InputsManager.DoublePressTimeout * 1000f, Utility.Units.TimeAccurate, Utility.UnitType.Metric, true)), Utility.Units.TimeAccurate, Utility.UnitType.Metric) * .001f;

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

				EditorGUILayout.HelpBox("The Inputs Manager is an open-source asset available on GitHub and is created by BxB Studio.", MessageType.None);

				if (GUILayout.Button("Report Error/Issue..."))
					ReportError();

				if (GUILayout.Button("Visit the GitHub Repository..."))
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
						EndBindAxis();
					else if (addingInput)
						SwitchNewInput(false);
					else if (inputName.IsNullOrEmpty() || inputName.IsNullOrWhiteSpace())
						EditorUtility.DisplayDialog("Inputs Manager: Error", "The input name cannot be empty.", "Okay");
					else if (!EditorApplication.isPlaying && InputsManager.IndexOf(inputName) > -1)
						EditorUtility.DisplayDialog("Inputs Manager: Info", "We didn't save the input name because it matches another one.", "Okay");
					else
						SaveInput();
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
					if (GUILayout.Button($"Add {(input.Name.IsNullOrEmpty() || input.Name.IsNullOrWhiteSpace() ? "New Input" : inputName)}"))
					{
						if (inputName.IsNullOrEmpty() || inputName.IsNullOrWhiteSpace())
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
					sortingInputs = false;

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
				{
					InputsManager.EditorSaveData();
					AssetDatabase.Refresh();
				}

				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Add), unstretchableMiniButtonWide))
				{
					SwitchNewInput(true);

					sortingInputs = false;
					settings = false;
				}

				EditorGUI.BeginDisabledGroup(InputsManager.Count < 2);

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Sort), unstretchableMiniButtonWide))
				{
					sortingInputs = true;
					settings = false;
				}

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Trash), unstretchableMiniButtonWide))
					if (EditorUtility.DisplayDialog("Inputs Manager: Warning", "Are you sure of removing all of the existing inputs?", "Yes", "No"))
						InputsManager.RemoveAll();

				EditorGUI.EndDisabledGroup();
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.Settings), unstretchableMiniButtonWide))
				{
					settings = true;
					sortingInputs = false;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (EditorApplication.isPlaying)
			{
				EditorGUILayout.HelpBox($"Current Input Source: {InputsManager.LastDefaultInputSource}", MessageType.None);
				EditorGUILayout.HelpBox("You can't change or modify some settings in play mode. Keep in mind that any changes on the Inputs Manager editor won't be saved unless using a custom script to override this behaviour!", MessageType.Info);
				Repaint();
			}

			EditorGUILayout.Space();

			if (InputsManager.Count > 0)
				for (int i = 0; i < InputsManager.Count; i++)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.BeginHorizontal();

					if (sortingInputs)
					{
						EditorGUI.BeginDisabledGroup(i == 0);

						if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.CaretUp), unstretchableMiniButtonLeft))
							MoveInput(i, i - 1);

						EditorGUI.EndDisabledGroup();
						EditorGUI.BeginDisabledGroup(i == InputsManager.Count - 1);

						if (GUILayout.Button(new GUIContent(EditorUtilities.Icons.CaretDown), unstretchableMiniButtonRight))
							MoveInput(i, i + 1);

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

			if (input && !inputName.IsNullOrEmpty() && !inputName.IsNullOrWhiteSpace())
				SaveInput();
			else if (input)
				input = null;

			if (bindingAxis && bindingTarget != BindTarget.None)
				EndBindAxis();
			else if (bindingAxis)
				bindingAxis = null;

			if (InputsManager.DataChanged && EditorUtility.DisplayDialog("Inputs Manager: Warning", "You have some unsaved data that you might lose! Do you want to save it?", "Save", "Discard"))
			{
				InputsManager.EditorSaveData();
				AssetDatabase.Refresh();
			}

			InputsManager.ForceDataChange();
		}

		#endregion

		#endregion
	}
}
