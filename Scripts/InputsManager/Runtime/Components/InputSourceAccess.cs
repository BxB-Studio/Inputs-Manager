#region Namespaces

using UnityEngine.InputSystem.Controls;

#endregion

namespace Utilities.Inputs.Components
{
	/// <summary>
	/// A struct that contains the access information for an input source.
	/// Provides a comprehensive representation of an input's state including press, hold, and double-press detection
	/// for both keyboard and gamepad controls. This struct maintains all state information needed for input processing.
	/// </summary>
	public struct InputSourceAccess
	{
		#region Variables

		/// <summary>
		/// The source of the input (Keyboard or Gamepad).
		/// Determines which input device is being used for this input access instance.
		/// </summary>
		public InputSource source;

		/// <summary>
		/// The positive main hold timer.
		/// Tracks how long the positive main input has been held down, used for hold detection.
		/// </summary>
		public float positiveMainHoldTimer;
		/// <summary>
		/// The negative main hold timer.
		/// Tracks how long the negative main input has been held down, used for hold detection.
		/// </summary>
		public float negativeMainHoldTimer;
		/// <summary>
		/// The positive alt hold timer.
		/// Tracks how long the positive alternative input has been held down, used for hold detection.
		/// </summary>
		public float positiveAltHoldTimer;
		/// <summary>
		/// The negative alt hold timer.
		/// Tracks how long the negative alternative input has been held down, used for hold detection.
		/// </summary>
		public float negativeAltHoldTimer;
		/// <summary>
		/// The positive main double press timer.
		/// Tracks the time between consecutive presses of the positive main input for double-press detection.
		/// </summary>
		public float positiveMainDoublePressTimer;
		/// <summary>
		/// The negative main double press timer.
		/// Tracks the time between consecutive presses of the negative main input for double-press detection.
		/// </summary>
		public float negativeMainDoublePressTimer;
		/// <summary>
		/// The positive alt double press timer.
		/// Tracks the time between consecutive presses of the positive alternative input for double-press detection.
		/// </summary>
		public float positiveAltDoublePressTimer;
		/// <summary>
		/// The negative alt double press timer.
		/// Tracks the time between consecutive presses of the negative alternative input for double-press detection.
		/// </summary>
		public float negativeAltDoublePressTimer;
		/// <summary>
		/// The positive main value.
		/// Represents the analog value (0-1) of the positive main input, particularly useful for gamepad inputs.
		/// </summary>
		public float positiveMainValue;
		/// <summary>
		/// The negative main value.
		/// Represents the analog value (0-1) of the negative main input, particularly useful for gamepad inputs.
		/// </summary>
		public float negativeMainValue;
		/// <summary>
		/// The positive alt value.
		/// Represents the analog value (0-1) of the positive alternative input, particularly useful for gamepad inputs.
		/// </summary>
		public float positiveAltValue;
		/// <summary>
		/// The negative alt value.
		/// Represents the analog value (0-1) of the negative alternative input, particularly useful for gamepad inputs.
		/// </summary>
		public float negativeAltValue;
		/// <summary>
		/// The main value.
		/// Combined value from the main input axis, calculated from positive and negative main values.
		/// </summary>
		public float mainValue;
		/// <summary>
		/// The alt value.
		/// Combined value from the alternative input axis, calculated from positive and negative alt values.
		/// </summary>
		public float altValue;
		/// <summary>
		/// The value.
		/// The final combined input value, representing the overall input state from all sources.
		/// </summary>
		public float value;

		/// <summary>
		/// Whether the positive main is pressed.
		/// Indicates if the positive main input is currently being pressed (held down).
		/// </summary>
		public bool positiveMainPress;
		/// <summary>
		/// Whether the negative main is pressed.
		/// Indicates if the negative main input is currently being pressed (held down).
		/// </summary>
		public bool negativeMainPress;
		/// <summary>
		/// Whether the positive alt is pressed.
		/// Indicates if the positive alternative input is currently being pressed (held down).
		/// </summary>
		public bool positiveAltPress;
		/// <summary>
		/// Whether the negative alt is pressed.
		/// Indicates if the negative alternative input is currently being pressed (held down).
		/// </summary>
		public bool negativeAltPress;
		/// <summary>
		/// Whether the positive is pressed.
		/// Indicates if any positive input (main or alt) is currently being pressed.
		/// </summary>
		public bool positivePress;
		/// <summary>
		/// Whether the negative is pressed.
		/// Indicates if any negative input (main or alt) is currently being pressed.
		/// </summary>
		public bool negativePress;
		/// <summary>
		/// Whether the main is pressed.
		/// Indicates if any main input (positive or negative) is currently being pressed.
		/// </summary>
		public bool mainPress;
		/// <summary>
		/// Whether the alt is pressed.
		/// Indicates if any alternative input (positive or negative) is currently being pressed.
		/// </summary>
		public bool altPress;
		/// <summary>
		/// Whether the input is pressed.
		/// Indicates if any input (main or alt, positive or negative) is currently being pressed.
		/// </summary>
		public bool press;

