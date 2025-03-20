#define INPUTS_MANAGER

#region Namespaces

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.EnhancedTouch;
using Utilities.Inputs.Components;
using Utilities.Inputs.Jobs;

#endregion

namespace Utilities.Inputs
{
	#region Enumerators

	public enum InputType { Axis, Button }
	public enum InputAxisType { Main, Alt }
	public enum InputAxisSide { Positive, Negative }
	public enum InputAxisStrongSide { None, Positive, Negative, FirstPressing }
	public enum InputInterpolation { Instant = 2, Jump = 1, Smooth = 0 }
	public enum InputSource { Keyboard, Gamepad }
	public enum MouseButton { Left, Right, Middle, Back, Forward }
	public enum GamepadBinding
	{
		None = 0,
		DpadUp = 1,
		DpadDown = 2,
		DpadLeft = 3,
		DpadRight = 4,
		ButtonNorth = 5,
		ButtonEast = 6,
		ButtonSouth = 7,
		ButtonWest = 8,
		LeftStickButton = 9,
		LeftStickUp = 10,
		LeftStickRight = 11,
		LeftStickDown = 12,
		LeftStickLeft = 13,
		RightStickButton = 14,
		RightStickUp = 15,
		RightStickRight = 16,
		RightStickDown = 17,
		RightStickLeft = 18,
		LeftShoulder = 19,
		RightShoulder = 20,
		LeftTrigger = 21,
		RightTrigger = 22,
		StartButton = 23,
		SelectButton = 24,
	}

	#endregion

	#region Modules

	public static class InputsManager
	{
		#region Variables

