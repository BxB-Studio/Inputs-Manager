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
	internal struct InputGamepadJob : IJobFor
	{
		#region Variables

		public NativeArray<InputSourceAccess> sourceAccess;
		[ReadOnly]
		public InputAccess input;
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
			InputSourceAccess source = sourceAccess[i];

			// Init
			intervalA = input.invert ? input.valueInterval.y : input.valueInterval.x;
			intervalB = input.invert ? input.valueInterval.x : input.valueInterval.y;

			// Main
			positiveValue = source.positiveMainValue;
			negativeValue = source.negativeMainValue;
			positiveCoefficient = 1f;
			negativeCoefficient = 1f;

			switch (input.main.gamepadStrongSide)
			{
				case InputAxisStrongSide.FirstPressing:
					if (source.mainValue > 0f)
						positiveCoefficient = 2f;
					else if (source.mainValue < 0f)
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
				source.mainValue = Mathf.MoveTowards(source.mainValue, target, deltaTime / interpolationTime);
			else if (input.interpolation == InputInterpolation.Jump && interpolationTime > 0f)
				source.mainValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(source.mainValue) ? 0f : Mathf.MoveTowards(source.mainValue, target, deltaTime / interpolationTime);
			else
				source.mainValue = target;

			// Alt
			positiveValue = source.positiveAltValue;
			negativeValue = source.negativeAltValue;
			positiveCoefficient = 1f;
			negativeCoefficient = 1f;

			switch (input.alt.gamepadStrongSide)
			{
				case InputAxisStrongSide.FirstPressing:
					if (source.altValue > 0f)
						positiveCoefficient = 2f;
					else if (source.altValue < 0f)
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
				source.altValue = Mathf.MoveTowards(source.altValue, target, deltaTime / interpolationTime);
			else if (input.interpolation == InputInterpolation.Jump && interpolationTime > 0f)
				source.altValue = target != 0f && Mathf.Sign(target) != Mathf.Sign(source.altValue) ? 0f : Mathf.MoveTowards(source.altValue, target, deltaTime / interpolationTime);
			else
				source.altValue = target;

			// Values
			if (source.mainValue != 0f)
				source.value = source.mainValue;
			else
				source.value = source.altValue;

			if (source.bindable)
			{
				if (source.mainBindable)
				{
					source.mainPress = source.positiveMainPress || source.negativeMainPress;
					source.mainDown = source.positiveMainDown || source.negativeMainDown;
					source.mainUp = source.positiveMainUp || source.negativeMainUp;

					source.positiveMainHeld = false;

					if (source.positiveMainPress)
					{
						source.positiveMainHoldTimer -= deltaTime;
						source.positiveMainHeld = source.positiveMainHoldTimer <= 0f;

						if (source.positiveMainHeld)
							source.positiveMainHoldTimer = holdWaitTime;
					}
					else if (source.positiveMainHoldTimer != holdTriggerTime)
						source.positiveMainHoldTimer = holdTriggerTime;

					source.negativeMainHeld = false;

					if (source.negativeMainPress)
					{
						source.negativeMainHoldTimer -= deltaTime;
						source.negativeMainHeld = source.negativeMainHoldTimer <= 0f;

						if (source.negativeMainHeld)
							source.negativeMainHoldTimer = holdWaitTime;
					}
					else if (source.negativeMainHoldTimer != holdTriggerTime)
						source.negativeMainHoldTimer = holdTriggerTime;

					source.mainHeld = source.positiveMainHeld || source.negativeMainHeld;

					source.positiveMainDoublePressTimer += source.positiveMainDoublePressTimer > 0f ? -deltaTime : source.positiveMainDoublePressTimer;

					if (source.positiveMainUp)
						source.positiveMainDoublePressTimer = doublePressTimeout;

					source.positiveMainDoublePress = source.positiveMainDoublePressInitiated && source.positiveMainUp;

					if (source.positiveMainUp && source.positiveMainDoublePressTimer > 0f)
						source.positiveMainDoublePressInitiated = true;

					if (source.positiveMainDoublePressTimer <= 0f)
						source.positiveMainDoublePressInitiated = false;

					source.negativeMainDoublePressTimer += source.negativeMainDoublePressTimer > 0f ? -deltaTime : source.negativeMainDoublePressTimer;

					if (source.negativeMainUp)
						source.negativeMainDoublePressTimer = doublePressTimeout;

					source.negativeMainDoublePress = source.negativeMainDoublePressInitiated && source.negativeMainUp;

					if (source.negativeMainUp && source.negativeMainDoublePressTimer > 0f)
						source.negativeMainDoublePressInitiated = true;

					if (source.negativeMainDoublePressTimer <= 0f)
						source.negativeMainDoublePressInitiated = false;

					source.mainDoublePress = source.positiveMainDoublePress || source.negativeMainDoublePress;
				}

				if (source.altBindable)
				{
					source.altPress = source.positiveAltPress || source.negativeAltPress;
					source.altDown = source.positiveAltDown || source.negativeAltDown;
					source.altUp = source.positiveAltUp || source.negativeAltUp;

					source.positiveAltHeld = false;

					if (source.positiveAltPress)
					{
						source.positiveAltHoldTimer -= deltaTime;
						source.positiveAltHeld = source.positiveAltHoldTimer <= 0f;

						if (source.positiveAltHeld)
							source.positiveAltHoldTimer = holdWaitTime;
					}
					else if (source.positiveAltHoldTimer != holdTriggerTime)
						source.positiveAltHoldTimer = holdTriggerTime;

					source.negativeAltHeld = false;

					if (source.negativeAltPress)
					{
						source.negativeAltHoldTimer -= deltaTime;
						source.negativeAltHeld = source.negativeAltHoldTimer <= 0f;

						if (source.negativeAltHeld)
							source.negativeAltHoldTimer = holdWaitTime;
					}
					else if (source.negativeAltHoldTimer != holdTriggerTime)
						source.negativeAltHoldTimer = holdTriggerTime;

					source.altHeld = source.positiveAltHeld || source.negativeAltHeld;

					source.positiveAltDoublePressTimer += source.positiveAltDoublePressTimer > 0f ? -deltaTime : source.positiveAltDoublePressTimer;

					if (source.positiveAltUp)
						source.positiveAltDoublePressTimer = doublePressTimeout;

					source.positiveAltDoublePress = source.positiveAltDoublePressInitiated && source.positiveAltUp;

					if (source.positiveAltUp && source.positiveAltDoublePressTimer > 0f)
						source.positiveAltDoublePressInitiated = true;

					if (source.positiveAltDoublePressTimer <= 0f)
						source.positiveAltDoublePressInitiated = false;

					source.negativeAltDoublePressTimer += source.negativeAltDoublePressTimer > 0f ? -deltaTime : source.negativeAltDoublePressTimer;

					if (source.negativeAltUp)
						source.negativeAltDoublePressTimer = doublePressTimeout;

					source.negativeAltDoublePress = source.negativeAltDoublePressInitiated && source.negativeAltUp;

					if (source.negativeAltUp && source.negativeAltDoublePressTimer > 0f)
						source.negativeAltDoublePressInitiated = true;

					if (source.negativeAltDoublePressTimer <= 0f)
						source.negativeAltDoublePressInitiated = false;

					source.altDoublePress = source.positiveAltDoublePress || source.negativeAltDoublePress;
				}

				if (source.positiveBindable)
				{
					source.positivePress = source.positiveMainPress || source.positiveAltPress;
					source.positiveDown = source.positiveMainDown || source.positiveAltDown;
					source.positiveUp = source.positiveMainUp || source.positiveAltUp;
					source.positiveHeld = source.positiveMainHeld || source.positiveAltHeld;
					source.positiveDoublePress = source.positiveMainDoublePress || source.positiveAltDoublePress;
				}

				if (source.negativeBindable)
				{
					source.negativePress = source.negativeMainPress || source.negativeAltPress;
					source.negativeDown = source.negativeMainDown || source.negativeAltDown;
					source.negativeUp = source.negativeMainUp || source.negativeAltUp;
					source.negativeHeld = source.negativeMainHeld || source.negativeAltHeld;
					source.negativeDoublePress = source.negativeMainDoublePress || source.negativeAltDoublePress;
				}

				source.press = source.mainPress || source.altPress;
				source.down = source.mainDown || source.altDown;
				source.up = source.mainUp || source.altUp;
				source.held = source.mainHeld || source.altHeld;
				source.doublePress = source.mainDoublePress || source.altDoublePress;
			}

			sourceAccess[i] = source;
		}

		#endregion
	}
}