		/// <summary>
		/// Whether the positive main is down.
		/// Indicates if the positive main input was just pressed this frame (initial press).
		/// </summary>
		public bool positiveMainDown;
		/// <summary>
		/// Whether the negative main is down.
		/// Indicates if the negative main input was just pressed this frame (initial press).
		/// </summary>
		public bool negativeMainDown;
		/// <summary>
		/// Whether the positive alt is down.
		/// Indicates if the positive alternative input was just pressed this frame (initial press).
		/// </summary>
		public bool positiveAltDown;
		/// <summary>
		/// Whether the negative alt is down.
		/// Indicates if the negative alternative input was just pressed this frame (initial press).
		/// </summary>
		public bool negativeAltDown;
		/// <summary>
		/// Whether the positive is down.
		/// Indicates if any positive input (main or alt) was just pressed this frame.
		/// </summary>
		public bool positiveDown;
		/// <summary>
		/// Whether the negative is down.
		/// Indicates if any negative input (main or alt) was just pressed this frame.
		/// </summary>
		public bool negativeDown;
		/// <summary>
		/// Whether the main is down.
		/// Indicates if any main input (positive or negative) was just pressed this frame.
		/// </summary>
		public bool mainDown;
		/// <summary>
		/// Whether the alt is down.
		/// Indicates if any alternative input (positive or negative) was just pressed this frame.
		/// </summary>
		public bool altDown;
		/// <summary>
		/// Whether the input is down.
		/// Indicates if any input (main or alt, positive or negative) was just pressed this frame.
		/// </summary>
		public bool down;

		/// <summary>
		/// Whether the positive main is up.
		/// Indicates if the positive main input was just released this frame.
		/// </summary>
		public bool positiveMainUp;
		/// <summary>
		/// Whether the negative main is up.
		/// Indicates if the negative main input was just released this frame.
		/// </summary>
		public bool negativeMainUp;
		/// <summary>
		/// Whether the positive alt is up.
		/// Indicates if the positive alternative input was just released this frame.
		/// </summary>
		public bool positiveAltUp;
		/// <summary>
		/// Whether the negative alt is up.
		/// Indicates if the negative alternative input was just released this frame.
		/// </summary>
		public bool negativeAltUp;
		/// <summary>
		/// Whether the positive is up.
		/// Indicates if any positive input (main or alt) was just released this frame.
		/// </summary>
		public bool positiveUp;
		/// <summary>
		/// Whether the negative is up.
		/// Indicates if any negative input (main or alt) was just released this frame.
		/// </summary>
		public bool negativeUp;
		/// <summary>
		/// Whether the main is up.
		/// Indicates if any main input (positive or negative) was just released this frame.
		/// </summary>
		public bool mainUp;
		/// <summary>
		/// Whether the alt is up.
		/// Indicates if any alternative input (positive or negative) was just released this frame.
		/// </summary>
		public bool altUp;
		/// <summary>
		/// Whether the input is up.
		/// Indicates if any input (main or alt, positive or negative) was just released this frame.
		/// </summary>
		public bool up;

		/// <summary>
		/// Whether the positive main is held.
		/// Indicates if the positive main input has been held down for longer than the hold threshold.
		/// </summary>
		public bool positiveMainHeld;
		/// <summary>
		/// Whether the negative main is held.
		/// Indicates if the negative main input has been held down for longer than the hold threshold.
		/// </summary>
		public bool negativeMainHeld;
		/// <summary>
		/// Whether the positive alt is held.
		/// Indicates if the positive alternative input has been held down for longer than the hold threshold.
		/// </summary>
		public bool positiveAltHeld;
		/// <summary>
		/// Whether the negative alt is held.
		/// Indicates if the negative alternative input has been held down for longer than the hold threshold.
		/// </summary>
		public bool negativeAltHeld;
		/// <summary>
		/// Whether the positive is held.
		/// Indicates if any positive input (main or alt) has been held down for longer than the hold threshold.
		/// </summary>
		public bool positiveHeld;
		/// <summary>
		/// Whether the negative is held.
		/// Indicates if any negative input (main or alt) has been held down for longer than the hold threshold.
		/// </summary>
		public bool negativeHeld;
		/// <summary>
		/// Whether the main is held.
		/// Indicates if any main input (positive or negative) has been held down for longer than the hold threshold.
		/// </summary>
		public bool mainHeld;
		/// <summary>
		/// Whether the alt is held.
		/// Indicates if any alternative input (positive or negative) has been held down for longer than the hold threshold.
		/// </summary>
		public bool altHeld;
		/// <summary>
		/// Whether the input is held.
		/// Indicates if any input (main or alt, positive or negative) has been held down for longer than the hold threshold.
		/// </summary>
		public bool held;

