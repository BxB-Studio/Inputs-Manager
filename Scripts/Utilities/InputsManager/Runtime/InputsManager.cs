#define INPUTS_MANAGER

#region Namespaces

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#else
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Controls;
#endif

#endregion

namespace Utilities.Inputs
{
	public static class InputsManager
	{
		#region Modules & Enumerators

		#region Enumerators

		public enum InputType { Axis, Button }
		public enum InputAxisInterpolation { Instant = 2, Jump = 1, Smooth = 0 }
		public enum InputAxisStrongSide { None, Positive, Negative, FirstPressing }
		public enum InputMouseButton { Left = -1, Middle, Right }

		#endregion

		#region Modules

		[Serializable]
		public class DataSheet
		{
			#region Variables

			public Input[] Inputs => inputs;
			public float InterpolationTime => interpolationTime;
			public float HoldTriggerTime => holdTriggerTime;
			public float HoldWaitTime => holdWaitTime;
			public float DoublePressTimeout => doublePressTimeout;

			[SerializeField]
			private Input[] inputs;
			[SerializeField]
			private float interpolationTime;
			[SerializeField]
			private float holdTriggerTime;
			[SerializeField]
			private float holdWaitTime;
			[SerializeField]
			private float doublePressTimeout;
			[SerializeField]
			private bool autoSave;

			#endregion

			#region Constructors & Operators

			#region Constructors

			public DataSheet()
			{
				inputs = InputsManager.Inputs.ToArray();
				interpolationTime = InputsManager.InterpolationTime;
				holdTriggerTime = InputsManager.HoldTriggerTime;
				holdWaitTime = InputsManager.HoldWaitTime;
				doublePressTimeout = InputsManager.DoublePressTimeout;
			}

			#endregion

			#region Operators

			public static implicit operator bool(DataSheet data) => data != null;

			#endregion

			#endregion
		}
		[Serializable]
		public class InputAxis
		{
			#region Variables

			public InputAxisStrongSide StrongSide
			{
				get
				{
					return strongSide;
				}
				set
				{
					if (Application.isPlaying)
						return;

					strongSide = value;
					dataChanged = true;
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

			[SerializeField]
			private InputAxisStrongSide strongSide;
			[SerializeField]
			private Key positive;
			[SerializeField]
			private Key negative;

			#endregion

			#region Constructors & Opertators

			#region  Constructors

			public InputAxis()
			{
				strongSide = InputAxisStrongSide.None;
				positive = Key.None;
				negative = Key.None;
			}
			public InputAxis(InputAxis axis)
			{
				strongSide = axis.strongSide;
				positive = axis.positive;
				negative = axis.negative;
			}

			#endregion

			#region Operators

			public static implicit operator bool(InputAxis axis) => axis != null;

			#endregion

			#endregion
		}
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

					name = value;
					dataChanged = true;
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
					if (!Application.isPlaying)
						return;

					if (value == InputType.Button)
					{
						valueInterval.x = 0f;
						Main.Negative = Key.None;
						Alt.Negative = Key.None;
					}

					type = value;
					dataChanged = true;
				}
			}
			public InputAxisInterpolation Interpolation
			{
				get
				{
					return interpolation;
				}
				set
				{
					if (Application.isPlaying)
						return;

					interpolation = value;
					dataChanged = true;
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
					if (Application.isPlaying)
						return;

					valueInterval = value;
					dataChanged = true;
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
					dataChanged = true;
				}
			}
			public InputAxis Main
			{
				get
				{
					if (!main)
						main = new InputAxis();

					return main;
				}
				set
				{
					if (Application.isPlaying)
						return;

					if (!value)
						return;

					main = value;
					dataChanged = true;
				}
			}
			public InputAxis Alt
			{
				get
				{
					if (!alt)
						alt = new InputAxis();

					return alt;
				}
				set
				{
					if (Application.isPlaying)
						return;

					if (!value)
						return;

					main = value;
					dataChanged = true;
				}
			}