		public static readonly string DataAssetPath = $"Settings{Path.DirectorySeparatorChar}InputsManager_Data";
#if UNITY_EDITOR
		[Obsolete("Use `EditorDataAssetFullPath` instead.")]
		public static string DataAssetFullPath => EditorDataAssetFullPath;
		public static string EditorDataAssetFullPath => DataAssetExists ? UnityEditor.AssetDatabase.GetAssetPath(Resources.Load(DataAssetPath)) : $"{Application.dataPath}/Resources/{DataAssetPath}.bytes";
#endif
		public static InputSource LastDefaultInputSource
		{
			get
			{
				return lastDefaultInputSource;
			}
		}
		public static InputSource InputSourcePriority
		{
			get
			{
				if (!Application.isPlaying && (int)inputSourcePriority < -1)
					LoadData();

				return inputSourcePriority;
			}
			set
			{
				if (inputSourcePriority == value)
					return;

				inputSourcePriority = value;

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static float InterpolationTime
		{
			get
			{
				if (!Application.isPlaying && interpolationTime < 0f)
					LoadData();

				return interpolationTime;
			}
			set
			{
				if (interpolationTime == value)
					return;

				interpolationTime = math.max(value, 0f);

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static float HoldTriggerTime
		{
			get
			{
				if (!Application.isPlaying && holdTriggerTime < 0f)
					LoadData();

				return holdTriggerTime;
			}
			set
			{
				if (holdTriggerTime == value)
					return;

				holdTriggerTime = math.max(value, 0f);

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static float HoldWaitTime
		{
			get
			{
				if (!Application.isPlaying && holdWaitTime < 0f)
					LoadData();

				return holdWaitTime;
			}
			set
			{
				if (holdWaitTime == value)
					return;

				holdWaitTime = math.max(value, 0f);

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static float DoublePressTimeout
		{
			get
			{
				if (!Application.isPlaying && doublePressTimeout < 0f)
					LoadData();

				return doublePressTimeout;
			}
			set
			{
				if (doublePressTimeout == value)
					return;

				doublePressTimeout = math.max(value, 0f);

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static float GamepadThreshold
		{
			get
			{
				if (!Application.isPlaying && gamepadThreshold < 0f)
					LoadData();

				return gamepadThreshold;
			}
			set
			{
				if (Application.isPlaying || gamepadThreshold == value)
					return;

				gamepadThreshold = Mathf.Clamp01(value);

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static sbyte DefaultGamepadIndexFallback
		{
			get
			{
				return defaultGamepadIndex > -1 && defaultGamepadIndex < gamepadsCount ? defaultGamepadIndex : default;
			}
		}
		public static sbyte DefaultGamepadIndex
		{
			get
			{
				if (!Application.isPlaying && defaultGamepadIndex < 0)
					LoadData();

				return defaultGamepadIndex;
			}
			set
			{
				if (Application.isPlaying || defaultGamepadIndex == value)
					return;

				defaultGamepadIndex = value;

#if UNITY_EDITOR
				EditorSaveData();
#endif
			}
		}
		public static bool DataLoaded
		{
			get
			{
#if UNITY_EDITOR
				return DataAssetExists && File.GetLastWriteTime(EditorDataAssetFullPath) == dataLastWriteTime;
#else
				return dataLoadedOnBuild;
#endif
			}
		}
		public static bool DataChanged
		{
			get
			{
				if (!Application.isEditor)
					return false;

				return DataLoaded && dataChanged;
			}
			set
			{
				if (!Application.isEditor)
					return;

				dataChanged = value;
			}
		}
		public static bool DataAssetExists
		{
			get
			{
				return Resources.Load(DataAssetPath);
			}
		}
		public static int Count
		{
			get
			{
				return Inputs.Length;
			}
		}
		public static Keyboard Keyboard
		{
			get
			{
				if (keyboard == null || keyboard.deviceId == InputDevice.InvalidDeviceId)
					keyboard = Keyboard.current;

				return keyboard;
			}
		}
		public static Mouse Mouse
		{
			get
			{
				if (mouse == null || mouse.deviceId == InputDevice.InvalidDeviceId)
					mouse = Mouse.current;

				return mouse;
			}
		}
		public static Gamepad[] Gamepads
		{
			get
			{
				if (gamepads == null || gamepads.Length != gamepadsCount)
					gamepads = Gamepad.all.ToArray();

				return gamepads;
			}
		}
		public static string[] GamepadNames
		{
			get
			{
				if (gamepadNames == null || gamepadNames.Length != gamepadsCount)
					gamepadNames = Gamepads.Select(gamepad => gamepad.name).ToArray();

				return gamepadNames;
			}
		}
		public static int GamepadsCount
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
					gamepadsCount = Gamepad.all.Count;

#endif
				return gamepadsCount;
			}
		}
		public static UnityEngine.InputSystem.EnhancedTouch.Touch[] Touches
		{
			get
			{
				if (!EnhancedTouchSupport.enabled)
					EnhancedTouchSupport.Enable();

				return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.ToArray();
			}
		}
		public static int TouchCount
		{
			get
			{
				return Touches.Length;
			}
		}

		internal static Input[] Inputs
		{
			get
			{
				if (!Application.isPlaying && !DataLoaded && !editorSavingData)
					LoadData();

				inputs ??= new Input[] { };

				return inputs;
			}
		}

		private static InputSource lastDefaultInputSource;
		private static InputSource inputSourcePriority = (InputSource)(-1);
		private static float interpolationTime = -1f;
		private static float holdTriggerTime = -1f;
		private static float holdWaitTime = -1f;
		private static float doublePressTimeout = -1f;
		private static float gamepadThreshold = -1f;
		private static sbyte defaultGamepadIndex = -1;

		private static NativeArray<InputSourceAccess>[] inputsGamepadAccess;
		private static NativeArray<InputSourceAccess> inputsKeyboardAccess;
		private static NativeArray<InputAccess> inputsAccess;
		private static Dictionary<string, int> inputNamesDictionary;
		private static string[] inputNames;
		private static string[] gamepadNames;
		private static Gamepad[] gamepads;
		private static Input[] inputs;
		private static Keyboard keyboard;
		private static Mouse mouse;
		private static DateTime dataLastWriteTime;
		private static float mouseLeftHoldTimer;
		private static float mouseMiddleHoldTimer;
		private static float mouseRightHoldTimer;
		private static float mouseBackHoldTimer;
		private static float mouseForwardHoldTimer;
		private static float mouseLeftDoublePressTimer;
		private static float mouseMiddleDoublePressTimer;
		private static float mouseRightDoublePressTimer;
		private static float mouseBackDoublePressTimer;
		private static float mouseForwardDoublePressTimer;
		private static bool mouseLeftHeld;
		private static bool mouseMiddleHeld;
		private static bool mouseRightHeld;
		private static bool mouseBackHeld;
		private static bool mouseForwardHeld;
		private static bool mouseLeftDoublePress;
		private static bool mouseMiddleDoublePress;
		private static bool mouseRightDoublePress;
		private static bool mouseBackDoublePress;
		private static bool mouseForwardDoublePress;
		private static bool mouseLeftDoublePressInitiated;
		private static bool mouseMiddleDoublePressInitiated;
		private static bool mouseRightDoublePressInitiated;
		private static bool mouseBackDoublePressInitiated;
		private static bool mouseForwardDoublePressInitiated;
		private static bool mousePressed;
		private static bool dataChanged;
#if !UNITY_EDITOR
		private static bool dataLoadedOnBuild;
#endif
		private static bool editorSavingData;
		private static bool started;
		private static int newGamepadsCount;
		private static int gamepadsCount;
		private static int t_gamepadsCount;

		#endregion

		#region Methods

		#region Inputs

		/// <summary>
		/// Returns the current mouse movement delta as a Vector2.
		/// This represents how much the mouse has moved since the last frame.
		/// Checks if the InputsManager has been started and returns default(Vector2) if the mouse is null.
		/// </summary>
		/// <returns>A Vector2 representing the mouse movement delta. Returns default(Vector2) if mouse is unavailable.</returns>
		public static Vector2 InputMouseMovement()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.delta.value;
		}

		/// <summary>
		/// Returns the current mouse cursor position on screen as a Vector2.
		/// Checks if the InputsManager has been started and returns default(Vector2) if the mouse is null.
		/// The position is in screen coordinates (pixels).
		/// </summary>
		/// <returns>A Vector2 representing the mouse cursor position. Returns default(Vector2) if mouse is unavailable.</returns>
		public static Vector2 InputMousePosition()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.position.value;
		}

		/// <summary>
		/// Returns the current mouse scroll wheel input as a Vector2.
		/// This represents the scrolling amount and direction.
		/// Checks if the InputsManager has been started before accessing mouse data.
		/// </summary>
		/// <returns>A Vector2 representing the scroll wheel movement. Returns default(Vector2) if mouse is unavailable.</returns>
		public static Vector2 InputMouseScrollWheelVector()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.scroll.value;
		}

		/// <summary>
		/// Returns the main axis value for the specified input.
		/// This retrieves the primary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the main axis input, typically in range [-1, 1].</returns>
		public static float InputMainAxisValue(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Returns the main axis value for the input with the specified name.
		/// This retrieves the primary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the main axis input, typically in range [-1, 1].</returns>
		public static float InputMainAxisValue(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Returns the main axis value for the input at the specified index.
		/// This retrieves the primary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the main axis input, typically in range [-1, 1].</returns>
		public static float InputMainAxisValue(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}

		/// <summary>
		/// Returns the alternative axis value for the specified input.
		/// This retrieves the secondary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the alternative axis input, typically in range [-1, 1].</returns>
		public static float InputAltAxisValue(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Returns the alternative axis value for the input with the specified name.
		/// This retrieves the secondary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the alternative axis input, typically in range [-1, 1].</returns>
		public static float InputAltAxisValue(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Returns the alternative axis value for the input at the specified index.
		/// This retrieves the secondary axis value from either keyboard or gamepad input sources.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the alternative axis input, typically in range [-1, 1].</returns>
		public static float InputAltAxisValue(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}

		/// <summary>
		/// Returns the combined input value for the specified input.
		/// This retrieves the overall input value that may combine both main and alternative axes.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the combined input value, typically in range [-1, 1].</returns>
		public static float InputValue(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Returns the combined input value for the input with the specified name.
		/// This retrieves the overall input value that may combine both main and alternative axes.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the combined input value, typically in range [-1, 1].</returns>
		public static float InputValue(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Returns the combined input value for the input at the specified index.
		/// This retrieves the overall input value that may combine both main and alternative axes.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>A float value representing the combined input value, typically in range [-1, 1].</returns>
		public static float InputValue(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}

		/// <summary>
		/// Returns the magnitude of the mouse scroll wheel input.
		/// This provides the overall scroll intensity regardless of direction.
		/// </summary>
		/// <returns>A float value representing the magnitude of the scroll wheel movement.</returns>
		public static float InputMouseScrollWheel()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.scroll.value.magnitude;
		}

		/// <summary>
		/// Returns the horizontal component of the mouse scroll wheel input.
		/// This represents side-to-side scrolling (e.g., with a horizontal scroll wheel or touchpad).
		/// </summary>
		/// <returns>A float value representing the horizontal scroll movement, typically positive for right and negative for left.</returns>
		public static float InputMouseScrollWheelHorizontal()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.scroll.value.x;
		}

		/// <summary>
		/// Returns the vertical component of the mouse scroll wheel input.
		/// This represents the traditional up-down scrolling of a mouse wheel.
		/// </summary>
		/// <returns>A float value representing the vertical scroll movement, typically positive for up and negative for down.</returns>
		public static float InputMouseScrollWheelVertical()
		{
			CheckStarted();

			if (mouse == null)
				return default;

			return mouse.scroll.value.y;
		}

		/// <summary>
		/// Checks if any input device (keyboard, mouse, or gamepad) currently has a button pressed.
		/// This method detects continuous button presses from any input source.
		/// </summary>
		/// <param name="ignoreMouse">When set to true, mouse button presses will be ignored. Default is false.</param>
		/// <returns>True if any button is currently being pressed on any input device (subject to ignoreMouse parameter), otherwise false.</returns>
		public static bool AnyInputPress(bool ignoreMouse = false)
		{
			CheckStarted();

			return keyboard != null && keyboard.anyKey.isPressed || !ignoreMouse && InputMouseAnyButtonPress() || gamepads != null && gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.isPressed));
		}

		/// <summary>
		/// Checks if any input device (keyboard, mouse, or gamepad) has had a button pressed down in the current frame.
		/// This method detects the initial press of any button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="ignoreMouse">When set to true, mouse button presses will be ignored. Default is false.</param>
		/// <returns>True if any button was pressed down in this frame on any input device (subject to ignoreMouse parameter), otherwise false.</returns>
		public static bool AnyInputDown(bool ignoreMouse = false)
		{
			CheckStarted();

			return keyboard != null && keyboard.anyKey.wasPressedThisFrame || !ignoreMouse && InputMouseAnyButtonDown() || gamepads != null && gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasPressedThisFrame));
		}

		/// <summary>
		/// Checks if any input device (keyboard, mouse, or gamepad) has had a button released in the current frame.
		/// This method detects when any previously pressed button is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="ignoreMouse">When set to true, mouse button releases will be ignored. Default is false.</param>
		/// <returns>True if any button was released in this frame on any input device (subject to ignoreMouse parameter), otherwise false.</returns>
		public static bool AnyInputUp(bool ignoreMouse = false)
		{
			CheckStarted();

			return keyboard != null && keyboard.anyKey.wasReleasedThisFrame || !ignoreMouse && InputMouseAnyButtonUp() || gamepads != null && gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasReleasedThisFrame));
		}

		/// <summary>
		/// Checks if the specified input is currently being pressed.
		/// This method detects continuous button presses for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input with the specified name is currently being pressed.
		/// This method detects continuous button presses for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input at the specified index is currently being pressed.
		/// This method detects continuous button presses for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the specified input is currently being pressed.
		/// This method detects continuous presses on the primary axis binding for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputMainAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input with the specified name is currently being pressed.
		/// This method detects continuous presses on the primary axis binding for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputMainAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input at the specified index is currently being pressed.
		/// This method detects continuous presses on the primary axis binding for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputMainAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the specified input is currently being pressed.
		/// This method detects continuous presses on the secondary axis binding for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputAltAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input with the specified name is currently being pressed.
		/// This method detects continuous presses on the secondary axis binding for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputAltAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input at the specified index is currently being pressed.
		/// This method detects continuous presses on the secondary axis binding for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputAltAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the specified input is currently being pressed.
		/// This method detects continuous presses in the positive direction for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPositivePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the positive direction for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPositivePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the positive direction for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputPositivePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the specified input is currently being pressed.
		/// This method detects continuous presses in the negative direction for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputNegativePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the negative direction for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputNegativePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the negative direction for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is currently being pressed, otherwise false.</returns>
		public static bool InputNegativePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		/// <summary>
		/// Checks if the positive direction of the main axis for the specified input is currently being pressed.
		/// This method detects continuous presses in the positive direction for the primary axis binding of the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the positive direction for the primary axis binding of the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the positive direction for the primary axis binding of the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the specified input is currently being pressed.
		/// This method detects continuous presses in the negative direction for the primary axis binding of the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the negative direction for the primary axis binding of the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the negative direction for the primary axis binding of the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the specified input is currently being pressed.
		/// This method detects continuous presses in the positive direction for the secondary axis binding of the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the positive direction for the secondary axis binding of the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the positive direction for the secondary axis binding of the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the specified input is currently being pressed.
		/// This method detects continuous presses in the negative direction for the secondary axis binding of the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisPress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input with the specified name is currently being pressed.
		/// This method detects continuous presses in the negative direction for the secondary axis binding of the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisPress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input at the specified index is currently being pressed.
		/// This method detects continuous presses in the negative direction for the secondary axis binding of the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is currently being pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisPress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		/// <summary>
		/// Checks if the specified input has been pressed down in the current frame.
		/// This method detects the initial press of a button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press of a button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press of a button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the specified input has been pressed down in the current frame.
		/// This method detects the initial press on the primary axis binding for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputMainAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press on the primary axis binding for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputMainAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press on the primary axis binding for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputMainAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the specified input has been pressed down in the current frame.
		/// This method detects the initial press on the secondary axis binding for the given input.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputAltAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press on the secondary axis binding for the given input name.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputAltAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press on the secondary axis binding for the given input index.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputAltAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the primary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the positive direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the specified input has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisDown(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input with the specified name has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisDown(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input at the specified index has been pressed down in the current frame.
		/// This method detects the initial press in the negative direction for the secondary axis binding, triggering only on the first frame.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was pressed down in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisDown(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		/// <summary>
		/// Checks if the specified input has been released in the current frame.
		/// This method detects when a previously pressed input is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was released in this frame, otherwise false.</returns>
		public static bool InputUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed input is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was released in this frame, otherwise false.</returns>
		public static bool InputUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed input is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was released in this frame, otherwise false.</returns>
		public static bool InputUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the specified input has been released in the current frame.
		/// This method detects when a previously pressed main axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputMainAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed main axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputMainAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed main axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputMainAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the specified input has been released in the current frame.
		/// This method detects when a previously pressed alternative axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputAltAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed alternative axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputAltAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed alternative axis binding is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputAltAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the specified input has been released in the current frame.
		/// This method detects when a previously pressed positive direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputPositiveUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed positive direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputPositiveUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed positive direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputPositiveUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the specified input has been released in the current frame.
		/// This method detects when a previously pressed negative direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputNegativeUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed negative direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputNegativeUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed negative direction is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was released in this frame, otherwise false.</returns>
		public static bool InputNegativeUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the specified input has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveMainAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the specified input has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the main axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeMainAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the specified input has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed positive direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputPositiveAltAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the specified input has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisUp(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input with the specified name has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisUp(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input at the specified index has been released in the current frame.
		/// This method detects when a previously pressed negative direction of the alternative axis is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was released in this frame, otherwise false.</returns>
		public static bool InputNegativeAltAxisUp(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		/// <summary>
		/// Checks if the specified input is being held down.
		/// This method detects if an input is continuously being held, returning true for every frame the input remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is being held down, otherwise false.</returns>
		public static bool InputHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input with the specified name is being held down.
		/// This method detects if an input is continuously being held, returning true for every frame the input remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is being held down, otherwise false.</returns>
		public static bool InputHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input at the specified index is being held down.
		/// This method detects if an input is continuously being held, returning true for every frame the input remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input is being held down, otherwise false.</returns>
		public static bool InputHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the specified input is being held down.
		/// This method detects if the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputMainAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input with the specified name is being held down.
		/// This method detects if the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputMainAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input at the specified index is being held down.
		/// This method detects if the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputMainAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the specified input is being held down.
		/// This method detects if the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputAltAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input with the specified name is being held down.
		/// This method detects if the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputAltAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input at the specified index is being held down.
		/// This method detects if the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input is being held down, otherwise false.</returns>
		public static bool InputAltAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the specified input is being held down.
		/// This method detects if the positive direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputPositiveHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input with the specified name is being held down.
		/// This method detects if the positive direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputPositiveHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input at the specified index is being held down.
		/// This method detects if the positive direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputPositiveHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the specified input is being held down.
		/// This method detects if the negative direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputNegativeHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input with the specified name is being held down.
		/// This method detects if the negative direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputNegativeHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input at the specified index is being held down.
		/// This method detects if the negative direction is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input is being held down, otherwise false.</returns>
		public static bool InputNegativeHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the specified input is being held down.
		/// This method detects if the positive direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputPositiveMainAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input with the specified name is being held down.
		/// This method detects if the positive direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputPositiveMainAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input at the specified index is being held down.
		/// This method detects if the positive direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputPositiveMainAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the specified input is being held down.
		/// This method detects if the negative direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputNegativeMainAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input with the specified name is being held down.
		/// This method detects if the negative direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputNegativeMainAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input at the specified index is being held down.
		/// This method detects if the negative direction of the primary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis is being held down, otherwise false.</returns>
		public static bool InputNegativeMainAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the specified input is being held down.
		/// This method detects if the positive direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputPositiveAltAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input with the specified name is being held down.
		/// This method detects if the positive direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputPositiveAltAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input at the specified index is being held down.
		/// This method detects if the positive direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputPositiveAltAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the specified input is being held down.
		/// This method detects if the negative direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputNegativeAltAxisHold(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input with the specified name is being held down.
		/// This method detects if the negative direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputNegativeAltAxisHold(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input at the specified index is being held down.
		/// This method detects if the negative direction of the secondary axis binding is continuously being held, returning true for every frame it remains pressed.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis is being held down, otherwise false.</returns>
		public static bool InputNegativeAltAxisHold(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		/// <summary>
		/// Checks if the specified input has been double-pressed within a short time interval.
		/// This method detects when an input is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was double-pressed, otherwise false.</returns>
		public static bool InputDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when an input is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was double-pressed, otherwise false.</returns>
		public static bool InputDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when an input is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the specified input was double-pressed, otherwise false.</returns>
		public static bool InputDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the specified input has been double-pressed within a short time interval.
		/// This method detects when the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the main axis of the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the main axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the specified input has been double-pressed within a short time interval.
		/// This method detects when the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the alternative axis of the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the alternative axis of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the specified input has been double-pressed within a short time interval.
		/// This method detects when the positive direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputPositiveDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the positive direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputPositiveDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the positive direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputPositiveDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the specified input has been double-pressed within a short time interval.
		/// This method detects when the negative direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputNegativeDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the negative direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputNegativeDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the negative direction is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the specified input was double-pressed, otherwise false.</returns>
		public static bool InputNegativeDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the specified input has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the main axis for the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the specified input has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the main axis for the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the primary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the main axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the specified input has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the positive direction of the alternative axis for the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the positive direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the positive direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputPositiveAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the specified input has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="input">The input enum to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input with the specified name has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="name">The name of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}

		/// <summary>
		/// Checks if the negative direction of the alternative axis for the input at the specified index has been double-pressed within a short time interval.
		/// This method detects when the negative direction of the secondary axis binding is pressed twice in quick succession.
		/// </summary>
		/// <param name="index">The index of the input to check.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check (default is 0).</param>
		/// <returns>True if the negative direction of the alternative axis was double-pressed, otherwise false.</returns>
		public static bool InputNegativeAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			CheckStarted();

			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		/// <summary>
		/// Checks if the specified keyboard key is currently being pressed.
		/// This method detects continuous presses, returning true for every frame the key remains pressed.
		/// </summary>
		/// <param name="key">The keyboard key to check.</param>
		/// <returns>True if the specified key is currently being pressed, otherwise false.</returns>
		public static bool InputKeyPress(Key key)
		{
			CheckStarted();

			return InputUtilities.KeyToKeyControl(key).isPressed;
		}

		/// <summary>
		/// Checks if the specified keyboard key has been pressed down in the current frame.
		/// This method detects the initial press of a key, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="key">The keyboard key to check.</param>
		/// <returns>True if the specified key was pressed down in this frame, otherwise false.</returns>
		public static bool InputKeyDown(Key key)
		{
			CheckStarted();

			return InputUtilities.KeyToKeyControl(key).wasPressedThisFrame;
		}

		/// <summary>
		/// Checks if the specified keyboard key has been released in the current frame.
		/// This method detects when a previously pressed key is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="key">The keyboard key to check.</param>
		/// <returns>True if the specified key was released in this frame, otherwise false.</returns>
		public static bool InputKeyUp(Key key)
		{
			CheckStarted();

			return InputUtilities.KeyToKeyControl(key).wasReleasedThisFrame;
		}

		/// <summary>
		/// Checks if any mouse button is currently being pressed.
		/// This method detects continuous presses for any mouse button, returning true for every frame any button remains pressed.
		/// </summary>
		/// <returns>True if any mouse button is currently being pressed, otherwise false.</returns>
		public static bool InputMouseAnyButtonPress()
		{
			CheckStarted();

			if (mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonPress(i))
					return true;

			return false;
		}

		/// <summary>
		/// Checks if a specific mouse button is currently being pressed.
		/// This method detects continuous presses for the specified button, returning true for every frame the button remains pressed.
		/// </summary>
		/// <param name="type">The index of the mouse button to check (0-4: left, right, middle, back, forward).</param>
		/// <returns>True if the specified mouse button is currently being pressed, otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown when the type parameter is outside the valid range (0-4).</exception>
		public static bool InputMouseButtonPress(int type)
		{
			CheckStarted();

			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (mouse == null)
				return false;

			return type switch
			{
				0 => mouse.leftButton.isPressed,
				1 => mouse.rightButton.isPressed,
				2 => mouse.middleButton.isPressed,
				3 => mouse.backButton.isPressed,
				4 => mouse.forwardButton.isPressed,
				_ => false,
			};
		}

		/// <summary>
		/// Checks if a specific mouse button is currently being pressed.
		/// This method detects continuous presses for the specified button, returning true for every frame the button remains pressed.
		/// </summary>
		/// <param name="type">The mouse button to check (Left, Right, Middle, Back, Forward).</param>
		/// <returns>True if the specified mouse button is currently being pressed, otherwise false.</returns>
		public static bool InputMouseButtonPress(MouseButton type)
		{
			return InputMouseButtonPress((int)type);
		}

		/// <summary>
		/// Checks if any mouse button has been pressed down in the current frame.
		/// This method detects the initial press of any mouse button, triggering only on the first frame of the press.
		/// </summary>
		/// <returns>True if any mouse button was pressed down in this frame, otherwise false.</returns>
		public static bool InputMouseAnyButtonDown()
		{
			CheckStarted();

			if (mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDown(i))
					return true;

			return false;
		}

		/// <summary>
		/// Checks if a specific mouse button has been pressed down in the current frame.
		/// This method detects the initial press of the specified button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="type">The index of the mouse button to check (0-4: left, right, middle, back, forward).</param>
		/// <returns>True if the specified mouse button was pressed down in this frame, otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown when the type parameter is outside the valid range (0-4).</exception>
		public static bool InputMouseButtonDown(int type)
		{
			CheckStarted();

			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (mouse == null)
				return false;

			return type switch
			{
				0 => mouse.leftButton.wasPressedThisFrame,
				1 => mouse.rightButton.wasPressedThisFrame,
				2 => mouse.middleButton.wasPressedThisFrame,
				3 => mouse.backButton.wasPressedThisFrame,
				4 => mouse.forwardButton.wasPressedThisFrame,
				_ => false,
			};
		}

		/// <summary>
		/// Checks if a specific mouse button has been pressed down in the current frame.
		/// This method detects the initial press of the specified button, triggering only on the first frame of the press.
		/// </summary>
		/// <param name="type">The mouse button to check (Left, Right, Middle, Back, Forward).</param>
		/// <returns>True if the specified mouse button was pressed down in this frame, otherwise false.</returns>
		public static bool InputMouseButtonDown(MouseButton type)
		{
			return InputMouseButtonDown((int)type);
		}

		/// <summary>
		/// Checks if any mouse button has been released in the current frame.
		/// This method detects when any previously pressed mouse button is released, triggering only on the frame of release.
		/// </summary>
		/// <returns>True if any mouse button was released in this frame, otherwise false.</returns>
		public static bool InputMouseAnyButtonUp()
		{
			CheckStarted();

			if (mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonUp(i))
					return true;

			return false;
		}

		/// <summary>
		/// Checks if a specific mouse button has been released in the current frame.
		/// This method detects when a previously pressed button is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="type">The index of the mouse button to check (0-4: left, right, middle, back, forward).</param>
		/// <returns>True if the specified mouse button was released in this frame, otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown when the type parameter is outside the valid range (0-4).</exception>
		public static bool InputMouseButtonUp(int type)
		{
			CheckStarted();

			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (mouse == null)
				return false;

			return type switch
			{
				0 => mouse.leftButton.wasReleasedThisFrame,
				1 => mouse.rightButton.wasReleasedThisFrame,
				2 => mouse.middleButton.wasReleasedThisFrame,
				3 => mouse.backButton.wasReleasedThisFrame,
				4 => mouse.forwardButton.wasReleasedThisFrame,
				_ => false,
			};
		}

		/// <summary>
		/// Checks if a specific mouse button has been released in the current frame.
		/// This method detects when a previously pressed button is released, triggering only on the frame of release.
		/// </summary>
		/// <param name="type">The mouse button to check (Left, Right, Middle, Back, Forward).</param>
		/// <returns>True if the specified mouse button was released in this frame, otherwise false.</returns>
		public static bool InputMouseButtonUp(MouseButton type)
		{
			return InputMouseButtonUp((int)type);
		}

		/// <summary>
		/// Checks if any mouse button is being held down.
		/// This method detects if any mouse button is continuously being held, using internal tracking variables.
		/// </summary>
		/// <returns>True if any mouse button is being held down, otherwise false.</returns>
		public static bool InputMouseAnyButtonHold()
		{
			CheckStarted();

			if (mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonHold(i))
					return true;

			return false;
		}

		/// <summary>
		/// Checks if a specific mouse button is being held down.
		/// This method uses internal tracking variables to detect if a button is continuously being held.
		/// </summary>
		/// <param name="type">The index of the mouse button to check (0-4: left, right, middle, back, forward).</param>
		/// <returns>True if the specified mouse button is being held down, otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown when the type parameter is outside the valid range (0-4).</exception>
		public static bool InputMouseButtonHold(int type)
		{
			CheckStarted();

			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (mouse == null)
				return false;

			return type switch
			{
				0 => mouseLeftHeld,
				1 => mouseRightHeld,
				2 => mouseMiddleHeld,
				3 => mouseBackHeld,
				4 => mouseForwardHeld,
				_ => false,
			};
		}

		/// <summary>
		/// Checks if a specific mouse button is being held down.
		/// This method uses internal tracking variables to detect if a button is continuously being held.
		/// </summary>
		/// <param name="type">The mouse button to check (Left, Right, Middle, Back, Forward).</param>
		/// <returns>True if the specified mouse button is being held down, otherwise false.</returns>
		public static bool InputMouseButtonHold(MouseButton type)
		{
			return InputMouseButtonHold((int)type);
		}

		/// <summary>
		/// Checks if any mouse button has been double-pressed within a short time interval.
		/// This method detects when any mouse button is pressed twice in quick succession.
		/// </summary>
		/// <returns>True if any mouse button was double-pressed, otherwise false.</returns>
		public static bool InputMouseAnyButtonDoublePress()
		{
			CheckStarted();

			if (mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDoublePress(i))
					return true;

			return false;
		}

		/// <summary>
		/// Checks if a specific mouse button has been double-pressed within a short time interval.
		/// This method detects when the specified button is pressed twice in quick succession, using internal tracking variables.
		/// </summary>
		/// <param name="type">The index of the mouse button to check (0-4: left, right, middle, back, forward).</param>
		/// <returns>True if the specified mouse button was double-pressed, otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown when the type parameter is outside the valid range (0-4).</exception>
		public static bool InputMouseButtonDoublePress(int type)
		{
			CheckStarted();

			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (mouse == null)
				return false;

			return type switch
			{
				0 => mouseLeftDoublePress,
				1 => mouseRightDoublePress,
				2 => mouseMiddleDoublePress,
				3 => mouseBackDoublePress,
				4 => mouseForwardDoublePress,
				_ => false,
			};
		}

		/// <summary>
		/// Checks if a specific mouse button has been double-pressed within a short time interval.
		/// This method detects when the specified button is pressed twice in quick succession, using internal tracking variables.
		/// </summary>
		/// <param name="type">The mouse button to check (Left, Right, Middle, Back, Forward).</param>
		/// <returns>True if the specified mouse button was double-pressed, otherwise false.</returns>
		public static bool InputMouseButtonDoublePress(MouseButton type)
		{
			return InputMouseButtonDoublePress((int)type);
		}

		/// <summary>
		/// Verifies that the InputsManager has been properly initialized before accessing input values.
		/// This method is called internally by all input-checking methods to ensure the system is ready.
		/// If the system is not started and the application is in Play mode, it throws an InvalidOperationException.
		/// If the application is in Editor mode, it logs a warning instead of throwing an exception.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when the InputsManager has not been started in Play mode.</exception>
		private static void CheckStarted()
		{
			if (started)
				return;
			else if (!Application.isPlaying)
			{
				Debug.LogWarning("The Inputs Manager cannot be used in the Editor. Use `InputsManager.Start()` to initialize the Inputs Manager and `InputsManager.Update()` to refresh value at Runtime.");

				return;
			}

			throw new InvalidOperationException("The Inputs Manager needs to be started before accessing Input values. Use `InputsManager.Start()` to initialize the Inputs Manager and `InputsManager.Update()` to refresh value at Runtime.");
		}

		#endregion

		#region Gamepad Outputs

		/// <summary>
		/// Activates vibration on a specific gamepad by setting the intensity of its motors.
		/// This method directly controls the haptic feedback of the specified gamepad device.
		/// </summary>
		/// <param name="leftMotorIntensity">The intensity of the left motor vibration, ranging from 0.0 (no vibration) to 1.0 (maximum vibration).</param>
		/// <param name="rightMotorIntensity">The intensity of the right motor vibration, ranging from 0.0 (no vibration) to 1.0 (maximum vibration).</param>
		/// <param name="gamepad">The specific gamepad device to apply vibration to.</param>
		public static void GamepadVibration(float leftMotorIntensity, float rightMotorIntensity, Gamepad gamepad)
		{
			if (gamepad == null)
				return;

			gamepad.SetMotorSpeeds(leftMotorIntensity, rightMotorIntensity);
		}

		/// <summary>
		/// Activates vibration on a gamepad at the specified index by setting the intensity of its motors.
		/// This method provides a convenient way to control haptic feedback using the gamepad's index rather than the device reference.
		/// </summary>
		/// <param name="leftMotorIntensity">The intensity of the left motor vibration, ranging from 0.0 (no vibration) to 1.0 (maximum vibration).</param>
		/// <param name="rightMotorIntensity">The intensity of the right motor vibration, ranging from 0.0 (no vibration) to 1.0 (maximum vibration).</param>
		/// <param name="gamepadIndex">The index of the gamepad to apply vibration to (default is 0). If the index is invalid, the method returns without effect.</param>
		public static void GamepadVibration(float leftMotorIntensity, float rightMotorIntensity, int gamepadIndex = 0)
		{
			if (gamepadIndex < 0 || gamepadIndex >= gamepadsCount)
				return;

			GamepadVibration(leftMotorIntensity, rightMotorIntensity, gamepads[gamepadIndex]);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the InputsManager system for use during runtime.
		/// This method must be called before any input detection functions can be used.
		/// It sets up the necessary native arrays for input tracking, loads input configuration data,
		/// and prepares both keyboard and gamepad input handling.
		/// </summary>
		/// <exception cref="Exception">Thrown when attempting to call this method outside of Play mode.</exception>
		public static void Start()
		{
			if (!Application.isPlaying)
				throw new Exception("The `Start` method can only be called during Play mode.");

			newGamepadsCount = gamepadsCount = Gamepad.all.Count;
			lastDefaultInputSource = gamepadsCount > 0 ? inputSourcePriority : InputSource.Keyboard;

			LoadData();

			if (inputs == null || inputs.Length < 1)
				return;

			if (!inputsAccess.IsCreated)
				inputsAccess = new NativeArray<InputAccess>(inputs.Length, Allocator.Persistent);

			if (!inputsKeyboardAccess.IsCreated)
				inputsKeyboardAccess = new NativeArray<InputSourceAccess>(inputs.Length, Allocator.Persistent);

			if (inputsGamepadAccess == null || inputsGamepadAccess.Length != inputs.Length)
			{
				if (inputsGamepadAccess != null && inputsGamepadAccess.Length > 0)
					for (int i = 0; i < inputsGamepadAccess.Length; i++)
						if (inputsGamepadAccess[i].IsCreated)
							inputsGamepadAccess[i].Dispose();

				inputsGamepadAccess = new NativeArray<InputSourceAccess>[inputs.Length];

				for (int i = 0; i < inputs.Length; i++)
					inputsGamepadAccess[i] = new NativeArray<InputSourceAccess>(gamepadsCount, Allocator.Persistent);
			}

			for (int i = 0; i < inputs.Length; i++)
			{
				inputsAccess[i] = new InputAccess(inputs[i]);
				inputsKeyboardAccess[i] = new InputSourceAccess(inputs[i], InputSource.Keyboard);

				for (int j = 0; j < gamepadsCount; j++)
					inputsGamepadAccess[i][j] = new InputSourceAccess(inputs[i], InputSource.Gamepad);
			}

			started = true;
		}
		/// <summary>
		/// Updates the InputsManager system for the current frame.
		/// This method must be called every frame to process input states and detect changes.
		/// It handles gamepad connection/disconnection, updates mouse state, refreshes input controls,
		/// and maintains the native arrays used for input tracking across different input sources.
		/// </summary>
		/// <exception cref="Exception">Thrown when attempting to call this method outside of Play mode.</exception>
		public static void Update()
		{
			if (!Application.isPlaying)
				throw new Exception("The `Update` method can only be called during Play mode.");

			CheckStarted();

			newGamepadsCount = Gamepad.all.Count;

			if (gamepadsCount != newGamepadsCount)
			{
				lastDefaultInputSource = newGamepadsCount > 0 ? inputSourcePriority : InputSource.Keyboard;
				gamepadsCount = newGamepadsCount;
				gamepads = Gamepads;
			}

			if (Mouse != null)
				UpdateMouse();

			if (inputs == null || inputs.Length < 1 || Keyboard == null && Gamepads.Length < 1)
				return;

			if (gamepadsCount > 0)
				if (Gamepad.current != gamepads[DefaultGamepadIndexFallback])
					gamepads[DefaultGamepadIndexFallback].MakeCurrent();

			for (int i = 0; i < inputs.Length; i++)
			{
				if (dataChanged)
				{
					inputsAccess[i] = new InputAccess(inputs[i]);
					inputsKeyboardAccess[i] = new InputSourceAccess(inputs[i], InputSource.Keyboard);

					for (int j = 0; j < gamepadsCount; j++)
						inputsGamepadAccess[i][j] = new InputSourceAccess(inputs[i], InputSource.Gamepad);
				}
				else if (gamepadsCount != t_gamepadsCount)
				{
					if (inputsGamepadAccess[i].IsCreated)
						inputsGamepadAccess[i].Dispose();

					inputsGamepadAccess[i] = new NativeArray<InputSourceAccess>(gamepadsCount, Allocator.Persistent);

					for (int j = 0; j < gamepadsCount; j++)
						inputsGamepadAccess[i][j] = new InputSourceAccess(inputs[i], InputSource.Gamepad);
				}

				inputsKeyboardAccess[i] = inputsKeyboardAccess[i].UpdateControls(inputs[i]);

				for (int j = 0; j < gamepadsCount; j++)
					inputsGamepadAccess[i][j] = inputsGamepadAccess[i][j].UpdateControls(inputs[i], j);
			}

			t_gamepadsCount = gamepadsCount;
			dataChanged = false;

			NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(inputs.Length + 2, Allocator.Temp);
			InputKeyboardJob inputKeyboardJob = new InputKeyboardJob()
			{
				sourceAccess = inputsKeyboardAccess,
				access = inputsAccess,
				interpolationTime = interpolationTime,
				holdTriggerTime = holdTriggerTime,
				holdWaitTime = holdWaitTime,
				doublePressTimeout = doublePressTimeout,
				deltaTime = Utility.DeltaTime
			};

			jobHandles.Add(inputKeyboardJob.ScheduleParallel(inputs.Length, 1, default));

			if (gamepadsCount > 0)
				for (int i = 0; i < inputs.Length; i++)
				{
					InputGamepadJob inputGamepadJob = new InputGamepadJob()
					{
						sourceAccess = inputsGamepadAccess[i],
						input = inputsAccess[i],
						interpolationTime = interpolationTime,
						holdTriggerTime = holdTriggerTime,
						holdWaitTime = holdWaitTime,
						doublePressTimeout = doublePressTimeout,
						deltaTime = Utility.DeltaTime
					};

					jobHandles.Add(inputGamepadJob.ScheduleParallel(gamepadsCount, 1, default));
				}

			NativeArray<JobHandle> jobHandlesArray = jobHandles.ToArray(Allocator.Temp);
			
			JobHandle.CompleteAll(jobHandlesArray);
			jobHandlesArray.Dispose();
			jobHandles.Dispose();

			if (gamepadsCount > 0)
				for (int i = 0; i < inputs.Length; i++)
				{
					bool gamepadUsed = DefaultGamepadIndexFallback < inputsGamepadAccess[i].Length && inputsGamepadAccess[i][DefaultGamepadIndexFallback].mainValue != 0f;
					bool gamepadPrioritized = inputSourcePriority == InputSource.Gamepad;
					bool keyboardUsed = inputsKeyboardAccess[i].mainValue != 0f;

					if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed)
						lastDefaultInputSource = InputSource.Gamepad;
					else if (keyboardUsed)
						lastDefaultInputSource = InputSource.Keyboard;
				}
		}
		/// <summary>
		/// Releases all allocated resources used by the InputsManager.
		/// This method disposes of all native collections including input access arrays for keyboard and gamepad inputs,
		/// cleans up any allocated memory, and resets the system to an uninitialized state.
		/// Call this method when shutting down the input system or when the application is closing to prevent memory leaks.
		/// </summary>
		public static void Dispose()
		{
			if (inputsAccess.IsCreated)
				inputsAccess.Dispose();

			if (inputsKeyboardAccess.IsCreated)
				inputsKeyboardAccess.Dispose();

			if (inputsGamepadAccess != null && inputsGamepadAccess.Length > 0)
				for (int i = 0; i < inputsGamepadAccess.Length; i++)
					if (inputsGamepadAccess[i].IsCreated)
						inputsGamepadAccess[i].Dispose();

			inputsGamepadAccess = null;
			started = false;
		}

		/// <summary>
		/// Updates the state of mouse input for the current frame.
		/// This method tracks and processes mouse button states including:
		/// - Left and middle mouse button hold detection with timing thresholds
		/// - Double-click detection for left and middle mouse buttons
		/// - Management of timers for hold and double-press functionality
		/// The method maintains internal state variables to properly detect these input patterns
		/// and is called each frame during the InputsManager update cycle.
		/// </summary>
		private static void UpdateMouse()
		{
			mouseLeftHeld = false;

			if (InputMouseButtonPress(MouseButton.Left))
			{
				mouseLeftHoldTimer -= Utility.DeltaTime;
				mouseLeftHeld = mouseLeftHoldTimer <= 0f;

				if (mouseLeftHeld)
					mouseLeftHoldTimer = HoldWaitTime;
			}
			else if (mouseLeftHoldTimer != HoldTriggerTime)
				mouseLeftHoldTimer = HoldTriggerTime;

			mouseLeftDoublePressTimer += mouseLeftDoublePressTimer > 0f ? -Utility.DeltaTime : mouseLeftDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Left);

			if (mousePressed)
				mouseLeftDoublePressTimer = DoublePressTimeout;

			mouseLeftDoublePress = mouseLeftDoublePressInitiated && mousePressed;

			if (mousePressed && mouseLeftDoublePressTimer > 0f)
				mouseLeftDoublePressInitiated = true;

			if (mouseLeftDoublePressTimer <= 0f)
				mouseLeftDoublePressInitiated = false;

			mouseMiddleHeld = false;

			if (InputMouseButtonPress(MouseButton.Middle))
			{
				mouseMiddleHoldTimer -= Utility.DeltaTime;
				mouseMiddleHeld = mouseMiddleHoldTimer <= 0f;

				if (mouseMiddleHeld)
					mouseMiddleHoldTimer = HoldWaitTime;
			}
			else if (mouseMiddleHoldTimer != HoldTriggerTime)
				mouseMiddleHoldTimer = HoldTriggerTime;

			mouseMiddleDoublePressTimer += mouseMiddleDoublePressTimer > 0f ? -Utility.DeltaTime : mouseMiddleDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Middle);

			if (mousePressed)
				mouseMiddleDoublePressTimer = DoublePressTimeout;

			mouseMiddleDoublePress = mouseMiddleDoublePressInitiated && mousePressed;

			if (mousePressed && mouseMiddleDoublePressTimer > 0f)
				mouseMiddleDoublePressInitiated = true;

			if (mouseMiddleDoublePressTimer <= 0f)
				mouseMiddleDoublePressInitiated = false;

			mouseRightHeld = false;

			if (InputMouseButtonPress(MouseButton.Right))
			{
				mouseRightHoldTimer -= Utility.DeltaTime;
				mouseRightHeld = mouseRightHoldTimer <= 0f;

				if (mouseRightHeld)
					mouseRightHoldTimer = HoldWaitTime;
			}
			else if (mouseRightHoldTimer != HoldTriggerTime)
				mouseRightHoldTimer = HoldTriggerTime;

			mouseRightDoublePressTimer += mouseRightDoublePressTimer > 0f ? -Utility.DeltaTime : mouseRightDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Right);

			if (mousePressed)
				mouseRightDoublePressTimer = DoublePressTimeout;

			mouseRightDoublePress = mouseRightDoublePressInitiated && mousePressed;

			if (mousePressed && mouseRightDoublePressTimer > 0f)
				mouseRightDoublePressInitiated = true;

			if (mouseRightDoublePressTimer <= 0f)
				mouseRightDoublePressInitiated = false;

			mouseBackHeld = false;

			if (InputMouseButtonPress(MouseButton.Back))
			{
				mouseBackHoldTimer -= Utility.DeltaTime;
				mouseBackHeld = mouseBackHoldTimer <= 0f;

				if (mouseBackHeld)
					mouseBackHoldTimer = HoldWaitTime;
			}
			else if (mouseBackHoldTimer != HoldTriggerTime)
				mouseBackHoldTimer = HoldTriggerTime;

			mouseBackDoublePressTimer += mouseBackDoublePressTimer > 0f ? -Utility.DeltaTime : mouseBackDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Back);

			if (mousePressed)
				mouseBackDoublePressTimer = DoublePressTimeout;

			mouseBackDoublePress = mouseBackDoublePressInitiated && mousePressed;

			if (mousePressed && mouseBackDoublePressTimer > 0f)
				mouseBackDoublePressInitiated = true;

			if (mouseBackDoublePressTimer <= 0f)
				mouseBackDoublePressInitiated = false;

			mouseForwardHeld = false;

			if (InputMouseButtonPress(MouseButton.Forward))
			{
				mouseForwardHoldTimer -= Utility.DeltaTime;
				mouseForwardHeld = mouseForwardHoldTimer <= 0f;

				if (mouseForwardHeld)
					mouseForwardHoldTimer = HoldWaitTime;
			}
			else if (mouseForwardHoldTimer != HoldTriggerTime)
				mouseForwardHoldTimer = HoldTriggerTime;

			mouseForwardDoublePressTimer += mouseForwardDoublePressTimer > 0f ? -Utility.DeltaTime : mouseForwardDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Forward);

			if (mousePressed)
				mouseForwardDoublePressTimer = DoublePressTimeout;

			mouseForwardDoublePress = mouseForwardDoublePressInitiated && mousePressed;

			if (mousePressed && mouseForwardDoublePressTimer > 0f)
				mouseForwardDoublePressInitiated = true;

			if (mouseForwardDoublePressTimer <= 0f)
				mouseForwardDoublePressInitiated = false;
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Finds all inputs that use a specific keyboard key.
		/// </summary>
		/// <param name="key">The keyboard key to check for.</param>
		/// <returns>An array of Input objects that use the specified key in any binding position (main positive/negative or alt positive/negative).</returns>
		public static Input[] KeyUsed(Key key)
		{
			if (key == Key.None)
				return new Input[] { };

			List<Input> usedInputs = new List<Input>();

			for (int i = 0; i < Count; i++)
				if (inputs[i].Main.Positive == key || inputs[i].Main.Negative == key || inputs[i].Alt.Positive == key || inputs[i].Alt.Negative == key)
					usedInputs.Add(Inputs[i]);

			return usedInputs.ToArray();
		}

		/// <summary>
		/// Finds all inputs that use a specific gamepad binding.
		/// </summary>
		/// <param name="binding">The gamepad binding to check for.</param>
		/// <returns>An array of Input objects that use the specified gamepad binding in any position (main positive/negative or alt positive/negative).</returns>
		public static Input[] GamepadBindingUsed(GamepadBinding binding)
		{
			if (binding == GamepadBinding.None)
				return new Input[] { };

			List<Input> usedInputs = new List<Input>();

			for (int i = 0; i < Count; i++)
				if (inputs[i].Main.GamepadPositive == binding || inputs[i].Main.GamepadNegative == binding || inputs[i].Alt.GamepadPositive == binding || inputs[i].Alt.GamepadNegative == binding)
					usedInputs.Add(Inputs[i]);

			return usedInputs.ToArray();
		}

		/// <summary>
		/// Converts a Unity KeyCode to the corresponding Key enum value used by the InputsManager.
		/// </summary>
		/// <param name="keyCode">The Unity KeyCode to convert.</param>
		/// <returns>The corresponding Key enum value, or Key.None if no match is found.</returns>
		public static Key KeyCodeToKey(KeyCode keyCode)
		{
			return keyCode switch
			{
				KeyCode.A => Key.A,
				KeyCode.Alpha0 => Key.Digit0,
				KeyCode.Alpha1 => Key.Digit1,
				KeyCode.Alpha2 => Key.Digit2,
				KeyCode.Alpha3 => Key.Digit3,
				KeyCode.Alpha4 => Key.Digit4,
				KeyCode.Alpha5 => Key.Digit5,
				KeyCode.Alpha6 => Key.Digit6,
				KeyCode.Alpha7 => Key.Digit7,
				KeyCode.Alpha8 => Key.Digit8,
				KeyCode.Alpha9 => Key.Digit9,
				KeyCode.AltGr => Key.AltGr,
				KeyCode.B => Key.B,
				KeyCode.BackQuote => Key.Backquote,
				KeyCode.Backslash => Key.Backslash,
				KeyCode.Backspace => Key.Backspace,
				KeyCode.C => Key.C,
				KeyCode.CapsLock => Key.CapsLock,
				KeyCode.Comma => Key.Comma,
				KeyCode.D => Key.D,
				KeyCode.Delete => Key.Delete,
				KeyCode.DownArrow => Key.DownArrow,
				KeyCode.E => Key.E,
				KeyCode.End => Key.End,
				KeyCode.Equals => Key.Equals,
				KeyCode.Escape => Key.Escape,
				KeyCode.F => Key.F,
				KeyCode.F1 => Key.F1,
				KeyCode.F2 => Key.F2,
				KeyCode.F3 => Key.F3,
				KeyCode.F4 => Key.F4,
				KeyCode.F5 => Key.F5,
				KeyCode.F6 => Key.F6,
				KeyCode.F7 => Key.F7,
				KeyCode.F8 => Key.F8,
				KeyCode.F9 => Key.F9,
				KeyCode.F10 => Key.F10,
				KeyCode.F11 => Key.F11,
				KeyCode.F12 => Key.F12,
				KeyCode.G => Key.G,
				KeyCode.H => Key.H,
				KeyCode.Home => Key.Home,
				KeyCode.I => Key.I,
				KeyCode.Insert => Key.Insert,
				KeyCode.J => Key.J,
				KeyCode.K => Key.K,
				KeyCode.Keypad0 => Key.Numpad0,
				KeyCode.Keypad1 => Key.Numpad1,
				KeyCode.Keypad2 => Key.Numpad2,
				KeyCode.Keypad3 => Key.Numpad3,
				KeyCode.Keypad4 => Key.Numpad4,
				KeyCode.Keypad5 => Key.Numpad5,
				KeyCode.Keypad6 => Key.Numpad6,
				KeyCode.Keypad7 => Key.Numpad7,
				KeyCode.Keypad8 => Key.Numpad8,
				KeyCode.Keypad9 => Key.Numpad9,
				KeyCode.KeypadDivide => Key.NumpadDivide,
				KeyCode.KeypadEnter => Key.NumpadEnter,
				KeyCode.KeypadEquals => Key.NumpadEquals,
				KeyCode.KeypadMinus => Key.NumpadMinus,
				KeyCode.KeypadMultiply => Key.NumpadMultiply,
				KeyCode.KeypadPeriod => Key.NumpadPeriod,
				KeyCode.KeypadPlus => Key.NumpadPlus,
				KeyCode.L => Key.L,
				KeyCode.LeftAlt => Key.LeftAlt,
				KeyCode.LeftApple => Key.LeftApple,
				KeyCode.LeftArrow => Key.LeftArrow,
				KeyCode.LeftBracket => Key.LeftBracket,
				KeyCode.LeftControl => Key.LeftCtrl,
				KeyCode.LeftShift => Key.LeftShift,
				KeyCode.LeftWindows => Key.LeftWindows,
				KeyCode.M => Key.M,
				KeyCode.Minus => Key.Minus,
				KeyCode.N => Key.N,
				KeyCode.Numlock => Key.NumLock,
				KeyCode.O => Key.O,
				KeyCode.P => Key.P,
				KeyCode.PageDown => Key.PageDown,
				KeyCode.PageUp => Key.PageUp,
				KeyCode.Pause => Key.Pause,
				KeyCode.Period => Key.Period,
				KeyCode.Q => Key.Q,
				KeyCode.Quote => Key.Quote,
				KeyCode.R => Key.R,
				KeyCode.Return => Key.Enter,
				KeyCode.RightAlt => Key.RightAlt,
				KeyCode.RightApple => Key.RightApple,
				KeyCode.RightArrow => Key.RightArrow,
				KeyCode.RightBracket => Key.RightBracket,
				KeyCode.RightControl => Key.RightCtrl,
				KeyCode.RightShift => Key.RightShift,
				KeyCode.RightWindows => Key.RightWindows,
				KeyCode.S => Key.S,
				KeyCode.ScrollLock => Key.ScrollLock,
				KeyCode.Semicolon => Key.Semicolon,
				KeyCode.Slash => Key.Slash,
				KeyCode.Space => Key.Space,
				KeyCode.T => Key.T,
				KeyCode.Tab => Key.Tab,
				KeyCode.U => Key.U,
				KeyCode.UpArrow => Key.UpArrow,
				KeyCode.V => Key.V,
				KeyCode.W => Key.W,
				KeyCode.X => Key.X,
				KeyCode.Y => Key.Y,
				KeyCode.Z => Key.Z,
				KeyCode.SysReq => Key.PrintScreen,
				_ => Key.None,
			};
		}

		/// <summary>
		/// Finds the index of an input by its name in the inputs collection.
		/// </summary>
		/// <param name="name">The name of the input to find.</param>
		/// <returns>The index of the input if found, or -1 if not found.</returns>
		/// <exception cref="ArgumentException">Thrown when the input name is null or empty.</exception>
		public static int IndexOf(string name)
		{
			if (name.IsNullOrEmpty())
				throw new ArgumentException("The input name cannot be empty or `null`", "name");

			Dictionary<string, int> indexes = GetInputsNamesAndIndexesAsDictionary();

			if (indexes != null && indexes.ContainsKey(name))
				return indexes[name];

			return -1;
		}

		/// <summary>
		/// Gets an array of all input names currently registered in the system.
		/// Rebuilds the array if data has changed or if it hasn't been initialized yet.
		/// </summary>
		/// <returns>An array containing all input names.</returns>
		public static string[] GetInputsNames()
		{
			if (dataChanged || inputNames == null || inputNames.Length != Count)
			{
				if (inputNames == null || inputNames.Length != inputs.Length)
					inputNames = new string[inputs.Length];

				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].Name.IsNullOrEmpty())
						continue;
						
					inputNames[i] = inputs[i].Name;
				}
			}

			return inputNames;
		}

		/// <summary>
		/// Gets a dictionary mapping input names to their corresponding indices.
		/// Rebuilds the dictionary if data has changed or if it hasn't been initialized yet.
		/// </summary>
		/// <returns>A dictionary with input names as keys and their indices as values.</returns>
		public static Dictionary<string, int> GetInputsNamesAndIndexesAsDictionary()
		{
			if (inputs != null && (dataChanged || inputNamesDictionary == null || inputNamesDictionary.Count != Count))
			{
				if (inputNamesDictionary == null || inputNamesDictionary.Count != inputs.Length)
					inputNamesDictionary = new Dictionary<string, int>();
				else
					inputNamesDictionary.Clear();

				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].Name.IsNullOrEmpty())
						continue;

					inputNamesDictionary.Add(inputs[i].Name, i);
				}
			}

