#define INPUTS_MANAGER

#region Namespaces

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
#if UNITY_2019_1_OR_NEWER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.EnhancedTouch;
#else
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Controls;
#endif
#endif

#endregion

namespace Utilities.Inputs
{
	#region Enumerators

	public enum MouseButton { Left, Right, Middle, Back, Forward }
	public enum GamepadBinding
	{
		None = 0,
		DpadUp = 1,
		DpadDown = 2,
		DpadLeft = 3,
		DpadRight = 4,
		ButtonNorth = 5,
		ButtonEast = 6,
		ButtonSouth = 7,
		ButtonWest = 8,
		LeftStickButton = 9,
		LeftStickUp = 10,
		LeftStickRight = 11,
		LeftStickDown = 12,
		LeftStickLeft = 13,
		RightStickButton = 14,
		RightStickUp = 15,
		RightStickRight = 16,
		RightStickDown = 17,
		RightStickLeft = 18,
		LeftShoulder = 19,
		RightShoulder = 20,
		LeftTrigger = 21,
		RightTrigger = 22,
		StartButton = 23,
		SelectButton = 24,
	}

	#endregion

	#region Modules

	public static class InputsManager
	{
		#region Enumerators & Modules

		#region Enumerators

		public enum InputSource { Keyboard, Gamepad }

		#endregion

		#region Modules

		[Serializable]
		public class DataSheet
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

			#endregion

			#region Constructors & Operators

			#region Constructors

			public DataSheet()
			{
				inputs = InputsManager.Inputs ?? new Input[] { };
				inputSourcePriority = InputsManager.InputSourcePriority;
				interpolationTime = InputsManager.InterpolationTime;
				holdTriggerTime = InputsManager.HoldTriggerTime;
				holdWaitTime = InputsManager.HoldWaitTime;
				doublePressTimeout = InputsManager.DoublePressTimeout;
				gamepadThreshold = InputsManager.GamepadThreshold;
			}

			#endregion

			#region Operators

			public static implicit operator bool(DataSheet data) => data != null;

			#endregion

			#endregion
		}
		[Serializable]
		public class Input
		{
			#region Modules & Enumerators

			#region Enumerators

			public enum InputType { Axis, Button }
			public enum InputAxisType { Main, Alt }
			public enum InputAxisSide { Positive, Negative }
			public enum InputInterpolation { Instant = 2, Jump = 1, Smooth = 0 }

			#endregion

			#region Modules

			[Serializable]
			public class Axis
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
						dataChanged = !Application.isPlaying;
					}
				}
#if ENABLE_INPUT_SYSTEM
				public Key Positive
				{
					get
					{
						return positive;
					}
					set
					{
						positive = value;
						dataChanged = true;
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
						dataChanged = true;
					}
				}
#endif
				public Side GamepadStrongSide
				{
					get
					{
						return gamepadStrongSide;
					}
					set
					{
						gamepadStrongSide = value;
						dataChanged = !Application.isPlaying;
					}
				}
#if ENABLE_INPUT_SYSTEM
				public GamepadBinding GamepadPositive
				{
					get
					{
						return gamepadPositive;
					}
					set
					{
						gamepadPositive = value;
						dataChanged = !Application.isPlaying;
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
						dataChanged = !Application.isPlaying;
					}
				}
#endif

				[SerializeField]
				private Side strongSide;
#if ENABLE_INPUT_SYSTEM
				[SerializeField]
				private Key positive;
				[SerializeField]
				private Key negative;
#endif
				[SerializeField]
				private Side gamepadStrongSide;
#if ENABLE_INPUT_SYSTEM
				[SerializeField]
				private GamepadBinding gamepadPositive;
				[SerializeField]
				private GamepadBinding gamepadNegative;
#endif

				#endregion

				#region Constructors & Opertators

				#region  Constructors

				public Axis()
				{
					strongSide = Side.None;
#if ENABLE_INPUT_SYSTEM
					positive = Key.None;
					negative = Key.None;
#endif
					gamepadStrongSide = Side.None;
#if ENABLE_INPUT_SYSTEM
					gamepadPositive = GamepadBinding.None;
					gamepadNegative = GamepadBinding.None;
#endif
				}
				public Axis(Axis axis)
				{
					strongSide = axis.strongSide;
#if ENABLE_INPUT_SYSTEM
					positive = axis.positive;
					negative = axis.negative;
#endif
					gamepadStrongSide = axis.gamepadStrongSide;
#if ENABLE_INPUT_SYSTEM
					gamepadPositive = axis.gamepadPositive;
					gamepadNegative = axis.gamepadNegative;
#endif
				}

				#endregion

				#region Operators

				public static implicit operator bool(Axis axis) => axis != null;

				#endregion

				#endregion
			}

			#endregion

			#endregion

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

					dataChanged = dataChanged || !value.IsNullOrEmpty() && name != value;
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
#if ENABLE_INPUT_SYSTEM
						Main.Negative = Key.None;
						Alt.Negative = Key.None;