			internal float MainValue
			{
				get
				{
					if (mainValueUpdatedFrame == Time.frameCount)
						return mainValue;

					float positiveValue = Utility.BoolToInt(PositiveMainPress);
					float negativeValue = Utility.BoolToInt(NegativeMainPress);
					float positiveCoeficient = 1f;
					float negativeCoeficient = 1f;

					switch (Main.StrongSide)
					{
						case InputAxisStrongSide.FirstPressing:
							if (negativeCoeficient == 1f && positiveValue != 0f && negativeValue == 0f)
								positiveCoeficient = 2f;

							if (positiveCoeficient == 1f && negativeValue != 0f && positiveValue == 0f)
								negativeCoeficient = 2f;
								
							break;

						case InputAxisStrongSide.Positive:
							positiveCoeficient = 2f;
							break;

						case InputAxisStrongSide.Negative:
							negativeCoeficient = 2f;
							break;
					}

					positiveValue *= positiveCoeficient;
					negativeValue *= negativeCoeficient;

					float valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

					if (Type == InputType.Axis)
					{
						valueFactor += 1f;
						valueFactor *= .5f;
					}

					float intervalA = invert ? valueInterval.y : valueInterval.x;
					float intervalB = invert ? valueInterval.x : valueInterval.y;
					float target = Mathf.Lerp(intervalA, intervalB, valueFactor);

					if (interpolation == InputAxisInterpolation.Smooth)
						mainValue = Mathf.Lerp(mainValue, target, Time.time / InterpolationTime);
					else if (interpolation == InputAxisInterpolation.Jump)
						mainValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(mainValue) ? 0f : Mathf.Lerp(mainValue, target, Time.time / InterpolationTime);
					else
						mainValue = target;

					mainValueUpdatedFrame = Time.frameCount;

					return mainValue;
				}
			}
			internal float AltValue
			{
				get
				{
					if (altValueUpdatedFrame == Time.frameCount)
						return altValue;

					float positiveValue = Utility.BoolToInt(PositiveAltPress);
					float negativeValue = Utility.BoolToInt(NegativeAltPress);
					float positiveCoeficient = 1f;
					float negativeCoeficient = 1f;

					switch (Alt.StrongSide)
					{
						case InputAxisStrongSide.FirstPressing:
							if (negativeCoeficient == 1f && positiveValue != 0f && negativeValue == 0f)
								positiveCoeficient = 2f;

							if (positiveCoeficient == 1f && negativeValue != 0f && positiveValue == 0f)
								negativeCoeficient = 2f;
								
							break;

						case InputAxisStrongSide.Positive:
							positiveCoeficient = 2f;
							break;

						case InputAxisStrongSide.Negative:
							negativeCoeficient = 2f;
							break;
					}

					positiveValue *= positiveCoeficient;
					negativeValue *= negativeCoeficient;

					float valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

					if (Type == InputType.Axis)
					{
						valueFactor += 1f;
						valueFactor *= .5f;
					}

					float intervalA = invert ? valueInterval.y : valueInterval.x;
					float intervalB = invert ? valueInterval.x : valueInterval.y;
					float target = Mathf.Lerp(intervalA, intervalB, valueFactor);

					if (interpolation == InputAxisInterpolation.Smooth)
						altValue = Mathf.Lerp(altValue, target, Time.time / InterpolationTime);
					else if (interpolation == InputAxisInterpolation.Jump)
						altValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(altValue) ? 0f : Mathf.Lerp(altValue, target, Time.time / InterpolationTime);
					else
						altValue = target;

					altValueUpdatedFrame = Time.frameCount;

					return mainValue;
				}
			}
			internal float Value
			{
				get
				{
					float mainValue = MainValue;

					if (mainValue != 0f)
						return mainValue;

					return AltValue;
				}
			}
			internal bool PositiveMainBindable
			{
				get
				{
					if (!trimmed)
						Trim();
					
					return Main.Positive != Key.None;
				}
			}
			internal bool NegativeMainBindable
			{
				get
				{
					if (!trimmed)
						Trim();

					return Main.Negative != Key.None;
				}
			}
			internal bool PositiveAltBindable
			{
				get
				{
					if (!trimmed)
						Trim();

					return Alt.Positive != Key.None;
				}
			}
			internal bool NegativeAltBindable
			{
				get
				{
					if (!trimmed)
						Trim();

					return Alt.Negative != Key.None;
				}
			}
			internal bool MainBindable => PositiveMainBindable || NegativeMainBindable;
			internal bool AltBindable => PositiveAltBindable || NegativeAltBindable;
			internal bool PositiveBindable => PositiveMainBindable || PositiveAltBindable;
			internal bool NegativeBindable => NegativeMainBindable || NegativeAltBindable;
			internal bool Bindable => MainBindable || AltBindable;
			internal bool PositiveMainPress => PositiveMainBindable && PositiveMainControl.isPressed;
			internal bool NegativeMainPress => NegativeMainBindable && NegativeMainControl.isPressed;
			internal bool PositiveAltPress => PositiveAltBindable && PositiveAltControl.isPressed;
			internal bool NegativeAltPress => NegativeAltBindable && NegativeAltControl.isPressed;
			internal bool MainPress => PositiveMainPress || NegativeMainPress;
			internal bool AltPress => PositiveAltPress || NegativeAltPress;
			internal bool PositivePress => PositiveMainPress || PositiveAltPress;
			internal bool NegativePress => NegativeMainPress || NegativeAltPress;
			internal bool Press => MainPress || AltPress;
			internal bool PositiveMainDown => PositiveMainBindable && PositiveMainControl.wasPressedThisFrame;
			internal bool NegativeMainDown => NegativeMainBindable && NegativeMainControl.wasPressedThisFrame;
			internal bool PositiveAltDown => PositiveAltBindable && PositiveAltControl.wasPressedThisFrame;
			internal bool NegativeAltDown => NegativeAltBindable && NegativeAltControl.wasPressedThisFrame;
			internal bool MainDown => PositiveMainDown || NegativeMainDown;
			internal bool AltDown => PositiveAltDown || NegativeAltDown;
			internal bool PositiveDown => PositiveMainDown || PositiveAltDown;
			internal bool NegativeDown => NegativeMainDown || NegativeAltDown;
			internal bool Down => MainDown || AltDown;
			internal bool PositiveMainUp => PositiveMainBindable && PositiveMainControl.wasReleasedThisFrame;
			internal bool NegativeMainUp => NegativeMainBindable && NegativeMainControl.wasReleasedThisFrame;
			internal bool PositiveAltUp => PositiveAltBindable && PositiveAltControl.wasReleasedThisFrame;
			internal bool NegativeAltUp => NegativeAltBindable && NegativeAltControl.wasReleasedThisFrame;
			internal bool MainUp => PositiveMainUp || NegativeMainUp;
			internal bool AltUp => PositiveAltUp || NegativeAltUp;
			internal bool PositiveUp => PositiveMainUp || PositiveAltUp;
			internal bool NegativeUp => NegativeMainUp || NegativeAltUp;
			internal bool Up => MainUp || AltUp;
			internal bool PositiveMainHeld
			{
				get
				{
					if (positiveMainHeldUpdateFrame == Time.frameCount)
						return positiveMainHeld;
						
					positiveMainHeldUpdateFrame = Time.frameCount;

					if (PositiveMainPress)
					{
						positiveMainHoldTimer -= Time.deltaTime;

						bool held = positiveMainHoldTimer <= 0f;

						if (held)
							positiveMainHoldTimer = HoldWaitTime;

						positiveMainHeld = held;

						return held;
					}

					if (positiveMainHoldTimer != HoldTriggerTime)
						positiveMainHoldTimer = HoldTriggerTime;

					positiveMainHeld = false;

					return false;
				}
			}
			internal bool NegativeMainHeld
			{
				get
				{
					if (negativeMainHeldUpdateFrame == Time.frameCount)
						return negativeMainHeld;
						
					negativeMainHeldUpdateFrame = Time.frameCount;

					if (NegativeMainPress)
					{
						negativeMainHoldTimer -= Time.deltaTime;

						bool held = negativeMainHoldTimer <= 0f;

						if (held)
							negativeMainHoldTimer = HoldWaitTime;

						negativeMainHeld = held;

						return held;
					}
					
					if (negativeMainHoldTimer != HoldTriggerTime)
						negativeMainHoldTimer = HoldTriggerTime;

					negativeMainHeld = false;

					return false;
				}
			}
			internal bool PositiveAltHeld
			{
				get
				{
					if (positiveAltHeldUpdateFrame == Time.frameCount)
						return positiveAltHeld;
						
					positiveAltHeldUpdateFrame = Time.frameCount;

					if (PositiveAltPress)
					{
						positiveAltHoldTimer -= Time.deltaTime;

						bool held = positiveAltHoldTimer <= 0f;

						if (held)
							positiveAltHoldTimer = HoldWaitTime;

						positiveAltHeld = held;

						return held;
					}
					
					if (positiveAltHoldTimer != HoldTriggerTime)
						positiveAltHoldTimer = HoldTriggerTime;

					positiveAltHeld = false;

					return false;
				}
			}
			internal bool NegativeAltHeld
			{
				get
				{
					if (negativeAltHeldUpdateFrame == Time.frameCount)
						return negativeAltHeld;

					negativeAltHeldUpdateFrame = Time.frameCount;

					if (NegativeAltPress)
					{
						negativeAltHoldTimer -= Time.deltaTime;

						bool held = negativeAltHoldTimer <= 0f;

						if (held)
							negativeAltHoldTimer = HoldWaitTime;

						negativeAltHeld = held;

						return held;
					}
					
					if (negativeAltHoldTimer != HoldTriggerTime)
						negativeAltHoldTimer = HoldTriggerTime;

					negativeAltHeld = false;

					return false;
				}
			}
			internal bool MainHeld => PositiveMainHeld || NegativeMainHeld;
			internal bool AltHeld => PositiveAltHeld || NegativeAltHeld;
			internal bool PositiveHeld => PositiveMainHeld || PositiveAltHeld;
			internal bool NegativeHeld => NegativeMainHeld || NegativeAltHeld;
			internal bool Held => MainHeld || AltHeld;

