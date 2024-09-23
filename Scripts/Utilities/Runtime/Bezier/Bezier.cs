#region Namespaces

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Utilities
{
	public static class Bezier
	{
		#region Modules

		[Serializable]
		public class Path
		{
			#region Variables

			#region Global Variables

			public int SegmentsCount => points.Count / 3;
			public int PointsCount => points.Count;
			public bool AutoCalculateControls
			{
				get
				{
					return autoCalculateControls;
				}
				set
				{
					if (autoCalculateControls == value)
						return;

					autoCalculateControls = value;

					if (autoCalculateControls)
						AutoSetControls();
				}
			}
			public bool LoopedPath
			{
				get
				{
					if (SegmentsCount < 1)
						return false;

					return loopedPath;
				}
				set
				{
					if (SegmentsCount < 1 || loopedPath == value)
						return;

					loopedPath = value;

					if (SegmentsCount < 1)
						return;

					if (loopedPath)
					{
						points.Add(points[^1] * 2f - points[^2]);
						points.Add(points[0] * 2f - points[1]);

						if (AutoCalculateControls)
						{
							AutoSetAnchorControls(0);
							AutoSetAnchorControls(points.Count - 3);
						}
					}
					else if (SegmentsCount > 0)
					{
						points.RemoveRange(points.Count - 2, 2);

						if (AutoCalculateControls)
							AutoSetStartEndControls();
					}
				}
			}
			public List<int> disabledSegments;
			public LayerMask groundLayerMask;

			private bool ShouldRefreshPointsNormals
			{
				get
				{
					return pointsNormals == null || pointsNormals.Count != SegmentsCount + 1;
				}
			}
			[SerializeField, HideInInspector]
			private List<Vector3> points;
			[SerializeField, HideInInspector]
			private List<Vector3> pointsNormals;
			[SerializeField, HideInInspector]
			private bool loopedPath;
			[SerializeField, HideInInspector]
			private bool autoCalculateControls;

			#endregion

			#region Indexers

			public Vector3 this[int index]
			{
				get
				{
					return points[index];
				}
				set
				{
					if (points[index] == value || AutoCalculateControls && !IsAnchorPoint(index))
						return;

					if (IsAnchorPoint(index))
						SetAnchorPoint(index / 3, value);
					else
					{
						points[index] = value;

						bool nextPointIsAnchor = IsAnchorPoint(index + 1);
						int correspondingControlIndex = nextPointIsAnchor ? index + 2 : index - 2;
						int anchorIndex = nextPointIsAnchor ? index + 1 : index - 1;

						if (correspondingControlIndex > -1 && correspondingControlIndex < points.Count || loopedPath)
						{
							anchorIndex = LoopIndex(anchorIndex);
							correspondingControlIndex = LoopIndex(correspondingControlIndex);

							float controlDistanceFromAnchor = Utility.Distance(points[anchorIndex], points[correspondingControlIndex]);
							Vector3 controlDirectionFromAnchor = Utility.Direction(value, points[anchorIndex]);

							points[correspondingControlIndex] = points[anchorIndex] + controlDirectionFromAnchor * controlDistanceFromAnchor;
						}
					}
				}
			}

			#endregion

			#endregion

			#region Methods

			public Mesh CreateMesh(float width, float spacing, int resolution, float tiling = 1f)
			{
				Vector3[] spacedPoints = GetSpacedPoints(spacing, resolution);

				if (spacedPoints == null || spacedPoints.Length < 2)
					return new Mesh();

				Vector3[] vertices = new Vector3[spacedPoints.Length * 2];
				Vector2[] uv = new Vector2[vertices.Length];
				int trianglesCount = 2 * (spacedPoints.Length - 1) + (loopedPath ? 2 : 0);
				int[] triangles = new int[trianglesCount * 3];
				int vertexIndex = 0;
				int triangleIndex = 0;

				for (int i = 0; i < spacedPoints.Length; i++)
				{
					if ((i < spacedPoints.Length - 1 || loopedPath) && IsSegmentDisbaled(ClosestAnchorPoint(spacedPoints[i]) / 3) && Utility.Distance(spacedPoints[i], spacedPoints[(i + 1) % spacedPoints.Length]) > spacing * 2f)
					{
						vertexIndex += 2;
						triangleIndex += 6;

						continue;
					}

					Vector3 forward = Vector3.zero;

					if (i < spacedPoints.Length - 1 || loopedPath)
						forward += Utility.DirectionUnNormalized(spacedPoints[i], spacedPoints[(i + 1) % spacedPoints.Length]);

					if ((i > 0 || loopedPath) && !IsSegmentDisbaled(ClosestAnchorPoint(spacedPoints[(i - 1 + spacedPoints.Length) % spacedPoints.Length]) / 3))
						forward += Utility.DirectionUnNormalized(spacedPoints[(i - 1 + spacedPoints.Length) % spacedPoints.Length], spacedPoints[i]);

					forward.Normalize();

					PointTransform transform = new PointTransform(spacedPoints[i], forward);
					Vector3 left = -transform.right;

					vertices[vertexIndex] = spacedPoints[i] + .5f * width * left;
					vertices[vertexIndex + 1] = spacedPoints[i] - .5f * width * left;

					float positionInPath = i / (float)(spacedPoints.Length - 1);
					float newPositionInPath = 1f - Mathf.Abs(2f * positionInPath - 1f);

					uv[vertexIndex] = new Vector2(0, newPositionInPath * spacedPoints.Length * spacing * .05f * tiling);
					uv[vertexIndex + 1] = new Vector2(1, newPositionInPath * spacedPoints.Length * spacing * .05f * tiling);

					if (i < spacedPoints.Length - 1 || loopedPath)
					{
						triangles[triangleIndex] = vertexIndex;
						triangles[triangleIndex + 1] = (vertexIndex + 2) % vertices.Length;
						triangles[triangleIndex + 2] = vertexIndex + 1;
						triangles[triangleIndex + 3] = vertexIndex + 1;
						triangles[triangleIndex + 4] = (vertexIndex + 2) % vertices.Length;
						triangles[triangleIndex + 5] = (vertexIndex + 3) % vertices.Length;
					}

					vertexIndex += 2;
					triangleIndex += 6;
				}

				return new Mesh
				{
					vertices = vertices,
					triangles = triangles,
					uv = uv
				};
			}
			public float EstimatedLength()
			{
				float length = 0f;

				for (int i = 0; i < SegmentsCount; i++)
					length += EstimatedSegmentLength(i);

				return length;
			}
			public float EstimatedSegmentLength(int index)
			{
				Vector3[] segmentPoints = GetSegmentPoints(index);
				float controlNetLength = Utility.Distance(segmentPoints[0], segmentPoints[1]) + Utility.Distance(segmentPoints[1], segmentPoints[2]) + Utility.Distance(segmentPoints[2], segmentPoints[3]);
				
				return Utility.Distance(segmentPoints[0], segmentPoints[3]) + controlNetLength * .5f;
			}
			public void AddSegment(Vector3 position)
			{
				if (points.Count < 1)
				{
					points.Add(position);

					return;
				}
				else if (points.Count == 1)
				{
					points.Add(Utility.Average(points[^1], position));
					points.Add(Utility.Average(points[0], position));
				}
				else
				{
					points.Add(points[^1] * 2f - points[^2]);
					points.Add(Utility.Average(points[^1], position));
				}

				points.Add(position);

				if (AutoCalculateControls)
					AutoSetMovedAnchorControls(points.Count - 1);

				RefreshAnchorNormals();
			}
			public void SplitSegment(Vector3 position, int index)
			{
				if (SegmentsCount < 1)
					return;

				points.InsertRange(index * 3 + 2,
					new Vector3[]
					{
						Vector3.zero,
						position,
						Vector3.zero
					}
				);

				if (AutoCalculateControls)
					AutoSetMovedAnchorControls(index * 3 + 3);
				else
					AutoSetAnchorControls(index * 3 + 3);
			}
			public bool IsSegmentDisbaled(int index)
			{
				return disabledSegments.IndexOf(index) > -1;
			}
			public void EnableSegment(int index)
			{
				disabledSegments.Remove(index);
			}
			public void DisableSegment(int index)
			{
				if (disabledSegments.IndexOf(index) < 0)
					disabledSegments.Add(index);
			}
			public void RemoveSegment(int anchorIndex)
			{
				if (SegmentsCount < 3 && loopedPath || SegmentsCount < 2)
					return;

				if (anchorIndex == 0)
				{
					if (loopedPath)
						points[^1] = points[2];

					points.RemoveRange(0, 3);
				}
				else if (anchorIndex == points.Count - 1 && !loopedPath)
					points.RemoveRange(anchorIndex - 2, 3);
				else
					points.RemoveRange(anchorIndex - 1, 3);
			}
			public Vector3[] GetSpacedPoints(float spacing, out Vector3[] pointsNormals, int resolution = 1)
			{
				pointsNormals = null;

				if (SegmentsCount < 1)
					return null;

				spacing = Utility.ClampInfinity(spacing, .1f);
				resolution = Utility.ClampInfinity(resolution, 1);

				List<Vector3> spacedPoints = new List<Vector3>();
				List<Vector3> newPointsNormals = new List<Vector3>();
				Vector3 lastPoint = points.FirstOrDefault();
				float distanceFromLastPoint = 0f;

				spacedPoints.Add(lastPoint);
				newPointsNormals.Add(GetAnchorPointNormal(0));

				for (int i = 0; i < SegmentsCount; i++)
				{
					Vector3[] segmentPoints = GetSegmentPoints(i);	
					int divisions = Mathf.CeilToInt(EstimatedSegmentLength(i) * resolution * 10f);

					for (float t = 0f; t <= 1f; t += 1f / divisions)
					{
						Vector3 pointOnCurve = EvaluateCubic(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3], t);

						distanceFromLastPoint += Utility.Distance(lastPoint, pointOnCurve);

						while (distanceFromLastPoint >= spacing)
						{
							float overshootDistance = distanceFromLastPoint - spacing;
							Vector3 newPointOnCurve = pointOnCurve + Utility.Direction(pointOnCurve, lastPoint) * overshootDistance;

							if (disabledSegments.IndexOf(i) < 0)
							{
								spacedPoints.Add(newPointOnCurve);
								newPointsNormals.Add(Vector3.Lerp(GetAnchorPointNormal(i), GetAnchorPointNormal(i + 1), t));
							}

							distanceFromLastPoint = overshootDistance;
							lastPoint = newPointOnCurve;
						}

						lastPoint = pointOnCurve;
					}
				}

				pointsNormals = newPointsNormals.ToArray();

				return spacedPoints.ToArray();
			}
			public Vector3[] GetSpacedPoints(float spacing, int resolution = 1)
			{
				return GetSpacedPoints(spacing, out _, resolution);
			}
			public Vector3[] GetSegmentPoints(int index)
			{
				if (SegmentsCount < 1)
					return new Vector3[] { };

				return new Vector3[]
				{
					points[index * 3],
					points[index * 3 + 1],
					points[index * 3 + 2],
					points[LoopIndex(index * 3 + 3)]
				};
			}
			public int ClosestSegment(Vector3 position, float distanceRange)
			{
				if (SegmentsCount < 2)
					return SegmentsCount - 1;

				int closestSegmentIndex = -1;

				for (int i = 0; i < points.Count - 3; i += 3)
				{
					float distanceToFirstAnchor = Utility.Distance(position, points[i]);
					float distanceToSecondAnchor = Utility.Distance(position, points[i + 3]);
					float distanceToAnchors = Utility.Average(distanceToFirstAnchor, distanceToSecondAnchor);

					if (distanceRange > distanceToAnchors)
					{
						distanceRange = distanceToAnchors;
						closestSegmentIndex = i;
					}
				}

				return closestSegmentIndex;
			}
			public int ClosestSegment(Vector3 position)
			{
				return ClosestSegment(position, Mathf.Infinity);
			}
			public bool IsAnchorPoint(int index)
			{
				return index % 3 == 0;
			}
			public int ClosestAnchorPoint(Vector3 position, float distanceRange)
			{
				int closestAnchorIndex = -1;

				for (int i = 0; i < points.Count; i += 3)
				{
					float distance = Utility.Distance(points[i], position);

					if (distanceRange > distance)
					{
						distanceRange = distance;
						closestAnchorIndex = i;
					}
				}

				return closestAnchorIndex;
			}
			public int ClosestAnchorPoint(Vector3 position)
			{
				return ClosestAnchorPoint(position, Mathf.Infinity);
			}
			public Vector3 GetAnchorPoint(int index)
			{
				if (PointsCount < 1)
					return default;

				if (!Application.isPlaying && ShouldRefreshPointsNormals)
					RefreshAnchorNormals();

				return points[LoopIndex(index * 3)];
			}
			public Vector3 GetAnchorPointNormal(int index)
			{
				if (PointsCount < 1)
					return Vector3.up;

				if (ShouldRefreshPointsNormals)
					RefreshAnchorNormals();

				return pointsNormals[LoopIndex(index * 3) / 3];
			}
			public void SetAnchorPoint(int index, Vector3 value)
			{
				index = LoopIndex(index * 3);

				Vector3 deltaMove = value - points[index];

				points[index] = value;

				if (ShouldRefreshPointsNormals)
					RefreshAnchorNormals();
				else
					pointsNormals[index / 3] = GetPointNormal(value);

				if (AutoCalculateControls)
				{
					AutoSetMovedAnchorControls(index);

					return;
				}

				if (index + 1 < points.Count || loopedPath)
					points[LoopIndex(index + 1)] += deltaMove;

				if (index - 1 > -1 || loopedPath)
					points[LoopIndex(index - 1)] += deltaMove;
			}
			public Vector3[] GetAnchorPoints()
			{
				return points.Where((point, index) => index % 3 == 0).ToArray();
			}
			public void OffsetAllPoints(Vector3 offset)
			{
				for (int i = 0; i < points.Count; i++)
					points[i] += offset;
			}

			private void AutoSetControls()
			{
				for (int i = 0; i < points.Count; i += 3)
					AutoSetAnchorControls(i);

				AutoSetStartEndControls();
			}
			private void AutoSetMovedAnchorControls(int anchorIndex)
			{
				if (SegmentsCount < 1)
					return;

				for (int i = anchorIndex - 3; i <= anchorIndex + 3; i += 3)
					if (i > -1 && i < points.Count || loopedPath)
						AutoSetAnchorControls(LoopIndex(i));

				AutoSetStartEndControls();
			}
			private void AutoSetAnchorControls(int anchorIndex)
			{
				if (SegmentsCount < 1)
					return;

				Vector3 anchorPosition = points[anchorIndex];
				Vector3 direction = Vector3.zero;
				float[] neighborDistances = new float[2];

				if (anchorIndex - 3 >= 0 || loopedPath)
				{
					Vector3 offset = points[LoopIndex(anchorIndex - 3)] - anchorPosition;

					direction += offset.normalized;
					neighborDistances[0] = offset.magnitude;
				}

				if (anchorIndex + 3 >= 0 || loopedPath)
				{
					Vector3 offset = points[LoopIndex(anchorIndex + 3)] - anchorPosition;

					direction -= offset.normalized;
					neighborDistances[1] = -offset.magnitude;
				}

				direction.Normalize();

				for (int i = 0; i < 2; i++)
				{
					int controlIndex = anchorIndex + i * 2 - 1;

					if (controlIndex > -1 && controlIndex < points.Count || loopedPath)
						points[LoopIndex(controlIndex)] = anchorPosition + .5f * neighborDistances[i] * direction;
				}
			}
			private void AutoSetStartEndControls()
			{
				if (loopedPath || SegmentsCount < 1)
					return;

				points[1] = Utility.Average(points[0], points[2]);
				points[^2] = Utility.Average(points[^1], points[^3]);
			}
			private void RefreshAnchorNormals()
			{
				pointsNormals = new List<Vector3>();

				if (SegmentsCount < 1)
					return;

				Vector3[] segmentPoints = GetSegmentPoints(0);

				pointsNormals.Add(GetPointNormal(segmentPoints[0]));
				pointsNormals.Add(GetPointNormal(segmentPoints[3]));

				for (int i = 1; i < SegmentsCount; i++)
					pointsNormals.Add(GetPointNormal(GetSegmentPoints(i)[3]));
			}
			private Vector3 GetPointNormal(Vector3 point)
			{
				RaycastHit[] hits = new RaycastHit[2];

				Physics.Raycast(point + Vector3.up, Vector3.down, out hits[0], 10f, groundLayerMask, QueryTriggerInteraction.Ignore);
				Physics.Raycast(point + Vector3.down, Vector3.up, out hits[1], 10f, groundLayerMask, QueryTriggerInteraction.Ignore);

				if (hits[0].collider && hits[1].collider)
					return hits[0].distance > hits[1].distance ? -hits[1].normal : hits[0].normal;
				else if (hits[0].collider)
					return hits[0].normal;
				else if (hits[1].collider)
					return -hits[1].normal;

				return Vector3.up;
			}
			private int LoopIndex(int index)
			{
				while (index < 0)
					index += PointsCount;

				return index % PointsCount;
			}

			#endregion

			#region Constructors & Operators

			#region Constructors

			public Path(LayerMask groundLayerMask)
			{
				points = new List<Vector3>();
				disabledSegments = new List<int>();
				this.groundLayerMask = groundLayerMask;
			}
			public Path(Vector3 center, LayerMask groundLayerMask)
			{
				points = new List<Vector3>
				{
					center + Vector3.back,
					center + (Vector3.back + Vector3.left) * .5f,
					center + (Vector3.forward + Vector3.right) * .5f,
					center + Vector3.forward
				};
				disabledSegments = new List<int>();
				this.groundLayerMask = groundLayerMask;
			}

			#endregion

			#region Operators

			public static implicit operator bool(Path path) => path != null;

			#endregion

			#endregion
		}

		private struct PointTransform
		{
			#region Variables

			public Vector3 position;
			public Quaternion rotation;
			public Vector3 forward;
			public Vector3 normal;
			public Vector3 right;

			#endregion

			#region Constructors

			public PointTransform(Vector3 point, Vector3 forward)
			{
				position = point;
				rotation = Quaternion.LookRotation(forward);

				Transform transform = new GameObject().transform;

				transform.SetPositionAndRotation(position, rotation);

				this.forward = transform.forward;
				normal = transform.up;
				right = transform.right;

				Utility.Destroy(true, transform.gameObject);
			}

			#endregion
		}

		#endregion

		#region Methods

		public static Vector3 EvaluateLinear(Vector3 a, Vector3 b, float t)
		{
			return Vector3.Lerp(a, b, t);
		}
		public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
		{
			Vector3 p0 = EvaluateLinear(a, b, t);
			Vector3 p1 = EvaluateLinear(b, c, t);

			return Vector3.Lerp(p0, p1, t);
		}
		public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
		{
			Vector3 p0 = EvaluateQuadratic(a, b, c, t);
			Vector3 p1 = EvaluateQuadratic(b, c, d, t);

			return Vector3.Lerp(p0, p1, t);
		}

		#endregion
	}
}