#endif
					}

					type = value;
					dataChanged = true;
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
					dataChanged = !Application.isPlaying;
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
					dataChanged = !Application.isPlaying;
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
					dataChanged = !Application.isPlaying;
				}
			}
			public Axis Main
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
					dataChanged = true;
				}
			}
			public Axis Alt
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
					dataChanged = true;
				}
			}

			public float KeyboardMainValue
			{
				get
				{
					return keyboardMainValue;
				}
			}
			public float[] GamepadMainValues
			{
				get
				{
					return gamepadMainValues;
				}
			}
			public float KeyboardAltValue
			{
				get
				{
					return keyboardAltValue;
				}
			}
			public float[] GamepadAltValues
			{
				get
				{
					return gamepadAltValues;
				}
			}
			public float KeyboardValue
			{
				get
				{
					return keyboardValue;
				}
			}
			public float[] GamepadValues
			{
				get
				{
					return gamepadValues;
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
			internal bool KeyboardMainBindable { get; private set; }
			internal bool GamepadMainBindable { get; private set; }
			internal bool KeyboardAltBindable { get; private set; }
			internal bool GamepadAltBindable { get; private set; }
			internal bool KeyboardPositiveBindable { get; private set; }
			internal bool GamepadPositiveBindable { get; private set; }
			internal bool KeyboardNegativeBindable { get; private set; }
			internal bool GamepadNegativeBindable { get; private set; }
			internal bool KeyboardBindable { get; private set; }
			internal bool GamepadBindable { get; private set; }
			internal bool KeyboardPositiveMainPress { get; private set; }
			internal bool[] GamepadsPositiveMainPress { get; private set; }
			internal bool KeyboardNegativeMainPress { get; private set; }
			internal bool[] GamepadsNegativeMainPress { get; private set; }
			internal bool KeyboardPositiveAltPress { get; private set; }
			internal bool[] GamepadsPositiveAltPress { get; private set; }
			internal bool KeyboardNegativeAltPress { get; private set; }
			internal bool[] GamepadsNegativeAltPress { get; private set; }
			internal bool KeyboardMainPress { get; private set; }
			internal bool[] GamepadsMainPress { get; private set; }
			internal bool KeyboardAltPress { get; private set; }
			internal bool[] GamepadsAltPress { get; private set; }
			internal bool KeyboardPositivePress { get; private set; }
			internal bool[] GamepadsPositivePress { get; private set; }
			internal bool KeyboardNegativePress { get; private set; }
			internal bool[] GamepadsNegativePress { get; private set; }
			internal bool KeyboardPress { get; private set; }
			internal bool[] GamepadsPress { get; private set; }
			internal bool KeyboardPositiveMainDown { get; private set; }
			internal bool[] GamepadsPositiveMainDown { get; private set; }
			internal bool KeyboardNegativeMainDown { get; private set; }
			internal bool[] GamepadsNegativeMainDown { get; private set; }
			internal bool KeyboardPositiveAltDown { get; private set; }
			internal bool[] GamepadsPositiveAltDown { get; private set; }
			internal bool KeyboardNegativeAltDown { get; private set; }
			internal bool[] GamepadsNegativeAltDown { get; private set; }
			internal bool KeyboardMainDown { get; private set; }
			internal bool[] GamepadsMainDown { get; private set; }
			internal bool KeyboardAltDown { get; private set; }
			internal bool[] GamepadsAltDown { get; private set; }
			internal bool KeyboardPositiveDown { get; private set; }
			internal bool[] GamepadsPositiveDown { get; private set; }
			internal bool KeyboardNegativeDown { get; private set; }
			internal bool[] GamepadsNegativeDown { get; private set; }
			internal bool KeyboardDown { get; private set; }
			internal bool[] GamepadsDown { get; private set; }
			internal bool KeyboardPositiveMainUp { get; private set; }
			internal bool[] GamepadsPositiveMainUp { get; private set; }
			internal bool KeyboardNegativeMainUp { get; private set; }
			internal bool[] GamepadsNegativeMainUp { get; private set; }
			internal bool KeyboardPositiveAltUp { get; private set; }
			internal bool[] GamepadsPositiveAltUp { get; private set; }
			internal bool KeyboardNegativeAltUp { get; private set; }
			internal bool[] GamepadsNegativeAltUp { get; private set; }
			internal bool KeyboardMainUp { get; private set; }
			internal bool[] GamepadsMainUp { get; private set; }
			internal bool KeyboardAltUp { get; private set; }
			internal bool[] GamepadsAltUp { get; private set; }
			internal bool KeyboardPositiveUp { get; private set; }
			internal bool[] GamepadsPositiveUp { get; private set; }
			internal bool KeyboardNegativeUp { get; private set; }
			internal bool[] GamepadsNegativeUp { get; private set; }
			internal bool KeyboardUp { get; private set; }
			internal bool[] GamepadsUp { get; private set; }
			internal bool KeyboardPositiveMainHeld { get; private set; }
			internal bool[] GamepadsPositiveMainHeld { get; private set; }
			internal bool KeyboardNegativeMainHeld { get; private set; }
			internal bool[] GamepadsNegativeMainHeld { get; private set; }
			internal bool KeyboardPositiveAltHeld { get; private set; }
			internal bool[] GamepadsPositiveAltHeld { get; private set; }
			internal bool KeyboardNegativeAltHeld { get; private set; }
			internal bool[] GamepadsNegativeAltHeld { get; private set; }
			internal bool KeyboardMainHeld { get; private set; }
			internal bool[] GamepadsMainHeld { get; private set; }
			internal bool KeyboardAltHeld { get; private set; }
			internal bool[] GamepadsAltHeld { get; private set; }
			internal bool KeyboardPositiveHeld { get; private set; }
			internal bool[] GamepadsPositiveHeld { get; private set; }
			internal bool KeyboardNegativeHeld { get; private set; }
			internal bool[] GamepadsNegativeHeld { get; private set; }
			internal bool KeyboardHeld { get; private set; }
			internal bool[] GamepadsHeld { get; private set; }
			internal bool KeyboardPositiveMainDoublePress { get; private set; }
			internal bool[] GamepadsPositiveMainDoublePress { get; private set; }
			internal bool KeyboardNegativeMainDoublePress { get; private set; }
			internal bool[] GamepadsNegativeMainDoublePress { get; private set; }
			internal bool KeyboardPositiveAltDoublePress { get; private set; }
			internal bool[] GamepadsPositiveAltDoublePress { get; private set; }
			internal bool KeyboardNegativeAltDoublePress { get; private set; }
			internal bool[] GamepadsNegativeAltDoublePress { get; private set; }
			internal bool KeyboardMainDoublePress { get; private set; }
			internal bool[] GamepadsMainDoublePress { get; private set; }
			internal bool KeyboardAltDoublePress { get; private set; }
			internal bool[] GamepadsAltDoublePress { get; private set; }
			internal bool KeyboardPositiveDoublePress { get; private set; }
			internal bool[] GamepadsPositiveDoublePress { get; private set; }
			internal bool KeyboardNegativeDoublePress { get; private set; }
			internal bool[] GamepadsNegativeDoublePress { get; private set; }
			internal bool KeyboardDoublePress { get; private set; }
			internal bool[] GamepadsDoublePress { get; private set; }

			[SerializeField]
			private string name;
			[SerializeField]
			private InputType type;
			[SerializeField]
			private Axis main;
			[SerializeField]
			private Axis alt;
			[SerializeField]
			private InputInterpolation interpolation;
			[SerializeField]
			private Utility.SerializableVector2 valueInterval;
			[SerializeField]
			private bool invert;
#if ENABLE_INPUT_SYSTEM
			[NonSerialized]
			private KeyControl keyboardPositiveMainControl;
			[NonSerialized]
			private ButtonControl[] gamepadPositiveMainControls;
			[NonSerialized]
			private KeyControl keyboardNegativeMainControl;
			[NonSerialized]
			private ButtonControl[] gamepadNegativeMainControls;
			[NonSerialized]
			private KeyControl keyboardPositiveAltControl;
			[NonSerialized]
			private ButtonControl[] gamepadPositiveAltControls;
			[NonSerialized]
			private KeyControl keyboardNegativeAltControl;
			[NonSerialized]
			private ButtonControl[] gamepadNegativeAltControls;
#endif
			[NonSerialized]
			private float keyboardMainValue;
			[NonSerialized]
			private float[] gamepadMainValues;
			[NonSerialized]
			private float keyboardAltValue;
			[NonSerialized]
			private float[] gamepadAltValues;
			[NonSerialized]
			private float keyboardValue;
			[NonSerialized]
			private float[] gamepadValues;
			[NonSerialized]
			private float keyboardPositiveMainHoldTimer;
			[NonSerialized]
			private float[] gamepadPositiveMainHoldTimers;
			[NonSerialized]
			private float keyboardNegativeMainHoldTimer;
			[NonSerialized]
			private float[] gamepadNegativeMainHoldTimers;
			[NonSerialized]
			private float keyboardPositiveAltHoldTimer;
			[NonSerialized]
			private float[] gamepadPositiveAltHoldTimers;
			[NonSerialized]
			private float keyboardNegativeAltHoldTimer;
			[NonSerialized]
			private float[] gamepadNegativeAltHoldTimers;
			[NonSerialized]
			private float keyboardPositiveMainDoublePressTimer;
			[NonSerialized]
			private float[] gamepadPositiveMainDoublePressTimers;
			[NonSerialized]
			private float keyboardNegativeMainDoublePressTimer;
			[NonSerialized]
			private float[] gamepadNegativeMainDoublePressTimers;
			[NonSerialized]
			private float keyboardPositiveAltDoublePressTimer;
			[NonSerialized]
			private float[] gamepadPositiveAltDoublePressTimers;
			[NonSerialized]
			private float keyboardNegativeAltDoublePressTimer;
			[NonSerialized]
			private float[] gamepadNegativeAltDoublePressTimers;
			[NonSerialized]
			private float positiveValue;
			[NonSerialized]
			private float negativeValue;
			[NonSerialized]
			private float valueFactor;
			[NonSerialized]
			private float positiveCoeficient;
			[NonSerialized]
			private float negativeCoeficient;
			[NonSerialized]
			private float intervalA;
			[NonSerialized]
			private float intervalB;
			[NonSerialized]
			private float target;
			[NonSerialized]
			private bool keyboardPositiveMainDoublePressInitiated;
			[NonSerialized]
			private bool[] gamepadsPositiveMainDoublePressInitiated;
			[NonSerialized]
			private bool keyboardNegativeMainDoublePressInitiated;
			[NonSerialized]
			private bool[] gamepadsNegativeMainDoublePressInitiated;
			[NonSerialized]
			private bool keyboardPositiveAltDoublePressInitiated;
			[NonSerialized]
			private bool[] gamepadsPositiveAltDoublePressInitiated;
			[NonSerialized]
			private bool keyboardNegativeAltDoublePressInitiated;
			[NonSerialized]
			private bool[] gamepadsNegativeAltDoublePressInitiated;
			[NonSerialized]
			private bool trimmed;

			#endregion

			#region Methods

#if ENABLE_INPUT_SYSTEM
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
					Start();
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
					Start();
			}
#endif

			internal void Start()
			{
				if (!trimmed)
					Trim();

#if ENABLE_INPUT_SYSTEM
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
					keyboardPositiveMainControl = KeyToKeyControl(Main.Positive);

				if (keyboardNegativeMainControl == null && KeyboardNegativeMainBindable)
					keyboardNegativeMainControl = KeyToKeyControl(Main.Negative);

				if (keyboardPositiveAltControl == null && KeyboardPositiveAltBindable)
					keyboardPositiveAltControl = KeyToKeyControl(Alt.Positive);

				if (keyboardNegativeAltControl == null && KeyboardNegativeAltBindable)
					keyboardNegativeAltControl = KeyToKeyControl(Alt.Negative);

				GamepadPositiveMainBindable = Main.GamepadPositive != GamepadBinding.None;
				GamepadNegativeMainBindable = Main.GamepadNegative != GamepadBinding.None;
				GamepadPositiveAltBindable = Alt.GamepadPositive != GamepadBinding.None;
				GamepadNegativeAltBindable = Alt.GamepadNegative != GamepadBinding.None;
#endif
				GamepadMainBindable = GamepadPositiveMainBindable || GamepadNegativeMainBindable;
				GamepadAltBindable = GamepadPositiveAltBindable || GamepadNegativeAltBindable;
				GamepadPositiveBindable = GamepadPositiveMainBindable || GamepadPositiveAltBindable;
				GamepadNegativeBindable = GamepadNegativeMainBindable || GamepadNegativeAltBindable;
				GamepadBindable = GamepadMainBindable || GamepadAltBindable;

#if ENABLE_INPUT_SYSTEM
				InitializeGamepads();
#endif
			}
			internal void Update()
			{
#if ENABLE_INPUT_SYSTEM
				positiveValue = Utility.BoolToNumber(KeyboardPositiveMainPress);
				negativeValue = Utility.BoolToNumber(KeyboardNegativeMainPress);
				positiveCoeficient = 1f;
				negativeCoeficient = 1f;

				switch (Main.StrongSide)
				{
					case Axis.Side.FirstPressing:
						if (KeyboardMainValue > 0f)
							positiveCoeficient = 2f;
						else if (KeyboardMainValue < 0f)
							negativeCoeficient = 2f;

						break;

					case Axis.Side.Positive:
						positiveCoeficient = 2f;

						break;

					case Axis.Side.Negative:
						negativeCoeficient = 2f;

						break;
				}

				positiveValue *= positiveCoeficient;
				negativeValue *= negativeCoeficient;
				valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

				if (Type == InputType.Axis)
				{
					valueFactor += 1f;
					valueFactor *= .5f;
				}

				intervalA = invert ? valueInterval.y : valueInterval.x;
				intervalB = invert ? valueInterval.x : valueInterval.y;
				target = Mathf.Lerp(intervalA, intervalB, valueFactor);

				if (interpolation == Input.InputInterpolation.Smooth && InterpolationTime > 0f)
					keyboardMainValue = Mathf.MoveTowards(KeyboardMainValue, target, deltaTime / InterpolationTime);
				else if (interpolation == Input.InputInterpolation.Jump && InterpolationTime > 0f)
					keyboardMainValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(KeyboardMainValue) ? 0f : Mathf.MoveTowards(KeyboardMainValue, target, deltaTime / InterpolationTime);
				else
					keyboardMainValue = target;

				positiveValue = Utility.BoolToNumber(KeyboardPositiveAltPress);
				negativeValue = Utility.BoolToNumber(KeyboardNegativeAltPress);
				positiveCoeficient = 1f;
				negativeCoeficient = 1f;

				switch (Alt.StrongSide)
				{
					case Axis.Side.FirstPressing:
						if (KeyboardAltValue > 0f)
							positiveCoeficient = 2f;
						else if (KeyboardAltValue < 0f)
							negativeCoeficient = 2f;

						break;

					case Axis.Side.Positive:
						positiveCoeficient = 2f;

						break;

					case Axis.Side.Negative:
						negativeCoeficient = 2f;

						break;
				}

				positiveValue *= positiveCoeficient;
				negativeValue *= negativeCoeficient;
				valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

				if (Type == InputType.Axis)
				{
					valueFactor += 1f;
					valueFactor *= .5f;
				}

				target = Mathf.Lerp(intervalA, intervalB, valueFactor);

				if (interpolation == Input.InputInterpolation.Smooth && InterpolationTime > 0f)
					keyboardAltValue = Mathf.MoveTowards(KeyboardAltValue, target, deltaTime / InterpolationTime);
				else if (interpolation == Input.InputInterpolation.Jump && InterpolationTime > 0f)
					keyboardAltValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(KeyboardAltValue) ? 0f : Mathf.MoveTowards(KeyboardAltValue, target, deltaTime / InterpolationTime);
				else
					keyboardAltValue = target;

				if (KeyboardMainValue != 0f)
					keyboardValue = KeyboardMainValue;
				else
					keyboardValue = KeyboardAltValue;

				if (KeyboardBindable)
				{
					if (KeyboardMainBindable)
					{
						KeyboardPositiveMainPress = KeyboardPositiveMainBindable && keyboardPositiveMainControl.isPressed;
						KeyboardNegativeMainPress = KeyboardNegativeMainBindable && keyboardNegativeMainControl.isPressed;
						KeyboardMainPress = KeyboardPositiveMainPress || KeyboardNegativeMainPress;
						KeyboardPositiveMainDown = KeyboardPositiveMainBindable && keyboardPositiveMainControl.wasPressedThisFrame;
						KeyboardNegativeMainDown = KeyboardNegativeMainBindable && keyboardNegativeMainControl.wasPressedThisFrame;
						KeyboardMainDown = KeyboardPositiveMainDown || KeyboardNegativeMainDown;
						KeyboardPositiveMainUp = KeyboardPositiveMainBindable && keyboardPositiveMainControl.wasReleasedThisFrame;
						KeyboardNegativeMainUp = KeyboardNegativeMainBindable && keyboardNegativeMainControl.wasReleasedThisFrame;
						KeyboardMainUp = KeyboardPositiveMainUp || KeyboardNegativeMainUp;
						KeyboardPositiveMainHeld = false;

						if (KeyboardPositiveMainPress)
						{
							keyboardPositiveMainHoldTimer -= deltaTime;
							KeyboardPositiveMainHeld = keyboardPositiveMainHoldTimer <= 0f;

							if (KeyboardPositiveMainHeld)
								keyboardPositiveMainHoldTimer = HoldWaitTime;
						}
						else if (keyboardPositiveMainHoldTimer != HoldTriggerTime)
							keyboardPositiveMainHoldTimer = HoldTriggerTime;

						KeyboardNegativeMainHeld = false;

						if (KeyboardNegativeMainPress)
						{
							keyboardNegativeMainHoldTimer -= deltaTime;
							KeyboardNegativeMainHeld = keyboardNegativeMainHoldTimer <= 0f;

							if (KeyboardNegativeMainHeld)
								keyboardNegativeMainHoldTimer = HoldWaitTime;
						}
						else if (keyboardNegativeMainHoldTimer != HoldTriggerTime)
							keyboardNegativeMainHoldTimer = HoldTriggerTime;

						KeyboardMainHeld = KeyboardPositiveMainHeld || KeyboardNegativeMainHeld;
						keyboardPositiveMainDoublePressTimer += keyboardPositiveMainDoublePressTimer > 0f ? -deltaTime : keyboardPositiveMainDoublePressTimer;

						if (KeyboardPositiveMainUp)
							keyboardPositiveMainDoublePressTimer = DoublePressTimeout;

						KeyboardPositiveMainDoublePress = keyboardPositiveMainDoublePressInitiated && KeyboardPositiveMainUp;

						if (KeyboardPositiveMainUp && keyboardPositiveMainDoublePressTimer > 0f)
							keyboardPositiveMainDoublePressInitiated = true;

						if (keyboardPositiveMainDoublePressTimer <= 0f)
							keyboardPositiveMainDoublePressInitiated = false;

						keyboardNegativeMainDoublePressTimer += keyboardNegativeMainDoublePressTimer > 0f ? -deltaTime : keyboardNegativeMainDoublePressTimer;

						if (KeyboardNegativeMainUp)
							keyboardNegativeMainDoublePressTimer = DoublePressTimeout;

						KeyboardNegativeMainDoublePress = keyboardNegativeMainDoublePressInitiated && KeyboardNegativeMainUp;

						if (KeyboardNegativeMainUp && keyboardNegativeMainDoublePressTimer > 0f)
							keyboardNegativeMainDoublePressInitiated = true;

						if (keyboardNegativeMainDoublePressTimer <= 0f)
							keyboardNegativeMainDoublePressInitiated = false;

						KeyboardMainDoublePress = KeyboardPositiveMainDoublePress || KeyboardNegativeMainDoublePress;
					}

					if (KeyboardAltBindable)
					{
						KeyboardPositiveAltPress = KeyboardPositiveAltBindable && keyboardPositiveAltControl.isPressed;
						KeyboardNegativeAltPress = KeyboardNegativeAltBindable && keyboardNegativeAltControl.isPressed;
						KeyboardAltPress = KeyboardPositiveAltPress || KeyboardNegativeAltPress;
						KeyboardPositiveAltDown = KeyboardPositiveAltBindable && keyboardPositiveAltControl.wasPressedThisFrame;
						KeyboardNegativeAltDown = KeyboardNegativeAltBindable && keyboardNegativeAltControl.wasPressedThisFrame;
						KeyboardAltDown = KeyboardPositiveAltDown || KeyboardNegativeAltDown;
						KeyboardPositiveAltUp = KeyboardPositiveAltBindable && keyboardPositiveAltControl.wasReleasedThisFrame;
						KeyboardNegativeAltUp = KeyboardNegativeAltBindable && keyboardNegativeAltControl.wasReleasedThisFrame;
						KeyboardAltUp = KeyboardPositiveAltUp || KeyboardNegativeAltUp;
						KeyboardPositiveAltHeld = false;

						if (KeyboardPositiveAltPress)
						{
							keyboardPositiveAltHoldTimer -= deltaTime;
							KeyboardPositiveAltHeld = keyboardPositiveAltHoldTimer <= 0f;

							if (KeyboardPositiveAltHeld)
								keyboardPositiveAltHoldTimer = HoldWaitTime;
						}
						else if (keyboardPositiveAltHoldTimer != HoldTriggerTime)
							keyboardPositiveAltHoldTimer = HoldTriggerTime;

						KeyboardNegativeAltHeld = false;

						if (KeyboardNegativeAltPress)
						{
							keyboardNegativeAltHoldTimer -= deltaTime;
							KeyboardNegativeAltHeld = keyboardNegativeAltHoldTimer <= 0f;

							if (KeyboardNegativeAltHeld)
								keyboardNegativeAltHoldTimer = HoldWaitTime;
						}
						else if (keyboardNegativeAltHoldTimer != HoldTriggerTime)
							keyboardNegativeAltHoldTimer = HoldTriggerTime;

						KeyboardAltHeld = KeyboardPositiveAltHeld || KeyboardNegativeAltHeld;
						keyboardPositiveAltDoublePressTimer += keyboardPositiveAltDoublePressTimer > 0f ? -deltaTime : keyboardPositiveAltDoublePressTimer;

						if (KeyboardPositiveAltUp)
							keyboardPositiveAltDoublePressTimer = DoublePressTimeout;

						KeyboardPositiveAltDoublePress = keyboardPositiveAltDoublePressInitiated && KeyboardPositiveAltUp;

						if (KeyboardPositiveAltUp && keyboardPositiveAltDoublePressTimer > 0f)
							keyboardPositiveAltDoublePressInitiated = true;

						if (keyboardPositiveAltDoublePressTimer <= 0f)
							keyboardPositiveAltDoublePressInitiated = false;

						keyboardNegativeAltDoublePressTimer += keyboardNegativeAltDoublePressTimer > 0f ? -deltaTime : keyboardNegativeAltDoublePressTimer;

						if (KeyboardNegativeAltUp)
							keyboardNegativeAltDoublePressTimer = DoublePressTimeout;

						KeyboardNegativeAltDoublePress = keyboardNegativeAltDoublePressInitiated && KeyboardNegativeAltUp;

						if (KeyboardNegativeAltUp && keyboardNegativeAltDoublePressTimer > 0f)
							keyboardNegativeAltDoublePressInitiated = true;

						if (keyboardNegativeAltDoublePressTimer <= 0f)
							keyboardNegativeAltDoublePressInitiated = false;

						KeyboardAltDoublePress = KeyboardPositiveAltDoublePress || KeyboardNegativeAltDoublePress;
					}

					if (KeyboardPositiveBindable)
					{
						KeyboardPositivePress = KeyboardPositiveMainPress || KeyboardPositiveAltPress;
						KeyboardPositiveDown = KeyboardPositiveMainDown || KeyboardPositiveAltDown;
						KeyboardPositiveUp = KeyboardPositiveMainUp || KeyboardPositiveAltUp;
						KeyboardPositiveHeld = KeyboardPositiveMainHeld || KeyboardPositiveAltHeld;
						KeyboardPositiveDoublePress = KeyboardPositiveMainDoublePress || KeyboardPositiveAltDoublePress;
					}

					if (KeyboardNegativeBindable)
					{
						KeyboardNegativePress = KeyboardNegativeMainPress || KeyboardNegativeAltPress;
						KeyboardNegativeDown = KeyboardNegativeMainDown || KeyboardNegativeAltDown;
						KeyboardNegativeUp = KeyboardNegativeMainUp || KeyboardNegativeAltUp;
						KeyboardNegativeHeld = KeyboardNegativeMainHeld || KeyboardNegativeAltHeld;
						KeyboardNegativeDoublePress = KeyboardNegativeMainDoublePress || KeyboardNegativeAltDoublePress;
					}

					KeyboardPress = KeyboardMainPress || KeyboardAltPress;
					KeyboardDown = KeyboardMainDown || KeyboardAltDown;
					KeyboardUp = KeyboardMainUp || KeyboardAltUp;
					KeyboardHeld = KeyboardMainHeld || KeyboardAltHeld;
					KeyboardDoublePress = KeyboardMainDoublePress || KeyboardAltDoublePress;
				}

				for (int i = 0; i < GamepadsCount; i++)
				{
					positiveValue = GamepadPositiveMainBindable ? gamepadPositiveMainControls[i].ReadValue() : 0f;
					negativeValue = GamepadNegativeMainBindable ? gamepadNegativeMainControls[i].ReadValue() : 0f;
					positiveCoeficient = 1f;
					negativeCoeficient = 1f;

					switch (Main.GamepadStrongSide)
					{
						case Axis.Side.FirstPressing:
							if (GamepadMainValues[i] > 0f)
								positiveCoeficient = 2f;
							else if (GamepadMainValues[i] < 0f)
								negativeCoeficient = 2f;

							break;

						case Axis.Side.Positive:
							positiveCoeficient = 2f;

							break;

						case Axis.Side.Negative:
							negativeCoeficient = 2f;

							break;
					}

					positiveValue *= positiveCoeficient;
					negativeValue *= negativeCoeficient;
					valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

					if (Type == InputType.Axis)
					{
						valueFactor += 1f;
						valueFactor *= .5f;
					}

					intervalA = invert ? valueInterval.y : valueInterval.x;
					intervalB = invert ? valueInterval.x : valueInterval.y;
					target = Mathf.Lerp(intervalA, intervalB, valueFactor);

					if (interpolation == Input.InputInterpolation.Smooth && InterpolationTime > 0f)
						GamepadMainValues[i] = Mathf.MoveTowards(GamepadMainValues[i], target, deltaTime / InterpolationTime);
					else if (interpolation == Input.InputInterpolation.Jump && InterpolationTime > 0f)
						GamepadMainValues[i] = target != 0f && Mathf.Sign(target) != Mathf.Sign(GamepadMainValues[i]) ? 0f : Mathf.MoveTowards(GamepadMainValues[i], target, deltaTime / InterpolationTime);
					else
						GamepadMainValues[i] = target;

					positiveValue = GamepadPositiveAltBindable ? gamepadPositiveAltControls[i].ReadValue() : 0f;
					negativeValue = GamepadNegativeAltBindable ? gamepadNegativeAltControls[i].ReadValue() : 0f;
					positiveCoeficient = 1f;
					negativeCoeficient = 1f;

					switch (Alt.GamepadStrongSide)
					{
						case Axis.Side.FirstPressing:
							if (GamepadAltValues[i] > 0f)
								positiveCoeficient = 2f;
							else if (GamepadAltValues[i] < 0f)
								negativeCoeficient = 2f;

							break;

						case Axis.Side.Positive:
							positiveCoeficient = 2f;

							break;

						case Axis.Side.Negative:
							negativeCoeficient = 2f;

							break;
					}

					positiveValue *= positiveCoeficient;
					negativeValue *= negativeCoeficient;
					valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

					if (Type == InputType.Axis)
					{
						valueFactor += 1f;
						valueFactor *= .5f;
					}

					intervalA = invert ? valueInterval.y : valueInterval.x;
					intervalB = invert ? valueInterval.x : valueInterval.y;
					target = Mathf.Lerp(intervalA, intervalB, valueFactor);

					if (interpolation == Input.InputInterpolation.Smooth && InterpolationTime > 0f)
						gamepadAltValues[i] = Mathf.MoveTowards(GamepadAltValues[i], target, deltaTime / InterpolationTime);
					else if (interpolation == Input.InputInterpolation.Jump && InterpolationTime > 0f)
						gamepadAltValues[i] = target != 0f && Mathf.Sign(target) != Mathf.Sign(GamepadAltValues[i]) ? 0f : Mathf.MoveTowards(GamepadAltValues[i], target, deltaTime / InterpolationTime);
					else
						gamepadAltValues[i] = target;

					if (GamepadMainValues[i] != 0f)
						gamepadValues[i] = GamepadMainValues[i];
					else
						gamepadValues[i] = GamepadAltValues[i];

					if (GamepadBindable)
					{
						if (GamepadMainBindable)
						{
							GamepadsPositiveMainPress[i] = GamepadPositiveMainBindable && gamepadPositiveMainControls[i].isPressed;
							GamepadsNegativeMainPress[i] = GamepadNegativeMainBindable && gamepadNegativeMainControls[i].isPressed;
							GamepadsMainPress[i] = GamepadsPositiveMainPress[i] || GamepadsNegativeMainPress[i];
							GamepadsPositiveMainDown[i] = GamepadPositiveMainBindable && gamepadPositiveMainControls[i].wasPressedThisFrame;
							GamepadsNegativeMainDown[i] = GamepadNegativeMainBindable && gamepadNegativeMainControls[i].wasPressedThisFrame;
							GamepadsMainDown[i] = GamepadsPositiveMainDown[i] || GamepadsNegativeMainDown[i];
							GamepadsPositiveMainUp[i] = GamepadPositiveMainBindable && gamepadPositiveMainControls[i].wasReleasedThisFrame;
							GamepadsNegativeMainUp[i] = GamepadNegativeMainBindable && gamepadNegativeMainControls[i].wasReleasedThisFrame;
							GamepadsMainUp[i] = GamepadsPositiveMainUp[i] || GamepadsNegativeMainUp[i];
							GamepadsPositiveMainHeld[i] = false;

							if (GamepadsPositiveMainPress[i])
							{
								gamepadPositiveMainHoldTimers[i] -= deltaTime;
								GamepadsPositiveMainHeld[i] = gamepadPositiveMainHoldTimers[i] <= 0f;

								if (GamepadsPositiveMainHeld[i])
									gamepadPositiveMainHoldTimers[i] = HoldWaitTime;
							}
							else if (gamepadPositiveMainHoldTimers[i] != HoldTriggerTime)
								gamepadPositiveMainHoldTimers[i] = HoldTriggerTime;

							GamepadsNegativeMainHeld[i] = false;

							if (GamepadsNegativeMainPress[i])
							{
								gamepadNegativeMainHoldTimers[i] -= deltaTime;
								GamepadsNegativeMainHeld[i] = gamepadNegativeMainHoldTimers[i] <= 0f;

								if (GamepadsNegativeMainHeld[i])
									gamepadNegativeMainHoldTimers[i] = HoldWaitTime;
							}
							else if (gamepadNegativeMainHoldTimers[i] != HoldTriggerTime)
								gamepadNegativeMainHoldTimers[i] = HoldTriggerTime;

							GamepadsMainHeld[i] = GamepadsPositiveMainHeld[i] || GamepadsNegativeMainHeld[i];
							gamepadPositiveMainDoublePressTimers[i] += gamepadPositiveMainDoublePressTimers[i] > 0f ? -deltaTime : gamepadPositiveMainDoublePressTimers[i];

							if (GamepadsPositiveMainUp[i])
								gamepadPositiveMainDoublePressTimers[i] = DoublePressTimeout;

							GamepadsPositiveMainDoublePress[i] = gamepadsPositiveMainDoublePressInitiated[i] && GamepadsPositiveMainUp[i];

							if (GamepadsPositiveMainUp[i] && gamepadPositiveMainDoublePressTimers[i] > 0f)
								gamepadsPositiveMainDoublePressInitiated[i] = true;

							if (gamepadPositiveMainDoublePressTimers[i] <= 0f)
								gamepadsPositiveMainDoublePressInitiated[i] = false;

							gamepadNegativeMainDoublePressTimers[i] += gamepadNegativeMainDoublePressTimers[i] > 0f ? -deltaTime : gamepadNegativeMainDoublePressTimers[i];

							if (GamepadsNegativeMainUp[i])
								gamepadNegativeMainDoublePressTimers[i] = DoublePressTimeout;

							GamepadsNegativeMainDoublePress[i] = gamepadsNegativeMainDoublePressInitiated[i] && GamepadsNegativeMainUp[i];

							if (GamepadsNegativeMainUp[i] && gamepadNegativeMainDoublePressTimers[i] > 0f)
								gamepadsNegativeMainDoublePressInitiated[i] = true;

							if (gamepadNegativeMainDoublePressTimers[i] <= 0f)
								gamepadsNegativeMainDoublePressInitiated[i] = false;

							GamepadsMainDoublePress[i] = GamepadsPositiveMainDoublePress[i] || GamepadsNegativeMainDoublePress[i];
						}

						if (GamepadAltBindable)
						{
							GamepadsPositiveAltPress[i] = GamepadPositiveAltBindable && gamepadPositiveAltControls[i].isPressed;
							GamepadsNegativeAltPress[i] = GamepadNegativeAltBindable && gamepadNegativeAltControls[i].isPressed;
							GamepadsAltPress[i] = GamepadsPositiveAltPress[i] || GamepadsNegativeAltPress[i];
							GamepadsPositiveAltDown[i] = GamepadPositiveAltBindable && gamepadPositiveAltControls[i].wasPressedThisFrame;
							GamepadsNegativeAltDown[i] = GamepadNegativeAltBindable && gamepadNegativeAltControls[i].wasPressedThisFrame;
							GamepadsAltDown[i] = GamepadsPositiveAltDown[i] || GamepadsNegativeAltDown[i];
							GamepadsPositiveAltUp[i] = GamepadPositiveAltBindable && gamepadPositiveAltControls[i].wasReleasedThisFrame;
							GamepadsNegativeAltUp[i] = GamepadNegativeAltBindable && gamepadNegativeAltControls[i].wasReleasedThisFrame;
							GamepadsAltUp[i] = GamepadsPositiveAltUp[i] || GamepadsNegativeAltUp[i];
							GamepadsPositiveAltHeld[i] = false;

							if (GamepadsPositiveAltPress[i])
							{
								gamepadPositiveAltHoldTimers[i] -= deltaTime;
								GamepadsPositiveAltHeld[i] = gamepadPositiveAltHoldTimers[i] <= 0f;

								if (GamepadsPositiveAltHeld[i])
									gamepadPositiveAltHoldTimers[i] = HoldWaitTime;
							}
							else if (gamepadPositiveAltHoldTimers[i] != HoldTriggerTime)
								gamepadPositiveAltHoldTimers[i] = HoldTriggerTime;

							GamepadsNegativeAltHeld[i] = false;

							if (GamepadsNegativeAltPress[i])
							{
								gamepadNegativeAltHoldTimers[i] -= deltaTime;
								GamepadsNegativeAltHeld[i] = gamepadNegativeAltHoldTimers[i] <= 0f;

								if (GamepadsNegativeAltHeld[i])
									gamepadNegativeAltHoldTimers[i] = HoldWaitTime;
							}
							else if (gamepadNegativeAltHoldTimers[i] != HoldTriggerTime)
								gamepadNegativeAltHoldTimers[i] = HoldTriggerTime;

							GamepadsAltHeld[i] = GamepadsPositiveAltHeld[i] || GamepadsNegativeAltHeld[i];
							gamepadPositiveAltDoublePressTimers[i] += gamepadPositiveAltDoublePressTimers[i] > 0f ? -deltaTime : gamepadPositiveAltDoublePressTimers[i];

							if (GamepadsPositiveAltUp[i])
								gamepadPositiveAltDoublePressTimers[i] = DoublePressTimeout;

							GamepadsPositiveAltDoublePress[i] = gamepadsPositiveAltDoublePressInitiated[i] && GamepadsPositiveAltUp[i];

							if (GamepadsPositiveAltUp[i] && gamepadPositiveAltDoublePressTimers[i] > 0f)
								gamepadsPositiveAltDoublePressInitiated[i] = true;

							if (gamepadPositiveAltDoublePressTimers[i] <= 0f)
								gamepadsPositiveAltDoublePressInitiated[i] = false;

							gamepadNegativeAltDoublePressTimers[i] += gamepadNegativeAltDoublePressTimers[i] > 0f ? -deltaTime : gamepadNegativeAltDoublePressTimers[i];

							if (GamepadsNegativeAltUp[i])
								gamepadNegativeAltDoublePressTimers[i] = DoublePressTimeout;

							GamepadsNegativeAltDoublePress[i] = gamepadsNegativeAltDoublePressInitiated[i] && GamepadsNegativeAltUp[i];

							if (GamepadsNegativeAltUp[i] && gamepadNegativeAltDoublePressTimers[i] > 0f)
								gamepadsNegativeAltDoublePressInitiated[i] = true;

							if (gamepadNegativeAltDoublePressTimers[i] <= 0f)
								gamepadsNegativeAltDoublePressInitiated[i] = false;

							GamepadsAltDoublePress[i] = GamepadsPositiveAltDoublePress[i] || GamepadsNegativeAltDoublePress[i];
						}

						if (GamepadPositiveBindable)
						{
							GamepadsPositivePress[i] = GamepadsPositiveMainPress[i] || GamepadsPositiveAltPress[i];
							GamepadsPositiveDown[i] = GamepadsPositiveMainDown[i] || GamepadsPositiveAltDown[i];
							GamepadsPositiveUp[i] = GamepadsPositiveMainUp[i] || GamepadsPositiveAltUp[i];
							GamepadsPositiveHeld[i] = GamepadsPositiveMainHeld[i] || GamepadsPositiveAltHeld[i];
							GamepadsPositiveDoublePress[i] = GamepadsPositiveMainDoublePress[i] || GamepadsPositiveAltDoublePress[i];
						}

						if (GamepadNegativeBindable)
						{
							GamepadsNegativePress[i] = GamepadsNegativeMainPress[i] || GamepadsNegativeAltPress[i];
							GamepadsNegativeDown[i] = GamepadsNegativeMainDown[i] || GamepadsNegativeAltDown[i];
							GamepadsNegativeUp[i] = GamepadsNegativeMainUp[i] || GamepadsNegativeAltUp[i];
							GamepadsNegativeHeld[i] = GamepadsNegativeMainHeld[i] || GamepadsNegativeAltHeld[i];
							GamepadsNegativeDoublePress[i] = GamepadsNegativeMainDoublePress[i] || GamepadsNegativeAltDoublePress[i];
						}

						GamepadsPress[i] = GamepadsMainPress[i] || GamepadsAltPress[i];
						GamepadsDown[i] = GamepadsMainDown[i] || GamepadsAltDown[i];
						GamepadsUp[i] = GamepadsMainUp[i] || GamepadsAltUp[i];
						GamepadsHeld[i] = GamepadsMainHeld[i] || GamepadsAltHeld[i];
						GamepadsDoublePress[i] = GamepadsMainDoublePress[i] || GamepadsAltDoublePress[i];
					}
				}
#endif
			}
#if ENABLE_INPUT_SYSTEM
			internal void InitializeGamepads()
			{
				if (GamepadsPositiveMainPress == null || GamepadsPositiveMainPress.Length != GamepadsCount)
					GamepadsPositiveMainPress = new bool[GamepadsCount];

				if (GamepadsNegativeMainPress == null || GamepadsNegativeMainPress.Length != GamepadsCount)
					GamepadsNegativeMainPress = new bool[GamepadsCount];

				if (GamepadsMainPress == null || GamepadsMainPress.Length != GamepadsCount)
					GamepadsMainPress = new bool[GamepadsCount];

				if (GamepadsPositiveAltPress == null || GamepadsPositiveAltPress.Length != GamepadsCount)
					GamepadsPositiveAltPress = new bool[GamepadsCount];

				if (GamepadsNegativeAltPress == null || GamepadsNegativeAltPress.Length != GamepadsCount)
					GamepadsNegativeAltPress = new bool[GamepadsCount];

				if (GamepadsAltPress == null || GamepadsAltPress.Length != GamepadsCount)
					GamepadsAltPress = new bool[GamepadsCount];

				if (GamepadsPositivePress == null || GamepadsPositivePress.Length != GamepadsCount)
					GamepadsPositivePress = new bool[GamepadsCount];

				if (GamepadsNegativePress == null || GamepadsNegativePress.Length != GamepadsCount)
					GamepadsNegativePress = new bool[GamepadsCount];

				if (GamepadsPress == null || GamepadsPress.Length != GamepadsCount)
					GamepadsPress = new bool[GamepadsCount];

				if (GamepadsPositiveMainDown == null || GamepadsPositiveMainDown.Length != GamepadsCount)
					GamepadsPositiveMainDown = new bool[GamepadsCount];

				if (GamepadsNegativeMainDown == null || GamepadsNegativeMainDown.Length != GamepadsCount)
					GamepadsNegativeMainDown = new bool[GamepadsCount];

				if (GamepadsMainDown == null || GamepadsMainDown.Length != GamepadsCount)
					GamepadsMainDown = new bool[GamepadsCount];

				if (GamepadsPositiveAltDown == null || GamepadsPositiveAltDown.Length != GamepadsCount)
					GamepadsPositiveAltDown = new bool[GamepadsCount];

				if (GamepadsNegativeAltDown == null || GamepadsNegativeAltDown.Length != GamepadsCount)
					GamepadsNegativeAltDown = new bool[GamepadsCount];

				if (GamepadsAltDown == null || GamepadsAltDown.Length != GamepadsCount)
					GamepadsAltDown = new bool[GamepadsCount];

				if (GamepadsPositiveDown == null || GamepadsPositiveDown.Length != GamepadsCount)
					GamepadsPositiveDown = new bool[GamepadsCount];

				if (GamepadsNegativeDown == null || GamepadsNegativeDown.Length != GamepadsCount)
					GamepadsNegativeDown = new bool[GamepadsCount];

				if (GamepadsDown == null || GamepadsDown.Length != GamepadsCount)
					GamepadsDown = new bool[GamepadsCount];

				if (GamepadsPositiveMainUp == null || GamepadsPositiveMainUp.Length != GamepadsCount)
					GamepadsPositiveMainUp = new bool[GamepadsCount];

				if (GamepadsNegativeMainUp == null || GamepadsNegativeMainUp.Length != GamepadsCount)
					GamepadsNegativeMainUp = new bool[GamepadsCount];

				if (GamepadsMainUp == null || GamepadsMainUp.Length != GamepadsCount)
					GamepadsMainUp = new bool[GamepadsCount];

				if (GamepadsPositiveAltUp == null || GamepadsPositiveAltUp.Length != GamepadsCount)
					GamepadsPositiveAltUp = new bool[GamepadsCount];

				if (GamepadsNegativeAltUp == null || GamepadsNegativeAltUp.Length != GamepadsCount)
					GamepadsNegativeAltUp = new bool[GamepadsCount];

				if (GamepadsAltUp == null || GamepadsAltUp.Length != GamepadsCount)
					GamepadsAltUp = new bool[GamepadsCount];

				if (GamepadsPositiveUp == null || GamepadsPositiveUp.Length != GamepadsCount)
					GamepadsPositiveUp = new bool[GamepadsCount];

				if (GamepadsNegativeUp == null || GamepadsNegativeUp.Length != GamepadsCount)
					GamepadsNegativeUp = new bool[GamepadsCount];

				if (GamepadsUp == null || GamepadsUp.Length != GamepadsCount)
					GamepadsUp = new bool[GamepadsCount];

				if (GamepadsPositiveMainHeld == null || GamepadsPositiveMainHeld.Length != GamepadsCount)
					GamepadsPositiveMainHeld = new bool[GamepadsCount];

				if (GamepadsNegativeMainHeld == null || GamepadsNegativeMainHeld.Length != GamepadsCount)
					GamepadsNegativeMainHeld = new bool[GamepadsCount];

				if (GamepadsMainHeld == null || GamepadsMainHeld.Length != GamepadsCount)
					GamepadsMainHeld = new bool[GamepadsCount];

				if (GamepadsPositiveAltHeld == null || GamepadsPositiveAltHeld.Length != GamepadsCount)
					GamepadsPositiveAltHeld = new bool[GamepadsCount];

				if (GamepadsNegativeAltHeld == null || GamepadsNegativeAltHeld.Length != GamepadsCount)
					GamepadsNegativeAltHeld = new bool[GamepadsCount];

				if (GamepadsAltHeld == null || GamepadsAltHeld.Length != GamepadsCount)
					GamepadsAltHeld = new bool[GamepadsCount];

				if (GamepadsPositiveHeld == null || GamepadsPositiveHeld.Length != GamepadsCount)
					GamepadsPositiveHeld = new bool[GamepadsCount];

				if (GamepadsNegativeHeld == null || GamepadsNegativeHeld.Length != GamepadsCount)
					GamepadsNegativeHeld = new bool[GamepadsCount];

				if (GamepadsHeld == null || GamepadsHeld.Length != GamepadsCount)
					GamepadsHeld = new bool[GamepadsCount];

				if (GamepadsPositiveMainDoublePress == null || GamepadsPositiveMainDoublePress.Length != GamepadsCount)
					GamepadsPositiveMainDoublePress = new bool[GamepadsCount];

				if (GamepadsNegativeMainDoublePress == null || GamepadsNegativeMainDoublePress.Length != GamepadsCount)
					GamepadsNegativeMainDoublePress = new bool[GamepadsCount];

				if (GamepadsMainDoublePress == null || GamepadsMainDoublePress.Length != GamepadsCount)
					GamepadsMainDoublePress = new bool[GamepadsCount];

				if (GamepadsPositiveAltDoublePress == null || GamepadsPositiveAltDoublePress.Length != GamepadsCount)
					GamepadsPositiveAltDoublePress = new bool[GamepadsCount];

				if (GamepadsNegativeAltDoublePress == null || GamepadsNegativeAltDoublePress.Length != GamepadsCount)
					GamepadsNegativeAltDoublePress = new bool[GamepadsCount];

				if (GamepadsAltDoublePress == null || GamepadsAltDoublePress.Length != GamepadsCount)
					GamepadsAltDoublePress = new bool[GamepadsCount];

				if (GamepadsPositiveDoublePress == null || GamepadsPositiveDoublePress.Length != GamepadsCount)
					GamepadsPositiveDoublePress = new bool[GamepadsCount];

				if (GamepadsNegativeDoublePress == null || GamepadsNegativeDoublePress.Length != GamepadsCount)
					GamepadsNegativeDoublePress = new bool[GamepadsCount];

				if (GamepadsDoublePress == null || GamepadsDoublePress.Length != GamepadsCount)
					GamepadsDoublePress = new bool[GamepadsCount];

				if (gamepadPositiveMainControls == null)
					gamepadPositiveMainControls = new ButtonControl[] { };

				if (gamepadNegativeMainControls == null)
					gamepadNegativeMainControls = new ButtonControl[] { };

				if (gamepadMainValues == null)
					gamepadMainValues = new float[] { };

				if (gamepadPositiveAltControls == null)
					gamepadPositiveAltControls = new ButtonControl[] { };

				if (gamepadNegativeAltControls == null)
					gamepadNegativeAltControls = new ButtonControl[] { };

				if (gamepadAltValues == null)
					gamepadAltValues = new float[] { };

				if (gamepadValues == null)
					gamepadValues = new float[] { };

				if (gamepadPositiveMainHoldTimers == null)
					gamepadPositiveMainHoldTimers = new float[] { };

				if (gamepadNegativeMainHoldTimers == null)
					gamepadNegativeMainHoldTimers = new float[] { };

				if (gamepadPositiveAltHoldTimers == null)
					gamepadPositiveAltHoldTimers = new float[] { };

				if (gamepadNegativeAltHoldTimers == null)
					gamepadNegativeAltHoldTimers = new float[] { };

				if (gamepadPositiveMainDoublePressTimers == null)
					gamepadPositiveMainDoublePressTimers = new float[] { };

				if (gamepadNegativeMainDoublePressTimers == null)
					gamepadNegativeMainDoublePressTimers = new float[] { };

				if (gamepadPositiveAltDoublePressTimers == null)
					gamepadPositiveAltDoublePressTimers = new float[] { };

				if (gamepadNegativeAltDoublePressTimers == null)
					gamepadNegativeAltDoublePressTimers = new float[] { };

				if (gamepadsPositiveMainDoublePressInitiated == null)
					gamepadsPositiveMainDoublePressInitiated = new bool[] { };

				if (gamepadsNegativeMainDoublePressInitiated == null)
					gamepadsNegativeMainDoublePressInitiated = new bool[] { };

				if (gamepadsPositiveAltDoublePressInitiated == null)
					gamepadsPositiveAltDoublePressInitiated = new bool[] { };

				if (gamepadsNegativeAltDoublePressInitiated == null)
					gamepadsNegativeAltDoublePressInitiated = new bool[] { };

				Array.Resize(ref gamepadPositiveMainControls, GamepadsCount);
				Array.Resize(ref gamepadNegativeMainControls, GamepadsCount);
				Array.Resize(ref gamepadMainValues, GamepadsCount);
				Array.Resize(ref gamepadPositiveAltControls, GamepadsCount);
				Array.Resize(ref gamepadNegativeAltControls, GamepadsCount);
				Array.Resize(ref gamepadAltValues, GamepadsCount);
				Array.Resize(ref gamepadValues, GamepadsCount);
				Array.Resize(ref gamepadPositiveMainHoldTimers, GamepadsCount);
				Array.Resize(ref gamepadNegativeMainHoldTimers, GamepadsCount);
				Array.Resize(ref gamepadPositiveAltHoldTimers, GamepadsCount);
				Array.Resize(ref gamepadNegativeAltHoldTimers, GamepadsCount);
				Array.Resize(ref gamepadPositiveMainDoublePressTimers, GamepadsCount);
				Array.Resize(ref gamepadNegativeMainDoublePressTimers, GamepadsCount);
				Array.Resize(ref gamepadPositiveAltDoublePressTimers, GamepadsCount);
				Array.Resize(ref gamepadNegativeAltDoublePressTimers, GamepadsCount);
				Array.Resize(ref gamepadsPositiveMainDoublePressInitiated, GamepadsCount);
				Array.Resize(ref gamepadsNegativeMainDoublePressInitiated, GamepadsCount);
				Array.Resize(ref gamepadsPositiveAltDoublePressInitiated, GamepadsCount);
				Array.Resize(ref gamepadsNegativeAltDoublePressInitiated, GamepadsCount);

				for (int i = 0; i < GamepadsCount; i++)
				{
					if (gamepadPositiveMainControls[i] == null && GamepadPositiveMainBindable)
						gamepadPositiveMainControls[i] = GamepadBindingToButtonControl(Gamepads[i], Main.GamepadPositive);

					if (gamepadNegativeMainControls[i] == null && GamepadNegativeMainBindable)
						gamepadNegativeMainControls[i] = GamepadBindingToButtonControl(Gamepads[i], Main.GamepadNegative);

					if (gamepadPositiveAltControls[i] == null && GamepadPositiveAltBindable)
						gamepadPositiveAltControls[i] = GamepadBindingToButtonControl(Gamepads[i], Alt.GamepadPositive);

					if (gamepadNegativeAltControls[i] == null && GamepadNegativeAltBindable)
						gamepadNegativeAltControls[i] = GamepadBindingToButtonControl(Gamepads[i], Alt.GamepadNegative);
				}
			}
#endif

			private void Trim()
			{
				if (!Main)
					main = new Axis();

				if (!Alt)
					alt = new Axis();

#if ENABLE_INPUT_SYSTEM
				if (Main.Positive == Key.None && Alt.Positive != Key.None)
				{
					Main.Positive = Alt.Positive;
					Alt.Positive = Key.None;
				}

				if (Main.GamepadPositive == GamepadBinding.None && Alt.GamepadPositive != GamepadBinding.None)
				{
					Main.GamepadPositive = Alt.GamepadPositive;
					Alt.GamepadPositive = GamepadBinding.None;
				}

				if (Main.Negative == Key.None && Alt.Negative != Key.None)
				{
					Main.Negative = Alt.Negative;
					Alt.Negative = Key.None;
				}

				if (Main.GamepadNegative == GamepadBinding.None && Alt.GamepadNegative != GamepadBinding.None)
				{
					Main.GamepadNegative = Alt.GamepadNegative;
					Alt.GamepadNegative = GamepadBinding.None;
				}
#endif

				if (Type == InputType.Button)
				{
					if (valueInterval.x != 0f)
						valueInterval.x = 0f;

#if ENABLE_INPUT_SYSTEM
					if (Main.Negative != Key.None)
						Main.Negative = Key.None;

					if (Main.GamepadNegative != GamepadBinding.None)
						Main.GamepadNegative = GamepadBinding.None;

					if (Alt.Negative != Key.None)
						Alt.Negative = Key.None;

					if (Alt.GamepadNegative != GamepadBinding.None)
						Alt.GamepadNegative = GamepadBinding.None;
#endif
				}

				if (Application.isPlaying)
					trimmed = true;
			}
#if ENABLE_INPUT_SYSTEM
			private void SetAxisKeyOrButton(Axis axis, InputAxisSide binding, int keyOrButton, bool gamepad)
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
#endif
			#endregion

			#region Constructors & Operators

			#region Constructors

			public Input(string name)
			{
				this.name = name;
				valueInterval = new Vector2(-1f, 1f);
				main = new Axis();
				alt = new Axis();
			}
			public Input(Input input)
			{
				name = $"{input.name} (Clone)";
				type = input.type;
				interpolation = input.interpolation;
				valueInterval = input.valueInterval;
				main = new Axis(input.main);
				alt = new Axis(input.alt);

				Trim();
			}

			#endregion

			#region Operators

			public static implicit operator bool(Input input) => input != null;

			#endregion

			#endregion
		}

		#endregion

		#endregion

		#region Variables

		public static InputSource InputSourcePriority
		{
			get
			{
				if (!Application.isPlaying && inputSourcePriority == 0f)
					LoadData();

				return inputSourcePriority;
			}
			set
			{
				if (inputSourcePriority == value)
					return;

				inputSourcePriority = value;

				SaveData();
			}
		}
		public static readonly string DataAssetPath = $"Assets{Path.DirectorySeparatorChar}InputsManager_Data";
		public static float InterpolationTime
		{
			get
			{
				if (!Application.isPlaying && interpolationTime == 0f)
					LoadData();

				return interpolationTime;
			}
			set
			{
				if (interpolationTime == value)
					return;

				interpolationTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float HoldTriggerTime
		{
			get
			{
				if (!Application.isPlaying && holdTriggerTime == 0f)
					LoadData();

				return holdTriggerTime;
			}
			set
			{
				if (holdTriggerTime == value)
					return;

				holdTriggerTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float HoldWaitTime
		{
			get
			{
				if (!Application.isPlaying && holdWaitTime == 0f)
					LoadData();

				return holdWaitTime;
			}
			set
			{
				if (holdWaitTime == value)
					return;

				holdWaitTime = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float DoublePressTimeout
		{
			get
			{
				if (!Application.isPlaying && doublePressTimeout == 0f)
					LoadData();

				return doublePressTimeout;
			}
			set
			{
				if (doublePressTimeout == value)
					return;

				doublePressTimeout = Utility.ClampInfinity(value, 0f);

				SaveData();
			}
		}
		public static float GamepadThreshold
		{
			get
			{
				if (!Application.isPlaying && gamepadThreshold == 0f)
					LoadData();

				return gamepadThreshold;
			}
			set
			{
				if (Application.isPlaying || gamepadThreshold == value)
					return;

				gamepadThreshold = Mathf.Clamp01(value);

				SaveData();
			}
		}
		public static bool DataLoaded
		{
			get
			{
				if (!Application.isEditor)
					return dataLoadedOnBuild;

				return DataAssetExists && File.GetLastWriteTime(Path.Combine(Application.dataPath, "Resources", $"{DataAssetPath}.bytes")) == dataLastWriteTime;
			}
		}
		public static bool DataChanged
		{
			get
			{
				if (!Application.isEditor)
					return false;

				return DataLoaded && dataChanged;
			}
		}
		public static bool DataAssetExists
		{
			get
			{
				return Application.isEditor ? File.Exists(Path.Combine(Application.dataPath, "Resources", $"{DataAssetPath}.bytes")) : Resources.Load<TextAsset>(DataAssetPath);
			}
		}
		public static int Count
		{
			get
			{
				return Inputs.Length;
			}
		}
#if ENABLE_INPUT_SYSTEM
		public static Keyboard Keyboard
		{
			get
			{
				return Keyboard.current;
			}
		}
		public static Mouse Mouse
		{
			get
			{
				return Mouse.current;
			}
		}
		public static Gamepad[] Gamepads
		{
			get
			{
				return Gamepad.all.ToArray();
			}
		}
		public static string[] GamepadNames
		{
			get
			{
				if (gamepadNames == null || gamepadNames.Length != GamepadsCount)
					gamepadNames = Gamepad.all.Select(gamepad => gamepad.name).ToArray();

				return gamepadNames;
			}
		}
		public static int GamepadsCount
		{
			get
			{
				return Gamepads.Length;
			}
		}
#if UNITY_2019_1_OR_NEWER
		public static UnityEngine.InputSystem.EnhancedTouch.Touch[] Touches
		{
			get
			{
				if (!EnhancedTouchSupport.enabled)
					EnhancedTouchSupport.Enable();

				return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.ToArray();
			}
		}
#else
		public static UnityEngine.Touch[] Touches
		{
			get
			{
				if (!UnityEngine.Input.multiTouchEnabled)
					UnityEngine.Input.multiTouchEnabled = true;

				return UnityEngine.Input.touches;
			}
		}
#endif
		public static int TouchCount
		{
			get
			{
#if UNITY_2019_1_OR_NEWER
				return Touches.Length;
#else
				if (!UnityEngine.Input.multiTouchEnabled)
					UnityEngine.Input.multiTouchEnabled = true;

				return UnityEngine.Input.touchCount;
#endif
			}
		}
#endif

		private static Input[] Inputs
		{
			get
			{
				if (!Application.isPlaying)
					LoadData();

				if (inputs == null)
					inputs = new Input[] { };

				return inputs;
			}
		}
		private static string[] inputNames;
		private static string[] gamepadNames;
		private static DateTime dataLastWriteTime;
		private static Input[] inputs;
		private static InputSource inputSourcePriority;
		private static float interpolationTime = .25f;
		private static float holdTriggerTime = .3f;
		private static float holdWaitTime = .1f;
		private static float doublePressTimeout = .2f;
		private static float gamepadThreshold = .5f;
		private static float mouseLeftHoldTimer;
		private static float mouseMiddleHoldTimer;
		private static float mouseRightHoldTimer;
		private static float mouseBackHoldTimer;
		private static float mouseForwardHoldTimer;
		private static float mouseLeftDoublePressTimer;
		private static float mouseMiddleDoublePressTimer;
		private static float mouseRightDoublePressTimer;
		private static float mouseBackDoublePressTimer;
		private static float mouseForwardDoublePressTimer;
		private static float deltaTime;
#if !UNITY_2019_1_OR_NEWER
		private static bool anyKeyPressed;
#endif
		private static bool mouseLeftHeld;
		private static bool mouseMiddleHeld;
		private static bool mouseRightHeld;
		private static bool mouseBackHeld;
		private static bool mouseForwardHeld;
		private static bool MouseLeftDoublePress;
		private static bool MouseMiddleDoublePress;
		private static bool MouseRightDoublePress;
		private static bool MouseBackDoublePress;
		private static bool MouseForwardDoublePress;
		private static bool mouseLeftDoublePressInitiated;
		private static bool mouseMiddleDoublePressInitiated;
		private static bool mouseRightDoublePressInitiated;
		private static bool mouseBackDoublePressInitiated;
		private static bool mouseForwardDoublePressInitiated;
		private static bool mousePressed;
		private static bool dataChanged;
		private static bool dataLoadedOnBuild;
#if ENABLE_INPUT_SYSTEM
		private static int gamepadsCount;
#endif

		#endregion

		#region Methods

		#region Inputs

		public static Vector2 InputMouseMovement()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.delta.ReadValue();
#else
			return Vector2.zero;
#endif
		}
		public static Vector2 InputMousePosition()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.position.ReadValue();
#else
			return Vector2.zero;
#endif
		}
		public static Vector2 InputMouseScrollWheelVector()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.scroll.ReadValue();
#else
			return Vector2.zero;
#endif
		}
#if ENABLE_INPUT_SYSTEM
		public static float InputValue(Input input, int gamepadIndex = 0)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadValues[gamepadIndex] != 0f;
			bool keyboardUsed = input.KeyboardValue != 0f;
			bool gamepadPerioritized = InputSourcePriority == InputSource.Gamepad;

			if ((gamepadPerioritized || !keyboardUsed) && gamepadUsed)
				return input.GamepadValues[gamepadIndex];
			else
				return input.KeyboardValue;
		}
		public static float InputValue(string name, int gamepadIndex = 0) => InputValue(GetInput(name), gamepadIndex);
		public static float InputValue(int index, int gamepadIndex = 0) => InputValue(GetInput(index), gamepadIndex);
		public static float InputMainAxisValue(Input input, int gamepadIndex = 0)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadMainValues[gamepadIndex] != 0f;
			bool keyboardUsed = input.KeyboardMainValue != 0f;
			bool gamepadPerioritized = InputSourcePriority == InputSource.Gamepad;

			if ((gamepadPerioritized || !keyboardUsed) && gamepadUsed)
				return input.GamepadMainValues[gamepadIndex];
			else
				return input.KeyboardMainValue;
		}
		public static float InputMainAxisValue(string name, int gamepadIndex = 0) => InputMainAxisValue(GetInput(name), gamepadIndex);
		public static float InputMainAxisValue(int index, int gamepadIndex = 0) => InputMainAxisValue(GetInput(index), gamepadIndex);
		public static float InputAltAxisValue(Input input, int gamepadIndex = 0)
		{
			bool gamepadUsed = gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadAltValues[gamepadIndex] != 0f;
			bool keyboardUsed = input.KeyboardAltValue != 0f;
			bool gamepadPerioritized = InputSourcePriority == InputSource.Gamepad;

			if ((gamepadPerioritized || !keyboardUsed) && gamepadUsed)
				return input.GamepadAltValues[gamepadIndex];
			else
				return input.KeyboardAltValue;
		}
		public static float InputAltAxisValue(string name, int gamepadIndex = 0) => InputAltAxisValue(GetInput(name), gamepadIndex);
		public static float InputAltAxisValue(int index, int gamepadIndex = 0) => InputAltAxisValue(GetInput(index), gamepadIndex);
		public static float InputMouseScrollWheel()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.scroll.ReadValue().magnitude;
#else
			return 0f;
#endif
		}
		public static float InputMouseScrollWheelHorizontal()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.scroll.ReadValue().x;