			[SerializeField]
			private string name;
			[SerializeField]
			private InputType type;
			[SerializeField]
			private InputAxis main;
			[SerializeField]
			private InputAxis alt;
			[SerializeField]
			private InputAxisInterpolation interpolation;
			[SerializeField]
			private Utility.SerializableVector2 valueInterval;
			[SerializeField]
			private bool invert;

			private KeyControl PositiveMainControl
			{
				get
				{
					if (PositiveMainBindable && positiveMain == null)
						positiveMain = KeyToKeyControl(Main.Positive);

					return positiveMain;
				}
			}	
			private KeyControl NegativeMainControl
			{
				get
				{
					if (NegativeMainBindable && negativeMain == null)
						negativeMain = KeyToKeyControl(Main.Negative);

					return negativeMain;
				}
			}
			private KeyControl PositiveAltControl
			{
				get
				{
					if (PositiveAltBindable && positiveAlt == null)
						positiveAlt = KeyToKeyControl(Alt.Positive);

					return positiveAlt;
				}
			}
			private KeyControl NegativeAltControl
			{
				get
				{
					if (NegativeAltBindable && negativeAlt == null)
						negativeAlt = KeyToKeyControl(Alt.Negative);

					return negativeAlt;
				}
			}
			[NonSerialized]
			private KeyControl positiveMain;
			[NonSerialized]
			private KeyControl negativeMain;
			[NonSerialized]
			private KeyControl positiveAlt;
			[NonSerialized]
			private KeyControl negativeAlt;
			[NonSerialized]
			private float mainValue;
			[NonSerialized]
			private float altValue;
			[NonSerialized]
			private float positiveMainHoldTimer;
			[NonSerialized]
			private float negativeMainHoldTimer;
			[NonSerialized]
			private float positiveAltHoldTimer;
			[NonSerialized]
			private float negativeAltHoldTimer;
			[NonSerialized]
			private bool positiveMainHeld;
			[NonSerialized]
			private bool negativeMainHeld;
			[NonSerialized]
			private bool positiveAltHeld;
			[NonSerialized]
			private bool negativeAltHeld;
			[NonSerialized]
			private int mainValueUpdatedFrame;
			[NonSerialized]
			private int altValueUpdatedFrame;
			[NonSerialized]
			private int positiveMainHeldUpdateFrame;
			[NonSerialized]
			private int negativeMainHeldUpdateFrame;
			[NonSerialized]
			private int positiveAltHeldUpdateFrame;
			[NonSerialized]
			private int negativeAltHeldUpdateFrame;
			[NonSerialized]
			private bool trimmed;

