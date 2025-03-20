#region Namespaces

using System;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Utilities.Inputs
{
	/// <summary>
	/// Represents an input axis with support for keyboard and gamepad controls.
	/// Provides configuration for positive and negative bindings, as well as strong side behavior
	/// to handle cases when both directions are pressed simultaneously.
	/// </summary>
	[Serializable]
	public class InputAxis
	{
		#region Enumerators

		/// <summary>
		/// Represents the side of the input axis.
		/// </summary>
		[Obsolete("Use `InputAxisStrongSide` instead.", true)]
		public enum Side { None, Positive, Negative, FirstPressing }

		#endregion

		#region Variables

		/// <summary>
		/// Gets or sets the strong side behavior for keyboard input when both positive and negative keys are pressed.
		/// Controls which direction takes precedence during simultaneous input.
		/// Changes to this property will mark the InputsManager data as changed when not in play mode.
		/// </summary>
		public InputAxisStrongSide StrongSide
		{
			get
			{
				return strongSide;
			}
			set
			{
				strongSide = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets the keyboard key assigned to the positive direction of this axis.
		/// When pressed, this key will generate a positive value for the input.
		/// Changes to this property will always mark the InputsManager data as changed.
		/// </summary>
		public Key Positive
		{
			get
			{
				return positive;
			}
			set
			{
				positive = value;
				InputsManager.DataChanged = true;
			}
		}
		/// <summary>
		/// Gets or sets the keyboard key assigned to the negative direction of this axis.
		/// When pressed, this key will generate a negative value for the input.
		/// Changes to this property will always mark the InputsManager data as changed.
		/// </summary>
		public Key Negative
		{
			get
			{
				return negative;
			}
			set
			{
				negative = value;
				InputsManager.DataChanged = true;
			}
		}
		/// <summary>
		/// Gets or sets the strong side behavior for gamepad input when both positive and negative buttons are pressed.
		/// Controls which direction takes precedence during simultaneous gamepad input.
		/// Changes to this property will mark the InputsManager data as changed when not in play mode.
		/// </summary>
		public InputAxisStrongSide GamepadStrongSide
		{
			get
			{
				return gamepadStrongSide;
			}
			set
			{
				gamepadStrongSide = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets the gamepad button or stick assigned to the positive direction of this axis.
		/// When activated, this binding will generate a positive value for the input.
		/// Changes to this property will mark the InputsManager data as changed when not in play mode.
		/// </summary>
		public GamepadBinding GamepadPositive
		{
			get
			{
				return gamepadPositive;
			}
			set
			{
				gamepadPositive = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}
		/// <summary>
		/// Gets or sets the gamepad button or stick assigned to the negative direction of this axis.
		/// When activated, this binding will generate a negative value for the input.
		/// Changes to this property will mark the InputsManager data as changed when not in play mode.
		/// </summary>
		public GamepadBinding GamepadNegative
		{
			get
			{
				return gamepadNegative;
			}
			set
			{
				gamepadNegative = value;
				InputsManager.DataChanged = !Application.isPlaying;
			}
		}

		/// <summary>
		/// The strong side behavior for keyboard input when both positive and negative keys are pressed.
		/// Determines which direction takes precedence during simultaneous input.
		/// </summary>
		[SerializeField]
		private InputAxisStrongSide strongSide;
		/// <summary>
		/// The keyboard key assigned to the positive direction of this axis.
		/// Generates a positive value when pressed.
		/// </summary>
		[SerializeField]
		private Key positive;
		/// <summary>
		/// The keyboard key assigned to the negative direction of this axis.
		/// Generates a negative value when pressed.
		/// </summary>
		[SerializeField]
		private Key negative;
		/// <summary>
		/// The strong side behavior for gamepad input when both positive and negative buttons are pressed.
		/// Determines which direction takes precedence during simultaneous gamepad input.
		/// </summary>
		[SerializeField]
		private InputAxisStrongSide gamepadStrongSide;
		/// <summary>
		/// The gamepad button or stick assigned to the positive direction of this axis.
		/// Generates a positive value when activated.
		/// </summary>
		[SerializeField]
		private GamepadBinding gamepadPositive;
		/// <summary>
		/// The gamepad button or stick assigned to the negative direction of this axis.
		/// Generates a negative value when activated.
		/// </summary>
		[SerializeField]
		private GamepadBinding gamepadNegative;

		#endregion

		#region Constructors & Opertators

		#region  Constructors

		/// <summary>
		/// Initializes a new instance of the InputAxis class with default values.
		/// Sets all bindings to None and strong side behaviors to None for both keyboard and gamepad.
		/// </summary>
		public InputAxis()
		{
			strongSide = InputAxisStrongSide.None;
			positive = Key.None;
			negative = Key.None;
			gamepadStrongSide = InputAxisStrongSide.None;
			gamepadPositive = GamepadBinding.None;
			gamepadNegative = GamepadBinding.None;
		}
		/// <summary>
		/// Initializes a new instance of the InputAxis class by copying values from an existing InputAxis.
		/// Creates a deep copy of all binding and strong side configurations.
		/// </summary>
		/// <param name="axis">The source InputAxis to clone values from.</param>
		public InputAxis(InputAxis axis)
		{
			strongSide = axis.strongSide;
			positive = axis.positive;
			negative = axis.negative;
			gamepadStrongSide = axis.gamepadStrongSide;
			gamepadPositive = axis.gamepadPositive;
			gamepadNegative = axis.gamepadNegative;
		}

		#endregion

		#region Operators

		/// <summary>
		/// Implicitly converts an InputAxis to a boolean value.
		/// Returns true if the InputAxis instance is not null, otherwise false.
		/// Allows for convenient null-checking with syntax like "if (inputAxis)".
		/// </summary>
		/// <param name="axis">The InputAxis instance to check.</param>
		public static implicit operator bool(InputAxis axis) => axis != null;

		#endregion

		#endregion
	}
}
