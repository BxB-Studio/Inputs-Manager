#region Namespaces

using UnityEngine;
using UnityEngine.InputSystem.Controls;

#endregion

namespace Utilities.Inputs.Components
{
	public struct InputSourceAccess
	{
		#region Variables

		public InputSource source;

		public float positiveMainHoldTimer;
		public float negativeMainHoldTimer;
		public float positiveAltHoldTimer;
		public float negativeAltHoldTimer;

		public float positiveMainDoublePressTimer;
		public float negativeMainDoublePressTimer;
		public float positiveAltDoublePressTimer;
		public float negativeAltDoublePressTimer;

		public float positiveMainValue;
		public float negativeMainValue;
		public float positiveAltValue;
		public float negativeAltValue;
		public float mainValue;
		public float altValue;
		public float value;

		public bool positiveMainPress;
		public bool negativeMainPress;
		public bool positiveAltPress;
		public bool negativeAltPress;
		public bool positivePress;
		public bool negativePress;
		public bool mainPress;
		public bool altPress;
		public bool press;

		public bool positiveMainDown;
		public bool negativeMainDown;
		public bool positiveAltDown;
		public bool negativeAltDown;
		public bool positiveDown;
		public bool negativeDown;
		public bool mainDown;
		public bool altDown;
		public bool down;

		public bool positiveMainUp;
		public bool negativeMainUp;
		public bool positiveAltUp;
		public bool negativeAltUp;
		public bool positiveUp;
		public bool negativeUp;
		public bool mainUp;
		public bool altUp;
		public bool up;

		public bool positiveMainHeld;
		public bool negativeMainHeld;
		public bool positiveAltHeld;
		public bool negativeAltHeld;
		public bool positiveHeld;
		public bool negativeHeld;
		public bool mainHeld;
		public bool altHeld;
		public bool held;

		public bool positiveMainDoublePressInitiated;
		public bool negativeMainDoublePressInitiated;
		public bool positiveAltDoublePressInitiated;
		public bool negativeAltDoublePressInitiated;

		public bool positiveMainDoublePress;
		public bool negativeMainDoublePress;
		public bool positiveAltDoublePress;
		public bool negativeAltDoublePress;
		public bool positiveDoublePress;
		public bool negativeDoublePress;
		public bool mainDoublePress;
		public bool altDoublePress;
		public bool doublePress;

		public bool positiveMainBindable;
		public bool negativeMainBindable;
		public bool positiveAltBindable;
		public bool negativeAltBindable;
		public bool positiveBindable;
		public bool negativeBindable;
		public bool mainBindable;
		public bool altBindable;
		public bool bindable;

		#endregion

		#region Utilities

		public InputSourceAccess UpdateControls(Input input)
		{
			if (!bindable || source != InputSource.Keyboard)
				return this;

			if (mainBindable)
			{
				if (positiveMainBindable)
				{
					positiveMainPress = input.keyboardPositiveMainControl.isPressed;
					positiveMainDown = input.keyboardPositiveMainControl.wasPressedThisFrame;
					positiveMainUp = input.keyboardPositiveMainControl.wasReleasedThisFrame;
				}

				if (negativeMainBindable)
				{
					negativeMainPress = input.keyboardNegativeMainControl.isPressed;
					negativeMainDown = input.keyboardNegativeMainControl.wasPressedThisFrame;
					negativeMainUp = input.keyboardNegativeMainControl.wasReleasedThisFrame;
				}
			}

			if (altBindable)
			{
				if (positiveAltBindable)
				{
					positiveAltPress = input.keyboardPositiveAltControl.isPressed;
					positiveAltDown = input.keyboardPositiveAltControl.wasPressedThisFrame;
					positiveAltUp = input.keyboardPositiveAltControl.wasReleasedThisFrame;
				}

				if (negativeAltBindable)
				{
					negativeAltPress = input.keyboardNegativeAltControl.isPressed;
					negativeAltDown = input.keyboardNegativeAltControl.wasPressedThisFrame;
					negativeAltUp = input.keyboardNegativeAltControl.wasReleasedThisFrame;
				}
			}

			return this;
		}
		public InputSourceAccess UpdateControls(Input input, int gamepadIndex)
		{
			if (!bindable || source != InputSource.Gamepad)
				return this;

			if (mainBindable)
			{
				if (positiveMainBindable)
				{
					ButtonControl buttonControl = input.gamepadPositiveMainControls[gamepadIndex];
					
					positiveMainValue = buttonControl.value;
					positiveMainPress = buttonControl.isPressed;
					positiveMainDown = buttonControl.wasPressedThisFrame;
					positiveMainUp = buttonControl.wasReleasedThisFrame;
				}

				if (negativeMainBindable)
				{
					ButtonControl buttonControl = input.gamepadNegativeMainControls[gamepadIndex];

					negativeMainValue = buttonControl.value;
					negativeMainPress = buttonControl.isPressed;
					negativeMainDown = buttonControl.wasPressedThisFrame;
					negativeMainUp = buttonControl.wasReleasedThisFrame;
				}
			}

			if (altBindable)
			{
				if (positiveAltBindable)
				{
					ButtonControl buttonControl = input.gamepadPositiveAltControls[gamepadIndex];

					positiveAltValue = buttonControl.value;
					positiveAltPress = buttonControl.isPressed;
					positiveAltDown = buttonControl.wasPressedThisFrame;
					positiveAltUp = buttonControl.wasReleasedThisFrame;
				}

				if (negativeAltBindable)
				{
					ButtonControl buttonControl = input.gamepadNegativeAltControls[gamepadIndex];

					negativeAltValue = buttonControl.value;
					negativeAltPress = buttonControl.isPressed;
					negativeAltDown = buttonControl.wasPressedThisFrame;
					negativeAltUp = buttonControl.wasReleasedThisFrame;
				}
			}

			return this;
		}

		#endregion

		#region Constructors

		public InputSourceAccess(Input input, InputSource source) : this()
		{
			this.source = source;

			switch (source)
			{
				case InputSource.Gamepad:
					positiveMainBindable = input.GamepadPositiveMainBindable;
					negativeMainBindable = input.GamepadNegativeMainBindable;
					positiveAltBindable = input.GamepadPositiveAltBindable;
					negativeAltBindable = input.GamepadNegativeAltBindable;
					positiveBindable = input.GamepadPositiveBindable;
					negativeBindable = input.GamepadNegativeBindable;
					mainBindable = input.GamepadMainBindable;
					altBindable = input.GamepadAltBindable;
					bindable = input.GamepadBindable;

					break;

				default:
					positiveMainBindable = input.KeyboardPositiveMainBindable;
					negativeMainBindable = input.KeyboardNegativeMainBindable;
					positiveAltBindable = input.KeyboardPositiveAltBindable;
					negativeAltBindable = input.KeyboardNegativeAltBindable;
					positiveBindable = input.KeyboardPositiveBindable;
					negativeBindable = input.KeyboardNegativeBindable;
					mainBindable = input.KeyboardMainBindable;
					altBindable = input.KeyboardAltBindable;
					bindable = input.KeyboardBindable;

					break;
			}
		}

		#endregion
	}
}