			#endregion

			#region Methods

			private void Trim()
			{
				if (Main.Positive == Key.None && Alt.Positive != Key.None)
				{
					Main.Positive = Alt.Positive;
					Alt.Positive = Key.None;
				}

				if (Main.Negative == Key.None && Alt.Negative != Key.None)
				{
					Main.Negative = Alt.Negative;
					Alt.Negative = Key.None;
				}

				if (Type == InputType.Button)
				{
					if (valueInterval.x != 0f)
						valueInterval.x = 0f;

					if (Main.Negative != Key.None)
						Main.Negative = Key.None;
				}

				if (Application.isPlaying)
					trimmed = true;
			}

			#endregion

			#region Constructors & Operators

			#region Constructors

			public Input(string name)
			{
				this.name = name;
				valueInterval = new Vector2(-1f, 1f);
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

		public static readonly string DataAssetPath = "Resources/Assets/InputsManager_Data.imdata";
		public static float InterpolationTime
		{
			get
			{
				return interpolationTime;
			}
			set
			{
				if (Application.isPlaying)
					return;

				interpolationTime = value;
				dataChanged = true;
			}
		}
		public static float HoldTriggerTime
		{
			get
			{
				return holdTriggerTime;
			}
			set
			{
				if (Application.isPlaying)
					return;

				holdTriggerTime = value;
				dataChanged = true;
			}
		}
		public static float HoldWaitTime
		{
			get
			{
				return holdWaitTime;
			}
			set
			{
				if (Application.isPlaying)
					return;

				holdWaitTime = value;
				dataChanged = true;
			}
		}
		public static float DoublePressTimeout
		{
			get
			{
				return doublePressTimeout;
			}
			set
			{
				if (Application.isPlaying)
					return;

				doublePressTimeout = value;
				dataChanged = true;
			}
		}
		public static bool DataLoaded => DataAssetExists && File.GetLastWriteTime($"{Application.dataPath}/{DataAssetPath}") == dataLastWriteTime;
		public static bool DataChanged => DataLoaded && dataChanged;
		public static bool DataAssetExists => File.Exists($"{Application.dataPath}/{DataAssetPath}");
		public static int Count => Inputs.Count;

		private static DateTime dataLastWriteTime;
		private static List<Input> inputs = new List<Input>();
		private static List<Input> Inputs
		{
			get
			{
				LoadData();

				if (inputs == null)
					inputs = new List<Input>();

				return inputs;
			}
		}
		private static Keyboard Keyboard => Keyboard.current;
		private static Mouse Mouse => Mouse.current;
		private static float interpolationTime = .5f;
		private static float holdTriggerTime = .3f;
		private static float holdWaitTime = .1f;
		private static float doublePressTimeout = .2f;
		private static float mouseLeftHoldTimer;
		private static float mouseMiddleHoldTimer;
		private static float mouseRightHoldTimer;
		private static float mouseLeftDoubleTimer;
		private static float mouseMiddleDoubleTimer;
		private static float mouseRightDoubleTimer;
		private static bool MouseLeftHeld
		{
			get
			{
				if (mouseLeftHeldUpdateFrame == Time.frameCount)
					return mouseLeftHeld;

				mouseLeftHeldUpdateFrame = Time.frameCount;

				if (InputMouseButtonPress(InputMouseButton.Left))
				{
					mouseLeftHoldTimer -= Time.deltaTime;

					bool held = mouseLeftHoldTimer <= 0f;

					if (held)
						mouseLeftHoldTimer = HoldWaitTime;

					mouseLeftHeld = held;

					return held;
				}

				if (mouseLeftHoldTimer != HoldTriggerTime)
					mouseLeftHoldTimer = HoldTriggerTime;
				
				mouseLeftHeld = false;

				return false;
			}
		}
		private static bool MouseMiddleHeld
		{
			get
			{
				if (mouseMiddleHeldUpdateFrame == Time.frameCount)
					return mouseMiddleHeld;

				mouseMiddleHeldUpdateFrame = Time.frameCount;

				if (InputMouseButtonPress(InputMouseButton.Middle))
				{
					mouseMiddleHoldTimer -= Time.deltaTime;

					bool held = mouseMiddleHoldTimer <= 0f;

					if (held)
						mouseMiddleHoldTimer = HoldWaitTime;

					mouseMiddleHeld = held;

					return held;
				}

				if (mouseMiddleHoldTimer != HoldTriggerTime)
					mouseMiddleHoldTimer = HoldTriggerTime;

				mouseMiddleHeld = false;

				return false;
			}
		}
		private static bool MouseRightHeld
		{
			get
			{
				if (mouseRightHeldUpdateFrame == Time.frameCount)
					return mouseRightHeld;

				mouseRightHeldUpdateFrame = Time.frameCount;

				if (InputMouseButtonPress(InputMouseButton.Right))
				{
					mouseRightHoldTimer -= Time.deltaTime;

					bool held = mouseRightHoldTimer <= 0f;

					if (held)
						mouseRightHoldTimer = HoldWaitTime;

					mouseRightHeld = held;

					return held;
				}

				if (mouseRightHoldTimer != HoldTriggerTime)
					mouseRightHoldTimer = HoldTriggerTime;
				
				mouseRightHeld = false;

				return false;
			}
		}
		private static bool mouseLeftHeld;
		private static bool mouseMiddleHeld;
		private static bool mouseRightHeld;
		private static bool MouseLeftDouble
		{
			get
			{
				if (mouseLeftDoubleUpdateFrame == Time.frameCount)
					return mouseLeftDouble;

				mouseLeftDoubleUpdateFrame = Time.frameCount;
				mouseLeftDoubleTimer += mouseLeftDoubleTimer > 0f ? -Time.deltaTime : mouseLeftDoubleTimer;

				bool doubled = InputMouseButtonUp(InputMouseButton.Left) && mouseLeftDoubleTimer > 0f;

				if (doubled)
					mouseLeftDoubleTimer = DoublePressTimeout;

				mouseLeftDouble = doubled;

				return doubled;
			}
		}
		private static bool MouseMiddleDouble
		{
			get
			{
				if (mouseMiddleDoubleUpdateFrame == Time.frameCount)
					return mouseMiddleDouble;

				mouseMiddleDoubleUpdateFrame = Time.frameCount;
				mouseMiddleDoubleTimer += mouseMiddleDoubleTimer > 0f ? -Time.deltaTime : mouseMiddleDoubleTimer;

				bool doubled = InputMouseButtonUp(InputMouseButton.Middle) && mouseMiddleDoubleTimer > 0f;

				if (doubled)
					mouseMiddleDoubleTimer = DoublePressTimeout;

				mouseMiddleDouble = doubled;

				return doubled;
			}
		}
		private static bool MouseRightDouble
		{
			get
			{
				if (mouseRightDoubleUpdateFrame == Time.frameCount)
					return mouseRightDouble;

				mouseRightDoubleUpdateFrame = Time.frameCount;
				mouseRightDoubleTimer += mouseRightDoubleTimer > 0f ? -Time.deltaTime : mouseRightDoubleTimer;

				bool doubled = InputMouseButtonUp(InputMouseButton.Right) && mouseRightDoubleTimer > 0f;

				if (doubled)
					mouseRightDoubleTimer = DoublePressTimeout;

				mouseRightDouble = doubled;

				return doubled;
			}
		}
		private static bool mouseLeftDouble;
		private static bool mouseMiddleDouble;
		private static bool mouseRightDouble;
		private static bool dataChanged;
		private static int mouseLeftHeldUpdateFrame;
		private static int mouseMiddleHeldUpdateFrame;
		private static int mouseRightHeldUpdateFrame;
		private static int mouseLeftDoubleUpdateFrame;
		private static int mouseMiddleDoubleUpdateFrame;
		private static int mouseRightDoubleUpdateFrame;

		#endregion

		#region Methods

		#region Inputs

		public static Vector2 InputMouseMovement() => Mouse.delta.ReadValue();
		public static Vector2 InputMousePosition() => Mouse.position.ReadValue();
		public static Vector2 InputMouseScrollWheelVector() => Mouse.scroll.ReadValue();
		public static float InputValue(Input input) => input.Value;
		public static float InputValue(string name) => GetInput(name).Value;
		public static float InputValue(int index) => GetInput(index).Value;
		public static float InputMainAxisValue(Input input) => input.MainValue;
		public static float InputMainAxisValue(string name) => GetInput(name).MainValue;
		public static float InputMainAxisValue(int index) => GetInput(index).MainValue;
		public static float InputAltAxisValue(Input input) => input.AltValue;
		public static float InputAltAxisValue(string name) => GetInput(name).AltValue;
		public static float InputAltAxisValue(int index) => GetInput(index).AltValue;
		public static float InputMouseScrollWheel() => Mouse.scroll.ReadValue().magnitude;
		public static float InputMouseScrollWheelHorizontal() => Mouse.scroll.ReadValue().x;
		public static float InputMouseScrollWheelVertical() => Mouse.scroll.ReadValue().y;
		public static bool InputPress(Input input) => input.Press;
		public static bool InputPress(string name) => GetInput(name).Press;
		public static bool InputPress(int index) => GetInput(index).Press;
		public static bool InputMainAxisPress(Input input) => input.MainPress;
		public static bool InputMainAxisPress(string name) => GetInput(name).MainPress;
		public static bool InputMainAxisPress(int index) => GetInput(index).MainPress;
		public static bool InputAltAxisPress(Input input) => input.AltPress;
		public static bool InputAltAxisPress(string name) => GetInput(name).AltPress;
		public static bool InputAltAxisPress(int index) => GetInput(index).AltPress;
		public static bool InputPositivePress(Input input) => input.PositivePress;
		public static bool InputPositivePress(string name) => GetInput(name).PositivePress;
		public static bool InputPositivePress(int index) => GetInput(index).PositivePress;
		public static bool InputNegativePress(Input input) => input.NegativePress;
		public static bool InputNegativePress(string name) => GetInput(name).NegativePress;
		public static bool InputNegativePress(int index) => GetInput(index).NegativePress;
		public static bool InputPositiveMainAxisPress(Input input) => input.PositiveMainPress;
		public static bool InputPositiveMainAxisPress(string name) => GetInput(name).PositiveMainPress;
		public static bool InputPositiveMainAxisPress(int index) => GetInput(index).PositiveMainPress;
		public static bool InputNegativeMainAxisPress(Input input) => input.NegativeMainPress;
		public static bool InputNegativeMainAxisPress(string name) => GetInput(name).NegativeMainPress;
		public static bool InputNegativeMainAxisPress(int index) => GetInput(index).NegativeMainPress;
		public static bool InputDown(Input input) => input.Down;
		public static bool InputDown(string name) => GetInput(name).Down;
		public static bool InputDown(int index) => GetInput(index).Down;
		public static bool InputMainAxisDown(Input input) => input.MainDown;
		public static bool InputMainAxisDown(string name) => GetInput(name).MainDown;
		public static bool InputMainAxisDown(int index) => GetInput(index).MainDown;
		public static bool InputAltAxisDown(Input input) => input.AltDown;
		public static bool InputAltAxisDown(string name) => GetInput(name).AltDown;
		public static bool InputAltAxisDown(int index) => GetInput(index).AltDown;
		public static bool InputPositiveDown(Input input) => input.PositiveDown;
		public static bool InputPositiveDown(string name) => GetInput(name).PositiveDown;
		public static bool InputPositiveDown(int index) => GetInput(index).PositiveDown;
		public static bool InputNegativeDown(Input input) => input.NegativeDown;
		public static bool InputNegativeDown(string name) => GetInput(name).NegativeDown;
		public static bool InputNegativeDown(int index) => GetInput(index).NegativeDown;
		public static bool InputPositiveMainAxisDown(Input input) => input.PositiveMainDown;
		public static bool InputPositiveMainAxisDown(string name) => GetInput(name).PositiveMainDown;
		public static bool InputPositiveMainAxisDown(int index) => GetInput(index).PositiveMainDown;
		public static bool InputNegativeMainAxisDown(Input input) => input.NegativeMainDown;
		public static bool InputNegativeMainAxisDown(string name) => GetInput(name).NegativeMainDown;
		public static bool InputNegativeMainAxisDown(int index) => GetInput(index).NegativeMainDown;
		public static bool InputUp(Input input) => input.Up;
		public static bool InputUp(string name) => GetInput(name).Up;
		public static bool InputUp(int index) => GetInput(index).Up;
		public static bool InputMainAxisUp(Input input) => input.MainUp;
		public static bool InputMainAxisUp(string name) => GetInput(name).MainUp;
		public static bool InputMainAxisUp(int index) => GetInput(index).MainUp;
		public static bool InputAltAxisUp(Input input) => input.AltUp;
		public static bool InputAltAxisUp(string name) => GetInput(name).AltUp;
		public static bool InputAltAxisUp(int index) => GetInput(index).AltUp;
		public static bool InputPositiveUp(Input input) => input.PositiveUp;
		public static bool InputPositiveUp(string name) => GetInput(name).PositiveUp;
		public static bool InputPositiveUp(int index) => GetInput(index).PositiveUp;
		public static bool InputNegativeUp(Input input) => input.NegativeUp;
		public static bool InputNegativeUp(string name) => GetInput(name).NegativeUp;
		public static bool InputNegativeUp(int index) => GetInput(index).NegativeUp;
		public static bool InputPositiveMainAxisUp(Input input) => input.PositiveMainUp;
		public static bool InputPositiveMainAxisUp(string name) => GetInput(name).PositiveMainUp;
		public static bool InputPositiveMainAxisUp(int index) => GetInput(index).PositiveMainUp;
		public static bool InputNegativeMainAxisUp(Input input) => input.NegativeMainUp;
		public static bool InputNegativeMainAxisUp(string name) => GetInput(name).NegativeMainUp;
		public static bool InputNegativeMainAxisUp(int index) => GetInput(index).NegativeMainUp;
		public static bool InputHold(Input input) => input.Held;
		public static bool InputHold(string name) => GetInput(name).Held;
		public static bool InputHold(int index) => GetInput(index).Held;
		public static bool InputMainAxisHold(Input input) => input.MainHeld;
		public static bool InputMainAxisHold(string name) => GetInput(name).MainHeld;
		public static bool InputMainAxisHold(int index) => GetInput(index).MainHeld;
		public static bool InputAltAxisHold(Input input) => input.AltHeld;
		public static bool InputAltAxisHold(string name) => GetInput(name).AltHeld;
		public static bool InputAltAxisHold(int index) => GetInput(index).AltHeld;
		public static bool InputPositiveHold(Input input) => input.PositiveHeld;
		public static bool InputPositiveHold(string name) => GetInput(name).PositiveHeld;
		public static bool InputPositiveHold(int index) => GetInput(index).PositiveHeld;
		public static bool InputNegativeHold(Input input) => input.NegativeHeld;
		public static bool InputNegativeHold(string name) => GetInput(name).NegativeHeld;
		public static bool InputNegativeHold(int index) => GetInput(index).NegativeHeld;
		public static bool InputPositiveMainAxisHold(Input input) => input.PositiveMainHeld;
		public static bool InputPositiveMainAxisHold(string name) => GetInput(name).PositiveMainHeld;
		public static bool InputPositiveMainAxisHold(int index) => GetInput(index).PositiveMainHeld;
		public static bool InputNegativeMainAxisHold(Input input) => input.NegativeMainHeld;
		public static bool InputNegativeMainAxisHold(string name) => GetInput(name).NegativeMainHeld;
		public static bool InputNegativeMainAxisHold(int index) => GetInput(index).NegativeMainHeld;
		public static bool InputKey(Key key) => KeyToKeyControl(key).isPressed;
		public static bool InputKeyDown(Key key) => KeyToKeyControl(key).wasPressedThisFrame;
		public static bool InputKeyUp(Key key) => KeyToKeyControl(key).wasReleasedThisFrame;
		public static bool InputMouseButtonPress(int type)
		{
			if (type < -1 || type > 1)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case -1:
					return Mouse.leftButton.isPressed;

				case 0:
					return Mouse.middleButton.isPressed;

				case 1:
					return Mouse.rightButton.isPressed;
			}
			
			return false;
		}
		public static bool InputMouseButtonPress(InputMouseButton type) => InputMouseButtonPress((int)type);
		public static bool InputMouseButtonDown(int type)
		{
			if (type < -1 || type > 1)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case -1:
					return Mouse.leftButton.wasPressedThisFrame;

				case 0:
					return Mouse.middleButton.wasPressedThisFrame;

				case 1:
					return Mouse.rightButton.wasPressedThisFrame;
			}
			
