#region Namespaces

using UnityEngine;

#endregion

namespace Utilities
{
	public class BezierSample : MonoBehaviour
	{
		#region Variables

		[HideInInspector]
		public Bezier.Path path;
		public Material meshMaterial;
		public float meshSpacing = .1f;
		public int meshResolution = 1;
		public float meshTiling = 1f;
		public float meshWidth = 3f;
		public bool ShowMesh
		{
			get
			{
				return showMesh;
			}
			set
			{
				if (showMesh == value)
					return;

				showMesh = value;

				UpdateMesh();
			}
		}

		[SerializeField, HideInInspector]
		private bool showMesh;

		#endregion

		#region Methods

		#region Utilities

		public void CreatePath()
		{
			path = new Bezier.Path(transform.position, -1);
		}
		public void ResetPath()
		{
			path = new Bezier.Path(-1);
		}
		public void UpdateMesh()
		{
			Transform meshTransform = transform.Find("Mesh");

			if (!meshTransform)
			{
				meshTransform = new GameObject("Mesh").transform;
				meshTransform.parent = transform;
				meshTransform.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}

			if (ShowMesh)
			{
				MeshFilter filter = meshTransform.GetComponent<MeshFilter>();

				if (!filter)
					filter = meshTransform.gameObject.AddComponent<MeshFilter>();

				filter.mesh = path.CreateMesh(meshWidth, meshSpacing, meshResolution, meshTiling);

				MeshRenderer renderer = meshTransform.GetComponent<MeshRenderer>();

				if (!renderer)
					renderer = meshTransform.gameObject.AddComponent<MeshRenderer>();

				renderer.material = meshMaterial;
			}
			else if (meshTransform)
				Utility.Destroy(true, meshTransform.gameObject);
		}

		#endregion

		#region Reset

		private void Reset()
		{
			ResetPath();
		}

		#endregion

		#endregion
	}
}
