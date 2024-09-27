#region Namespaces

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#endregion

namespace Utilities.Inputs
{
	[Serializable]
	public class Input
	{
		#region Variables

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

		internal bool KeyboardPositiveMainBindable { get; private set; }
		internal bool GamepadPositiveMainBindable { get; private set; }
		internal bool KeyboardNegativeMainBindable { get; private set; }
		internal bool GamepadNegativeMainBindable { get; private set; }
		internal bool KeyboardPositiveAltBindable { get; private set; }
		internal bool GamepadPositiveAltBindable { get; private set; }
		internal bool KeyboardNegativeAltBindable { get; private set; }
		internal bool GamepadNegativeAltBindable { get; private set; }
		internal bool KeyboardPositiveBindable { get; private set; }
		internal bool GamepadPositiveBindable { get; private set; }
		internal bool KeyboardNegativeBindable { get; private set; }
		internal bool GamepadNegativeBindable { get; private set; }
		internal bool KeyboardMainBindable { get; private set; }
		internal bool GamepadMainBindable { get; private set; }
		internal bool KeyboardAltBindable { get; private set; }
		internal bool GamepadAltBindable { get; private set; }
		internal bool KeyboardBindable { get; private set; }
		internal bool GamepadBindable { get; private set; }

		[SerializeField]
		private string name;
		[SerializeField]
		private InputType type;
		[SerializeField]
		private InputAxis main;
		[SerializeField]
		private InputAxis alt;
		[SerializeField]
		private InputInterpolation interpolation;
		[SerializeField]
		private Utility.SerializableVector2 valueInterval;
		[SerializeField]
		private bool invert;

		[NonSerialized]
		internal KeyControl keyboardPositiveMainControl;
		[NonSerialized]
		internal ButtonControl[] gamepadPositiveMainControls;
		[NonSerialized]
		internal KeyControl keyboardNegativeMainControl;
		[NonSerialized]
		internal ButtonControl[] gamepadNegativeMainControls;
		[NonSerialized]
		internal KeyControl keyboardPositiveAltControl;
		[NonSerialized]
		internal ButtonControl[] gamepadPositiveAltControls;
		[NonSerialized]
		internal KeyControl keyboardNegativeAltControl;
		[NonSerialized]
		internal ButtonControl[] gamepadNegativeAltControls;
		[NonSerialized]
		private bool trimmed;

		#endregion

		#region Methods

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

		public Input(string name)
		{
			this.name = name;
			valueInterval = new Utility.SerializableVector2(-1f, 1f);
			main = new InputAxis();
			alt = new InputAxis();
		}
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

		public static implicit operator bool(Input input) => input != null;

		#endregion

		#endregion
	}
}