			return inputNamesDictionary;
		}

		/// <summary>
		/// Gets an input by its index in the inputs collection.
		/// </summary>
		/// <param name="index">The index of the input to retrieve.</param>
		/// <returns>The Input object at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		public static Input GetInput(int index)
		{
			return Inputs[GetInputIndex(index)];
		}

		/// <summary>
		/// Gets an input by its name.
		/// </summary>
		/// <param name="name">The name of the input to retrieve.</param>
		/// <returns>The Input object with the specified name.</returns>
		/// <exception cref="ArgumentException">Thrown when no input with the given name exists.</exception>
		public static Input GetInput(string name)
		{
			return Inputs[GetInputIndex(name)];
		}

		/// <summary>
		/// Sets an input at the specified index to a new value.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="index">The index of the input to modify.</param>
		/// <param name="input">The new Input object to set.</param>
		/// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		public static void SetInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot set input in Play Mode");

				return;
			}

			if (!input)
				throw new ArgumentNullException("input");

			Inputs[GetInputIndex(index)] = input;
			dataChanged = true;
		}

		/// <summary>
		/// Sets an input with the specified name to a new value.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="name">The name of the input to modify.</param>
		/// <param name="input">The new Input object to set.</param>
		/// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
		/// <exception cref="ArgumentException">Thrown when no input with the given name exists.</exception>
		public static void SetInput(string name, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot set input in Play Mode");

				return;
			}

			if (!input)
				throw new ArgumentNullException("input");

			Inputs[GetInputIndex(name)] = input;
			dataChanged = true;
		}

		/// <summary>
		/// Adds a new input to the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="input">The Input object to add.</param>
		/// <returns>The added Input object, or null if the operation failed.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
		/// <exception cref="ArgumentException">Thrown when the input name is invalid or already exists.</exception>
		public static Input AddInput(Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot add input in Play Mode");

				return null;
			}

			if (!input)
				throw new ArgumentNullException("input");

			if (input.Name.IsNullOrEmpty() || input.Name.IsNullOrWhiteSpace())
				throw new ArgumentException("The input name cannot be empty or `null`", "input.name");

			if (IndexOf(input.Name) > -1)
				throw new ArgumentException($"We couldn't add the input `{input.Name}` to the list because its name matches another one", "input");

			Array.Resize(ref inputs, inputs.Length + 1);
			Array.Resize(ref inputNames, inputNames.Length + 1);