#else
			return 0f;
#endif
		}
		public static float InputMouseScrollWheelVertical()
		{
#if ENABLE_INPUT_SYSTEM
			return Mouse.scroll.ReadValue().y;
#else
			return 0f;
#endif
		}
		public static bool AnyInputPress(bool ignoreMouse = false)
		{
#if ENABLE_INPUT_SYSTEM
			return Keyboard.anyKey.isPressed || !ignoreMouse && InputMouseAnyButtonPress() || Gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.isPressed));
#else
			return false;
#endif
		}
		public static bool AnyInputDown(bool ignoreMouse = false)
		{
#if ENABLE_INPUT_SYSTEM
#if UNITY_2019_1_OR_NEWER
			return Keyboard.anyKey.wasPressedThisFrame || !ignoreMouse && InputMouseAnyButtonDown() || Gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasPressedThisFrame));
#else
			return Utility.IsDownFromLastState(Keyboard.anyKey.isPressed, anyKeyPressed);
#endif
#else
			return false;
#endif
		}
		public static bool AnyInputUp(bool ignoreMouse = false)
		{
#if ENABLE_INPUT_SYSTEM
#if UNITY_2019_1_OR_NEWER
			return Keyboard.anyKey.wasReleasedThisFrame || !ignoreMouse && InputMouseAnyButtonUp() || Gamepads.Any(gamepad => gamepad.allControls.Any(control => control is ButtonControl button && !button.synthetic && button.wasReleasedThisFrame));
#else
			return Utility.IsUpFromLastState(Keyboard.anyKey.isPressed, anyKeyPressed);
#endif
#else
			return false;
#endif
		}
		public static bool InputPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPress[gamepadIndex];
		}
		public static bool InputPress(string name, int gamepadIndex = 0)
		{
			return InputPress(GetInput(name), gamepadIndex);
		}
		public static bool InputPress(int index, int gamepadIndex = 0)
		{
			return InputPress(GetInput(index), gamepadIndex);
		}
		public static bool InputMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardMainPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsMainPress[gamepadIndex];
		}
		public static bool InputMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputMainAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputMainAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardAltPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsAltPress[gamepadIndex];
		}
		public static bool InputAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputAltAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputAltAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositivePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositivePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositivePress[gamepadIndex];
		}
		public static bool InputPositivePress(string name, int gamepadIndex = 0)
		{
			return InputPositivePress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositivePress(int index, int gamepadIndex = 0)
		{
			return InputPositivePress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativePress[gamepadIndex];
		}
		public static bool InputNegativePress(string name, int gamepadIndex = 0)
		{
			return InputNegativePress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativePress(int index, int gamepadIndex = 0)
		{
			return InputNegativePress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveMainPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveMainPress[gamepadIndex];
		}
		public static bool InputPositiveMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeMainPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeMainPress[gamepadIndex];
		}
		public static bool InputNegativeMainAxisPress(string name, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisPress(int index, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveAltPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveAltPress[gamepadIndex];
		}
		public static bool InputPositiveAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisPress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeAltPress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeAltPress[gamepadIndex];
		}
		public static bool InputNegativeAltAxisPress(string name, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisPress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisPress(int index, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisPress(GetInput(index), gamepadIndex);
		}
		public static bool InputDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsDown[gamepadIndex];
		}
		public static bool InputDown(string name, int gamepadIndex = 0)
		{
			return InputDown(GetInput(name), gamepadIndex);
		}
		public static bool InputDown(int index, int gamepadIndex = 0)
		{
			return InputDown(GetInput(index), gamepadIndex);
		}
		public static bool InputMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardMainDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsMainDown[gamepadIndex];
		}
		public static bool InputMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputMainAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputMainAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardAltDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsAltDown[gamepadIndex];
		}
		public static bool InputAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputAltAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputAltAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveDown[gamepadIndex];
		}
		public static bool InputPositiveDown(string name, int gamepadIndex = 0)
		{
			return InputPositiveDown(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveDown(int index, int gamepadIndex = 0)
		{
			return InputPositiveDown(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeDown[gamepadIndex];
		}
		public static bool InputNegativeDown(string name, int gamepadIndex = 0)
		{
			return InputNegativeDown(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeDown(int index, int gamepadIndex = 0)
		{
			return InputNegativeDown(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveMainDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveMainDown[gamepadIndex];
		}
		public static bool InputPositiveMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeMainDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeMainDown[gamepadIndex];
		}
		public static bool InputNegativeMainAxisDown(string name, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDown(int index, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveAltDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveAltDown[gamepadIndex];
		}
		public static bool InputPositiveAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDown(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeAltDown || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeAltDown[gamepadIndex];
		}
		public static bool InputNegativeAltAxisDown(string name, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisDown(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDown(int index, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisDown(GetInput(index), gamepadIndex);
		}
		public static bool InputUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsUp[gamepadIndex];
		}
		public static bool InputUp(string name, int gamepadIndex = 0)
		{
			return InputUp(GetInput(name), gamepadIndex);
		}
		public static bool InputUp(int index, int gamepadIndex = 0)
		{
			return InputUp(GetInput(index), gamepadIndex);
		}
		public static bool InputMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardMainUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsMainUp[gamepadIndex];
		}
		public static bool InputMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputMainAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputMainAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardAltUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsAltUp[gamepadIndex];
		}
		public static bool InputAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputAltAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputAltAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveUp[gamepadIndex];
		}
		public static bool InputPositiveUp(string name, int gamepadIndex = 0)
		{
			return InputPositiveUp(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveUp(int index, int gamepadIndex = 0)
		{
			return InputPositiveUp(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeUp[gamepadIndex];
		}
		public static bool InputNegativeUp(string name, int gamepadIndex = 0)
		{
			return InputNegativeUp(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeUp(int index, int gamepadIndex = 0)
		{
			return InputNegativeUp(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveMainUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveMainUp[gamepadIndex];
		}
		public static bool InputPositiveMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeMainUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeMainUp[gamepadIndex];
		}
		public static bool InputNegativeMainAxisUp(string name, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisUp(int index, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveAltUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveAltUp[gamepadIndex];
		}
		public static bool InputPositiveAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisUp(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeAltUp || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeAltUp[gamepadIndex];
		}
		public static bool InputNegativeAltAxisUp(string name, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisUp(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisUp(int index, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisUp(GetInput(index), gamepadIndex);
		}
		public static bool InputHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsHeld[gamepadIndex];
		}
		public static bool InputHold(string name, int gamepadIndex = 0)
		{
			return InputHold(GetInput(name), gamepadIndex);
		}
		public static bool InputHold(int index, int gamepadIndex = 0)
		{
			return InputHold(GetInput(index), gamepadIndex);
		}
		public static bool InputMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardMainHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsMainHeld[gamepadIndex];
		}
		public static bool InputMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputMainAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputMainAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardAltHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsAltHeld[gamepadIndex];
		}
		public static bool InputAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputAltAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputAltAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveHeld[gamepadIndex];
		}
		public static bool InputPositiveHold(string name, int gamepadIndex = 0)
		{
			return InputPositiveHold(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveHold(int index, int gamepadIndex = 0)
		{
			return InputPositiveHold(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeHeld[gamepadIndex];
		}
		public static bool InputNegativeHold(string name, int gamepadIndex = 0)
		{
			return InputNegativeHold(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeHold(int index, int gamepadIndex = 0)
		{
			return InputNegativeHold(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveMainHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveMainHeld[gamepadIndex];
		}
		public static bool InputPositiveMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeMainHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeMainHeld[gamepadIndex];
		}
		public static bool InputNegativeMainAxisHold(string name, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisHold(int index, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveAltHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveAltHeld[gamepadIndex];
		}
		public static bool InputPositiveAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisHold(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeAltHeld || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeAltHeld[gamepadIndex];
		}
		public static bool InputNegativeAltAxisHold(string name, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisHold(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisHold(int index, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisHold(GetInput(index), gamepadIndex);
		}
		public static bool InputDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsDoublePress[gamepadIndex];
		}
		public static bool InputDoublePress(string name, int gamepadIndex = 0)
		{
			return InputDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputDoublePress(int index, int gamepadIndex = 0)
		{
			return InputDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardMainDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsMainDoublePress[gamepadIndex];
		}
		public static bool InputMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputMainAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputMainAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardAltDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsAltDoublePress[gamepadIndex];
		}
		public static bool InputAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputAltAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputAltAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveDoublePress[gamepadIndex];
		}
		public static bool InputPositiveDoublePress(string name, int gamepadIndex = 0)
		{
			return InputPositiveDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveDoublePress(int index, int gamepadIndex = 0)
		{
			return InputPositiveDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeDoublePress[gamepadIndex];
		}
		public static bool InputNegativeDoublePress(string name, int gamepadIndex = 0)
		{
			return InputNegativeDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeDoublePress(int index, int gamepadIndex = 0)
		{
			return InputNegativeDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveMainDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveMainDoublePress[gamepadIndex];
		}
		public static bool InputPositiveMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputPositiveMainAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeMainDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeMainDoublePress[gamepadIndex];
		}
		public static bool InputNegativeMainAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeMainAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputNegativeMainAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardPositiveAltDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsPositiveAltDoublePress[gamepadIndex];
		}
		public static bool InputPositiveAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputPositiveAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputPositiveAltAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDoublePress(Input input, int gamepadIndex = 0)
		{
			return input.KeyboardNegativeAltDoublePress || gamepadIndex > -1 && gamepadIndex < GamepadsCount && input.GamepadsNegativeAltDoublePress[gamepadIndex];
		}
		public static bool InputNegativeAltAxisDoublePress(string name, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisDoublePress(GetInput(name), gamepadIndex);
		}
		public static bool InputNegativeAltAxisDoublePress(int index, int gamepadIndex = 0)
		{
			return InputNegativeAltAxisDoublePress(GetInput(index), gamepadIndex);
		}
		public static bool InputKeyPress(Key key)
		{
			return KeyToKeyControl(key).isPressed;
		}
		public static bool InputKeyDown(Key key)
		{
			return KeyToKeyControl(key).wasPressedThisFrame;
		}
		public static bool InputKeyUp(Key key)
		{
			return KeyToKeyControl(key).wasReleasedThisFrame;
		}
#endif
		public static bool InputMouseAnyButtonPress()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonPress(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonPress(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

#if ENABLE_INPUT_SYSTEM
			switch (type)
			{
				case 0:
					return Mouse.leftButton.isPressed;

				case 1:
					return Mouse.rightButton.isPressed;

				case 2:
					return Mouse.middleButton.isPressed;

				case 3:
					return Mouse.backButton.isPressed;

				case 4:
					return Mouse.forwardButton.isPressed;
			}
#endif

			return false;
		}
		public static bool InputMouseButtonPress(MouseButton type)
		{
			return InputMouseButtonPress((int)type);
		}
		public static bool InputMouseAnyButtonDown()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDown(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonDown(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

#if ENABLE_INPUT_SYSTEM
			switch (type)
			{
				case 0:
					return Mouse.leftButton.wasPressedThisFrame;

				case 1:
					return Mouse.rightButton.wasPressedThisFrame;

				case 2:
					return Mouse.middleButton.wasPressedThisFrame;

				case 3:
					return Mouse.backButton.wasPressedThisFrame;

				case 4:
					return Mouse.forwardButton.wasPressedThisFrame;
			}
#endif

			return false;
		}
		public static bool InputMouseButtonDown(MouseButton type)
		{
			return InputMouseButtonDown((int)type);
		}
		public static bool InputMouseAnyButtonUp()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonUp(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonUp(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

#if ENABLE_INPUT_SYSTEM
			switch (type)
			{
				case 0:
					return Mouse.leftButton.wasReleasedThisFrame;

				case 1:
					return Mouse.rightButton.wasReleasedThisFrame;

				case 2:
					return Mouse.middleButton.wasReleasedThisFrame;

				case 3:
					return Mouse.backButton.wasReleasedThisFrame;

				case 4:
					return Mouse.forwardButton.wasReleasedThisFrame;
			}
#endif

			return false;
		}
		public static bool InputMouseButtonUp(MouseButton type)
		{
			return InputMouseButtonUp((int)type);
		}
		public static bool InputMouseAnyButtonHold()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonHold(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonHold(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return mouseLeftHeld;

				case 1:
					return mouseRightHeld;

				case 2:
					return mouseMiddleHeld;

				case 3:
					return mouseBackHeld;

				case 4:
					return mouseForwardHeld;
			}

			return false;
		}
		public static bool InputMouseButtonHold(MouseButton type)
		{
			return InputMouseButtonHold((int)type);
		}
		public static bool InputMouseAnyButtonDoublePress()
		{
			for (int i = 0; i < 5; i++)
				if (InputMouseButtonDoublePress(i))
					return true;

			return false;
		}
		public static bool InputMouseButtonDoublePress(int type)
		{
			if (type < 0 || type > 4)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return MouseLeftDoublePress;

				case 1:
					return MouseRightDoublePress;

				case 2:
					return MouseMiddleDoublePress;

				case 3:
					return MouseBackDoublePress;

				case 4:
					return MouseForwardDoublePress;
			}

			return false;
		}
		public static bool InputMouseButtonDoublePress(MouseButton type)
		{
			return InputMouseButtonDoublePress((int)type);
		}

		#endregion

		#region Gamepad Outputs

#if ENABLE_INPUT_SYSTEM
		public static void GamepadVibration(float lowFrequency, float highFrequency, Gamepad gamepad)
		{
			if (gamepad == null)
				return;

			gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
		}
		public static void GamepadVibration(float lowFrequency, float highFrequency, int gamepadIndex = 0)
		{
			if (gamepadIndex < 0 || gamepadIndex >= GamepadsCount)
				return;

			GamepadVibration(lowFrequency, highFrequency, Gamepads[gamepadIndex]);
		}
#endif

		#endregion

		#region Utilities

		public static void Start()
		{
			LoadData();

			if (Inputs == null || Inputs.Length < 1)
				return;

			for (int i = 0; i < Inputs.Length; i++)
				Inputs[i].Start();
		}
		public static void Update()
		{
			deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
#if ENABLE_INPUT_SYSTEM && !UNITY_2019_1_OR_NEWER
			anyKeyPressed = Keyboard.anyKey.isPressed;
#endif
			mouseLeftHeld = false;

			if (InputMouseButtonPress(MouseButton.Left))
			{
				mouseLeftHoldTimer -= deltaTime;
				mouseLeftHeld = mouseLeftHoldTimer <= 0f;

				if (mouseLeftHeld)
					mouseLeftHoldTimer = HoldWaitTime;
			}
			else if (mouseLeftHoldTimer != HoldTriggerTime)
				mouseLeftHoldTimer = HoldTriggerTime;

			mouseLeftDoublePressTimer += mouseLeftDoublePressTimer > 0f ? -deltaTime : mouseLeftDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Left);

			if (mousePressed)
				mouseLeftDoublePressTimer = DoublePressTimeout;

			MouseLeftDoublePress = mouseLeftDoublePressInitiated && mousePressed;

			if (mousePressed && mouseLeftDoublePressTimer > 0f)
				mouseLeftDoublePressInitiated = true;

			if (mouseLeftDoublePressTimer <= 0f)
				mouseLeftDoublePressInitiated = false;

			mouseMiddleHeld = false;

			if (InputMouseButtonPress(MouseButton.Middle))
			{
				mouseMiddleHoldTimer -= deltaTime;
				mouseMiddleHeld = mouseMiddleHoldTimer <= 0f;

				if (mouseMiddleHeld)
					mouseMiddleHoldTimer = HoldWaitTime;
			}
			else if (mouseMiddleHoldTimer != HoldTriggerTime)
				mouseMiddleHoldTimer = HoldTriggerTime;

			mouseMiddleDoublePressTimer += mouseMiddleDoublePressTimer > 0f ? -deltaTime : mouseMiddleDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Middle);

			if (mousePressed)
				mouseMiddleDoublePressTimer = DoublePressTimeout;

			MouseMiddleDoublePress = mouseMiddleDoublePressInitiated && mousePressed;

			if (mousePressed && mouseMiddleDoublePressTimer > 0f)
				mouseMiddleDoublePressInitiated = true;

			if (mouseMiddleDoublePressTimer <= 0f)
				mouseMiddleDoublePressInitiated = false;

			mouseRightHeld = false;

			if (InputMouseButtonPress(MouseButton.Right))
			{
				mouseRightHoldTimer -= deltaTime;
				mouseRightHeld = mouseRightHoldTimer <= 0f;

				if (mouseRightHeld)
					mouseRightHoldTimer = HoldWaitTime;
			}
			else if (mouseRightHoldTimer != HoldTriggerTime)
				mouseRightHoldTimer = HoldTriggerTime;

			mouseRightDoublePressTimer += mouseRightDoublePressTimer > 0f ? -deltaTime : mouseRightDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Right);

			if (mousePressed)
				mouseRightDoublePressTimer = DoublePressTimeout;

			MouseRightDoublePress = mouseRightDoublePressInitiated && mousePressed;

			if (mousePressed && mouseRightDoublePressTimer > 0f)
				mouseRightDoublePressInitiated = true;

			if (mouseRightDoublePressTimer <= 0f)
				mouseRightDoublePressInitiated = false;

			mouseBackHeld = false;

			if (InputMouseButtonPress(MouseButton.Back))
			{
				mouseBackHoldTimer -= deltaTime;
				mouseBackHeld = mouseBackHoldTimer <= 0f;

				if (mouseBackHeld)
					mouseBackHoldTimer = HoldWaitTime;
			}
			else if (mouseBackHoldTimer != HoldTriggerTime)
				mouseBackHoldTimer = HoldTriggerTime;

			mouseBackDoublePressTimer += mouseBackDoublePressTimer > 0f ? -deltaTime : mouseBackDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Back);

			if (mousePressed)
				mouseBackDoublePressTimer = DoublePressTimeout;

			MouseBackDoublePress = mouseBackDoublePressInitiated && mousePressed;

			if (mousePressed && mouseBackDoublePressTimer > 0f)
				mouseBackDoublePressInitiated = true;

			if (mouseBackDoublePressTimer <= 0f)
				mouseBackDoublePressInitiated = false;

			mouseForwardHeld = false;

			if (InputMouseButtonPress(MouseButton.Forward))
			{
				mouseForwardHoldTimer -= deltaTime;
				mouseForwardHeld = mouseForwardHoldTimer <= 0f;

				if (mouseForwardHeld)
					mouseForwardHoldTimer = HoldWaitTime;
			}
			else if (mouseForwardHoldTimer != HoldTriggerTime)
				mouseForwardHoldTimer = HoldTriggerTime;

			mouseForwardDoublePressTimer += mouseForwardDoublePressTimer > 0f ? -deltaTime : mouseForwardDoublePressTimer;
			mousePressed = InputMouseButtonUp(MouseButton.Forward);

			if (mousePressed)
				mouseForwardDoublePressTimer = DoublePressTimeout;

			MouseForwardDoublePress = mouseForwardDoublePressInitiated && mousePressed;

			if (mousePressed && mouseForwardDoublePressTimer > 0f)
				mouseForwardDoublePressInitiated = true;

			if (mouseForwardDoublePressTimer <= 0f)
				mouseForwardDoublePressInitiated = false;

			if (Inputs == null || Inputs.Length < 1)
				return;

#if ENABLE_INPUT_SYSTEM
			if (GamepadsCount > 0)
				if (Gamepad.current != Gamepads[0])
					Gamepads[0].MakeCurrent();
#endif

			for (int i = 0; i < Inputs.Length; i++)
			{
#if ENABLE_INPUT_SYSTEM
				if (GamepadsCount != gamepadsCount)
					Inputs[i].InitializeGamepads();
#endif

				Inputs[i].Update();
			}

#if ENABLE_INPUT_SYSTEM
			gamepadsCount = GamepadsCount;
#endif
		}
#if ENABLE_INPUT_SYSTEM
		public static Input[] KeyUsed(Key key)
		{
			if (key == Key.None)
				return new Input[] { };

			List<Input> inputs = new List<Input>();

			for (int i = 0; i < Count; i++)
				if (Inputs[i].Main.Positive == key || Inputs[i].Main.Negative == key || Inputs[i].Alt.Positive == key || Inputs[i].Alt.Negative == key)
					inputs.Add(Inputs[i]);

			return inputs.ToArray();
		}
		public static Input[] GamepadBindingUsed(GamepadBinding binding)
		{
			if (binding == GamepadBinding.None)
				return new Input[] { };

			List<Input> inputs = new List<Input>();

			for (int i = 0; i < Count; i++)
				if (Inputs[i].Main.GamepadPositive == binding || Inputs[i].Main.GamepadNegative == binding || Inputs[i].Alt.GamepadPositive == binding || Inputs[i].Alt.GamepadNegative == binding)
					inputs.Add(Inputs[i]);

			return inputs.ToArray();
		}
		public static Key KeyCodeToKey(KeyCode keyCode)
		{
			switch (keyCode)
			{
				case KeyCode.A:
					return Key.A;

				case KeyCode.Alpha0:
					return Key.Digit0;

				case KeyCode.Alpha1:
					return Key.Digit1;

				case KeyCode.Alpha2:
					return Key.Digit2;

				case KeyCode.Alpha3:
					return Key.Digit3;

				case KeyCode.Alpha4:
					return Key.Digit4;

				case KeyCode.Alpha5:
					return Key.Digit5;

				case KeyCode.Alpha6:
					return Key.Digit6;

				case KeyCode.Alpha7:
					return Key.Digit7;

				case KeyCode.Alpha8:
					return Key.Digit8;

				case KeyCode.Alpha9:
					return Key.Digit9;

				case KeyCode.AltGr:
					return Key.AltGr;

				case KeyCode.B:
					return Key.B;

				case KeyCode.BackQuote:
					return Key.Backquote;

				case KeyCode.Backslash:
					return Key.Backslash;

				case KeyCode.Backspace:
					return Key.Backspace;

				case KeyCode.C:
					return Key.C;

				case KeyCode.CapsLock:
					return Key.CapsLock;

				case KeyCode.Comma:
					return Key.Comma;

				case KeyCode.D:
					return Key.D;

				case KeyCode.Delete:
					return Key.Delete;

				case KeyCode.DownArrow:
					return Key.DownArrow;

				case KeyCode.E:
					return Key.E;

				case KeyCode.End:
					return Key.End;

				case KeyCode.Equals:
					return Key.Equals;

				case KeyCode.Escape:
					return Key.Escape;

				case KeyCode.F:
					return Key.F;

				case KeyCode.F1:
					return Key.F1;

				case KeyCode.F2:
					return Key.F2;

				case KeyCode.F3:
					return Key.F3;

				case KeyCode.F4:
					return Key.F4;

				case KeyCode.F5:
					return Key.F5;

				case KeyCode.F6:
					return Key.F6;

				case KeyCode.F7:
					return Key.F7;

				case KeyCode.F8:
					return Key.F8;

				case KeyCode.F9:
					return Key.F9;

				case KeyCode.F10:
					return Key.F10;

				case KeyCode.F11:
					return Key.F11;

				case KeyCode.F12:
					return Key.F12;

				case KeyCode.G:
					return Key.G;

				case KeyCode.H:
					return Key.H;

				case KeyCode.Home:
					return Key.Home;

				case KeyCode.I:
					return Key.I;

				case KeyCode.Insert:
					return Key.Insert;

				case KeyCode.J:
					return Key.J;

				case KeyCode.K:
					return Key.K;

				case KeyCode.Keypad0:
					return Key.Numpad0;

				case KeyCode.Keypad1:
					return Key.Numpad1;

				case KeyCode.Keypad2:
					return Key.Numpad2;

				case KeyCode.Keypad3:
					return Key.Numpad3;

				case KeyCode.Keypad4:
					return Key.Numpad4;

				case KeyCode.Keypad5:
					return Key.Numpad5;

				case KeyCode.Keypad6:
					return Key.Numpad6;

				case KeyCode.Keypad7:
					return Key.Numpad7;

				case KeyCode.Keypad8:
					return Key.Numpad8;

				case KeyCode.Keypad9:
					return Key.Numpad9;

				case KeyCode.KeypadDivide:
					return Key.NumpadDivide;

				case KeyCode.KeypadEnter:
					return Key.NumpadEnter;

				case KeyCode.KeypadEquals:
					return Key.NumpadEquals;

				case KeyCode.KeypadMinus:
					return Key.NumpadMinus;

				case KeyCode.KeypadMultiply:
					return Key.NumpadMultiply;

				case KeyCode.KeypadPeriod:
					return Key.NumpadPeriod;

				case KeyCode.KeypadPlus:
					return Key.NumpadPlus;

				case KeyCode.L:
					return Key.L;

				case KeyCode.LeftAlt:
					return Key.LeftAlt;

				case KeyCode.LeftApple:
					return Key.LeftApple;

				case KeyCode.LeftArrow:
					return Key.LeftArrow;

				case KeyCode.LeftBracket:
					return Key.LeftBracket;

				case KeyCode.LeftControl:
					return Key.LeftCtrl;

				case KeyCode.LeftShift:
					return Key.LeftShift;

				case KeyCode.LeftWindows:
					return Key.LeftWindows;

				case KeyCode.M:
					return Key.M;

				case KeyCode.Minus:
					return Key.Minus;

				case KeyCode.N:
					return Key.N;

				case KeyCode.Numlock:
					return Key.NumLock;

				case KeyCode.O:
					return Key.O;

				case KeyCode.P:
					return Key.P;

				case KeyCode.PageDown:
					return Key.PageDown;

				case KeyCode.PageUp:
					return Key.PageUp;

				case KeyCode.Pause:
					return Key.Pause;

				case KeyCode.Period:
					return Key.Period;

				case KeyCode.Q:
					return Key.Q;

				case KeyCode.Quote:
					return Key.Quote;

				case KeyCode.R:
					return Key.R;

				case KeyCode.Return:
					return Key.Enter;

				case KeyCode.RightAlt:
					return Key.RightAlt;

				case KeyCode.RightApple:
					return Key.RightApple;

				case KeyCode.RightArrow:
					return Key.RightArrow;

				case KeyCode.RightBracket:
					return Key.RightBracket;

				case KeyCode.RightControl:
					return Key.RightCtrl;

				case KeyCode.RightShift:
					return Key.RightShift;

				case KeyCode.RightWindows:
					return Key.RightWindows;

				case KeyCode.S:
					return Key.S;

				case KeyCode.ScrollLock:
					return Key.ScrollLock;

				case KeyCode.Semicolon:
					return Key.Semicolon;

				case KeyCode.Slash:
					return Key.Slash;

				case KeyCode.Space:
					return Key.Space;

				case KeyCode.T:
					return Key.T;

				case KeyCode.Tab:
					return Key.Tab;

				case KeyCode.U:
					return Key.U;

				case KeyCode.UpArrow:
					return Key.UpArrow;

				case KeyCode.V:
					return Key.V;

				case KeyCode.W:
					return Key.W;

				case KeyCode.X:
					return Key.X;

				case KeyCode.Y:
					return Key.Y;

				case KeyCode.Z:
					return Key.Z;

				case KeyCode.SysReq:
					return Key.PrintScreen;

				default:
					return Key.None;
			}
		}
#endif
		public static int IndexOf(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("The input name cannot be empty or `null`", "name");

			for (int i = 0; i < Count; i++)
				if (Inputs[i].Name == name)
					return i;

			return -1;
		}
		public static string[] GetInputsNames()
		{
			if (dataChanged || inputNames == null || inputNames.Length != Inputs.Length)
			{
				if (inputNames == null || inputNames.Length != Inputs.Length)
					inputNames = new string[Inputs.Length];

				for (int i = 0; i < Inputs.Length; i++)
				{
					if (Inputs[i].Name.IsNullOrEmpty())
						continue;
						
					inputNames[i] = Inputs[i].Name;
				}
			}

			return inputNames;
		}
		public static Input GetInput(int index)
		{
			if (index < 0 || index + 1 > Count)
				throw new ArgumentOutOfRangeException("index");

			return Inputs[index];
		}
		public static Input GetInput(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("The input name cannot be empty or `null`", "name");

			int index = Array.IndexOf(GetInputsNames(), name);

			if (index < 0)
				throw new ArgumentException($"We couldn't find an input with the name of `{name}` in the inputs list!");

			return GetInput(index);
		}
		public static void SetInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot set input in Play Mode");

				return;
			}

			if (!input)
				throw new ArgumentNullException("input");

			if (index < 0 || index + 1 > Count)
				throw new ArgumentOutOfRangeException("index");

			Inputs[index] = input;
			dataChanged = true;
		}
		public static void SetInput(string name, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot set input in Play Mode");

				return;
			}

			if (!input)
				throw new ArgumentNullException("input");

			int index = IndexOf(name);

			if (index < 0)
			{
				Debug.LogError($"<b>Inputs Manager:</b> We couldn't set the `{name}` input because it doesn't exist!");

				return;
			}

			Inputs[index] = input;
			dataChanged = true;
		}
		public static Input AddInput(Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot add input in Play Mode");

				return null;
			}

			if (!input)
				throw new ArgumentNullException("input");

			if (string.IsNullOrEmpty(input.Name) || string.IsNullOrWhiteSpace(input.Name))
				throw new ArgumentException("The input name cannot be empty or `null`", "input.name");

			if (IndexOf(input.Name) > -1)
				throw new ArgumentException($"We couldn't add the input `{input.Name}` to the list because its name matches another one", "input");

			Array.Resize(ref inputs, Inputs.Length + 1);
			Array.Resize(ref inputNames, inputNames.Length + 1);

			Inputs[Inputs.Length - 1] = input;
			dataChanged = true;

			return input;
		}
		public static Input AddInput(string name)
		{
			return AddInput(new Input(name));
		}
		public static Input DuplicateInput(Input input)
		{
			if (!input)
				throw new ArgumentNullException("input");

			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot duplicate input in Play Mode");

				return null;
			}

			Input newInput = new Input(input);
			int index = Inputs.Length;
			int newLength = index + 1;

			Array.Resize(ref inputs, newLength);
			Array.Resize(ref inputNames, newLength);

			Inputs[index] = newInput;
			dataChanged = true;

			return newInput;
		}
		public static Input DuplicateInput(string name)
		{
			return DuplicateInput(GetInput(name));
		}
		public static Input DuplicateInput(int index)
		{
			return DuplicateInput(GetInput(index));
		}
		public static void InsertInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot insert input in Play Mode");

				return;
			}

			if (index < 0 || index > Count)
				throw new ArgumentOutOfRangeException("index", input, $"We couldn't insert the `{input.Name}` because the index value is out range");

			List<Input> inputsList = Inputs.ToList();

			inputsList.Insert(index, input);

			inputs = inputsList.ToArray();
			dataChanged = true;
		}
		public static void RemoveInput(string name)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove input in Play Mode");

				return;
			}

			List<Input> inputsList = Inputs.ToList();

			inputsList.Remove(GetInput(name));

			inputs = inputsList.ToArray();
			dataChanged = true;
		}
		public static void RemoveInput(int index)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove input in Play Mode");

				return;
			}

			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException("index");

			List<Input> inputsList = Inputs.ToList();

			inputsList.RemoveAt(index);

			inputs = inputsList.ToArray();
			dataChanged = true;
		}
		public static void RemoveAll()
		{
			if (Application.isPlaying)
			{
				Debug.LogError("<b>Inputs Manager:</b> Cannot remove inputs in Play Mode");

				return;
			}

			Array.Clear(inputs, 0, inputs.Length);

			inputs = new Input[] { };
			dataChanged = true;
		}
		public static bool LoadDataFromSheet(DataSheet data)
		{
			if (!data)
			{
				Debug.LogError("<b>Inputs Manager:</b> We've had some issues while loading data!");

				return false;
			}

			inputs = data.Inputs;
			inputNames = inputs.Select(input => input.Name).ToArray();
			inputSourcePriority = data.InputSourcePriority;
			interpolationTime = data.InterpolationTime;
			holdTriggerTime = data.HoldTriggerTime;
			holdWaitTime = data.HoldWaitTime;
			doublePressTimeout = data.DoublePressTimeout;
			gamepadThreshold = data.GamepadThreshold;
			dataLastWriteTime = Application.isEditor ? File.GetLastWriteTime(Path.Combine(Application.dataPath, "Resources", $"{DataAssetPath}.bytes")) : DateTime.Now;
			dataChanged = !Application.isPlaying;
			dataLoadedOnBuild = !Application.isEditor;

			return data;
		}
		public static bool LoadData()
		{
			if (!DataAssetExists)
				return false;

			if (DataLoaded)
				return true;

			DataSerializationUtility<DataSheet> serializer = new DataSerializationUtility<DataSheet>($"{(Application.isEditor ? Path.Combine(Application.dataPath, $"Resources{Path.DirectorySeparatorChar}") : "")}{DataAssetPath}{(Application.isEditor ? ".bytes" : "")}", !Application.isEditor);
			bool dataLoaded = LoadDataFromSheet(serializer.Load());

			dataChanged = false;

			return dataLoaded;
		}
		[Obsolete("Use InputsManager.ForceDataChange() instead.", true)]
		public static void ForceLoadData() { }
		public static void ForceDataChange()
		{
			dataLastWriteTime = DateTime.MinValue;
			dataChanged = true;
		}
		public static bool SaveData()
		{
			if (Application.isPlaying || !Application.isEditor)
				return false;

			string fullPath = Path.Combine(Application.dataPath, "Resources", DataAssetPath);

			if (File.Exists(fullPath))
				File.Delete(fullPath);

			DataSerializationUtility<DataSheet> serializer = new DataSerializationUtility<DataSheet>(fullPath, true);

			inputNames = null;
			dataChanged = false;

			return serializer.SaveOrCreate(new DataSheet());
		}

#if ENABLE_INPUT_SYSTEM
		private static KeyControl KeyToKeyControl(Key key)
		{
			switch (key)
			{
				case Key.A:
					return Keyboard.aKey;

				case Key.B:
					return Keyboard.bKey;

				case Key.Backquote:
					return Keyboard.backquoteKey;

				case Key.Backslash:
					return Keyboard.backslashKey;

				case Key.Backspace:
					return Keyboard.backspaceKey;

				case Key.C:
					return Keyboard.cKey;

				case Key.Comma:
					return Keyboard.commaKey;

				case Key.ContextMenu:
					return Keyboard.contextMenuKey;

				case Key.D:
					return Keyboard.dKey;

				case Key.Delete:
					return Keyboard.deleteKey;

				case Key.Digit0:
					return Keyboard.digit0Key;

				case Key.Digit1:
					return Keyboard.digit1Key;

				case Key.Digit2:
					return Keyboard.digit2Key;

				case Key.Digit3:
					return Keyboard.digit3Key;

				case Key.Digit4:
					return Keyboard.digit4Key;

				case Key.Digit5:
					return Keyboard.digit5Key;

				case Key.Digit6:
					return Keyboard.digit6Key;

				case Key.Digit7:
					return Keyboard.digit7Key;

				case Key.Digit8:
					return Keyboard.digit8Key;

				case Key.Digit9:
					return Keyboard.digit9Key;

				case Key.DownArrow:
					return Keyboard.downArrowKey;

				case Key.E:
					return Keyboard.eKey;

				case Key.End:
					return Keyboard.endKey;

				case Key.Enter:
					return Keyboard.enterKey;

				case Key.Equals:
					return Keyboard.equalsKey;

				case Key.Escape:
					return Keyboard.escapeKey;

				case Key.F:
					return Keyboard.fKey;

				case Key.F1:
					return Keyboard.f1Key;

				case Key.F2:
					return Keyboard.f2Key;

				case Key.F3:
					return Keyboard.f3Key;

				case Key.F4:
					return Keyboard.f4Key;

				case Key.F5:
					return Keyboard.f5Key;

				case Key.F6:
					return Keyboard.f6Key;

				case Key.F7:
					return Keyboard.f7Key;

				case Key.F8:
					return Keyboard.f8Key;

				case Key.F9:
					return Keyboard.f9Key;

				case Key.F10:
					return Keyboard.f10Key;

				case Key.F11:
					return Keyboard.f11Key;

				case Key.F12:
					return Keyboard.f12Key;

				case Key.G:
					return Keyboard.gKey;

				case Key.H:
					return Keyboard.hKey;

				case Key.Home:
					return Keyboard.homeKey;

				case Key.I:
					return Keyboard.iKey;

				case Key.Insert:
					return Keyboard.insertKey;

				case Key.J:
					return Keyboard.jKey;

				case Key.K:
					return Keyboard.kKey;

				case Key.L:
					return Keyboard.lKey;

				case Key.LeftAlt:
					return Keyboard.leftAltKey;

				case Key.LeftArrow:
					return Keyboard.leftArrowKey;

				case Key.LeftBracket:
					return Keyboard.leftBracketKey;

				case Key.LeftCtrl:
					return Keyboard.leftCtrlKey;

				case Key.LeftMeta:
					return Keyboard.leftMetaKey;

				case Key.LeftShift:
					return Keyboard.leftShiftKey;

				case Key.M:
					return Keyboard.mKey;

				case Key.Minus:
					return Keyboard.minusKey;

				case Key.N:
					return Keyboard.nKey;

				case Key.NumLock:
					return Keyboard.numLockKey;

				case Key.Numpad0:
					return Keyboard.numpad0Key;

				case Key.Numpad1:
					return Keyboard.numpad1Key;

				case Key.Numpad2:
					return Keyboard.numpad2Key;

				case Key.Numpad3:
					return Keyboard.numpad3Key;

				case Key.Numpad4:
					return Keyboard.numpad4Key;

				case Key.Numpad5:
					return Keyboard.numpad5Key;

				case Key.Numpad6:
					return Keyboard.numpad6Key;

				case Key.Numpad7:
					return Keyboard.numpad7Key;

				case Key.Numpad8:
					return Keyboard.numpad8Key;

				case Key.Numpad9:
					return Keyboard.numpad9Key;

				case Key.NumpadDivide:
					return Keyboard.numpadDivideKey;

				case Key.NumpadEnter:
					return Keyboard.numpadEnterKey;

				case Key.NumpadEquals:
					return Keyboard.numpadEqualsKey;

				case Key.NumpadMinus:
					return Keyboard.numpadMinusKey;

				case Key.NumpadMultiply:
					return Keyboard.numpadMultiplyKey;

				case Key.NumpadPeriod:
					return Keyboard.numpadPeriodKey;

				case Key.NumpadPlus:
					return Keyboard.numpadPlusKey;

				case Key.O:
					return Keyboard.oKey;

				case Key.OEM1:
					return Keyboard.oem1Key;

				case Key.OEM2:
					return Keyboard.oem2Key;

				case Key.OEM3:
					return Keyboard.oem3Key;

				case Key.OEM4:
					return Keyboard.oem4Key;

				case Key.OEM5:
					return Keyboard.oem5Key;

				case Key.P:
					return Keyboard.pKey;

				case Key.PageDown:
					return Keyboard.pageDownKey;

				case Key.PageUp:
					return Keyboard.pageUpKey;

				case Key.Pause:
					return Keyboard.pauseKey;

				case Key.Period:
					return Keyboard.periodKey;

				case Key.Q:
					return Keyboard.qKey;

				case Key.Quote:
					return Keyboard.quoteKey;

				case Key.R:
					return Keyboard.rKey;

				case Key.RightAlt:
					return Keyboard.rightAltKey;

				case Key.RightArrow:
					return Keyboard.rightArrowKey;

				case Key.RightBracket:
					return Keyboard.rightBracketKey;

				case Key.RightCtrl:
					return Keyboard.rightCtrlKey;

				case Key.RightMeta:
					return Keyboard.rightMetaKey;

				case Key.RightShift:
					return Keyboard.rightShiftKey;

				case Key.S:
					return Keyboard.sKey;

				case Key.ScrollLock:
					return Keyboard.scrollLockKey;

				case Key.Semicolon:
					return Keyboard.semicolonKey;

				case Key.Slash:
					return Keyboard.slashKey;

				case Key.Space:
					return Keyboard.spaceKey;

				case Key.T:
					return Keyboard.tKey;

				case Key.Tab:
					return Keyboard.tabKey;

				case Key.U:
					return Keyboard.uKey;

				case Key.UpArrow:
					return Keyboard.upArrowKey;

				case Key.V:
					return Keyboard.vKey;

				case Key.W:
					return Keyboard.wKey;

				case Key.X:
					return Keyboard.xKey;

				case Key.Y:
					return Keyboard.yKey;

				case Key.Z:
					return Keyboard.zKey;

				default:
					return null;
			}
		}
		private static ButtonControl GamepadBindingToButtonControl(Gamepad gamepad, GamepadBinding binding)
		{
			switch (binding)
			{
				case GamepadBinding.DpadUp:
					return gamepad.dpad.up;

				case GamepadBinding.DpadRight:
					return gamepad.dpad.right;

				case GamepadBinding.DpadDown:
					return gamepad.dpad.down;

				case GamepadBinding.DpadLeft:
					return gamepad.dpad.left;

				case GamepadBinding.ButtonNorth:
					return gamepad.buttonNorth;

				case GamepadBinding.ButtonEast:
					return gamepad.buttonEast;

				case GamepadBinding.ButtonSouth:
					return gamepad.buttonSouth;

				case GamepadBinding.ButtonWest:
					return gamepad.buttonWest;

				case GamepadBinding.LeftStickButton:
					return gamepad.leftStickButton;

				case GamepadBinding.LeftStickUp:
					return gamepad.leftStick.up;

				case GamepadBinding.LeftStickRight:
					return gamepad.leftStick.right;

				case GamepadBinding.LeftStickDown:
					return gamepad.leftStick.down;

				case GamepadBinding.LeftStickLeft:
					return gamepad.leftStick.left;

				case GamepadBinding.RightStickButton:
					return gamepad.rightStickButton;

				case GamepadBinding.RightStickUp:
					return gamepad.rightStick.up;

				case GamepadBinding.RightStickRight:
					return gamepad.rightStick.right;

				case GamepadBinding.RightStickDown:
					return gamepad.rightStick.down;

				case GamepadBinding.RightStickLeft:
					return gamepad.rightStick.left;

				case GamepadBinding.LeftShoulder:
					return gamepad.leftShoulder;

				case GamepadBinding.RightShoulder:
					return gamepad.rightShoulder;

				case GamepadBinding.LeftTrigger:
					return gamepad.leftTrigger;

				case GamepadBinding.RightTrigger:
					return gamepad.rightTrigger;

				case GamepadBinding.StartButton:
					return gamepad.startButton;

				case GamepadBinding.SelectButton:
					return gamepad.selectButton;

				default:
					return null;
			}
		}
#endif

		#endregion

		#endregion
	}

	#endregion
}
