#region Namespaces

using Unity.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Utilities.Inputs.Components;

#endregion

namespace Utilities.Inputs
{
	/// <summary>
	/// A static utility class that provides helper methods for working with input controls, key mappings,
	/// and gamepad bindings within the Utilities.Inputs system. Facilitates conversion between different
	/// input representations and provides access to input device controls.
	/// </summary>
	public static class InputUtilities
	{
		/// <summary>
		/// Converts a key enumeration value to its corresponding KeyControl from the Unity Input System.
		/// This method maps the custom Key enum values to the actual keyboard controls that can be queried
		/// for input state. Returns null if the keyboard device is not available.
		/// </summary>
		/// <param name="key">The key enumeration value to convert to a KeyControl.</param>
		/// <returns>The KeyControl object corresponding to the specified key, or null if the keyboard is not available.</returns>
		public static KeyControl KeyToKeyControl(Key key)
		{
			if (InputsManager.Keyboard == null)
				return null;

			return key switch
			{
				Key.A => InputsManager.Keyboard.aKey,
				Key.B => InputsManager.Keyboard.bKey,
				Key.Backquote => InputsManager.Keyboard.backquoteKey,
				Key.Backslash => InputsManager.Keyboard.backslashKey,
				Key.Backspace => InputsManager.Keyboard.backspaceKey,
				Key.C => InputsManager.Keyboard.cKey,
				Key.Comma => InputsManager.Keyboard.commaKey,
				Key.ContextMenu => InputsManager.Keyboard.contextMenuKey,
				Key.D => InputsManager.Keyboard.dKey,
				Key.Delete => InputsManager.Keyboard.deleteKey,
				Key.Digit0 => InputsManager.Keyboard.digit0Key,
				Key.Digit1 => InputsManager.Keyboard.digit1Key,
				Key.Digit2 => InputsManager.Keyboard.digit2Key,
				Key.Digit3 => InputsManager.Keyboard.digit3Key,
				Key.Digit4 => InputsManager.Keyboard.digit4Key,
				Key.Digit5 => InputsManager.Keyboard.digit5Key,
				Key.Digit6 => InputsManager.Keyboard.digit6Key,
				Key.Digit7 => InputsManager.Keyboard.digit7Key,
				Key.Digit8 => InputsManager.Keyboard.digit8Key,
				Key.Digit9 => InputsManager.Keyboard.digit9Key,
				Key.DownArrow => InputsManager.Keyboard.downArrowKey,
				Key.E => InputsManager.Keyboard.eKey,
				Key.End => InputsManager.Keyboard.endKey,
				Key.Enter => InputsManager.Keyboard.enterKey,
				Key.Equals => InputsManager.Keyboard.equalsKey,
				Key.Escape => InputsManager.Keyboard.escapeKey,
				Key.F => InputsManager.Keyboard.fKey,
				Key.F1 => InputsManager.Keyboard.f1Key,
				Key.F2 => InputsManager.Keyboard.f2Key,
				Key.F3 => InputsManager.Keyboard.f3Key,
				Key.F4 => InputsManager.Keyboard.f4Key,
				Key.F5 => InputsManager.Keyboard.f5Key,
				Key.F6 => InputsManager.Keyboard.f6Key,
				Key.F7 => InputsManager.Keyboard.f7Key,
				Key.F8 => InputsManager.Keyboard.f8Key,
				Key.F9 => InputsManager.Keyboard.f9Key,
				Key.F10 => InputsManager.Keyboard.f10Key,
				Key.F11 => InputsManager.Keyboard.f11Key,
				Key.F12 => InputsManager.Keyboard.f12Key,
				Key.G => InputsManager.Keyboard.gKey,
				Key.H => InputsManager.Keyboard.hKey,
				Key.Home => InputsManager.Keyboard.homeKey,
				Key.I => InputsManager.Keyboard.iKey,
				Key.Insert => InputsManager.Keyboard.insertKey,
				Key.J => InputsManager.Keyboard.jKey,
				Key.K => InputsManager.Keyboard.kKey,
				Key.L => InputsManager.Keyboard.lKey,
				Key.LeftAlt => InputsManager.Keyboard.leftAltKey,
				Key.LeftArrow => InputsManager.Keyboard.leftArrowKey,
				Key.LeftBracket => InputsManager.Keyboard.leftBracketKey,
				Key.LeftCtrl => InputsManager.Keyboard.leftCtrlKey,
				Key.LeftMeta => InputsManager.Keyboard.leftMetaKey,
				Key.LeftShift => InputsManager.Keyboard.leftShiftKey,
				Key.M => InputsManager.Keyboard.mKey,
				Key.Minus => InputsManager.Keyboard.minusKey,
				Key.N => InputsManager.Keyboard.nKey,
				Key.NumLock => InputsManager.Keyboard.numLockKey,
				Key.Numpad0 => InputsManager.Keyboard.numpad0Key,
				Key.Numpad1 => InputsManager.Keyboard.numpad1Key,
				Key.Numpad2 => InputsManager.Keyboard.numpad2Key,
				Key.Numpad3 => InputsManager.Keyboard.numpad3Key,
				Key.Numpad4 => InputsManager.Keyboard.numpad4Key,
				Key.Numpad5 => InputsManager.Keyboard.numpad5Key,
				Key.Numpad6 => InputsManager.Keyboard.numpad6Key,
				Key.Numpad7 => InputsManager.Keyboard.numpad7Key,
				Key.Numpad8 => InputsManager.Keyboard.numpad8Key,
				Key.Numpad9 => InputsManager.Keyboard.numpad9Key,
				Key.NumpadDivide => InputsManager.Keyboard.numpadDivideKey,
				Key.NumpadEnter => InputsManager.Keyboard.numpadEnterKey,
				Key.NumpadEquals => InputsManager.Keyboard.numpadEqualsKey,
				Key.NumpadMinus => InputsManager.Keyboard.numpadMinusKey,
				Key.NumpadMultiply => InputsManager.Keyboard.numpadMultiplyKey,
				Key.NumpadPeriod => InputsManager.Keyboard.numpadPeriodKey,
				Key.NumpadPlus => InputsManager.Keyboard.numpadPlusKey,
				Key.O => InputsManager.Keyboard.oKey,
				Key.OEM1 => InputsManager.Keyboard.oem1Key,
				Key.OEM2 => InputsManager.Keyboard.oem2Key,
				Key.OEM3 => InputsManager.Keyboard.oem3Key,
				Key.OEM4 => InputsManager.Keyboard.oem4Key,
				Key.OEM5 => InputsManager.Keyboard.oem5Key,
				Key.P => InputsManager.Keyboard.pKey,
				Key.PageDown => InputsManager.Keyboard.pageDownKey,
				Key.PageUp => InputsManager.Keyboard.pageUpKey,
				Key.Pause => InputsManager.Keyboard.pauseKey,
				Key.Period => InputsManager.Keyboard.periodKey,
				Key.Q => InputsManager.Keyboard.qKey,
				Key.Quote => InputsManager.Keyboard.quoteKey,
				Key.R => InputsManager.Keyboard.rKey,
				Key.RightAlt => InputsManager.Keyboard.rightAltKey,
				Key.RightArrow => InputsManager.Keyboard.rightArrowKey,
				Key.RightBracket => InputsManager.Keyboard.rightBracketKey,
				Key.RightCtrl => InputsManager.Keyboard.rightCtrlKey,
				Key.RightMeta => InputsManager.Keyboard.rightMetaKey,
				Key.RightShift => InputsManager.Keyboard.rightShiftKey,
				Key.S => InputsManager.Keyboard.sKey,
				Key.ScrollLock => InputsManager.Keyboard.scrollLockKey,
				Key.Semicolon => InputsManager.Keyboard.semicolonKey,
				Key.Slash => InputsManager.Keyboard.slashKey,
				Key.Space => InputsManager.Keyboard.spaceKey,
				Key.T => InputsManager.Keyboard.tKey,
				Key.Tab => InputsManager.Keyboard.tabKey,
				Key.U => InputsManager.Keyboard.uKey,
				Key.UpArrow => InputsManager.Keyboard.upArrowKey,
				Key.V => InputsManager.Keyboard.vKey,
				Key.W => InputsManager.Keyboard.wKey,
				Key.X => InputsManager.Keyboard.xKey,
				Key.Y => InputsManager.Keyboard.yKey,
				Key.Z => InputsManager.Keyboard.zKey,
				_ => null,
			};
		}
		/// <summary>
		/// Converts a gamepad binding enumeration value to its corresponding Unity InputSystem ButtonControl.
		/// This method maps the abstract GamepadBinding enum values to concrete ButtonControl instances
		/// from the provided gamepad, enabling consistent access to gamepad inputs throughout the system.
		/// </summary>
		/// <param name="gamepad">The Unity InputSystem Gamepad device to get the button control from. If null, the method returns null.</param>
		/// <param name="binding">The GamepadBinding enumeration value that specifies which gamepad button or control to retrieve.</param>
		/// <returns>
		/// The ButtonControl corresponding to the specified binding on the provided gamepad.
		/// Returns null if the gamepad is null or if the binding doesn't match any known control.
		/// </returns>
		public static ButtonControl GamepadBindingToButtonControl(Gamepad gamepad, GamepadBinding binding)
		{
			if (gamepad == null)
				return null;

			return binding switch
			{
				GamepadBinding.DpadUp => gamepad.dpad.up,
				GamepadBinding.DpadRight => gamepad.dpad.right,
				GamepadBinding.DpadDown => gamepad.dpad.down,
				GamepadBinding.DpadLeft => gamepad.dpad.left,
				GamepadBinding.ButtonNorth => gamepad.buttonNorth,
				GamepadBinding.ButtonEast => gamepad.buttonEast,
				GamepadBinding.ButtonSouth => gamepad.buttonSouth,
				GamepadBinding.ButtonWest => gamepad.buttonWest,
				GamepadBinding.LeftStickButton => gamepad.leftStickButton,
				GamepadBinding.LeftStickUp => gamepad.leftStick.up,
				GamepadBinding.LeftStickRight => gamepad.leftStick.right,
				GamepadBinding.LeftStickDown => gamepad.leftStick.down,
				GamepadBinding.LeftStickLeft => gamepad.leftStick.left,
				GamepadBinding.RightStickButton => gamepad.rightStickButton,
				GamepadBinding.RightStickUp => gamepad.rightStick.up,
				GamepadBinding.RightStickRight => gamepad.rightStick.right,
				GamepadBinding.RightStickDown => gamepad.rightStick.down,
				GamepadBinding.RightStickLeft => gamepad.rightStick.left,
				GamepadBinding.LeftShoulder => gamepad.leftShoulder,
				GamepadBinding.RightShoulder => gamepad.rightShoulder,
				GamepadBinding.LeftTrigger => gamepad.leftTrigger,
				GamepadBinding.RightTrigger => gamepad.rightTrigger,
				GamepadBinding.StartButton => gamepad.startButton,
				GamepadBinding.SelectButton => gamepad.selectButton,
				_ => null,
			};
		}

