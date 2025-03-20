#region Namespaces

using System;
using UnityEngine;

#endregion

namespace Utilities.Inputs
{
	/// <summary>
	/// A class that contains the configuration data and settings for the Inputs Manager.
	/// Stores input bindings, timing parameters, and gamepad settings used throughout the input system.
	/// </summary>
	[Serializable]
	public class InputsManagerData
	{
		#region Variables

		/// <summary>
		/// Gets the collection of input definitions managed by the Inputs Manager.
		/// Each Input object contains binding information for keyboard and gamepad controls.
		/// </summary>
		public Input[] Inputs
		{
			get
			{
				return inputs;
			}
		}
		/// <summary>
		/// Gets the priority setting that determines which input source (keyboard or gamepad)
		/// takes precedence when multiple inputs are detected simultaneously.
		/// </summary>
		public InputSource InputSourcePriority
		{
			get
			{
				return inputSourcePriority;
			}
		}
		/// <summary>
		/// Gets the time in seconds over which input values are smoothly interpolated.
		/// Controls how quickly input values change from one state to another for smoother transitions.
		/// </summary>
		public float InterpolationTime
		{
			get
			{
				return interpolationTime;
			}
		}
		/// <summary>
		/// Gets the time in seconds that an input must be continuously pressed
		/// before it registers as being "held" rather than just pressed.
		/// </summary>
		public float HoldTriggerTime
		{
			get
			{
				return holdTriggerTime;
			}
		}
		/// <summary>
		/// Gets the time in seconds to wait between repeated input events
		/// when an input is being continuously held down.
		/// </summary>
		public float HoldWaitTime
		{
			get
			{
				return holdWaitTime;
			}
		}
		/// <summary>
		/// Gets the maximum time in seconds allowed between two presses
		/// for them to be recognized as a double-press action.
		/// </summary>
		public float DoublePressTimeout
		{
			get
			{
				return doublePressTimeout;
			}
		}
		/// <summary>
		/// Gets the minimum analog value (0.0 to 1.0) that gamepad inputs must exceed
		/// to be considered as active input. Helps filter out small unintentional movements.
		/// </summary>
		public float GamepadThreshold
		{
			get
			{
				return gamepadThreshold;
			}
		}
		/// <summary>
		/// Gets the index of the gamepad to use by default when multiple controllers are connected.
		/// A value of sbyte.MaxValue indicates that any connected gamepad should be used.
		/// </summary>
		public sbyte DefaultGamepadIndex
		{
			get
			{
				return defaultGamepadIndex;
			}
		}
		
		/// <summary>
		/// The serialized collection of input definitions with their bindings and configurations.
		/// </summary>
		[SerializeField]
		private Input[] inputs;
		/// <summary>
		/// The serialized setting that determines which input source takes priority when multiple inputs are detected.
		/// </summary>
		[SerializeField]
		private InputSource inputSourcePriority;
		/// <summary>
		/// The serialized time in seconds over which input values are smoothly interpolated.
		/// </summary>
		[SerializeField]
		private float interpolationTime;
		/// <summary>
		/// The serialized time in seconds required for an input to register as being held.
		/// </summary>
		[SerializeField]
		private float holdTriggerTime;
		/// <summary>
		/// The serialized time in seconds to wait between repeated input events when an input is held.
		/// </summary>
		[SerializeField]
		private float holdWaitTime;
		/// <summary>
		/// The serialized maximum time in seconds allowed between presses to register as a double-press.
		/// </summary>
		[SerializeField]
		private float doublePressTimeout;
		/// <summary>
		/// The serialized minimum threshold value that gamepad inputs must exceed to be considered active.
		/// </summary>
		[SerializeField]
		private float gamepadThreshold;
		/// <summary>
		/// The serialized index of the default gamepad to use when multiple controllers are connected.
		/// </summary>
		[SerializeField]
		private sbyte defaultGamepadIndex;

		#endregion

		#region Constructors & Operators

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the InputsManagerData class.
		/// Copies current settings from the InputsManager static class and applies default values
		/// for any settings that are invalid or uninitialized. This ensures all settings have
		/// reasonable values even if the InputsManager hasn't been fully configured.
		/// </summary>
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

		/// <summary>
		/// Implicitly converts an InputsManagerData object to a boolean.
		/// Enables convenient null-checking with syntax like "if (inputsManagerData)".
		/// </summary>
		/// <param name="data">The InputsManagerData object to convert.</param>
		/// <returns>True if the InputsManagerData object is not null, false otherwise.</returns>
		public static implicit operator bool(InputsManagerData data) => data != null;

		#endregion

		#endregion
	}
}
