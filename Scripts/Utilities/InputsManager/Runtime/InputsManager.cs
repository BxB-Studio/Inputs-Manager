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
		public enum InputMouseButton { Left, Right, Middle }

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
				inputs = InputsManager.Inputs;
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
					if (Application.isPlaying)
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

			internal float MainValue { get; private set; }
			internal float AltValue { get; private set; }
			internal float Value { get; private set; }
			internal bool PositiveMainBindable { get; private set; }
			internal bool NegativeMainBindable { get; private set; }
			internal bool PositiveAltBindable { get; private set; }
			internal bool NegativeAltBindable { get; private set; }
			internal bool MainBindable { get; private set; }
			internal bool AltBindable { get; private set; }
			internal bool PositiveBindable { get; private set; }
			internal bool NegativeBindable { get; private set; }
			internal bool Bindable { get; private set; }
			internal bool PositiveMainPress { get; private set; }
			internal bool NegativeMainPress { get; private set; }
			internal bool PositiveAltPress { get; private set; }
			internal bool NegativeAltPress { get; private set; }
			internal bool MainPress { get; private set; }
			internal bool AltPress { get; private set; }
			internal bool PositivePress { get; private set; }
			internal bool NegativePress { get; private set; }
			internal bool Press { get; private set; }
			internal bool PositiveMainDown { get; private set; }
			internal bool NegativeMainDown { get; private set; }
			internal bool PositiveAltDown { get; private set; }
			internal bool NegativeAltDown { get; private set; }
			internal bool MainDown { get; private set; }
			internal bool AltDown { get; private set; }
			internal bool PositiveDown { get; private set; }
			internal bool NegativeDown { get; private set; }
			internal bool Down { get; private set; }
			internal bool PositiveMainUp { get; private set; }
			internal bool NegativeMainUp { get; private set; }
			internal bool PositiveAltUp { get; private set; }
			internal bool NegativeAltUp { get; private set; }
			internal bool MainUp { get; private set; }
			internal bool AltUp { get; private set; }
			internal bool PositiveUp { get; private set; }
			internal bool NegativeUp { get; private set; }
			internal bool Up { get; private set; }
			internal bool PositiveMainHeld { get; private set; }
			internal bool NegativeMainHeld { get; private set; }
			internal bool PositiveAltHeld { get; private set; }
			internal bool NegativeAltHeld { get; private set; }
			internal bool MainHeld { get; private set; }
			internal bool AltHeld { get; private set; }
			internal bool PositiveHeld { get; private set; }
			internal bool NegativeHeld { get; private set; }
			internal bool Held { get; private set; }
			internal bool PositiveMainDoublePress { get; private set; }
			internal bool NegativeMainDoublePress { get; private set; }
			internal bool PositiveAltDoublePress { get; private set; }
			internal bool NegativeAltDoublePress { get; private set; }
			internal bool MainDoublePress { get; private set; }
			internal bool AltDoublePress { get; private set; }
			internal bool PositiveDoublePress { get; private set; }
			internal bool NegativeDoublePress { get; private set; }
			internal bool DoublePress { get; private set; }

			[SerializeField]
			private string name;
			[SerializeField]
			private InputType type;
			[SerializeField]
			private InputAxis main = new InputAxis();
			[SerializeField]
			private InputAxis alt = new InputAxis();
			[SerializeField]
			private InputAxisInterpolation interpolation;
			[SerializeField]
			private Utility.SerializableVector2 valueInterval;
			[SerializeField]
			private bool invert;
			[NonSerialized]
			private KeyControl positiveMainControl;
			[NonSerialized]
			private KeyControl negativeMainControl;
			[NonSerialized]
			private KeyControl positiveAltControl;
			[NonSerialized]
			private KeyControl negativeAltControl;
			[NonSerialized]
			private float positiveMainHoldTimer;
			[NonSerialized]
			private float negativeMainHoldTimer;
			[NonSerialized]
			private float positiveAltHoldTimer;
			[NonSerialized]
			private float negativeAltHoldTimer;
			[NonSerialized]
			private float positiveMainDoublePressTimer;
			[NonSerialized]
			private float negativeMainDoublePressTimer;
			[NonSerialized]
			private float positiveAltDoublePressTimer;
			[NonSerialized]
			private float negativeAltDoublePressTimer;
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
			private bool positiveMainDoublePressInitiated;
			[NonSerialized]
			private bool negativeMainDoublePressInitiated;
			[NonSerialized]
			private bool positiveAltDoublePressInitiated;
			[NonSerialized]
			private bool negativeAltDoublePressInitiated;
			[NonSerialized]
			private bool trimmed;

			#endregion

			#region Methods

			internal void Start()
			{
				if (!trimmed)
					Trim();

				PositiveMainBindable = Main.Positive != Key.None;
				NegativeMainBindable = Main.Negative != Key.None;
				PositiveAltBindable = Alt.Positive != Key.None;
				NegativeAltBindable = Alt.Negative != Key.None;
				MainBindable = PositiveMainBindable || NegativeMainBindable;
				AltBindable = PositiveAltBindable || NegativeAltBindable;
				PositiveBindable = PositiveMainBindable || PositiveAltBindable;
				NegativeBindable = NegativeMainBindable || NegativeAltBindable;
				Bindable = MainBindable || AltBindable;

				if (positiveMainControl == null && PositiveMainBindable)
					positiveMainControl = KeyToKeyControl(Main.Positive);

				if (negativeMainControl == null && NegativeMainBindable)
					negativeMainControl = KeyToKeyControl(Main.Negative);

				if (positiveAltControl == null && PositiveAltBindable)
					positiveAltControl = KeyToKeyControl(Alt.Positive);

				if (negativeAltControl == null && NegativeAltBindable)
					negativeAltControl = KeyToKeyControl(Alt.Negative);
			}
			internal void Update()
			{
				positiveValue = Utility.BoolToNumber(PositiveMainPress);
				negativeValue = Utility.BoolToNumber(NegativeMainPress);
				positiveCoeficient = 1f;
				negativeCoeficient = 1f;

				switch (Main.StrongSide)
				{
					case InputAxisStrongSide.FirstPressing:
						if (MainValue > 0f)
							positiveCoeficient = 2f;
						else if (MainValue < 0f)
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
				valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

				if (Type == InputType.Axis)
				{
					valueFactor += 1f;
					valueFactor *= .5f;
				}

				intervalA = invert ? valueInterval.y : valueInterval.x;
				intervalB = invert ? valueInterval.x : valueInterval.y;
				target = Mathf.Lerp(intervalA, intervalB, valueFactor);

				if (interpolation == InputAxisInterpolation.Smooth && InterpolationTime > 0f)
					MainValue = Mathf.MoveTowards(MainValue, target, deltaTime / InterpolationTime);
				else if (interpolation == InputAxisInterpolation.Jump && InterpolationTime > 0f)
					MainValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(MainValue) ? 0f : Mathf.MoveTowards(MainValue, target, deltaTime / InterpolationTime);
				else
					MainValue = target;
				
				positiveValue = Utility.BoolToNumber(PositiveAltPress);
				negativeValue = Utility.BoolToNumber(NegativeAltPress);
				positiveCoeficient = 1f;
				negativeCoeficient = 1f;

				switch (Alt.StrongSide)
				{
					case InputAxisStrongSide.FirstPressing:
						if (AltValue > 0f)
							positiveCoeficient = 2f;
						else if (AltValue < 0f)
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
				valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

				if (Type == InputType.Axis)
				{
					valueFactor += 1f;
					valueFactor *= .5f;
				}

				target = Mathf.Lerp(intervalA, intervalB, valueFactor);

				if (interpolation == InputAxisInterpolation.Smooth && InterpolationTime > 0f)
					AltValue = Mathf.MoveTowards(AltValue, target, deltaTime / InterpolationTime);
				else if (interpolation == InputAxisInterpolation.Jump && InterpolationTime > 0f)
					AltValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(AltValue) ? 0f : Mathf.MoveTowards(AltValue, target, deltaTime / InterpolationTime);
				else
					AltValue = target;
				
				if (MainValue != 0f)
					Value = MainValue;
				else
					Value = AltValue;

				if (Bindable)
				{
					if (MainBindable)
					{
						PositiveMainPress = PositiveMainBindable && positiveMainControl.isPressed;
						NegativeMainPress = NegativeMainBindable && negativeMainControl.isPressed;
						MainPress = PositiveMainPress || NegativeMainPress;
						PositiveMainDown = PositiveMainBindable && positiveMainControl.wasPressedThisFrame;
						NegativeMainDown = NegativeMainBindable && negativeMainControl.wasPressedThisFrame;
						MainDown = PositiveMainDown || NegativeMainDown;
						PositiveMainUp = PositiveMainBindable && positiveMainControl.wasReleasedThisFrame;
						NegativeMainUp = NegativeMainBindable && negativeMainControl.wasReleasedThisFrame;
						MainUp = PositiveMainUp || NegativeMainUp;
						PositiveMainHeld = false;

						if (PositiveMainPress)
						{
							positiveMainHoldTimer -= deltaTime;
							PositiveMainHeld = positiveMainHoldTimer <= 0f;

							if (PositiveMainHeld)
								positiveMainHoldTimer = HoldWaitTime;
						}
						else if (positiveMainHoldTimer != HoldTriggerTime)
							positiveMainHoldTimer = HoldTriggerTime;

						NegativeMainHeld = false;

						if (NegativeMainPress)
						{
							negativeMainHoldTimer -= deltaTime;
							NegativeMainHeld = negativeMainHoldTimer <= 0f;

							if (NegativeMainHeld)
								negativeMainHoldTimer = HoldWaitTime;
						}
						else if (negativeMainHoldTimer != HoldTriggerTime)
							negativeMainHoldTimer = HoldTriggerTime;

						MainHeld = PositiveMainHeld || NegativeMainHeld;
						positiveMainDoublePressTimer += positiveMainDoublePressTimer > 0f ? -deltaTime : positiveMainDoublePressTimer;

						if (PositiveMainUp)
							positiveMainDoublePressTimer = DoublePressTimeout;

						PositiveMainDoublePress = positiveMainDoublePressInitiated && PositiveMainUp;

						if (PositiveMainUp && positiveMainDoublePressTimer > 0f)
							positiveMainDoublePressInitiated = true;

						if (positiveMainDoublePressTimer <= 0f)
							positiveMainDoublePressInitiated = false;

						negativeMainDoublePressTimer += negativeMainDoublePressTimer > 0f ? -deltaTime : negativeMainDoublePressTimer;

						if (NegativeMainUp)
							negativeMainDoublePressTimer = DoublePressTimeout;

						NegativeMainDoublePress = negativeMainDoublePressInitiated && NegativeMainUp;

						if (NegativeMainUp && negativeMainDoublePressTimer > 0f)
							negativeMainDoublePressInitiated = true;

						if (negativeMainDoublePressTimer <= 0f)
							negativeMainDoublePressInitiated = false;

						MainDoublePress = PositiveMainDoublePress || NegativeMainDoublePress;
					}

					if (AltBindable)
					{
						PositiveAltPress = PositiveAltBindable && positiveAltControl.isPressed;
						NegativeAltPress = NegativeAltBindable && negativeAltControl.isPressed;
						AltPress = PositiveAltPress || NegativeAltPress;
						PositiveAltDown = PositiveAltBindable && positiveAltControl.wasPressedThisFrame;
						NegativeAltDown = NegativeAltBindable && negativeAltControl.wasPressedThisFrame;
						AltDown = PositiveAltDown || NegativeAltDown;
						PositiveAltUp = PositiveAltBindable && positiveAltControl.wasReleasedThisFrame;
						NegativeAltUp = NegativeAltBindable && negativeAltControl.wasReleasedThisFrame;
						AltUp = PositiveAltUp || NegativeAltUp;
						PositiveAltHeld = false;

						if (PositiveAltPress)
						{
							positiveAltHoldTimer -= deltaTime;
							PositiveAltHeld = positiveAltHoldTimer <= 0f;

							if (PositiveAltHeld)
								positiveAltHoldTimer = HoldWaitTime;
						}
						else if (positiveAltHoldTimer != HoldTriggerTime)
							positiveAltHoldTimer = HoldTriggerTime;

						NegativeAltHeld = false;

						if (NegativeAltPress)
						{
							negativeAltHoldTimer -= deltaTime;
							NegativeAltHeld = negativeAltHoldTimer <= 0f;

							if (NegativeAltHeld)
								negativeAltHoldTimer = HoldWaitTime;
						}
						else if (negativeAltHoldTimer != HoldTriggerTime)
							negativeAltHoldTimer = HoldTriggerTime;

						AltHeld = PositiveAltHeld || NegativeAltHeld;
						positiveAltDoublePressTimer += positiveAltDoublePressTimer > 0f ? -deltaTime : positiveAltDoublePressTimer;

						if (PositiveAltUp)
							positiveAltDoublePressTimer = DoublePressTimeout;

						PositiveAltDoublePress = positiveAltDoublePressInitiated && PositiveAltUp;

						if (PositiveAltUp && positiveAltDoublePressTimer > 0f)
							positiveAltDoublePressInitiated = true;

						if (positiveAltDoublePressTimer <= 0f)
							positiveAltDoublePressInitiated = false;

						negativeAltDoublePressTimer += negativeAltDoublePressTimer > 0f ? -deltaTime : negativeAltDoublePressTimer;

						if (NegativeAltUp)
							negativeAltDoublePressTimer = DoublePressTimeout;

						NegativeAltDoublePress = negativeAltDoublePressInitiated && NegativeAltUp;

						if (NegativeAltUp && negativeAltDoublePressTimer > 0f)
							negativeAltDoublePressInitiated = true;

						if (negativeAltDoublePressTimer <= 0f)
							negativeAltDoublePressInitiated = false;

						AltDoublePress = PositiveAltDoublePress || NegativeAltDoublePress;
					}

					if (PositiveBindable)
					{
						PositivePress = PositiveMainPress || PositiveAltPress;
						PositiveDown = PositiveMainDown || PositiveAltDown;
						PositiveUp = PositiveMainUp || PositiveAltUp;
						PositiveHeld = PositiveMainHeld || PositiveAltHeld;
						PositiveDoublePress = PositiveMainDoublePress || PositiveAltDoublePress;
					}

					if (NegativeBindable)
					{
						NegativePress = NegativeMainPress || NegativeAltPress;
						NegativeDown = NegativeMainDown || NegativeAltDown;
						NegativeUp = NegativeMainUp || NegativeAltUp;
						NegativeHeld = NegativeMainHeld || NegativeAltHeld;
						NegativeDoublePress = NegativeMainDoublePress || NegativeAltDoublePress;
					}

					Press = MainPress || AltPress;
					Down = MainDown || AltDown;
					Up = MainUp || AltUp;
					Held = MainHeld || AltHeld;
					DoublePress = MainDoublePress || AltDoublePress;
				}
			}

			private void Trim()
			{
				if (!Main)
					main = new InputAxis();

				if (!Alt)
					alt = new InputAxis();

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

		public static readonly string DataAssetPath = "Resources/Assets/InputsManager_Data";
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
				if (Application.isPlaying || interpolationTime == value)
					return;

				interpolationTime = value;

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
				if (Application.isPlaying || holdTriggerTime == value)
					return;

				holdTriggerTime = value;

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
				if (Application.isPlaying || holdWaitTime == value)
					return;

				holdWaitTime = value;

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
				if (Application.isPlaying || doublePressTimeout == value)
					return;

				doublePressTimeout = value;

				SaveData();
			}
		}
		public static bool DataLoaded
		{
			get
			{
				return DataAssetExists && File.GetLastWriteTime($"{Application.dataPath}/{DataAssetPath}") == dataLastWriteTime;
			}
		}
		public static bool DataChanged
		{
			get
			{
				return DataLoaded && dataChanged;
			}
		}
		public static bool DataAssetExists
		{
			get
			{
				return File.Exists($"{Application.dataPath}/{DataAssetPath}");
			}
		}
		public static int Count
		{
			get
			{
				return Inputs.Length;
			}
		}

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
		private static Keyboard Keyboard
		{
			get
			{
				return Keyboard.current;
			}
		}
		private static Mouse Mouse
		{
			get
			{
				return Mouse.current;
			}
		}
		private static DateTime dataLastWriteTime;
		private static Input[] inputs;
		private static float interpolationTime = .25f;
		private static float holdTriggerTime = .3f;
		private static float holdWaitTime = .1f;
		private static float doublePressTimeout = .2f;
		private static float mouseLeftHoldTimer;
		private static float mouseMiddleHoldTimer;
		private static float mouseRightHoldTimer;
		private static float mouseLeftDoublePressTimer;
		private static float mouseMiddleDoublePressTimer;
		private static float mouseRightDoublePressTimer;
		private static float deltaTime;
		private static bool mouseLeftHeld;
		private static bool mouseMiddleHeld;
		private static bool mouseRightHeld;
		private static bool MouseLeftDoublePress;
		private static bool MouseMiddleDoublePress;
		private static bool MouseRightDoublePress;
		private static bool mouseLeftDoublePressInitiated;
		private static bool mouseMiddleDoublePressInitiated;
		private static bool mouseRightDoublePressInitiated;
		private static bool mousePressed;
		private static bool dataChanged;

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
		public static bool InputPositiveAltAxisPress(Input input) => input.PositiveAltPress;
		public static bool InputPositiveAltAxisPress(string name) => GetInput(name).PositiveAltPress;
		public static bool InputPositiveAltAxisPress(int index) => GetInput(index).PositiveAltPress;
		public static bool InputNegativeAltAxisPress(Input input) => input.NegativeAltPress;
		public static bool InputNegativeAltAxisPress(string name) => GetInput(name).NegativeAltPress;
		public static bool InputNegativeAltAxisPress(int index) => GetInput(index).NegativeAltPress;
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
		public static bool InputPositiveAltAxisDown(Input input) => input.PositiveAltDown;
		public static bool InputPositiveAltAxisDown(string name) => GetInput(name).PositiveAltDown;
		public static bool InputPositiveAltAxisDown(int index) => GetInput(index).PositiveAltDown;
		public static bool InputNegativeAltAxisDown(Input input) => input.NegativeAltDown;
		public static bool InputNegativeAltAxisDown(string name) => GetInput(name).NegativeAltDown;
		public static bool InputNegativeAltAxisDown(int index) => GetInput(index).NegativeAltDown;
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
		public static bool InputPositiveAltAxisUp(Input input) => input.PositiveAltUp;
		public static bool InputPositiveAltAxisUp(string name) => GetInput(name).PositiveAltUp;
		public static bool InputPositiveAltAxisUp(int index) => GetInput(index).PositiveAltUp;
		public static bool InputNegativeAltAxisUp(Input input) => input.NegativeAltUp;
		public static bool InputNegativeAltAxisUp(string name) => GetInput(name).NegativeAltUp;
		public static bool InputNegativeAltAxisUp(int index) => GetInput(index).NegativeAltUp;
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
		public static bool InputPositiveAltAxisHold(Input input) => input.PositiveAltHeld;
		public static bool InputPositiveAltAxisHold(string name) => GetInput(name).PositiveAltHeld;
		public static bool InputPositiveAltAxisHold(int index) => GetInput(index).PositiveAltHeld;
		public static bool InputNegativeAltAxisHold(Input input) => input.NegativeAltHeld;
		public static bool InputNegativeAltAxisHold(string name) => GetInput(name).NegativeAltHeld;
		public static bool InputNegativeAltAxisHold(int index) => GetInput(index).NegativeAltHeld;
		public static bool InputDoublePress(Input input) => input.DoublePress;
		public static bool InputDoublePress(string name) => GetInput(name).DoublePress;
		public static bool InputDoublePress(int index) => GetInput(index).DoublePress;
		public static bool InputMainAxisDoublePress(Input input) => input.MainDoublePress;
		public static bool InputMainAxisDoublePress(string name) => GetInput(name).MainDoublePress;
		public static bool InputMainAxisDoublePress(int index) => GetInput(index).MainDoublePress;
		public static bool InputAltAxisDoublePress(Input input) => input.AltDoublePress;
		public static bool InputAltAxisDoublePress(string name) => GetInput(name).AltDoublePress;
		public static bool InputAltAxisDoublePress(int index) => GetInput(index).AltDoublePress;
		public static bool InputPositiveDoublePress(Input input) => input.PositiveDoublePress;
		public static bool InputPositiveDoublePress(string name) => GetInput(name).PositiveDoublePress;
		public static bool InputPositiveDoublePress(int index) => GetInput(index).PositiveDoublePress;
		public static bool InputNegativeDoublePress(Input input) => input.NegativeDoublePress;
		public static bool InputNegativeDoublePress(string name) => GetInput(name).NegativeDoublePress;
		public static bool InputNegativeDoublePress(int index) => GetInput(index).NegativeDoublePress;
		public static bool InputPositiveMainAxisDoublePress(Input input) => input.PositiveMainDoublePress;
		public static bool InputPositiveMainAxisDoublePress(string name) => GetInput(name).PositiveMainDoublePress;
		public static bool InputPositiveMainAxisDoublePress(int index) => GetInput(index).PositiveMainDoublePress;
		public static bool InputNegativeMainAxisDoublePress(Input input) => input.NegativeMainDoublePress;
		public static bool InputNegativeMainAxisDoublePress(string name) => GetInput(name).NegativeMainDoublePress;
		public static bool InputNegativeMainAxisDoublePress(int index) => GetInput(index).NegativeMainDoublePress;
		public static bool InputPositiveAltAxisDoublePress(Input input) => input.PositiveAltDoublePress;
		public static bool InputPositiveAltAxisDoublePress(string name) => GetInput(name).PositiveAltDoublePress;
		public static bool InputPositiveAltAxisDoublePress(int index) => GetInput(index).PositiveAltDoublePress;
		public static bool InputNegativeAltAxisDoublePress(Input input) => input.NegativeAltDoublePress;
		public static bool InputNegativeAltAxisDoublePress(string name) => GetInput(name).NegativeAltDoublePress;
		public static bool InputNegativeAltAxisDoublePress(int index) => GetInput(index).NegativeAltDoublePress;
		public static bool InputKeyPress(Key key) => KeyToKeyControl(key).isPressed;
		public static bool InputKeyDown(Key key) => KeyToKeyControl(key).wasPressedThisFrame;
		public static bool InputKeyUp(Key key) => KeyToKeyControl(key).wasReleasedThisFrame;
		public static bool InputMouseButtonPress(int type)
		{
			if (type < 0 || type > 2)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return Mouse.leftButton.isPressed;

				case 1:
					return Mouse.rightButton.isPressed;

				case 2:
					return Mouse.middleButton.isPressed;
			}
			
			return false;
		}
		public static bool InputMouseButtonPress(InputMouseButton type)
		{
			switch (type)
			{
				case InputMouseButton.Left:
					return InputMouseButtonPress(0);

				case InputMouseButton.Right:
					return InputMouseButtonPress(1);

				case InputMouseButton.Middle:
					return InputMouseButtonPress(2);

				default:
					return false;
			}
		}
		public static bool InputMouseButtonDown(int type)
		{
			if (type < 0 || type > 2)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return Mouse.leftButton.wasPressedThisFrame;

				case 1:
					return Mouse.rightButton.wasPressedThisFrame;

				case 2:
					return Mouse.middleButton.wasPressedThisFrame;
			}
			
			return false;
		}
		public static bool InputMouseButtonDown(InputMouseButton type)
		{
			switch (type)
			{
				case InputMouseButton.Left:
					return InputMouseButtonPress(0);

				case InputMouseButton.Right:
					return InputMouseButtonPress(1);

				case InputMouseButton.Middle:
					return InputMouseButtonPress(2);

				default:
					return false;
			}
		}
		public static bool InputMouseButtonUp(int type)
		{
			if (type < 0 || type > 2)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return Mouse.leftButton.wasReleasedThisFrame;

				case 1:
					return Mouse.rightButton.wasReleasedThisFrame;

				case 2:
					return Mouse.middleButton.wasReleasedThisFrame;
			}

			return false;
		}
		public static bool InputMouseButtonUp(InputMouseButton type)
		{
			switch (type)
			{
				case InputMouseButton.Left:
					return InputMouseButtonUp(0);

				case InputMouseButton.Right:
					return InputMouseButtonUp(1);

				case InputMouseButton.Middle:
					return InputMouseButtonUp(2);

				default:
					return false;
			}
		}
		public static bool InputMouseButtonHold(int type)
		{
			if (type < 0 || type > 2)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return mouseLeftHeld;

				case 1:
					return mouseRightHeld;

				case 2:
					return mouseMiddleHeld;
			}

			return false;
		}
		public static bool InputMouseButtonHold(InputMouseButton type)
		{
			switch (type)
			{
				case InputMouseButton.Left:
					return InputMouseButtonHold(0);

				case InputMouseButton.Right:
					return InputMouseButtonHold(1);

				case InputMouseButton.Middle:
					return InputMouseButtonHold(2);

				default:
					return false;
			}
		}
		public static bool InputMouseButtonDoublePress(int type)
		{
			if (type < 0 || type > 2)
				throw new ArgumentException("The `type` argument has to be within the `InputMouseButton` enum range", "type");

			switch (type)
			{
				case 0:
					return MouseLeftDoublePress;

				case 1:
					return MouseRightDoublePress;

				case 2:
					return MouseMiddleDoublePress;
			}

			return false;
		}
		public static bool InputMouseButtonDoublePress(InputMouseButton type)
		{
			switch (type)
			{
				case InputMouseButton.Left:
					return InputMouseButtonDoublePress(0);

				case InputMouseButton.Right:
					return InputMouseButtonDoublePress(1);

				case InputMouseButton.Middle:
					return InputMouseButtonDoublePress(2);

				default:
					return false;
			}
		}

		#endregion

		#region Utilities

		public static void Start()
		{
			LoadData();

			if (Inputs == null || Inputs.Length == 0)
				return;

			for (int i = 0; i < Inputs.Length; i++)
				Inputs[i].Start();
		}
		public static void Update()
		{
			deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
			mouseLeftHeld = false;

			if (InputMouseButtonPress(InputMouseButton.Left))
			{
				mouseLeftHoldTimer -= deltaTime;
				mouseLeftHeld = mouseLeftHoldTimer <= 0f;

				if (mouseLeftHeld)
					mouseLeftHoldTimer = HoldWaitTime;
			}
			else if (mouseLeftHoldTimer != HoldTriggerTime)
				mouseLeftHoldTimer = HoldTriggerTime;

			mouseLeftDoublePressTimer += mouseLeftDoublePressTimer > 0f ? -deltaTime : mouseLeftDoublePressTimer;
			mousePressed = InputMouseButtonUp(InputMouseButton.Left);

			if (mousePressed)
				mouseLeftDoublePressTimer = DoublePressTimeout;

			MouseLeftDoublePress = mouseLeftDoublePressInitiated && mousePressed;

			if (mousePressed && mouseLeftDoublePressTimer > 0f)
				mouseLeftDoublePressInitiated = true;

			if (mouseLeftDoublePressTimer <= 0f)
				mouseLeftDoublePressInitiated = false;

			mouseMiddleHeld = false;

			if (InputMouseButtonPress(InputMouseButton.Middle))
			{
				mouseMiddleHoldTimer -= deltaTime;
				mouseMiddleHeld = mouseMiddleHoldTimer <= 0f;

				if (mouseMiddleHeld)
					mouseMiddleHoldTimer = HoldWaitTime;
			}
			else if (mouseMiddleHoldTimer != HoldTriggerTime)
				mouseMiddleHoldTimer = HoldTriggerTime;

			mouseMiddleDoublePressTimer += mouseMiddleDoublePressTimer > 0f ? -deltaTime : mouseMiddleDoublePressTimer;
			mousePressed = InputMouseButtonUp(InputMouseButton.Middle);

			if (mousePressed)
				mouseMiddleDoublePressTimer = DoublePressTimeout;

			MouseMiddleDoublePress = mouseMiddleDoublePressInitiated && mousePressed;

			if (mousePressed && mouseMiddleDoublePressTimer > 0f)
				mouseMiddleDoublePressInitiated = true;

			if (mouseMiddleDoublePressTimer <= 0f)
				mouseMiddleDoublePressInitiated = false;

			mouseRightHeld = false;

			if (InputMouseButtonPress(InputMouseButton.Right))
			{
				mouseRightHoldTimer -= deltaTime;
				mouseRightHeld = mouseRightHoldTimer <= 0f;

				if (mouseRightHeld)
					mouseRightHoldTimer = HoldWaitTime;
			}
			else if (mouseRightHoldTimer != HoldTriggerTime)
				mouseRightHoldTimer = HoldTriggerTime;

			mouseRightDoublePressTimer += mouseRightDoublePressTimer > 0f ? -deltaTime : mouseRightDoublePressTimer;
			mousePressed = InputMouseButtonUp(InputMouseButton.Right);

			if (mousePressed)
				mouseRightDoublePressTimer = DoublePressTimeout;

			MouseRightDoublePress = mouseRightDoublePressInitiated && mousePressed;

			if (mousePressed && mouseRightDoublePressTimer > 0f)
				mouseRightDoublePressInitiated = true;

			if (mouseRightDoublePressTimer <= 0f)
				mouseRightDoublePressInitiated = false;

			if (Inputs == null || Inputs.Length == 0)
				return;

			for (int i = 0; i < Inputs.Length; i++)
				Inputs[i].Update();
		}
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
			if (dataChanged || inputNames == null || inputNames.Length != Inputs.Length)
				inputNames = Inputs.Select(input => input.Name).ToArray();

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

			Input input = Array.Find(Inputs, query => query.Name == name);

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

			Array.Resize(ref inputs, Inputs.Length + 1);
			Array.Resize(ref inputNames, inputNames.Length + 1);

			Inputs[Inputs.Length - 1] = input;
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

			Array.Resize(ref inputs, Inputs.Length + 1);
			Array.Resize(ref inputNames, inputNames.Length + 1);

			Inputs[Inputs.Length - 1] = input;
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

			List<Input> inputsList = Inputs.ToList();

			inputsList.Insert(index, input);

			inputs = inputsList.ToArray();
			dataChanged = true;
		}
		public static void RemoveInput(string name)
		{
			if (Application.isPlaying)
			{
				Debug.LogError($"Inputs Manager: Cannot remove input in Play Mode");

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
				Debug.LogError($"Inputs Manager: Cannot remove input in Play Mode");

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
				Debug.LogError($"Inputs Manager: Cannot remove inputs in Play Mode");

				return;
			}

			Array.Clear(inputs, 0, inputs.Length);

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

			inputs = data.Inputs;
			inputNames = inputs.Select(input => input.Name).ToArray();
			interpolationTime = data.InterpolationTime;
			holdTriggerTime = data.HoldTriggerTime;
			holdWaitTime = data.HoldWaitTime;
			doublePressTimeout = data.DoublePressTimeout;
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
			if (Application.isPlaying)
				return false;

			string fullPath = $"{Application.dataPath}/{DataAssetPath}";

			if (File.Exists(fullPath))
				File.Delete(fullPath);

			string directory = Path.GetDirectoryName(fullPath);
			string file = Path.GetFileName(DataAssetPath);
			DataSerializationUtility<DataSheet> serializer = new DataSerializationUtility<DataSheet>(directory, file);

			dataChanged = false;

			return serializer.SaveOrCreate(new DataSheet());
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

		#endregion

		#endregion
	}
}
