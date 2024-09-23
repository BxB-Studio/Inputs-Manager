﻿#define INPUTS_MANAGER

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

		public static readonly string DataAssetPath = $"Assets{Path.DirectorySeparatorChar}InputsManager_Data";
		public static string DataAssetFullPath => Path.Combine(Application.dataPath, "Resources", $"{DataAssetPath}.bytes").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		public static InputSource InputSourcePriority
		{
			get
			{
				if (!Application.isPlaying && inputSourcePriority == 0)
					LoadData();

				return inputSourcePriority;
			}
			set
			{
				if (inputSourcePriority == value)
					return;

				inputSourcePriority = value;

				SaveData();
			}
		}
		public static float InterpolationTime
		{
			get
			{
				if (!Application.isPlaying && interpolationTime == 0f)
					LoadData();

				return interpolationTime;
			}
			set
			{
				if (interpolationTime == value)
					return;

				interpolationTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float HoldTriggerTime
		{
			get
			{
				if (!Application.isPlaying && holdTriggerTime == 0f)
					LoadData();

				return holdTriggerTime;
			}
			set
			{
				if (holdTriggerTime == value)
					return;

				holdTriggerTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float HoldWaitTime
		{
			get
			{
				if (!Application.isPlaying && holdWaitTime == 0f)
					LoadData();

				return holdWaitTime;
			}
			set
			{
				if (holdWaitTime == value)
					return;

				holdWaitTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float DoublePressTimeout
		{
			get
			{
				if (!Application.isPlaying && doublePressTimeout == 0f)
					LoadData();

				return doublePressTimeout;
			}
			set
			{
				if (doublePressTimeout == value)
					return;

				doublePressTimeout = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float GamepadThreshold
		{
			get
			{
				if (!Application.isPlaying && gamepadThreshold == 0f)
					LoadData();

				return gamepadThreshold;
			}
			set
			{
				if (Application.isPlaying || gamepadThreshold == value)
					return;

				gamepadThreshold = Mathf.Clamp01(value);

				SaveData();
			}
		}
		public static bool DataLoaded
		{
			get
			{
				if (!Application.isEditor)
					return dataLoadedOnBuild;

				return DataAssetExists && File.GetLastWriteTime(DataAssetFullPath) == dataLastWriteTime;
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
				return Application.isEditor ? File.Exists(DataAssetFullPath) : Resources.Load(DataAssetPath);
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
				if (gamepads == null || gamepads.Length != GamepadsCount)
					gamepads = Gamepad.all.ToArray();

				return gamepads;
			}
		}
		public static string[] GamepadNames
		{
			get
			{
				if (gamepadNames == null || gamepadNames.Length != GamepadsCount)
					gamepadNames = Gamepad.all.Select(gamepad => gamepad.name).ToArray();

				return gamepadNames;
			}
		}
		public static int GamepadsCount
		{
			get
			{
				return Gamepad.all.Count;
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
				if (!Application.isPlaying)
					LoadData();

				inputs ??= new Input[] { };

				return inputs;
			}
		}

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
		private static InputSource inputSourcePriority;
		private static float interpolationTime = .25f;
		private static float holdTriggerTime = .3f;
		private static float holdWaitTime = .1f;
		private static float doublePressTimeout = .2f;
		private static float gamepadThreshold = .5f;
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
		private static bool dataLoadedOnBuild;
		private static int gamepadsCount;

		#endregion

		#region Methods

		#region Inputs

		public static Vector2 InputMouseMovement()
		{
			if (mouse == null)
				return default;

			return mouse.delta.value;
		}
		public static Vector2 InputMousePosition()
		{
			if (mouse == null)
				return default;

			return mouse.position.value;
		}
		public static Vector2 InputMouseScrollWheelVector()
		{
			if (mouse == null)
				return default;

			return mouse.scroll.value;
		}

		public static float InputMainAxisValue(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static float InputMainAxisValue(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static float InputMainAxisValue(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainValue(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}
		public static float InputAltAxisValue(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static float InputAltAxisValue(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static float InputAltAxisValue(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltValue(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}
		public static float InputValue(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static float InputValue(string name, int gamepadIndex = 0)
		{
			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static float InputValue(int index, int gamepadIndex = 0)
		{
			return InputUtilities.Value(inputsKeyboardAccess, inputsGamepadAccess, index, gamepadIndex);
		}

		public static float InputMouseScrollWheel()
		{
			if (mouse == null)
				return default;

			return mouse.scroll.value.magnitude;
		}
		public static float InputMouseScrollWheelHorizontal()
		{
			if (mouse == null)
				return default;

			return mouse.scroll.value.x;
		}
		public static float InputMouseScrollWheelVertical()
		{
			if (mouse == null)
				return default;

			return mouse.scroll.value.y;
		}

		public static bool AnyInputPress(bool ignoreMouse = false)
		{
			return keyboard != null && keyboard.anyKey.isPressed || !ignoreMouse && InputMouseAnyButtonPress() || gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.isPressed));
		}
		public static bool AnyInputDown(bool ignoreMouse = false)
		{
			return keyboard != null && keyboard.anyKey.wasPressedThisFrame || !ignoreMouse && InputMouseAnyButtonDown() || gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasPressedThisFrame));
		}
		public static bool AnyInputUp(bool ignoreMouse = false)
		{
			return keyboard != null && keyboard.anyKey.wasReleasedThisFrame || !ignoreMouse && InputMouseAnyButtonUp() || gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasReleasedThisFrame));
		}

		public static bool InputPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.Press(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositivePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositivePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositivePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositivePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltPress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.Down(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDown(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.Up(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltUp(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.Hold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltHold(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.DoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.MainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.AltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeMainDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.PositiveAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(input), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputUtilities.NegativeAltDoublePress(inputsKeyboardAccess, inputsGamepadAccess, GetInputIndex(index), gamepadIndex);
		}

		public static bool InputKeyPress(Key key)
		{
			return InputUtilities.KeyToKeyControl(key).isPressed;
		}
		public static bool InputKeyDown(Key key)
		{
			return InputUtilities.KeyToKeyControl(key).wasPressedThisFrame;
		}
		public static bool InputKeyUp(Key key)
		{
			return InputUtilities.KeyToKeyControl(key).wasReleasedThisFrame;
		}

		public static bool InputMouseAnyButtonPress()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonPress(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonPress(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (Mouse == null)
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
		public static bool InputMouseButtonPress(MouseButton type)
		{
			return InputMouseButtonPress((int)type);
		}
		public static bool InputMouseAnyButtonDown()
		{
			if (Mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDown(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonDown(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (Mouse == null)
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
		public static bool InputMouseButtonDown(MouseButton type)
		{
			return InputMouseButtonDown((int)type);
		}
		public static bool InputMouseAnyButtonUp()
		{
			if (Mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonUp(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonUp(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (Mouse == null)
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
		public static bool InputMouseButtonUp(MouseButton type)
		{
			return InputMouseButtonUp((int)type);
		}
		public static bool InputMouseAnyButtonHold()
		{
			if (Mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonHold(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonHold(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (Mouse == null)
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
		public static bool InputMouseButtonHold(MouseButton type)
		{
			return InputMouseButtonHold((int)type);
		}
		public static bool InputMouseAnyButtonDoublePress()
		{
			if (Mouse == null)
				return false;

			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDoublePress(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonDoublePress(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			if (Mouse == null)
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
		public static bool InputMouseButtonDoublePress(MouseButton type)
		{
			return InputMouseButtonDoublePress((int)type);
		}

		#endregion

		#region Gamepad Outputs

		public static void GamepadVibration(float lowFrequency, float highFrequency, Gamepad gamepad)
		{
			if (gamepad == null)
				return;

			gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
		}
		public static void GamepadVibration(float lowFrequency, float highFrequency, int gamepadIndex = 0)
		{
			if (gamepadIndex < 0 || gamepadIndex >= GamepadsCount)
				return;

			GamepadVibration(lowFrequency, highFrequency, gamepads[gamepadIndex]);
		}

		#endregion

		#region Methods

		public static void Start()
		{
			if (!Application.isPlaying)
				throw new Exception("The `Start` method can only be called during Play mode.");

			LoadData();

			if (inputs == null || inputs.Length < 1)
				return;

			if (!inputsAccess.IsCreated)
				inputsAccess = new(inputs.Length, Allocator.Persistent);

			if (!inputsKeyboardAccess.IsCreated)
				inputsKeyboardAccess = new(inputs.Length, Allocator.Persistent);

			if (inputsGamepadAccess == null || inputsGamepadAccess.Length != inputs.Length)
			{
				if (inputsGamepadAccess != null && inputsGamepadAccess.Length > 0)
					for (int i = 0; i < inputsGamepadAccess.Length; i++)
						if (inputsGamepadAccess[i].IsCreated)
							inputsGamepadAccess[i].Dispose();

				inputsGamepadAccess = new NativeArray<InputSourceAccess>[inputs.Length];

				for (int i = 0; i < inputs.Length; i++)
					inputsGamepadAccess[i] = new(GamepadsCount, Allocator.Persistent);
			}

			for (int i = 0; i < inputs.Length; i++)
			{
				inputsAccess[i] = new(inputs[i]);
				inputsKeyboardAccess[i] = new(inputs[i], InputSource.Keyboard);

				for (int j = 0; j < GamepadsCount; j++)
					inputsGamepadAccess[i][j] = new(inputs[i], InputSource.Gamepad);
			}
		}
		public static void Update()
		{
			if (!Application.isPlaying)
				throw new Exception("The `Update` method can only be called during Play mode.");

			if (Mouse != null)
				UpdateMouse();

			if (inputs == null || inputs.Length < 1 || Keyboard == null && Gamepads.Length < 1)
				return;

			if (GamepadsCount > 0)
				if (Gamepad.current != gamepads[0])
					gamepads[0].MakeCurrent();

			for (int i = 0; i < inputs.Length; i++)
			{
				if (dataChanged)
				{
					inputsAccess[i] = new(inputs[i]);
					inputsKeyboardAccess[i] = new(inputs[i], InputSource.Keyboard);

					for (int j = 0; j < GamepadsCount; j++)
						inputsGamepadAccess[i][j] = new(inputs[i], InputSource.Gamepad);
				}
				else if (GamepadsCount != gamepadsCount)
				{
					if (inputsGamepadAccess[i].IsCreated)
						inputsGamepadAccess[i].Dispose();

					inputsGamepadAccess[i] = new(GamepadsCount, Allocator.Persistent);

					for (int j = 0; j < GamepadsCount; j++)
						inputsGamepadAccess[i][j] = new(inputs[i], InputSource.Gamepad);
				}

				inputsKeyboardAccess[i] = inputsKeyboardAccess[i].UpdateControls(inputs[i]);

				for (int j = 0; j < GamepadsCount; j++)
					inputsGamepadAccess[i][j] = inputsGamepadAccess[i][j].UpdateControls(inputs[i], j);
			}

			gamepadsCount = GamepadsCount;
			dataChanged = false;

			NativeList<JobHandle> jobHandles = new(inputs.Length + 1, Allocator.Temp);
			InputKeyboardJob inputKeyboardJob = new()
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

			if (GamepadsCount > 0)
				for (int i = 0; i < inputs.Length; i++)
				{
					InputGamepadJob inputGamepadJob = new()
					{
						sourceAccess = inputsGamepadAccess[i],
						input = inputsAccess[i],
						interpolationTime = interpolationTime,
						holdTriggerTime = holdTriggerTime,
						holdWaitTime = holdWaitTime,
						doublePressTimeout = doublePressTimeout,
						deltaTime = Utility.DeltaTime
					};

					jobHandles.Add(inputGamepadJob.ScheduleParallel(GamepadsCount, 1, default));
				}

			JobHandle.CompleteAll(jobHandles);
			jobHandles.Dispose();
		}
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
		}

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

		public static Input[] KeyUsed(Key key)
		{
			if (key == Key.None)
				return new Input[] { };

			List<Input> inputs = new();

			for (int i = 0; i < Count; i++)
				if (Inputs[i].Main.Positive == key || Inputs[i].Main.Negative == key || Inputs[i].Alt.Positive == key || Inputs[i].Alt.Negative == key)
					inputs.Add(Inputs[i]);

			return inputs.ToArray();
		}
		public static Input[] GamepadBindingUsed(GamepadBinding binding)
		{
			if (binding == GamepadBinding.None)
				return new Input[] { };

			List<Input> inputs = new();

			for (int i = 0; i < Count; i++)
				if (Inputs[i].Main.GamepadPositive == binding || Inputs[i].Main.GamepadNegative == binding || Inputs[i].Alt.GamepadPositive == binding || Inputs[i].Alt.GamepadNegative == binding)
					inputs.Add(Inputs[i]);

			return inputs.ToArray();
		}
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
		public static int IndexOf(string name)
		{
			if (name.IsNullOrEmpty())
				throw new ArgumentException("The input name cannot be empty or `null`", "name");

			Dictionary<string, int> indexes = GetInputsNamesAndIndexesAsDictionary();

			if (indexes.ContainsKey(name))
				return indexes[name];

			return -1;
		}
		public static string[] GetInputsNames()
		{
			if (dataChanged || inputNames == null || inputNames.Length != Inputs.Length)
			{
				if (inputNames == null || inputNames.Length != Inputs.Length)
					inputNames = new string[Inputs.Length];

				for (int i = 0; i < Inputs.Length; i++)
				{
					if (Inputs[i].Name.IsNullOrEmpty())
						continue;
						
					inputNames[i] = Inputs[i].Name;
				}
			}

			return inputNames;
		}
		public static Dictionary<string, int> GetInputsNamesAndIndexesAsDictionary()
		{
			if (dataChanged || inputNamesDictionary == null || inputNamesDictionary.Count != Inputs.Length)
			{
				if (inputNamesDictionary == null || inputNamesDictionary.Count != Inputs.Length)
					inputNamesDictionary = new();
				else
					inputNamesDictionary.Clear();

				for (int i = 0; i < Inputs.Length; i++)
				{
					if (Inputs[i].Name.IsNullOrEmpty())
						continue;

					inputNamesDictionary.Add(Inputs[i].Name, i);
				}
			}

			return inputNamesDictionary;
		}
		public static Input GetInput(int index)
		{
			return Inputs[GetInputIndex(index)];
		}
		public static Input GetInput(string name)
		{
			return Inputs[GetInputIndex(name)];
		}
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

			Array.Resize(ref inputs, Inputs.Length + 1);
			Array.Resize(ref inputNames, inputNames.Length + 1);

			Inputs[^1] = input;
			dataChanged = true;

			return input;
		}
		public static Input AddInput(string name)
		{
			return AddInput(new Input(name));
		}
		public static Input DuplicateInput(Input input)
		{
			if (!input)
				throw new ArgumentNullException("input");

			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot duplicate input in Play Mode");

				return null;
			}

			Input newInput = new(input);
			int index = Inputs.Length;
			int newLength = index + 1;

			Array.Resize(ref inputs, newLength);
			Array.Resize(ref inputNames, newLength);

			Inputs[index] = newInput;
			dataChanged = true;

			return newInput;
		}
		public static Input DuplicateInput(string name)
		{
			return DuplicateInput(GetInput(name));
		}
		public static Input DuplicateInput(int index)
		{
			return DuplicateInput(GetInput(index));
		}
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
			dataLastWriteTime = Application.isEditor ? File.GetLastWriteTime(DataAssetFullPath) : DateTime.Now;
			dataChanged = !Application.isPlaying;
			dataLoadedOnBuild = !Application.isEditor;

			return data;
		}
		public static bool LoadData()
		{
			if (!DataAssetExists)
				return false;

			if (DataLoaded)
				return true;

			DataSerializationUtility<InputsManagerData> serializer = new(DataAssetPath, true);
			bool dataLoaded = LoadDataFromSheet(serializer.Load());

			dataChanged = false;

			return dataLoaded;
		}
		[Obsolete("Use InputsManager.ForceDataChange() instead.", true)]
		public static void ForceLoadData() { }
		public static void ForceDataChange()
		{
			if (Application.isPlaying)
				return;

			dataLastWriteTime = DateTime.MinValue;
			dataChanged = true;
		}
		public static bool SaveData()
		{
			if (Application.isPlaying || !Application.isEditor)
				return false;

			if (File.Exists(DataAssetFullPath))
				File.Delete(DataAssetFullPath);

			DataSerializationUtility<InputsManagerData> serializer = new(DataAssetFullPath, false);

			inputNames = null;
			dataChanged = false;

			return serializer.SaveOrCreate(new());
		}

		private static int GetInputIndex(Input input)
		{
			if (!input)
				throw new ArgumentNullException("input");

			return GetInputIndex(input.Name);
		}
		private static int GetInputIndex(int index)
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException("index");

			return index;
		}
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