#region Namespaces

using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Utilities.Inputs.Components;

#endregion

namespace Utilities.Inputs.Jobs
{
	[BurstCompile]
	internal struct InputKeyboardJob : IJobFor
	{
		#region Variables

		public NativeArray<InputSourceAccess> sourceAccess;
		[ReadOnly]
		public NativeArray<InputAccess> access;
		[ReadOnly]
		public float interpolationTime;
		[ReadOnly]
		public float holdTriggerTime;
		[ReadOnly]
		public float holdWaitTime;
		[ReadOnly]
		public float doublePressTimeout;
		[ReadOnly]
		public float deltaTime;

		private float positiveValue;
		private float negativeValue;
		private float valueFactor;
		private float positiveCoefficient;
		private float negativeCoefficient;
		private float intervalA;
		private float intervalB;
		private float target;

		#endregion

		#region Methods

		public void Execute(int i)
		{
			InputSourceAccess inputSource = sourceAccess[i];
			InputAccess input = access[i];

			// Init
			intervalA = input.invert ? input.valueInterval.y : input.valueInterval.x;
			intervalB = input.invert ? input.valueInterval.x : input.valueInterval.y;

			// Main
			positiveValue = inputSource.positiveMainValue = Utility.BoolToNumber(inputSource.positiveMainPress);
			negativeValue = inputSource.negativeMainValue = Utility.BoolToNumber(inputSource.negativeMainPress);
			positiveCoefficient = 1f;
			negativeCoefficient = 1f;

			switch (input.main.strongSide)
			{
				case InputAxisStrongSide.FirstPressing:
					if (inputSource.mainValue > 0f)
						positiveCoefficient = 2f;
					else if (inputSource.mainValue < 0f)
						negativeCoefficient = 2f;

					break;

				case InputAxisStrongSide.Positive:
					positiveCoefficient = 2f;

					break;

				case InputAxisStrongSide.Negative:
					negativeCoefficient = 2f;

					break;
			}

			positiveValue *= positiveCoefficient;
			negativeValue *= negativeCoefficient;
			valueFactor = math.clamp(positiveValue - negativeValue, -1f, 1f);

			if (input.type == InputType.Axis)
			{
				valueFactor += 1f;
				valueFactor *= .5f;
			}

			target = math.lerp(intervalA, intervalB, valueFactor);

			if (input.interpolation == InputInterpolation.Smooth && interpolationTime > 0f)
				inputSource.mainValue = Mathf.MoveTowards(inputSource.mainValue, target, deltaTime / interpolationTime);
			else if (input.interpolation == InputInterpolation.Jump && interpolationTime > 0f)
				inputSource.mainValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(inputSource.mainValue) ? 0f : Mathf.MoveTowards(inputSource.mainValue, target, deltaTime / interpolationTime);
			else
				inputSource.mainValue = target;

			// Alt
			positiveValue = inputSource.positiveAltValue = Utility.BoolToNumber(inputSource.positiveAltPress);
			negativeValue = inputSource.negativeAltValue = Utility.BoolToNumber(inputSource.negativeAltPress);
			positiveCoefficient = 1f;
			negativeCoefficient = 1f;

			switch (input.alt.strongSide)
			{
				case InputAxisStrongSide.FirstPressing:
					if (inputSource.altValue > 0f)
						positiveCoefficient = 2f;
					else if (inputSource.altValue < 0f)
						negativeCoefficient = 2f;

					break;

				case InputAxisStrongSide.Positive:
					positiveCoefficient = 2f;

					break;

				case InputAxisStrongSide.Negative:
					negativeCoefficient = 2f;

					break;
			}

			positiveValue *= positiveCoefficient;
			negativeValue *= negativeCoefficient;
			valueFactor = Mathf.Clamp(positiveValue - negativeValue, -1f, 1f);

			if (input.type == InputType.Axis)
			{
				valueFactor += 1f;
				valueFactor *= .5f;
			}

			target = Mathf.Lerp(intervalA, intervalB, valueFactor);

			if (input.interpolation == InputInterpolation.Smooth && interpolationTime > 0f)
				inputSource.altValue = Mathf.MoveTowards(inputSource.altValue, target, deltaTime / interpolationTime);
			else if (input.interpolation == InputInterpolation.Jump && interpolationTime > 0f)
				inputSource.altValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(inputSource.altValue) ? 0f : Mathf.MoveTowards(inputSource.altValue, target, deltaTime / interpolationTime);
			else
				inputSource.altValue = target;

			// Values
			if (inputSource.mainValue != 0f)
				inputSource.value = inputSource.mainValue;
			else
				inputSource.value = inputSource.altValue;

			if (inputSource.bindable)
			{
				if (inputSource.mainBindable)
				{
					// Press, Down, and Up
					inputSource.mainPress = inputSource.positiveMainPress || inputSource.negativeMainPress;
					inputSource.mainDown = inputSource.positiveMainDown || inputSource.negativeMainDown;
					inputSource.mainUp = inputSource.positiveMainUp || inputSource.negativeMainUp;

					// Hold
					inputSource.positiveMainHeld = false;

					if (inputSource.positiveMainPress)
					{
						inputSource.positiveMainHoldTimer -= deltaTime;
						inputSource.positiveMainHeld = inputSource.positiveMainHoldTimer <= 0f;

						if (inputSource.positiveMainHeld)
							inputSource.positiveMainHoldTimer = holdWaitTime;
					}
					else if (inputSource.positiveMainHoldTimer != holdTriggerTime)
						inputSource.positiveMainHoldTimer = holdTriggerTime;

					inputSource.negativeMainHeld = false;

					if (inputSource.negativeMainPress)
					{
						inputSource.negativeMainHoldTimer -= deltaTime;
						inputSource.negativeMainHeld = inputSource.negativeMainHoldTimer <= 0f;

						if (inputSource.negativeMainHeld)
							inputSource.negativeMainHoldTimer = holdWaitTime;
					}
					else if (inputSource.negativeMainHoldTimer != holdTriggerTime)
						inputSource.negativeMainHoldTimer = holdTriggerTime;

					inputSource.mainHeld = inputSource.positiveMainHeld || inputSource.negativeMainHeld;

					// Double Press
					inputSource.positiveMainDoublePressTimer += inputSource.positiveMainDoublePressTimer > 0f ? -deltaTime : inputSource.positiveMainDoublePressTimer;

					if (inputSource.positiveMainUp)
						inputSource.positiveMainDoublePressTimer = doublePressTimeout;

					inputSource.positiveMainDoublePress = inputSource.positiveMainDoublePressInitiated && inputSource.positiveMainUp;

					if (inputSource.positiveMainUp && inputSource.positiveMainDoublePressTimer > 0f)
						inputSource.positiveMainDoublePressInitiated = true;

					if (inputSource.positiveMainDoublePressTimer <= 0f)
						inputSource.positiveMainDoublePressInitiated = false;

					inputSource.negativeMainDoublePressTimer += inputSource.negativeMainDoublePressTimer > 0f ? -deltaTime : inputSource.negativeMainDoublePressTimer;

					if (inputSource.negativeMainUp)
						inputSource.negativeMainDoublePressTimer = doublePressTimeout;

					inputSource.negativeMainDoublePress = inputSource.negativeMainDoublePressInitiated && inputSource.negativeMainUp;

					if (inputSource.negativeMainUp && inputSource.negativeMainDoublePressTimer > 0f)
						inputSource.negativeMainDoublePressInitiated = true;

					if (inputSource.negativeMainDoublePressTimer <= 0f)
						inputSource.negativeMainDoublePressInitiated = false;

					inputSource.mainDoublePress = inputSource.positiveMainDoublePress || inputSource.negativeMainDoublePress;
				}

				if (inputSource.altBindable)
				{
					// Press, Down, and Up
					inputSource.altPress = inputSource.positiveAltPress || inputSource.negativeAltPress;
					inputSource.altDown = inputSource.positiveAltDown || inputSource.negativeAltDown;
					inputSource.altUp = inputSource.positiveAltUp || inputSource.negativeAltUp;

					// Hold
					inputSource.positiveAltHeld = false;

					if (inputSource.positiveAltPress)
					{
						inputSource.positiveAltHoldTimer -= deltaTime;
						inputSource.positiveAltHeld = inputSource.positiveAltHoldTimer <= 0f;

						if (inputSource.positiveAltHeld)
							inputSource.positiveAltHoldTimer = holdWaitTime;
					}
					else if (inputSource.positiveAltHoldTimer != holdTriggerTime)
						inputSource.positiveAltHoldTimer = holdTriggerTime;

					inputSource.negativeAltHeld = false;

					if (inputSource.negativeAltPress)
					{
						inputSource.negativeAltHoldTimer -= deltaTime;
						inputSource.negativeAltHeld = inputSource.negativeAltHoldTimer <= 0f;

						if (inputSource.negativeAltHeld)
							inputSource.negativeAltHoldTimer = holdWaitTime;
					}
					else if (inputSource.negativeAltHoldTimer != holdTriggerTime)
						inputSource.negativeAltHoldTimer = holdTriggerTime;

					inputSource.altHeld = inputSource.positiveAltHeld || inputSource.negativeAltHeld;

					// Double Press
					inputSource.positiveAltDoublePressTimer += inputSource.positiveAltDoublePressTimer > 0f ? -deltaTime : inputSource.positiveAltDoublePressTimer;

					if (inputSource.positiveAltUp)
						inputSource.positiveAltDoublePressTimer = doublePressTimeout;

					inputSource.positiveAltDoublePress = inputSource.positiveAltDoublePressInitiated && inputSource.positiveAltUp;

					if (inputSource.positiveAltUp && inputSource.positiveAltDoublePressTimer > 0f)
						inputSource.positiveAltDoublePressInitiated = true;

					if (inputSource.positiveAltDoublePressTimer <= 0f)
						inputSource.positiveAltDoublePressInitiated = false;

					inputSource.negativeAltDoublePressTimer += inputSource.negativeAltDoublePressTimer > 0f ? -deltaTime : inputSource.negativeAltDoublePressTimer;

					if (inputSource.negativeAltUp)
						inputSource.negativeAltDoublePressTimer = doublePressTimeout;

					inputSource.negativeAltDoublePress = inputSource.negativeAltDoublePressInitiated && inputSource.negativeAltUp;

					if (inputSource.negativeAltUp && inputSource.negativeAltDoublePressTimer > 0f)
						inputSource.negativeAltDoublePressInitiated = true;

					if (inputSource.negativeAltDoublePressTimer <= 0f)
						inputSource.negativeAltDoublePressInitiated = false;

					inputSource.altDoublePress = inputSource.positiveAltDoublePress || inputSource.negativeAltDoublePress;
				}

				if (inputSource.positiveBindable)
				{
					inputSource.positivePress = inputSource.positiveMainPress || inputSource.positiveAltPress;
					inputSource.positiveDown = inputSource.positiveMainDown || inputSource.positiveAltDown;
					inputSource.positiveUp = inputSource.positiveMainUp || inputSource.positiveAltUp;
					inputSource.positiveHeld = inputSource.positiveMainHeld || inputSource.positiveAltHeld;
					inputSource.positiveDoublePress = inputSource.positiveMainDoublePress || inputSource.positiveAltDoublePress;
				}

				if (inputSource.negativeBindable)
				{
					inputSource.negativePress = inputSource.negativeMainPress || inputSource.negativeAltPress;
					inputSource.negativeDown = inputSource.negativeMainDown || inputSource.negativeAltDown;
					inputSource.negativeUp = inputSource.negativeMainUp || inputSource.negativeAltUp;
					inputSource.negativeHeld = inputSource.negativeMainHeld || inputSource.negativeAltHeld;
					inputSource.negativeDoublePress = inputSource.negativeMainDoublePress || inputSource.negativeAltDoublePress;
				}

				inputSource.press = inputSource.mainPress || inputSource.altPress;
				inputSource.down = inputSource.mainDown || inputSource.altDown;
				inputSource.up = inputSource.mainUp || inputSource.altUp;
				inputSource.held = inputSource.mainHeld || inputSource.altHeld;
				inputSource.doublePress = inputSource.mainDoublePress || inputSource.altDoublePress;
			}

			sourceAccess[i] = inputSource;
		}

		#endregion
	}
}