			return false;
		}
		public static bool InputMouseButtonDown(InputMouseButton type) => InputMouseButtonDown((int)type);
		public static bool InputMouseButtonUp(int type)
		{
			if (type < -1 || type > 1)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case -1:
					return Mouse.leftButton.wasReleasedThisFrame;

				case 0:
					return Mouse.middleButton.wasReleasedThisFrame;

				case 1:
					return Mouse.rightButton.wasReleasedThisFrame;
			}

			return false;
		}
		public static bool InputMouseButtonUp(InputMouseButton type) => InputMouseButtonUp((int)type);
		public static bool InputMouseButtonHold(int type)
		{
			if (type < -1 || type > 1)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case -1:
					return MouseLeftHeld;

				case 0:
					return MouseMiddleHeld;

				case 1:
					return MouseRightHeld;
			}

			return false;
		}
		public static bool InputMouseButtonHold(InputMouseButton type) => InputMouseButtonHold((int)type);
		public static bool InputMouseButtonDouble(int type)
		{
			if (type < -1 || type > 1)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case -1:
					return MouseLeftDouble;

				case 0:
					return MouseMiddleDouble;

				case 1:
					return MouseRightDouble;
			}

			return false;
		}
		public static bool InputMouseButtonDouble(InputMouseButton type) => InputMouseButtonDouble((int)type);

		#endregion

		#region Utilities

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
			string[] array = new string[Count];

			for (int i = 0; i < Count; i++)
				array[i] = Inputs[i].Name;

			return array;
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

			Input input = Inputs.Find(query => query.Name == name);

			if (!input)
				throw new ArgumentException($"We couldn't find an input with the name of `{name}` in the inputs list!");

			return input;
		}
		public static void SetInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot set input in Play Mode");

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
				Debug.LogError($"Inputs Manager: Cannot set input in Play Mode");

				return;
			}

			if (!input)
				throw new ArgumentNullException("input");

			int index = IndexOf(name);

			if (index == -1)
			{
				Debug.LogError($"Inputs Manager: We couldn't set the `{name}` input because it doesn't exist!");

				return;
			}

			Inputs[index] = input;
			dataChanged = true;
		}
		public static Input AddInput(Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot add input in Play Mode");

				return null;
			}

			if (string.IsNullOrEmpty(input.Name))
				throw new ArgumentException("The input name cannot be empty or `null`", "input.name");

			if (IndexOf(input.Name) != -1)
				throw new ArgumentException($"We couldn't add the input `{input.Name}` to the list because its name matches another one", "input");

			Inputs.Add(input);

			dataChanged = true;

			return input;
		}
		public static Input AddInput(string name) => AddInput(new Input(name));
		public static Input DuplicateInput(string name)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot duplicate input in Play Mode");

				return null;
			}

			Input oldInput = GetInput(name);
			Input input = new Input(oldInput);

			Inputs.Add(input);

			dataChanged = true;

			return input;
		}
		public static Input DuplicateInput(int index) => DuplicateInput(GetInput(index).Name);
		public static void InsertInput(int index, Input input)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot insert input in Play Mode");

				return;
			}

			if (index < 0 || index > Count)
				throw new ArgumentOutOfRangeException("index", input, $"We couldn't insert the `{input.Name}` because the index value is out range");

			Inputs.Insert(index, input);

			dataChanged = true;
		}
		public static void RemoveInput(string name)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot remove input in Play Mode");

				return;
			}

			Inputs.Remove(GetInput(name));

			dataChanged = true;
		}
		public static void RemoveInput(int index)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot remove input in Play Mode");

				return;
			}

			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException("index");

			Inputs.RemoveAt(index);

			dataChanged = true;
		}
		public static void RemoveAll()
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot remove inputs in Play Mode");

				return;
			}

			Inputs.Clear();

			dataChanged = true;
		}
		public static bool LoadData()
		{
			if (!DataAssetExists)
				return false;

			if (DataLoaded)
				return true;

			string directory = $"{Application.dataPath}/{Path.GetDirectoryName(DataAssetPath)}";
			string file = Path.GetFileName(DataAssetPath);
			DataSerializationUtility<DataSheet> serializer = new DataSerializationUtility<DataSheet>(directory, file);
			DataSheet data = serializer.Load();

			inputs = data.Inputs.ToList();
			InterpolationTime = data.InterpolationTime;
			HoldTriggerTime = data.HoldTriggerTime;
			HoldWaitTime = data.HoldWaitTime;
			DoublePressTimeout = data.DoublePressTimeout;
			dataLastWriteTime = File.GetLastWriteTime($"{Application.dataPath}/{DataAssetPath}");
			dataChanged = false;

			return data;
		}
		public static void UnloadData()
		{
			dataLastWriteTime = DateTime.MinValue;
			dataChanged = false;
		}
		public static bool SaveData()
		{
			string fullPath = $"{Application.dataPath}/{DataAssetPath}";

			if (File.Exists(fullPath))
				File.Delete(fullPath);

			string directory = Path.GetDirectoryName(fullPath);
			string file = Path.GetFileName(DataAssetPath);
			DataSerializationUtility<DataSheet> serializer = new DataSerializationUtility<DataSheet>(directory, file);

			dataChanged = false;

			return serializer.SaveOrCreate(new DataSheet());
		}

		#endregion

		#endregion
	}
}
