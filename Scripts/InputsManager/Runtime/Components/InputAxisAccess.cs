#region Namespaces

using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Utilities.Inputs.Components
{
	internal struct InputAxisAccess
	{
		#region Variables

		public InputAxis.Side strongSide;
		public Key positive;
		public Key negative;
		public InputAxis.Side gamepadStrongSide;
		public GamepadBinding gamepadPositive;
		public GamepadBinding gamepadNegative;

		#endregion

		#region Constructors

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
