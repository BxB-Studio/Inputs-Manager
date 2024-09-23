#region Namespaces

using System;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Utilities.Inputs
{
	[Serializable]
	public class InputAxis
	{
		#region Enumerators

		public enum Side { None, Positive, Negative, FirstPressing }

		#endregion

		#region Variables

		public Side StrongSide
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
		public Side GamepadStrongSide
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

		[SerializeField]
		private Side strongSide;
		[SerializeField]
		private Key positive;
		[SerializeField]
		private Key negative;
		[SerializeField]
		private Side gamepadStrongSide;
		[SerializeField]
		private GamepadBinding gamepadPositive;
		[SerializeField]
		private GamepadBinding gamepadNegative;

		#endregion

		#region Constructors & Opertators

		#region  Constructors

		public InputAxis()
		{
			strongSide = Side.None;
			positive = Key.None;
			negative = Key.None;
			gamepadStrongSide = Side.None;
			gamepadPositive = GamepadBinding.None;
			gamepadNegative = GamepadBinding.None;
		}
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

		public static implicit operator bool(InputAxis axis) => axis != null;

		#endregion

		#endregion
	}
}
