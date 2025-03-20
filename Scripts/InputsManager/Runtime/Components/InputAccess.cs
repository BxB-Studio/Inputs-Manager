#region Namespaces

using Unity.Mathematics;

#endregion

namespace Utilities.Inputs.Components
{
	/// <summary>
	/// A struct that contains the access information for an input.
	/// Provides a lightweight representation of an Input object's configuration
	/// for efficient runtime access and processing.
	/// </summary>
	internal struct InputAccess
	{
		#region Variables

		/// <summary>
		/// The type of input (Button, Axis, or Vector).
		/// Determines how the input values are processed and interpreted by the system.
		/// </summary>
		public InputType type;
		
		/// <summary>
		/// The main axis access for the input.
		/// Contains the primary binding information for keyboard and gamepad controls.
		/// Used as the primary source for input detection and value calculation.
		/// </summary>
		public InputAxisAccess main;
		
		/// <summary>
		/// The alternative axis access for the input.
		/// Provides secondary binding information that can be used as a fallback
		/// or additional control method for the same input action.
		/// </summary>
		public InputAxisAccess alt;
		
		/// <summary>
		/// The interpolation method for the input.
		/// Defines how the raw input values are smoothed or processed over time
		/// to create more natural or responsive behavior.
		/// </summary>
		public InputInterpolation interpolation;
		
		/// <summary>
		/// The value interval for the input as a float2 (min, max).
		/// Defines the range of possible values that this input can produce,
		/// allowing for remapping of raw input values to a specific range.
		/// </summary>
		public float2 valueInterval;
		
		/// <summary>
		/// Whether the input values should be inverted.
		/// When true, positive inputs become negative and vice versa,
		/// useful for creating controls with opposite behavior.
		/// </summary>
		public bool invert;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an InputAccess struct from an Input object.
		/// Initializes the struct with all necessary configuration data from the provided Input,
		/// ensuring the Input is properly started before accessing its properties.
		/// </summary>
		/// <param name="input">The Input object to extract configuration data from. Must not be null.</param>
		public InputAccess(Input input)
		{
			input.Start();

			type = input.Type;
			main = new InputAxisAccess(input.Main);
			alt = new InputAxisAccess(input.Alt);
			interpolation = input.Interpolation;
			valueInterval = input.ValueInterval;
			invert = input.Invert;
		}

		#endregion
	}
}
