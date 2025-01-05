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
		public sbyte DefaultGamepadIndex
		{
			get
			{
				return defaultGamepadIndex;
			}
		}

		[SerializeField]
		private Input[] inputs;
		[SerializeField]
		private InputSource inputSourcePriority;
		[SerializeField]
		private float interpolationTime;
		[SerializeField]
		private float holdTriggerTime;
		[SerializeField]
		private float holdWaitTime;
		[SerializeField]
		private float doublePressTimeout;
		[SerializeField]
		private float gamepadThreshold;
		[SerializeField]
		private sbyte defaultGamepadIndex;

		#endregion

		#region Constructors & Operators

		#region Constructors

		public InputsManagerData()
		{
			inputs = InputsManager.Inputs ?? new Input[] { };
			inputSourcePriority = InputsManager.InputSourcePriority;

			if (inputSourcePriority < 0)
				inputSourcePriority = 0;

				interpolationTime = InputsManager.InterpolationTime;

			if (interpolationTime < 0f)
				interpolationTime = .25f;

			holdTriggerTime = InputsManager.HoldTriggerTime;

			if (holdTriggerTime < 0f)
				holdTriggerTime = .3f;

			holdWaitTime = InputsManager.HoldWaitTime;

			if (holdWaitTime < 0f)
				holdWaitTime = .1f;

			doublePressTimeout = InputsManager.DoublePressTimeout;

			if (doublePressTimeout < 0f)
				doublePressTimeout = .2f;

			gamepadThreshold = InputsManager.GamepadThreshold;

			if (gamepadThreshold < 0f)
				gamepadThreshold = .5f;

			defaultGamepadIndex = InputsManager.DefaultGamepadIndex;

			if (defaultGamepadIndex < 0)
				defaultGamepadIndex = sbyte.MaxValue;
		}

		#endregion

		#region Operators

		public static implicit operator bool(InputsManagerData data) => data != null;

		#endregion

		#endregion
	}
}