#if UNITY_2021_2_OR_NEWER
			inputs[^1] = input;
#else
			inputs[inputs.Length - 1] = input;
#endif
			dataChanged = true;

			return input;
		}

		/// <summary>
		/// Creates and adds a new input with the specified name to the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="name">The name for the new input.</param>
		/// <returns>The newly created and added Input object.</returns>
		/// <exception cref="ArgumentException">Thrown when the name is invalid or already exists.</exception>
		public static Input AddInput(string name)
		{
			return AddInput(new Input(name));
		}

		/// <summary>
		/// Creates a duplicate of an existing input and adds it to the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="input">The Input object to duplicate.</param>
		/// <returns>The newly created duplicate Input object, or null if the operation failed.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
		public static Input DuplicateInput(Input input)
		{
			if (!input)
				throw new ArgumentNullException("input");

			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot duplicate input in Play Mode");

				return null;
			}

			Input newInput = new Input(input);
			int index = Count;
			int newLength = index + 1;

			Array.Resize(ref inputs, newLength);
			Array.Resize(ref inputNames, newLength);

			inputs[index] = newInput;
			dataChanged = true;

			return newInput;
		}

		/// <summary>
		/// Creates a duplicate of an existing input identified by name and adds it to the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="name">The name of the input to duplicate.</param>
		/// <returns>The newly created duplicate Input object, or null if the operation failed.</returns>
		/// <exception cref="ArgumentException">Thrown when no input with the given name exists.</exception>
		public static Input DuplicateInput(string name)
		{
			return DuplicateInput(GetInput(name));
		}

		/// <summary>
		/// Creates a duplicate of an existing input identified by index and adds it to the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="index">The index of the input to duplicate.</param>
		/// <returns>The newly created duplicate Input object, or null if the operation failed.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		public static Input DuplicateInput(int index)
		{
			return DuplicateInput(GetInput(index));
		}

		/// <summary>
		/// Inserts an input at the specified index in the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="index">The index at which to insert the input.</param>
		/// <param name="input">The Input object to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		public static void InsertInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot insert input in Play Mode");

				return;
			}

			if (index < 0 || index > Count)
				throw new ArgumentOutOfRangeException("index", input, $"We couldn't insert the `{input.Name}` because the index value is out range");

			List<Input> inputsList = Inputs.ToList();

			inputsList.Insert(index, input);

			inputs = inputsList.ToArray();
			dataChanged = true;
		}

		/// <summary>
		/// Removes an input with the specified name from the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="name">The name of the input to remove.</param>
		/// <exception cref="ArgumentException">Thrown when no input with the given name exists.</exception>
		public static void RemoveInput(string name)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove input in Play Mode");

				return;
			}

			List<Input> inputsList = Inputs.ToList();

			inputsList.RemoveAt(GetInputIndex(name));

			inputs = inputsList.ToArray();
			dataChanged = true;
		}

		/// <summary>
		/// Removes an input at the specified index from the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		/// <param name="index">The index of the input to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		public static void RemoveInput(int index)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove input in Play Mode");

				return;
			}

			List<Input> inputsList = Inputs.ToList();

			inputsList.RemoveAt(GetInputIndex(index));

			inputs = inputsList.ToArray();
			dataChanged = true;
		}

		/// <summary>
		/// Removes all inputs from the inputs collection.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		public static void RemoveAll()
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove inputs in Play Mode");

				return;
			}

			Array.Clear(inputs, 0, inputs.Length);

			inputs = new Input[] { };
			dataChanged = true;
		}

		/// <summary>
		/// Loads input configuration data from an InputsManagerData asset.
		/// </summary>
		/// <param name="data">The InputsManagerData asset to load from.</param>
		/// <returns>True if the data was successfully loaded, false otherwise.</returns>
		public static bool LoadDataFromSheet(InputsManagerData data)
		{
			if (!data)
			{
				Debug.LogError("<b>Inputs Manager:</b> We've had some issues while loading data!");

				return false;
			}

			inputs = data.Inputs;
			inputNames = inputs.Select(input => input.Name).ToArray();
			inputSourcePriority = data.InputSourcePriority;
			interpolationTime = data.InterpolationTime;
			holdTriggerTime = data.HoldTriggerTime;
			holdWaitTime = data.HoldWaitTime;
			doublePressTimeout = data.DoublePressTimeout;
			gamepadThreshold = data.GamepadThreshold;
			defaultGamepadIndex = data.DefaultGamepadIndex;
			dataLastWriteTime =
#if UNITY_EDITOR
				File.GetLastWriteTime(EditorDataAssetFullPath);
#else
				DateTime.Now;
			dataLoadedOnBuild = !Application.isEditor;
#endif
			dataChanged = !Application.isPlaying;

			return data;
		}

		/// <summary>
		/// Loads input configuration data from the default data asset file.
		/// </summary>
		/// <returns>True if the data was successfully loaded, false if the data asset doesn't exist or couldn't be loaded.</returns>
		public static bool LoadData()
		{
			if (!DataAssetExists)
				return false;

			if (DataLoaded)
				return true;

			DataSerializationUtility<InputsManagerData> serializer = new DataSerializationUtility<InputsManagerData>(DataAssetPath, true);
			bool dataLoaded = LoadDataFromSheet(serializer.Load());

			dataChanged = false;

			return dataLoaded;
		}

		/// <summary>
		/// [Obsolete] Use InputsManager.ForceDataChange() instead.
		/// </summary>
		[Obsolete("Use InputsManager.ForceDataChange() instead.", true)]
		public static void ForceLoadData() { }

		/// <summary>
		/// Forces the system to recognize that input data has changed, which will trigger a reload of the data.
		/// Only works in Edit mode, not during Play mode.
		/// </summary>
		public static void ForceDataChange()
		{
			if (Application.isPlaying)
				return;

			dataLastWriteTime = DateTime.MinValue;
			dataChanged = true;
		}

