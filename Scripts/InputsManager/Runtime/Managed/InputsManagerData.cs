#region Namespaces

using System;
using UnityEngine;

#endregion

namespace Utilities.Inputs
{
	[Serializable]
	public class InputsManagerData
	{
		#region Variables

		public Input[] Inputs
		{
			get
			{
				return inputs;
			}
		}
		public InputSource InputSourcePriority
		{
			get
			{
				return inputSourcePriority;
			}
		}
		public float InterpolationTime
		{
			get
			{
				return interpolationTime;
			}
		}
		public float HoldTriggerTime
		{
			get
			{
				return holdTriggerTime;
			}
		}
		public float HoldWaitTime
		{
			get
			{
				return holdWaitTime;
			}
		}
		public float DoublePressTimeout
		{
			get
			{
				return doublePressTimeout;
			}
		}
		public float GamepadThreshold
		{
			get
			{
				return gamepadThreshold;
			}
		}
		public byte DefaultGamepadIndex
		{
			get
			{
				return defaultGamepadIndex;
			}
		}

		[SerializeField]
		private readonly Input[] inputs;
		[SerializeField]
		private InputSource inputSourcePriority;
		[SerializeField]
		private readonly float interpolationTime;
		[SerializeField]
		private readonly float holdTriggerTime;
		[SerializeField]
		private readonly float holdWaitTime;
		[SerializeField]
		private readonly float doublePressTimeout;
		[SerializeField]
		private readonly float gamepadThreshold;
		[SerializeField]
		private readonly byte defaultGamepadIndex;

		#endregion

		#region Constructors & Operators

		#region Constructors

		public InputsManagerData()
		{
			inputs = InputsManager.Inputs ?? new Input[] { };
			inputSourcePriority = InputsManager.InputSourcePriority;
			interpolationTime = InputsManager.InterpolationTime;
			holdTriggerTime = InputsManager.HoldTriggerTime;
			holdWaitTime = InputsManager.HoldWaitTime;
			doublePressTimeout = InputsManager.DoublePressTimeout;
            gamepadThreshold = InputsManager.GamepadThreshold;
            defaultGamepadIndex = InputsManager.DefaultGamepadIndex;
        }

		#endregion

		#region Operators

		public static implicit operator bool(InputsManagerData data) => data != null;

		#endregion

		#endregion
	}
}
