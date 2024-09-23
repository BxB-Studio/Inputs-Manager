#region Namespaces

using Unity.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Utilities.Inputs.Components;

#endregion

namespace Utilities.Inputs
{
	public static class InputUtilities
	{
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

		internal static float MainValue(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainValue != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool keyboardUsed = keyboardAccess[index].mainValue != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed)
				return gamepadsAccess[index][gamepadIndex].mainValue;
			else
				return keyboardAccess[index].mainValue;
		}
		internal static float AltValue(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altValue != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool keyboardUsed = keyboardAccess[index].altValue != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed)
				return gamepadsAccess[index][gamepadIndex].altValue;
			else
				return keyboardAccess[index].altValue;
		}
		internal static float Value(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].value != 0f;
			bool gamepadPrioritized = InputsManager.InputSourcePriority == InputSource.Gamepad;
			bool keyboardUsed = keyboardAccess[index].value != 0f;

			if ((gamepadPrioritized || !keyboardUsed) && gamepadUsed)
				return gamepadsAccess[index][gamepadIndex].value;
			else
				return keyboardAccess[index].value;
		}

		internal static bool PositiveMainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveMainPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveMainPress;
		}
		internal static bool NegativeMainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeMainPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeMainPress;
		}
		internal static bool PositiveAltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveAltPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveAltPress;
		}
		internal static bool NegativeAltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeAltPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeAltPress;
		}
		internal static bool PositivePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positivePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positivePress;
		}
		internal static bool NegativePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativePress;
		}
		internal static bool MainPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].mainPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainPress;
		}
		internal static bool AltPress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].altPress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altPress;
		}
		internal static bool Press(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].press || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].press;
		}

		internal static bool PositiveMainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveMainDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveMainDown;
		}
		internal static bool NegativeMainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeMainDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeMainDown;
		}
		internal static bool PositiveAltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveAltDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveAltDown;
		}
		internal static bool NegativeAltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeAltDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeAltDown;
		}
		internal static bool PositiveDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveDown;
		}
		internal static bool NegativeDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeDown;
		}
		internal static bool MainDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].mainDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainDown;
		}
		internal static bool AltDown(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].altDown || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altDown;
		}
		internal static bool Down(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].down || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].down;
		}

		internal static bool PositiveMainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveMainUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveMainUp;
		}
		internal static bool NegativeMainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeMainUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeMainUp;
		}
		internal static bool PositiveAltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveAltUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveAltUp;
		}
		internal static bool NegativeAltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeAltUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeAltUp;
		}
		internal static bool PositiveUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveUp;
		}
		internal static bool NegativeUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeUp;
		}
		internal static bool MainUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].mainUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainUp;
		}
		internal static bool AltUp(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].altUp || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altUp;
		}
		internal static bool Up(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].up || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].up;
		}

		internal static bool PositiveMainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveMainHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveMainHeld;
		}
		internal static bool NegativeMainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeMainHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeMainHeld;
		}
		internal static bool PositiveAltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveAltHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveAltHeld;
		}
		internal static bool NegativeAltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeAltHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeAltHeld;
		}
		internal static bool PositiveHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveHeld;
		}
		internal static bool NegativeHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeHeld;
		}
		internal static bool MainHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].mainHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainHeld;
		}
		internal static bool AltHold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].altHeld || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altHeld;
		}
		internal static bool Hold(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].held || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].held;
		}

		internal static bool PositiveMainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveMainDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveMainDoublePress;
		}
		internal static bool NegativeMainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeMainDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeMainDoublePress;
		}
		internal static bool PositiveAltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveAltDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveAltDoublePress;
		}
		internal static bool NegativeAltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeAltDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeAltDoublePress;
		}
		internal static bool PositiveDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].positiveDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].positiveDoublePress;
		}
		internal static bool NegativeDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].negativeDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].negativeDoublePress;
		}
		internal static bool MainDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].mainDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].mainDoublePress;
		}
		internal static bool AltDoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].altDoublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].altDoublePress;
		}
		internal static bool DoublePress(NativeArray<InputSourceAccess> keyboardAccess, NativeArray<InputSourceAccess>[] gamepadsAccess, int index, int gamepadIndex)
		{
			return keyboardAccess[index].doublePress || gamepadIndex > -1 && gamepadIndex < InputsManager.GamepadsCount && gamepadsAccess[index][gamepadIndex].doublePress;
		}
	}
}