		/// <summary>
		/// Gets the main value of the input based on the current input source priority settings.
		/// This method determines whether to use keyboard or gamepad input values based on the
		/// InputsManager's priority settings, the availability of inputs, and the specified gamepad index.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to retrieve the value for. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input may be used depending on priority settings.</param>
		/// <returns>
		/// The main value of the input, ranging from 0.0 to 1.0. Returns 0.0 if the index is invalid.
		/// The value comes from either keyboard or gamepad based on priority rules and input availability.
		/// </returns>
		internal static float MainValue(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return 0f;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool gamepadUsed = validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainValue != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;
			bool keyboardUsed = keyboardAccess[index].mainValue != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed || ignoreKeyboard)
				return gamepadsAccess[index][gamepadIndex].mainValue;
			else
				return keyboardAccess[index].mainValue;
		}
		/// <summary>
		/// Gets the alt value of the input based on the current input source priority settings.
		/// This method determines whether to use keyboard or gamepad input values based on the
		/// InputsManager's priority settings, the availability of inputs, and the specified gamepad index.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to retrieve the value for. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input may be used depending on priority settings.</param>
		/// <returns>
		/// The alt value of the input, ranging from 0.0 to 1.0. Returns 0.0 if the index is invalid.
		/// The value comes from either keyboard or gamepad based on priority rules and input availability.
		/// </returns>
		internal static float AltValue(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return 0f;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool gamepadUsed = validGamepadIndex && gamepadsAccess[index][gamepadIndex].altValue != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;
			bool keyboardUsed = keyboardAccess[index].altValue != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed || ignoreKeyboard)
				return gamepadsAccess[index][gamepadIndex].altValue;
			else
				return keyboardAccess[index].altValue;
		}
		
		/// <summary>
		/// Gets the combined value of the input based on the current input source priority settings.
		/// This method determines whether to use keyboard or gamepad input values based on the
		/// InputsManager's priority settings, the availability of inputs, and the specified gamepad index.
		/// The combined value represents the overall input magnitude from all sources.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to retrieve the value for. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input may be used depending on priority settings.</param>
		/// <returns>
		/// The combined value of the input, ranging from 0.0 to 1.0. Returns 0.0 if the index is invalid.
		/// The value comes from either keyboard or gamepad based on priority rules and input availability.
		/// </returns>
		internal static float Value(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return 0f;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool gamepadUsed = validGamepadIndex && gamepadsAccess[index][gamepadIndex].value != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;
			bool keyboardUsed = keyboardAccess[index].value != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed || ignoreKeyboard)
				return gamepadsAccess[index][gamepadIndex].value;
			else
				return keyboardAccess[index].value;
		}
		
		/// <summary>
		/// Determines if the positive main input is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive main input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveMainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveMainPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveMainPress;
		}
		
		/// <summary>
		/// Determines if the negative main input is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative main input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeMainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeMainPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeMainPress;
		}
		
		/// <summary>
		/// Determines if the positive alt input is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive alt input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveAltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveAltPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveAltPress;
		}
		
		/// <summary>
		/// Determines if the negative alt input is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative alt input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeAltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeAltPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeAltPress;
		}
		
		/// <summary>
		/// Determines if any positive input (main or alt) is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any positive input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositivePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positivePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positivePress;
		}
		
		/// <summary>
		/// Determines if any negative input (main or alt) is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any negative input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativePress;
		}
		
		/// <summary>
		/// Determines if any main input (positive or negative) is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any main input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool MainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].mainPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainPress;
		}
		
		/// <summary>
		/// Determines if any alt input (positive or negative) is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any alt input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool AltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].altPress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].altPress;
		}
		
		/// <summary>
		/// Determines if any input (main or alt, positive or negative) is currently being pressed.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a comprehensive check for any input activity.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any input is being pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool Press(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].press || validGamepadIndex && gamepadsAccess[index][gamepadIndex].press;
		}
		/// <summary>
		/// Determines if the positive main input is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive main input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveMainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveMainDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveMainDown;
		}
		/// <summary>
		/// Determines if the negative main input is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative main input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeMainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeMainDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeMainDown;
		}
		/// <summary>
		/// Determines if the positive alternative input is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive alternative input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveAltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveAltDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveAltDown;
		}
		/// <summary>
		/// Determines if the negative alternative input is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative alternative input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeAltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeAltDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeAltDown;
		}
		/// <summary>
		/// Determines if any positive input (main or alternative) is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both main and alternative positive inputs.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any positive input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveDown;
		}
		/// <summary>
		/// Determines if any negative input (main or alternative) is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both main and alternative negative inputs.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any negative input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeDown;
		}
		/// <summary>
		/// Determines if any main input (positive or negative) is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative main inputs.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any main input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool MainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].mainDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainDown;
		}
		/// <summary>
		/// Determines if any alternative input (positive or negative) is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative alternative inputs.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any alternative input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool AltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].altDown || validGamepadIndex && gamepadsAccess[index][gamepadIndex].altDown;
		}
		/// <summary>
		/// Determines if any input (main or alternative, positive or negative) is currently being pressed down (first frame of press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a comprehensive check for any input being pressed down.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any input is being pressed down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool Down(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].down || validGamepadIndex && gamepadsAccess[index][gamepadIndex].down;
		}
		/// <summary>
		/// Determines if the positive main input is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting when a button has just been released.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive main input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveMainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveMainUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveMainUp;
		}
		/// <summary>
		/// Determines if the negative main input is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting when a button has just been released.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative main input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeMainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeMainUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeMainUp;
		}
		/// <summary>
		/// Determines if the positive alternative input is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting when a button has just been released.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive alternative input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveAltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveAltUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveAltUp;
		}
		/// <summary>
		/// Determines if the negative alternative input is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting when a button has just been released.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative alternative input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeAltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeAltUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeAltUp;
		}
		/// <summary>
		/// Determines if any positive input (main or alternative) is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both main and alternative positive input releases.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any positive input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveUp;
		}
		/// <summary>
		/// Determines if any negative input (main or alternative) is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both main and alternative negative input releases.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any negative input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeUp;
		}
		/// <summary>
		/// Determines if any main input (positive or negative) is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative main input releases.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any main input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool MainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].mainUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainUp;
		}
		/// <summary>
		/// Determines if any alternative input (positive or negative) is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative alternative input releases.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any alternative input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool AltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].altUp || validGamepadIndex && gamepadsAccess[index][gamepadIndex].altUp;
		}
		/// <summary>
		/// Determines if any input (main or alternative, positive or negative) is currently being released (first frame of release).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a comprehensive check for any input being released, combining all possible
		/// release states into a single boolean result for simplified input handling.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.
		/// Each element corresponds to an input defined in the InputsManager's collection.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.
		/// This structure allows checking input states across multiple connected controllers.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection. A negative value will result in an early return of false.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value
		/// (InputsManager.DefaultGamepadIndexFallback), keyboard input will also be considered. Otherwise, if a valid
		/// gamepad is specified, keyboard inputs may be ignored to prioritize the gamepad.</param>
		/// <returns>
		/// True if any input is being released on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid. This provides a single check for all release events.
		/// </returns>
		internal static bool Up(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].up || validGamepadIndex && gamepadsAccess[index][gamepadIndex].up;
		}
		/// <summary>
		/// Determines if the positive main input is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for actions that should continue while a button is held down.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive main input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveMainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveMainHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveMainHeld;
		}
		/// <summary>
		/// Determines if the negative main input is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for actions that should continue while a button is held down in the negative direction.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative main input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeMainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeMainHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeMainHeld;
		}
		/// <summary>
		/// Determines if the positive alternative input is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// Alternative inputs provide secondary control options for the same action.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive alternative input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveAltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveAltHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveAltHeld;
		}
		/// <summary>
		/// Determines if the negative alternative input is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// Alternative inputs provide secondary control options for the same action in the negative direction.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative alternative input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeAltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeAltHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeAltHeld;
		}
		/// <summary>
		/// Determines if any positive input (main or alternative) is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method combines all positive input states into a single boolean result for simplified input handling.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any positive input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveHeld;
		}
		/// <summary>
		/// Determines if any negative input (main or alternative) is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method combines all negative input states into a single boolean result for simplified input handling.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any negative input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeHeld;
		}
		/// <summary>
		/// Determines if any main input (positive or negative) is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful when you need to detect if any main input binding is active, regardless of direction.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any main input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool MainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].mainHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainHeld;
		}
		/// <summary>
		/// Determines if any alternative input (positive or negative) is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful when you need to detect if any alternative input binding is active, regardless of direction.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any alternative input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool AltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].altHeld || validGamepadIndex && gamepadsAccess[index][gamepadIndex].altHeld;
		}
		/// <summary>
		/// Determines if any input (main or alternative, positive or negative) is currently being held down (continuous press).
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a comprehensive check for any input being held, combining all possible
		/// hold states into a single boolean result for simplified input handling.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any input is being held down on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool Hold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].held || validGamepadIndex && gamepadsAccess[index][gamepadIndex].held;
		}
		/// <summary>
		/// Determines if the positive main input has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// A double press occurs when the same input is pressed twice in quick succession.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive main input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveMainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveMainDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveMainDoublePress;
		}
		/// <summary>
		/// Determines if the negative main input has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting rapid successive presses of the negative main input.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative main input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeMainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeMainDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeMainDoublePress;
		}
		/// <summary>
		/// Determines if the positive alternative input has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting rapid successive presses of the positive alternative input,
		/// which can be used for secondary actions or special moves.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the positive alternative input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveAltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveAltDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveAltDoublePress;
		}
		/// <summary>
		/// Determines if the negative alternative input has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method is useful for detecting rapid successive presses of the negative alternative input,
		/// which can be used for secondary actions or special moves in the opposite direction.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if the negative alternative input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeAltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeAltDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeAltDoublePress;
		}
		/// <summary>
		/// Determines if any positive input (main or alternative) has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive main and positive alternative double presses,
		/// simplifying input handling when the distinction between main and alternative inputs is not needed.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any positive input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool PositiveDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].positiveDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].positiveDoublePress;
		}
		/// <summary>
		/// Determines if any negative input (main or alternative) has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both negative main and negative alternative double presses,
		/// simplifying input handling when the distinction between main and alternative inputs is not needed.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any negative input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool NegativeDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].negativeDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].negativeDoublePress;
		}
		/// <summary>
		/// Determines if any main input (positive or negative) has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative main double presses,
		/// simplifying input handling when the direction of the input is not important.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any main input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool MainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].mainDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].mainDoublePress;
		}
		/// <summary>
		/// Determines if any alternative input (positive or negative) has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a combined check for both positive and negative alternative double presses,
		/// simplifying input handling when the direction of the alternative input is not important.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any alternative input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool AltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].altDoublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].altDoublePress;
		}
		/// <summary>
		/// Determines if any input (main or alternative, positive or negative) has been double-pressed within the configured timeout period.
		/// Checks both keyboard and gamepad inputs based on the specified gamepad index.
		/// If a specific gamepad is requested (not the default fallback), keyboard inputs may be ignored.
		/// This method provides a comprehensive check for any double press, combining all possible
		/// double press states into a single boolean result for simplified input handling.
		/// The timeout period for detecting double presses is defined by InputsManager.DoublePressTimeout.
		/// </summary>
		/// <param name="keyboardAccess">The native array containing keyboard input access data for all registered inputs.</param>
		/// <param name="gamepadsAccess">A jagged array of native arrays containing gamepad input access data,
		/// where the first dimension represents input indices and the second dimension represents different gamepads.</param>
		/// <param name="index">The index of the input to check. This corresponds to the position
		/// in the InputsManager's input collection.</param>
		/// <param name="gamepadIndex">The index of the gamepad to check for input. If set to the default fallback value,
		/// keyboard input will also be considered.</param>
		/// <returns>
		/// True if any input has been double-pressed on either the keyboard or the specified gamepad,
		/// false otherwise or if the index is invalid.
		/// </returns>
		internal static bool DoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			if (index < 0)
				return false;

			bool validGamepadIndex = gamepadIndex > -1 && gamepadIndex < gamepadsAccess[index].Length;
			bool ignoreKeyboard = gamepadIndex != InputsManager.DefaultGamepadIndexFallback && validGamepadIndex;

			return !ignoreKeyboard && keyboardAccess[index].doublePress || validGamepadIndex && gamepadsAccess[index][gamepadIndex].doublePress;
		}
	}
}
