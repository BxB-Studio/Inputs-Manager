#region Namespaces

using UnityEngine.InputSystem;

#endregion

namespace Utilities.Inputs.Components
{
	/// <summary>
	/// A struct that contains the access information for an input axis.
	/// Provides a lightweight representation of an InputAxis object's configuration
	/// for efficient runtime access and processing of both keyboard and gamepad inputs.
	/// </summary>
	internal struct InputAxisAccess
	{
		#region Variables

		/// <summary>
		/// The strong side of the input axis for keyboard inputs.
		/// Determines which direction (positive or negative) takes precedence when both
		/// positive and negative keys are pressed simultaneously.
		/// </summary>
		public InputAxisStrongSide strongSide;
		
		/// <summary>
		/// The positive key binding of the input axis for keyboard inputs.
		/// Represents the key that triggers the positive direction of the axis
		/// (e.g., moving right or forward).
		/// </summary>
		public Key positive;
		
		/// <summary>
		/// The negative key binding of the input axis for keyboard inputs.
		/// Represents the key that triggers the negative direction of the axis
		/// (e.g., moving left or backward).
		/// </summary>
		public Key negative;
		
		/// <summary>
		/// The strong side of the input axis for gamepad inputs.
		/// Determines which direction (positive or negative) takes precedence when both
		/// positive and negative gamepad buttons are pressed simultaneously.
		/// </summary>
		public InputAxisStrongSide gamepadStrongSide;
		
		/// <summary>
		/// The positive button binding of the input axis for gamepad inputs.
		/// Represents the gamepad button or axis that triggers the positive direction
		/// (e.g., right trigger, right stick movement).
		/// </summary>
		public GamepadBinding gamepadPositive;
		
		/// <summary>
		/// The negative button binding of the input axis for gamepad inputs.
		/// Represents the gamepad button or axis that triggers the negative direction
		/// (e.g., left trigger, left stick movement).
		/// </summary>
		public GamepadBinding gamepadNegative;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an InputAxisAccess struct from an InputAxis object.
		/// Initializes the struct with all necessary configuration data from the provided InputAxis,
		/// copying both keyboard and gamepad binding information for runtime access.
		/// </summary>
		/// <param name="axis">The InputAxis object to extract configuration data from. Must not be null.</param>
		public InputAxisAccess(InputAxis axis)
		{
			strongSide = axis.StrongSide;
			positive = axis.Positive;
			negative = axis.Negative;
			gamepadStrongSide = axis.GamepadStrongSide;
			gamepadPositive = axis.GamepadPositive;
			gamepadNegative = axis.GamepadNegative;
		}

		#endregion
	}
}