		/// <summary>
		/// Whether the positive main double press is initiated.
		/// Tracks if the first press of a potential double-press sequence has occurred for the positive main input.
		/// </summary>
		public bool positiveMainDoublePressInitiated;
		/// <summary>
		/// Whether the negative main double press is initiated.
		/// Tracks if the first press of a potential double-press sequence has occurred for the negative main input.
		/// </summary>
		public bool negativeMainDoublePressInitiated;
		/// <summary>
		/// Whether the positive alt double press is initiated.
		/// Tracks if the first press of a potential double-press sequence has occurred for the positive alternative input.
		/// </summary>
		public bool positiveAltDoublePressInitiated;
		/// <summary>
		/// Whether the negative alt double press is initiated.
		/// Tracks if the first press of a potential double-press sequence has occurred for the negative alternative input.
		/// </summary>
		public bool negativeAltDoublePressInitiated;

		/// <summary>
		/// Whether the positive main double press has occurred.
		/// Indicates if a complete double-press has been detected for the positive main input.
		/// </summary>
		public bool positiveMainDoublePress;
		/// <summary>
		/// Whether the negative main double press has occurred.
		/// Indicates if a complete double-press has been detected for the negative main input.
		/// </summary>
		public bool negativeMainDoublePress;
		/// <summary>
		/// Whether the positive alt double press has occurred.
		/// Indicates if a complete double-press has been detected for the positive alternative input.
		/// </summary>
		public bool positiveAltDoublePress;
		/// <summary>
		/// Whether the negative alt double press has occurred.
		/// Indicates if a complete double-press has been detected for the negative alternative input.
		/// </summary>
		public bool negativeAltDoublePress;
		/// <summary>
		/// Whether the positive double press has occurred.
		/// Indicates if a complete double-press has been detected for any positive input (main or alt).
		/// </summary>
		public bool positiveDoublePress;
		/// <summary>
		/// Whether the negative double press has occurred.
		/// Indicates if a complete double-press has been detected for any negative input (main or alt).
		/// </summary>
		public bool negativeDoublePress;
		/// <summary>
		/// Whether the main double press has occurred.
		/// Indicates if a complete double-press has been detected for any main input (positive or negative).
		/// </summary>
		public bool mainDoublePress;
		/// <summary>
		/// Whether the alt double press has occurred.
		/// Indicates if a complete double-press has been detected for any alternative input (positive or negative).
		/// </summary>
		public bool altDoublePress;
		/// <summary>
		/// Whether the double press has occurred.
		/// Indicates if a complete double-press has been detected for any input (main or alt, positive or negative).
		/// </summary>
		public bool doublePress;

		/// <summary>
		/// Whether the positive main is bindable.
		/// Indicates if the positive main input can be bound to a control (has a valid binding configuration).
		/// </summary>
		public bool positiveMainBindable;
		/// <summary>
		/// Whether the negative main is bindable.
		/// Indicates if the negative main input can be bound to a control (has a valid binding configuration).
		/// </summary>
		public bool negativeMainBindable;
		/// <summary>
		/// Whether the positive alt is bindable.
		/// Indicates if the positive alternative input can be bound to a control (has a valid binding configuration).
		/// </summary>
		public bool positiveAltBindable;
		/// <summary>
		/// Whether the negative alt is bindable.
		/// Indicates if the negative alternative input can be bound to a control (has a valid binding configuration).
		/// </summary>
		public bool negativeAltBindable;
		/// <summary>
		/// Whether the positive is bindable.
		/// Indicates if any positive input (main or alt) can be bound to a control.
		/// </summary>
		public bool positiveBindable;
		/// <summary>
		/// Whether the negative is bindable.
		/// Indicates if any negative input (main or alt) can be bound to a control.
		/// </summary>
		public bool negativeBindable;
		/// <summary>
		/// Whether the main is bindable.
		/// Indicates if any main input (positive or negative) can be bound to a control.
		/// </summary>
		public bool mainBindable;
		/// <summary>
		/// Whether the alt is bindable.
		/// Indicates if any alternative input (positive or negative) can be bound to a control.
		/// </summary>
		public bool altBindable;
		/// <summary>
		/// Whether the input is bindable.
		/// Indicates if any input (main or alt, positive or negative) can be bound to a control.
		/// </summary>
		public bool bindable;

		#endregion

		#region Utilities

