#region Namespaces

using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

#endregion

namespace Utilities.Inputs.Components
{
	internal struct InputAccess
	{
		#region Variables

		public InputType type;
		public InputAxisAccess main;
		public InputAxisAccess alt;
		public InputInterpolation interpolation;
		public float2 valueInterval;
		public bool invert;

		#endregion

		#region Constructors

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
