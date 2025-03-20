#region Namespaces

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#endregion

namespace Utilities.Inputs
{
	/// <summary>
	/// A class that represents an input with support for keyboard and gamepad controls, 
	/// providing various input types, interpolation methods, and binding capabilities.
	/// </summary>
	[Serializable]
	public class Input
	{
		#region Variables

		/// <summary>
		/// Gets or sets the name of the input. Changes are only allowed outside of play mode
		/// and will mark the InputsManager data as changed when modified.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (Application.isPlaying)
					return;

				InputsManager.DataChanged = InputsManager.DataChanged || !value.IsNullOrEmpty() && name != value;
				name = value;
			}
		}
		/// <summary>
		/// Gets or sets the type of the input (Button or Axis). Changes are only allowed outside of play mode.
		/// When set to Button type, negative bindings are cleared and value interval is adjusted accordingly.
		/// </summary>
		public InputType Type
		{
			get
			{
				return type;
			}
			set
			{
				if (Application.isPlaying)
					return;

				if (value == InputType.Button)
				{
					valueInterval.x = 0f;
					Main.Negative = Key.None;
					Alt.Negative = Key.None;
				}

				type = value;
				InputsManager.DataChanged = true;
			}
		}
		/// <summary>
		/// Gets or sets the interpolation method used for smoothing input values.
		/// Determines how input transitions between values (None, Smooth, or Jump).
		/// </summary>
		public InputInterpolation Interpolation
		{
			get
			{
				return interpolation;
			}
			set
			{
				interpolation = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets the minimum and maximum values for the input range.
		/// For buttons, this is typically 0 to 1, while axes can use custom ranges.
		/// </summary>
		public Vector2 ValueInterval
		{
			get
			{
				return valueInterval;
			}
			set
			{
				valueInterval = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets whether the input values should be inverted.
		/// When true, positive inputs become negative and vice versa.
		/// </summary>
		public bool Invert
		{
			get
			{
				return invert;
			}
			set
			{
				invert = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets the primary input axis configuration.
		/// Contains the positive and negative bindings for both keyboard and gamepad inputs.
		/// Changes are only allowed outside of play mode and with valid values.
		/// </summary>
		public InputAxis Main
		{
			get
			{
				return main;
			}
			set
			{
				if (Application.isPlaying || !value)
					return;

				main = value;
				InputsManager.DataChanged = true;
			}
		}
		/// <summary>
		/// Gets or sets the secondary (alternative) input axis configuration.
		/// Provides fallback bindings when the main axis is not activated.
		/// Changes are only allowed outside of play mode and with valid values.
		/// </summary>
		public InputAxis Alt
		{
			get
			{
				return alt;
			}
			set
			{
				if (Application.isPlaying || !value)
					return;

				main = value;
				InputsManager.DataChanged = true;
			}
		}

		/// <summary>
		/// Indicates whether the positive main keyboard key is bindable (not set to None).
		/// Used internally to determine if keyboard input processing is needed.
		/// </summary>
		internal bool KeyboardPositiveMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether the positive main gamepad button is bindable (not set to None).
		/// Used internally to determine if gamepad input processing is needed.
		/// </summary>
		internal bool GamepadPositiveMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether the negative main keyboard key is bindable (not set to None).
		/// Used internally to determine if keyboard input processing is needed for negative values.
		/// </summary>
		internal bool KeyboardNegativeMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether the negative main gamepad button is bindable (not set to None).
		/// Used internally to determine if gamepad input processing is needed for negative values.
		/// </summary>
		internal bool GamepadNegativeMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether the positive alternative keyboard key is bindable (not set to None).
		/// Used internally to determine if alternative keyboard input processing is needed.
		/// </summary>
		internal bool KeyboardPositiveAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether the positive alternative gamepad button is bindable (not set to None).
		/// Used internally to determine if alternative gamepad input processing is needed.
		/// </summary>
		internal bool GamepadPositiveAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether the negative alternative keyboard key is bindable (not set to None).
		/// Used internally to determine if alternative keyboard input processing is needed for negative values.
		/// </summary>
		internal bool KeyboardNegativeAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether the negative alternative gamepad button is bindable (not set to None).
		/// Used internally to determine if alternative gamepad input processing is needed for negative values.
		/// </summary>
		internal bool GamepadNegativeAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether any positive keyboard key (main or alt) is bindable.
		/// Combines KeyboardPositiveMainBindable and KeyboardPositiveAltBindable for convenience.
		/// </summary>
		internal bool KeyboardPositiveBindable { get; private set; }
		/// <summary>
		/// Indicates whether any positive gamepad button (main or alt) is bindable.
		/// Combines GamepadPositiveMainBindable and GamepadPositiveAltBindable for convenience.
		/// </summary>
		internal bool GamepadPositiveBindable { get; private set; }
		/// <summary>
		/// Indicates whether any negative keyboard key (main or alt) is bindable.
		/// Combines KeyboardNegativeMainBindable and KeyboardNegativeAltBindable for convenience.
		/// </summary>
		internal bool KeyboardNegativeBindable { get; private set; }
		/// <summary>
		/// Indicates whether any negative gamepad button (main or alt) is bindable.
		/// Combines GamepadNegativeMainBindable and GamepadNegativeAltBindable for convenience.
		/// </summary>
		internal bool GamepadNegativeBindable { get; private set; }
		/// <summary>
		/// Indicates whether any main keyboard key (positive or negative) is bindable.
		/// Combines KeyboardPositiveMainBindable and KeyboardNegativeMainBindable for convenience.
		/// </summary>
		internal bool KeyboardMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether any main gamepad button (positive or negative) is bindable.
		/// Combines GamepadPositiveMainBindable and GamepadNegativeMainBindable for convenience.
		/// </summary>
		internal bool GamepadMainBindable { get; private set; }
		/// <summary>
		/// Indicates whether any alternative keyboard key (positive or negative) is bindable.
		/// Combines KeyboardPositiveAltBindable and KeyboardNegativeAltBindable for convenience.
		/// </summary>
		internal bool KeyboardAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether any alternative gamepad button (positive or negative) is bindable.
		/// Combines GamepadPositiveAltBindable and GamepadNegativeAltBindable for convenience.
		/// </summary>
		internal bool GamepadAltBindable { get; private set; }
		/// <summary>
		/// Indicates whether any keyboard key (main or alt, positive or negative) is bindable.
		/// Combines KeyboardMainBindable and KeyboardAltBindable for convenience.
		/// </summary>
		internal bool KeyboardBindable { get; private set; }
		/// <summary>
		/// Indicates whether any gamepad button (main or alt, positive or negative) is bindable.
		/// Combines GamepadMainBindable and GamepadAltBindable for convenience.
		/// </summary>
		internal bool GamepadBindable { get; private set; }

		/// <summary>
		/// The name of the input.
		/// </summary>
		[SerializeField]
		private string name;
		/// <summary>
		/// The type of the input (Button or Axis).
		/// </summary>
		[SerializeField]
		private InputType type;
		/// <summary>
		/// The main axis of the input, containing primary keyboard and gamepad bindings.
		/// </summary>
		[SerializeField]
		private InputAxis main;
		/// <summary>
		/// The alt axis of the input, containing secondary keyboard and gamepad bindings.
		/// </summary>
		[SerializeField]
		private InputAxis alt;
		/// <summary>
		/// The interpolation method used for smoothing input values.
		/// </summary>
		[SerializeField]
		private InputInterpolation interpolation;
		/// <summary>
		/// The value interval of the input, defining the minimum and maximum range.
		/// </summary>
		[SerializeField]
		private Utility.SerializableVector2 valueInterval;
		/// <summary>
		/// Whether the input values should be inverted.
		/// </summary>
		[SerializeField]
		private bool invert;

		/// <summary>
		/// Reference to the keyboard control for the positive main binding.
		/// Used for efficient input polling during runtime.
		/// </summary>
		[NonSerialized]
		internal KeyControl keyboardPositiveMainControl;
		/// <summary>
		/// Array of gamepad controls for the positive main binding across all connected gamepads.
		/// Each index corresponds to a different gamepad device.
		/// </summary>
		[NonSerialized]
		internal ButtonControl[] gamepadPositiveMainControls;
		/// <summary>
		/// Reference to the keyboard control for the negative main binding.
		/// Used for efficient input polling during runtime.
		/// </summary>
		[NonSerialized]
		internal KeyControl keyboardNegativeMainControl;
		/// <summary>
		/// Array of gamepad controls for the negative main binding across all connected gamepads.
		/// Each index corresponds to a different gamepad device.
		/// </summary>
		[NonSerialized]
		internal ButtonControl[] gamepadNegativeMainControls;
		/// <summary>
		/// Reference to the keyboard control for the positive alternative binding.
		/// Used for efficient input polling during runtime.
		/// </summary>
		[NonSerialized]
		internal KeyControl keyboardPositiveAltControl;
		/// <summary>
		/// Array of gamepad controls for the positive alternative binding across all connected gamepads.
		/// Each index corresponds to a different gamepad device.
		/// </summary>
		[NonSerialized]
		internal ButtonControl[] gamepadPositiveAltControls;
		/// <summary>
		/// Reference to the keyboard control for the negative alternative binding.
		/// Used for efficient input polling during runtime.
		/// </summary>
		[NonSerialized]
		internal KeyControl keyboardNegativeAltControl;
		/// <summary>
		/// Array of gamepad controls for the negative alternative binding across all connected gamepads.
		/// Each index corresponds to a different gamepad device.
		/// </summary>
		[NonSerialized]
		internal ButtonControl[] gamepadNegativeAltControls;
		/// <summary>
		/// Flag indicating whether the input configuration has been optimized/trimmed.
		/// Prevents redundant trimming operations during runtime.
		/// </summary>
		[NonSerialized]
		private bool trimmed;

		#endregion

		#region Methods

		/// <summary>
		/// Sets a keyboard key binding for the specified axis and binding side.
		/// Updates the appropriate InputAxis (Main or Alt) with the new key binding.
		/// Marks the InputsManager data as changed if in play mode.
		/// </summary>
		/// <param name="axis">The axis type to modify (Main or Alt).</param>
		/// <param name="binding">The binding side to set (Positive or Negative).</param>
		/// <param name="key">The keyboard key to assign to the binding.</param>
		public void SetKeyboardKey(InputAxisType axis, InputAxisSide binding, Key key)
		{
			switch (axis)
			{
				case InputAxisType.Alt:
					SetAxisKeyOrButton(Alt, binding, (int)key, false);

					break;

				default:
					SetAxisKeyOrButton(Main, binding, (int)key, false);

					break;
			}

			if (Application.isPlaying)
				InputsManager.DataChanged = true;
		}
		/// <summary>
		/// Sets a gamepad button binding for the specified axis and binding side.
		/// Updates the appropriate InputAxis (Main or Alt) with the new gamepad binding.
		/// Marks the InputsManager data as changed if in play mode.
		/// </summary>
		/// <param name="axis">The axis type to modify (Main or Alt).</param>
		/// <param name="side">The side of the axis to set (Positive or Negative).</param>
		/// <param name="key">The gamepad binding to assign.</param>
		public void SetGamepadBinding(InputAxisType axis, InputAxisSide side, GamepadBinding key)
		{
			switch (axis)
			{
				case InputAxisType.Alt:
					SetAxisKeyOrButton(Alt, side, (int)key, true);

					break;

				default:
					SetAxisKeyOrButton(Main, side, (int)key, true);

					break;
			}

			if (Application.isPlaying)
				InputsManager.DataChanged = true;
		}

		/// <summary>
		/// Initializes the input for runtime use by trimming unnecessary bindings,
		/// setting up bindable flags, and initializing control references.
		/// Called when the input system starts to prepare for efficient input polling.
		/// </summary>
		internal void Start()
		{
			if (!trimmed)
				Trim();

			KeyboardPositiveMainBindable = Main.Positive != Key.None;
			KeyboardNegativeMainBindable = Main.Negative != Key.None;
			KeyboardPositiveAltBindable = Alt.Positive != Key.None;
			KeyboardNegativeAltBindable = Alt.Negative != Key.None;
			KeyboardMainBindable = KeyboardPositiveMainBindable || KeyboardNegativeMainBindable;
			KeyboardAltBindable = KeyboardPositiveAltBindable || KeyboardNegativeAltBindable;
			KeyboardPositiveBindable = KeyboardPositiveMainBindable || KeyboardPositiveAltBindable;
			KeyboardNegativeBindable = KeyboardNegativeMainBindable || KeyboardNegativeAltBindable;
			KeyboardBindable = KeyboardMainBindable || KeyboardAltBindable;

			if (keyboardPositiveMainControl == null && KeyboardPositiveMainBindable)
				keyboardPositiveMainControl = InputUtilities.KeyToKeyControl(Main.Positive);

			if (keyboardNegativeMainControl == null && KeyboardNegativeMainBindable)
				keyboardNegativeMainControl = InputUtilities.KeyToKeyControl(Main.Negative);

			if (keyboardPositiveAltControl == null && KeyboardPositiveAltBindable)
				keyboardPositiveAltControl = InputUtilities.KeyToKeyControl(Alt.Positive);

			if (keyboardNegativeAltControl == null && KeyboardNegativeAltBindable)
				keyboardNegativeAltControl = InputUtilities.KeyToKeyControl(Alt.Negative);

			GamepadPositiveMainBindable = Main.GamepadPositive != GamepadBinding.None;
			GamepadNegativeMainBindable = Main.GamepadNegative != GamepadBinding.None;
			GamepadPositiveAltBindable = Alt.GamepadPositive != GamepadBinding.None;
			GamepadNegativeAltBindable = Alt.GamepadNegative != GamepadBinding.None;
			GamepadMainBindable = GamepadPositiveMainBindable || GamepadNegativeMainBindable;
			GamepadAltBindable = GamepadPositiveAltBindable || GamepadNegativeAltBindable;
			GamepadPositiveBindable = GamepadPositiveMainBindable || GamepadPositiveAltBindable;
			GamepadNegativeBindable = GamepadNegativeMainBindable || GamepadNegativeAltBindable;
			GamepadBindable = GamepadMainBindable || GamepadAltBindable;

			InitializeGamepads();
		}
		/// <summary>
		/// Sets up gamepad control references for all connected gamepads.
		/// Resizes control arrays to match the current number of gamepads and
		/// initializes button controls for each gamepad based on the configured bindings.
		/// </summary>
		internal void InitializeGamepads()
		{
			gamepadPositiveMainControls ??= new ButtonControl[] { };
			gamepadNegativeMainControls ??= new ButtonControl[] { };
			gamepadPositiveAltControls ??= new ButtonControl[] { };
			gamepadNegativeAltControls ??= new ButtonControl[] { };

			Array.Resize(ref gamepadPositiveMainControls, InputsManager.GamepadsCount);
			Array.Resize(ref gamepadNegativeMainControls, InputsManager.GamepadsCount);
			Array.Resize(ref gamepadPositiveAltControls, InputsManager.GamepadsCount);
			Array.Resize(ref gamepadNegativeAltControls, InputsManager.GamepadsCount);

			for (int i = 0; i < InputsManager.GamepadsCount; i++)
			{
				if (GamepadPositiveMainBindable && gamepadPositiveMainControls[i] == null)
					gamepadPositiveMainControls[i] = InputUtilities.GamepadBindingToButtonControl(InputsManager.Gamepads[i], Main.GamepadPositive);

				if (GamepadNegativeMainBindable && gamepadNegativeMainControls[i] == null)
					gamepadNegativeMainControls[i] = InputUtilities.GamepadBindingToButtonControl(InputsManager.Gamepads[i], Main.GamepadNegative);

				if (GamepadPositiveAltBindable && gamepadPositiveAltControls[i] == null)
					gamepadPositiveAltControls[i] = InputUtilities.GamepadBindingToButtonControl(InputsManager.Gamepads[i], Alt.GamepadPositive);

				if (GamepadNegativeAltBindable && gamepadNegativeAltControls[i] == null)
					gamepadNegativeAltControls[i] = InputUtilities.GamepadBindingToButtonControl(InputsManager.Gamepads[i], Alt.GamepadNegative);
			}
		}
		/// <summary>
		/// Optimizes the input configuration by ensuring Main bindings are populated before Alt bindings,
		/// and enforcing Button type constraints (no negative bindings, value interval starts at 0).
		/// This prevents redundant or invalid configurations and improves input processing efficiency.
		/// </summary>
		private void Trim()
		{
			if (!Main)
				main = new InputAxis();

			if (!Alt)
				alt = new InputAxis();

			if (main.Positive == Key.None && alt.Positive != Key.None)
			{
				main.Positive = alt.Positive;
				alt.Positive = Key.None;
			}

			if (main.GamepadPositive == GamepadBinding.None && alt.GamepadPositive != GamepadBinding.None)
			{
				main.GamepadPositive = alt.GamepadPositive;
				alt.GamepadPositive = GamepadBinding.None;
			}

			if (main.Negative == Key.None && alt.Negative != Key.None)
			{
				main.Negative = alt.Negative;
				alt.Negative = Key.None;
			}

			if (main.GamepadNegative == GamepadBinding.None && alt.GamepadNegative != GamepadBinding.None)
			{
				main.GamepadNegative = alt.GamepadNegative;
				alt.GamepadNegative = GamepadBinding.None;
			}

			if (type == InputType.Button)
			{
				if (valueInterval.x != 0f)
					valueInterval.x = 0f;

				if (main.Negative != Key.None)
					main.Negative = Key.None;

				if (main.GamepadNegative != GamepadBinding.None)
					main.GamepadNegative = GamepadBinding.None;

				if (alt.Negative != Key.None)
					alt.Negative = Key.None;

				if (alt.GamepadNegative != GamepadBinding.None)
					alt.GamepadNegative = GamepadBinding.None;
			}

			if (Application.isPlaying)
				trimmed = true;
		}
		/// <summary>
		/// Helper method to set a key or gamepad button binding on an InputAxis.
		/// Updates either the positive or negative binding based on the specified parameters.
		/// </summary>
		/// <param name="axis">The InputAxis to modify.</param>
		/// <param name="binding">The binding side to set (Positive or Negative).</param>
		/// <param name="keyOrButton">The integer value of the key or gamepad button.</param>
		/// <param name="gamepad">Whether this is a gamepad binding (true) or keyboard binding (false).</param>
		private void SetAxisKeyOrButton(InputAxis axis, InputAxisSide binding, int keyOrButton, bool gamepad)
		{
			switch (binding)
			{
				case InputAxisSide.Negative:
					if (gamepad)
						axis.GamepadNegative = (GamepadBinding)keyOrButton;
					else
						axis.Negative = (Key)keyOrButton;

					break;

				default:
					if (gamepad)
						axis.GamepadPositive = (GamepadBinding)keyOrButton;
					else
						axis.Positive = (Key)keyOrButton;

					break;
			}
		}

		#endregion

		#region Constructors & Operators

		#region Constructors

		/// <summary>
		/// Constructs a new input with the specified name and default settings.
		/// Initializes with a standard axis range (-1 to 1) and empty bindings.
		/// </summary>
		/// <param name="name">The name of the input.</param>
		public Input(string name)
		{
			this.name = name;
			valueInterval = new Utility.SerializableVector2(-1f, 1f);
			main = new InputAxis();
			alt = new InputAxis();
		}
		/// <summary>
		/// Constructs a new input by cloning an existing one.
		/// Creates a deep copy with a modified name to indicate it's a clone.
		/// </summary>
		/// <param name="input">The input to clone.</param>
		public Input(Input input)
		{
			name = $"{input.name} (Clone)";
			type = input.type;
			interpolation = input.interpolation;
			valueInterval = input.valueInterval;
			main = new InputAxis(input.main);
			alt = new InputAxis(input.alt);
			invert = input.invert;

			Trim();
		}

		#endregion

		#region Operators

		/// <summary>
		/// Implicitly converts an input to a boolean, returning true if the input is not null.
		/// Allows for convenient null-checking with syntax like "if (input)".
		/// </summary>
		/// <param name="input">The input to check.</param>
		public static implicit operator bool(Input input) => input != null;

		#endregion

		#endregion
	}
}