		/// <summary>
		/// Updates the controls for the keyboard input source.
		/// Refreshes all keyboard input states (press, down, up) for both main and alternative bindings
		/// based on the current state of the associated input controls.
		/// </summary>
		/// <param name="input">The input to update the controls for, containing references to keyboard controls.</param>
		/// <returns>The updated InputSourceAccess instance with refreshed keyboard input states.</returns>
		public InputSourceAccess UpdateControls(Input input)
		{
			if (!bindable || source != InputSource.Keyboard)
				return this;

			if (mainBindable)
			{
				if (positiveMainBindable)
				{
					positiveMainPress = input.keyboardPositiveMainControl?.isPressed ?? false;
					positiveMainDown = input.keyboardPositiveMainControl?.wasPressedThisFrame ?? false;
					positiveMainUp = input.keyboardPositiveMainControl?.wasReleasedThisFrame ?? false;
				}

				if (negativeMainBindable)
				{
					negativeMainPress = input.keyboardNegativeMainControl?.isPressed ?? false;
					negativeMainDown = input.keyboardNegativeMainControl?.wasPressedThisFrame ?? false;
					negativeMainUp = input.keyboardNegativeMainControl?.wasReleasedThisFrame ?? false;
				}
			}

			if (altBindable)
			{
				if (positiveAltBindable)
				{
					positiveAltPress = input.keyboardPositiveAltControl?.isPressed ?? false;
					positiveAltDown = input.keyboardPositiveAltControl?.wasPressedThisFrame ?? false;
					positiveAltUp = input.keyboardPositiveAltControl?.wasReleasedThisFrame ?? false;
				}

				if (negativeAltBindable)
				{
					negativeAltPress = input.keyboardNegativeAltControl?.isPressed ?? false;
					negativeAltDown = input.keyboardNegativeAltControl?.wasPressedThisFrame ?? false;
					negativeAltUp = input.keyboardNegativeAltControl?.wasReleasedThisFrame ?? false;
				}
			}

			return this;
		}
		/// <summary>
		/// Updates the controls for the gamepad input source.
		/// Refreshes all gamepad input states (press, down, up) and analog values for both main and alternative bindings
		/// based on the current state of the associated gamepad controls for the specified gamepad.
		/// </summary>
		/// <param name="input">The input to update the controls for, containing references to gamepad controls.</param>
		/// <param name="gamepadIndex">The index of the gamepad to update the controls for, allowing multi-controller support.</param>
		/// <returns>The updated InputSourceAccess instance with refreshed gamepad input states and values.</returns>
		public InputSourceAccess UpdateControls(Input input, int gamepadIndex)
		{
			if (!bindable || source != InputSource.Gamepad)
				return this;

			if (mainBindable)
			{
				if (positiveMainBindable)
				{
					ButtonControl buttonControl = input.gamepadPositiveMainControls[gamepadIndex];

					if (buttonControl != null)
					{
						positiveMainValue = buttonControl.value;
						positiveMainPress = buttonControl.isPressed;
						positiveMainDown = buttonControl.wasPressedThisFrame;
						positiveMainUp = buttonControl.wasReleasedThisFrame;
					}
				}

				if (negativeMainBindable)
				{
					ButtonControl buttonControl = input.gamepadNegativeMainControls[gamepadIndex];

					if (buttonControl != null)
					{
						negativeMainValue = buttonControl.value;
						negativeMainPress = buttonControl.isPressed;
						negativeMainDown = buttonControl.wasPressedThisFrame;
						negativeMainUp = buttonControl.wasReleasedThisFrame;
					}
				}
			}

			if (altBindable)
			{
				if (positiveAltBindable)
				{
					ButtonControl buttonControl = input.gamepadPositiveAltControls[gamepadIndex];

					if (buttonControl != null)
					{
						positiveAltValue = buttonControl.value;
						positiveAltPress = buttonControl.isPressed;
						positiveAltDown = buttonControl.wasPressedThisFrame;
						positiveAltUp = buttonControl.wasReleasedThisFrame;
					}
				}

				if (negativeAltBindable)
				{
					ButtonControl buttonControl = input.gamepadNegativeAltControls[gamepadIndex];

					if (buttonControl != null)
					{
						negativeAltValue = buttonControl.value;
						negativeAltPress = buttonControl.isPressed;
						negativeAltDown = buttonControl.wasPressedThisFrame;
						negativeAltUp = buttonControl.wasReleasedThisFrame;
					}
				}
			}

			return this;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an InputSourceAccess struct from an Input object and an InputSource.
		/// Initializes the struct with bindable state information based on the input source type (Keyboard or Gamepad),
		/// setting up the appropriate flags to indicate which input bindings are available and can be processed.
		/// </summary>
		/// <param name="input">The input to construct the InputSourceAccess struct from, containing binding configurations.</param>
		/// <param name="source">The source of the input (Keyboard or Gamepad), determining which binding set to use.</param>
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
