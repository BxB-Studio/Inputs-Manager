#region Namespaces

using UnityEngine;

#endregion

namespace Utilities.Inputs.Samples
{
	public class Ball : MonoBehaviour
	{
		#region Variables

		public Rigidbody ballRigidbody;
		public SphereCollider ballCollider;
		public float speed = 1f;
		public float jumpIntensity = 1f;

		private bool grounded;
		private float distanceToGround;

		#endregion

		#region Methods

		private void Awake()
		{
			// Check for the ball collider component availability to prevent null reference exceptions
			if (!ballCollider)
				return;

			// Get the collider radius
			distanceToGround = ballCollider.bounds.extents.y;
		}
		private void Start()
		{
			InputsManager.Start();
		}

		private void Update()
		{
			InputsManager.Update();
		}
		private void FixedUpdate()
		{
			// Check for components availability to prevent null reference exceptions
			if (!ballRigidbody || !ballCollider)
				return;

			// Check if the ball is grounded
			grounded = Physics.Raycast(transform.position, -Vector3.up, distanceToGround + .05f);

			if (grounded)
			{
				// Get inputs
				float vertical = InputsManager.InputValue("Vertical");
				float horizontal = InputsManager.InputValue("Horizontal");
				float jump;

				if (InputsManager.InputDown("Jump"))
					jump = 1f;
				else
					jump = 0f;

				// Calculate ball force and velocity
				Vector3 force = Vector3.zero;

				force += speed * vertical * Vector3.forward;
				force += speed * horizontal * Vector3.right;
				force += jumpIntensity * 100f * jump * Vector3.up;

				// Apply force to ball
				ballRigidbody.AddForce(force);
			}
		}

		private void OnDestroy()
		{
			InputsManager.Dispose();
		}

		#endregion
	}
}