#if UNITY_EDITOR
		/// <summary>
		/// [Obsolete] Use EditorSaveData() instead.
		/// </summary>
		/// <returns>True if the data was successfully saved, false otherwise.</returns>
		[Obsolete("Use `EditorSaveData` instead.")]
		public static bool SaveData()
		{
			return EditorSaveData();
		}

		/// <summary>
		/// Saves the current input configuration to the data asset file.
		/// Only works in the Unity Editor and in Edit mode, not during Play mode.
		/// </summary>
		/// <returns>True if the data was successfully saved, false otherwise.</returns>
		public static bool EditorSaveData()
		{
			if (Application.isPlaying || !Application.isEditor)
				return false;

			editorSavingData = true;

			if (File.Exists(EditorDataAssetFullPath))
				File.Delete(EditorDataAssetFullPath);

			dataChanged = false;
			inputNames = null;

			DataSerializationUtility<InputsManagerData> serializationUtility = new DataSerializationUtility<InputsManagerData>(EditorDataAssetFullPath, false);
			bool result = serializationUtility.SaveOrCreate(new InputsManagerData());

			editorSavingData = false;

			return result;
		}
#endif

		/// <summary>
		/// Gets the index of an input in the inputs collection.
		/// </summary>
		/// <param name="input">The Input object to find.</param>
		/// <returns>The index of the input.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
		/// <exception cref="ArgumentException">Thrown when the input is not found in the collection.</exception>
		private static int GetInputIndex(Input input)
		{
			if (!input)
				throw new ArgumentNullException("input");

			return GetInputIndex(input.Name);
		}

		/// <summary>
		/// Validates and returns an input index, ensuring it's within the valid range.
		/// </summary>
		/// <param name="index">The index to validate.</param>
		/// <returns>The validated index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		private static int GetInputIndex(int index)
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException("index");

			return index;
		}

		/// <summary>
		/// Gets the index of an input by its name.
		/// </summary>
		/// <param name="name">The name of the input to find.</param>
		/// <returns>The index of the input.</returns>
		/// <exception cref="ArgumentException">Thrown when the name is null or empty, or when no input with the given name exists.</exception>
		private static int GetInputIndex(string name)
		{
			if (name.IsNullOrEmpty())
				throw new ArgumentException("The input name cannot be empty or `null`", "name");

			int index = IndexOf(name);

			if (index < 0)
				throw new ArgumentException($"We couldn't find an input with the name of `{name}` in the inputs list!");

			return GetInputIndex(index);
		}

		#endregion

		#endregion
	}

	#endregion
}
