using UnityEngine;

namespace Utilities.Camera
{
	public class LookFollow : MonoBehaviour
	{
		public Transform target;
		public Vector3 offset;

		private Vector3 targetPosition;
		
		private void LateUpdate()
		{
			// Check for target availability to prevent null reference exceptions
			if (!target)
				return;

			// Calculating target position
			targetPosition = target.position;
			targetPosition += offset.x * target.right;
			targetPosition += offset.y * target.up;
			targetPosition += offset.z * target.forward;

			// Following the target by looking at its target position
			transform.LookAt(targetPosition);
		}
	}
}
