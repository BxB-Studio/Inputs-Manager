using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Utilities
{
	public static class Utility
	{
		#region Methods

		public static void Dummy()
		{

		}
		public static float UnitMultiplier(Units unit, UnitType unitType)
		{
			switch (unit)
			{
				case Units.Distance:
					return unitType == UnitType.Metric ? 1f : 3.28084f;

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? 1f : 1.09361f;

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? 1f : 0.621371f;

				case Units.Liquid:
					return unitType == UnitType.Metric ? 1f : 1f / 4.546f;

				case Units.Power:
					return unitType == UnitType.Metric ? 1f : 0.7457f;

				case Units.Size:
					return unitType == UnitType.Metric ? 1f : 1f / 2.54f;

				case Units.Speed:
					return unitType == UnitType.Metric ? 1f : 0.621371f;

				case Units.Torque:
					return unitType == UnitType.Metric ? 1f : 0.73756f;

				case Units.Weight:
					return unitType == UnitType.Metric ? 1f : 2.20462262185f;

				default:
					return 1f;
			}
		}
		public static string Unit(Units unit, UnitType unitType)
		{
			switch (unit)
			{
				case Units.Distance:
					return unitType == UnitType.Metric ? "m" : "ft";

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? "m" : "yd";

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? "km" : "m";

				case Units.Force:
					return unitType == UnitType.Metric ? "N" : "N";

				case Units.Liquid:
					return unitType == UnitType.Metric ? "L" : "gal";

				case Units.Power:
					return unitType == UnitType.Metric ? "hp" : "kW";

				case Units.Size:
					return unitType == UnitType.Metric ? "cm" : "in";

				case Units.SizeAccurate:
					return unitType == UnitType.Metric ? "mm" : "mm";

				case Units.Speed:
					return unitType == UnitType.Metric ? "km/h" : "mph";

				case Units.Time:
					return unitType == UnitType.Metric ? "s" : "s";

				case Units.TimeAccurate:
					return unitType == UnitType.Metric ? "ms" : "ms";

				case Units.Torque:
					return unitType == UnitType.Metric ? "N⋅m" : "ft/lb";

				case Units.Weight:
					return unitType == UnitType.Metric ? "kg" : "lbs";

				default:
					return default;
			}
		}
		public static string FullUnit(Units unit, UnitType unitType)
		{
			switch (unit)
			{
				case Units.Distance:
					return unitType == UnitType.Metric ? "Metre" : "Feet";

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? "Metre" : "Yard";

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? "Kilometre" : "Miles";

				case Units.Force:
					return unitType == UnitType.Metric ? "Newton" : "Newton";

				case Units.Liquid:
					return unitType == UnitType.Metric ? "Litre" : "Gallon";

				case Units.Power:
					return unitType == UnitType.Metric ? "Horsepower" : "Kilowatt";

				case Units.Size:
					return unitType == UnitType.Metric ? "Centimetre" : "Inch";

				case Units.SizeAccurate:
					return unitType == UnitType.Metric ? "Millimetre" : "Millimetre";

				case Units.Speed:
					return unitType == UnitType.Metric ? "Kilometres per Hour" : "Miles per Hour";

				case Units.Time:
					return unitType == UnitType.Metric ? "Second" : "Second";

				case Units.TimeAccurate:
					return unitType == UnitType.Metric ? "Millisecond" : "Millisecond";

				case Units.Torque:
					return unitType == UnitType.Metric ? "Newton⋅Meter" : "Foot per Pound";

				case Units.Weight:
					return unitType == UnitType.Metric ? "Kilogram" : "Pound";

				default:
					return default;
			}
		}
		public static float ValueWithUnitToNumber(string value)
		{
			string[] valueArray = value.Trim().Split(' ');

			value = "";

			for (int i = 0; i < valueArray.Length; i++)
				if (float.TryParse(valueArray[i], out float number))
					value += number + (i < valueArray.Length - 1 ? " " : "");

			return !string.IsNullOrEmpty(value) && float.TryParse(value, out float result) ? result : 0f;
		}
		public static float ValueWithUnitToNumber(string value, Units unit, UnitType unitType)
		{
			string[] valueArray = value.Trim().Split(' ');

			value = "";

			for (int i = 0; i < valueArray.Length; i++)
				if (float.TryParse(valueArray[i], out float number))
					value += number + (i < valueArray.Length - 1 ? " " : "");

			return !string.IsNullOrEmpty(value) && float.TryParse(value, out float result) ? result / UnitMultiplier(unit, unitType) : 0f;
		}
		public static string NumberToValueWithUnit(float number, string unit, bool rounded)
		{
			return $"{(rounded ? Mathf.Round(number) : number)} {unit}";
		}
		public static string NumberToValueWithUnit(float number, Units unit, UnitType unitType, bool rounded)
		{
			number *= UnitMultiplier(unit, unitType);

			return $"{(rounded ? Mathf.Round(number) : number)} {Unit(unit, unitType)}";
		}
		public static string NumberToValueWithUnit(float number, Units unit, UnitType unitType, int decimals)
		{
			float multiplier = 1;

			if (decimals > 0)
				for (int i = 0; i < decimals; i++)
					multiplier *= 10f;

			number *= UnitMultiplier(unit, unitType);

			return $"{(decimals > 0 ? Mathf.Round(number * multiplier) / multiplier : number)} {Unit(unit, unitType)}";
		}
		public static string ClassifyNumber(int number)
		{
			switch (number.ToString().LastOrDefault())
			{
				case '1':
					return number + "st";

				case '2':
					return number + "nd";

				case '3':
					return number + "rd";

				default:
					return number + "th";
			}
		}
		public static GameObject[] GetChilds(GameObject gameObject)
		{
			return (from Transform child in gameObject.GetComponentsInChildren<Transform>() where gameObject.transform != child select child.gameObject).ToArray();
		}
		public static float EvaluateFriction(float slip, PhysicMaterial refMaterial, PhysicMaterial material)
		{
			switch (refMaterial.frictionCombine)
			{
				case PhysicMaterialCombine.Average:
					return !Mathf.Approximately(slip, 0f) ? (refMaterial.dynamicFriction + material.dynamicFriction) * .5f : (refMaterial.staticFriction + material.staticFriction) * .5f;

				case PhysicMaterialCombine.Multiply:
					return !Mathf.Approximately(slip, 0f) ? refMaterial.dynamicFriction * material.dynamicFriction : refMaterial.staticFriction * material.staticFriction;

				case PhysicMaterialCombine.Minimum:
					return !Mathf.Approximately(slip, 0f) ? Mathf.Min(refMaterial.dynamicFriction, material.dynamicFriction) : Mathf.Max(refMaterial.staticFriction, material.staticFriction);

				case PhysicMaterialCombine.Maximum:
					return !Mathf.Approximately(slip, 0f) ? Mathf.Max(refMaterial.dynamicFriction, material.dynamicFriction) : Mathf.Max(refMaterial.staticFriction, material.staticFriction);

				default:
					return 0f;
			}
		}
		public static float EvaluateFriction(float slip, PhysicMaterial refMaterial, float stiffness)
		{
			switch (refMaterial.frictionCombine)
			{
				case PhysicMaterialCombine.Average:
					return !Mathf.Approximately(slip, 0f) ? (refMaterial.dynamicFriction + stiffness) * .5f : (refMaterial.staticFriction + stiffness) * .5f;

				case PhysicMaterialCombine.Multiply:
					return !Mathf.Approximately(slip, 0f) ? refMaterial.dynamicFriction * stiffness : refMaterial.staticFriction * stiffness;

				case PhysicMaterialCombine.Minimum:
					return !Mathf.Approximately(slip, 0f) ? Mathf.Min(refMaterial.dynamicFriction, stiffness) : Mathf.Max(refMaterial.staticFriction, stiffness);

				case PhysicMaterialCombine.Maximum:
					return !Mathf.Approximately(slip, 0f) ? Mathf.Max(refMaterial.dynamicFriction, stiffness) : Mathf.Max(refMaterial.staticFriction, stiffness);

				default:
					return 0f;
			}
		}
		public static Bounds GetObjectBounds(GameObject gameObject, bool keepRotation = false, bool keepScale = true)
		{
			Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
			Bounds bounds = new Bounds();
			Quaternion orgRotation = gameObject.transform.rotation;
			Vector3 orgScale = gameObject.transform.localScale;

			if (!keepRotation)
				gameObject.transform.rotation = Quaternion.identity;

			if (!keepScale)
				gameObject.transform.localScale = Vector3.one;

			for (int i = 0; i < renderers.Length; i++)
				if (!(renderers[i] is TrailRenderer || renderers[i] is ParticleSystemRenderer))
				{
					if (bounds.size == Vector3.zero)
						bounds = renderers[i].bounds;
					else
						bounds.Encapsulate(renderers[i].bounds);
				}

			if (!keepRotation)
				gameObject.transform.rotation = orgRotation;

			if (!keepScale)
				gameObject.transform.localScale = orgScale;

			return bounds;
		}
		public static bool MaskHasLayer(LayerMask mask, int layer)
		{
			return (mask.value & 1 << layer) != 0;
		}
		public static int LayerMask(string name)
		{
			return ~(1 << UnityEngine.LayerMask.NameToLayer(name));
		}
		public static int BoolToInt(bool condition)
		{
			return condition ? 1 : 0;
		}
		public static float BoolToInt(bool condition, float source, float damping = 2.5f)
		{
			return Mathf.LerpUnclamped(source, BoolToInt(condition), Time.time * damping);
		}
		public static bool IntToBool(int number)
		{
			return Mathf.Clamp01(Mathf.RoundToInt(number)) != 0;
		}
		public static bool ValidDate(string date)
		{
			if (date.Length != 10)
				return false;
			else if (date[2] != '/' || date[5] != '/')
				return false;
			else if (date.Length == 10)
				return false;
			else
				for (int i = 0; i < date.Length; i++)
					if (date[i] != '0' && date[i] != '1' && date[i] != '2' && date[i] != '3' && date[i] != '4' && date[i] != '5' && date[i] != '6' && date[i] != '7' && date[i] != '8' && date[i] != '9' && date[i] != '/')
						return false;

			return true;
		}
		public static string TimeConverter(float time)
		{
			int seconds = Mathf.FloorToInt(time % 60);
			int minutes = Mathf.FloorToInt(time / 60);
			int hours = Mathf.FloorToInt(time / 3600);

			return (hours == 0 ? minutes.ToString() : (hours + ":" + minutes.ToString("00"))) + ":" + seconds.ToString("00");
		}
		public static void DrawArrowForGizmos(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}
		public static void DrawArrowForGizmos(Vector3 pos, Vector3 direction, UnityEngine.Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			UnityEngine.Color orgColor = Gizmos.color;

			Gizmos.color = color;

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);

			Gizmos.color = orgColor;
		}
		public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Debug.DrawRay(pos, direction);
			Debug.DrawRay(pos + direction, right * arrowHeadLength);
			Debug.DrawRay(pos + direction, left * arrowHeadLength);
		}
		public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, UnityEngine.Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Debug.DrawRay(pos, direction, color);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
		public static Vector3 PointFromCircle(Vector3 center, float radius, float angle, float angleOffset = 0f)
		{
			Vector3 newPosition;

			newPosition.x = center.x + radius * Mathf.Sin((angle + angleOffset) * Mathf.Deg2Rad);
			newPosition.y = center.y + radius * Mathf.Cos((angle + angleOffset) * Mathf.Deg2Rad);
			newPosition.z = center.z;

			return newPosition;
		}
		public static Vector3 PointFromLine(Vector3 start, Vector3 direction, float length, float position)
		{
			return start + direction * position * length;
		}
		public static Vector3 VectorAbs(Vector3 vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}
		public static Vector3 VectorsOffset(Vector3 a, Vector3 b, bool abs = false)
		{
			Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

			return abs ? VectorAbs(vector) : vector;
		}
		public static Vector3 Vector3Round(Vector3 vector)
		{
			return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
		}
		public static Vector3 Direction(Vector3 origin, Vector3 destination)
		{
			return (destination - origin);
		}
		public static string RandomString(int length, bool upperCase = true, bool lowerCase = true, bool numbers = true, bool symbols = true)
		{
			string chars = "";

			chars += upperCase ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : "";
			chars += lowerCase ? "abcdefghijklmnopqrstuvwxyz" : "";
			chars += numbers ? "0123456789" : "";
			chars += symbols ? "!@#$%^&()_+-{}[],.;" : "";

			return new string(Enumerable.Repeat(chars, length).Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
		}
		public static float Distance(Vector3 a, Vector3 b)
		{
			return (a - b).magnitude;
		}
		public static float Distance(float a, float b)
		{
			return Mathf.Max(a, b) - Mathf.Min(a, b);
		}
		public static bool IsDownFromLastState(bool current, bool last)
		{
			return !last && current;
		}
		public static bool IsUpFromLastState(bool current, bool last)
		{
			return last && !current;
		}
		public static AudioSource NewAudioSource(string sourceName, float minDistance, float maxDistance, float volume, AudioClip clip, bool loop, bool playNow, bool destroyAfterFinished, bool mute = false, Transform parent = null, AudioMixerGroup mixer = null, bool spatialize = false)
		{
			AudioSource source = new GameObject(sourceName).AddComponent<AudioSource>();

			if (parent)
				source.transform.SetParent(parent, false);

			source.minDistance = minDistance;
			source.maxDistance = maxDistance;
			source.volume = volume;
			source.mute = mute;
			source.clip = clip;
			source.loop = loop && !destroyAfterFinished;
			source.outputAudioMixerGroup = mixer;
			source.spatialize = spatialize;
			source.spatialBlend = minDistance == 0 && maxDistance == 0 ? 0f : 1f;
			source.playOnAwake = playNow;

			if (playNow)
				source.Play();

			if (destroyAfterFinished)
			{
				if (clip)
					UnityEngine.Object.Destroy(source.gameObject, clip.length * 5f);
				else
					UnityEngine.Object.Destroy(source.gameObject);
			}

			return source;
		}
		public static string[] GetEventListeners(UnityEvent unityEvent)
		{
			List<string> result = new List<string>();

			for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
				result.Add(unityEvent.GetPersistentMethodName(i));

			return result.ToArray();
		}
		public static Texture2D TakeScreenshot(Camera camera, Vector2Int size)
		{
			RenderTexture renderTexture = new RenderTexture(size.x, size.y, 24);

			camera.targetTexture = renderTexture;

			Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);

			camera.Render();

			RenderTexture.active = renderTexture;

			texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);

			camera.targetTexture = null;

			RenderTexture.active = null;

			return texture;
		}
		public static Sprite TextureToSprite(Texture texture, float pixelsPerUnit = 100f)
		{
			if (texture is Texture2D)
				return Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), pixelsPerUnit);

			return null;
		}
		public static Vector3 Devide(params Vector3[] vectors)
		{
			Vector3 result = vectors[0];

			for (int i = 1; i < vectors.Length; i++)
			{
				result.x /= vectors[i].x != 0f ? vectors[i].x : 1f;
				result.y /= vectors[i].y != 0f ? vectors[i].y : 1f;
				result.z /= vectors[i].z != 0f ? vectors[i].z : 1f;
			}

			return result;
		}
		public static Vector3 Multiply(params Vector3[] vectors)
		{
			Vector3 result = vectors[0];

			for (int i = 1; i < vectors.Length; i++)
			{
				result.x *= vectors[i].x;
				result.y *= vectors[i].y;
				result.z *= vectors[i].z;
			}

			return result;
		}
		public static Vector3 Average(params Vector3[] vectors)
		{
			if (vectors.Length == 0)
				return Vector3.zero;

			Vector3 result = Vector3.zero;

			result.x = vectors.Average(vector => vector.x);
			result.y = vectors.Average(vector => vector.y);
			result.z = vectors.Average(vector => vector.z);

			return result;
		}
		public static float Average(params float[] floats)
		{
			if (floats.Length == 0)
				return 0f;

			return floats.Average();
		}
		public static float ClampInfinity(float number, float min = 0f)
		{
			return min >= 0f ? Mathf.Max(number, min) : Mathf.Min(number, min);
		}
		public static int ClampInfinity(int number, int min = 0)
		{
			return min >= 0 ? Mathf.Max(number, min) : Mathf.Min(number, min);
		}
		public static bool FindIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
		{
			intersection = Vector3.zero;

			var d = (p2.x - p1.x) * (p4.y - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);

			if (d == 0f)
				return false;

			var u = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / d;
			var v = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / d;

			if (u < 0f || u > 1f || v < 0f || v > 1f)
			{
				return false;
			}

			intersection.x = p1.x + u * (p2.x - p1.x);
			intersection.z = p1.z + u * (p2.z - p1.z);

			return true;
		}
		public static string ParsePath(params string[] path)
		{
			string newPath = string.Join("/", path);

			while (newPath.IndexOf('\\') != -1)
				newPath = newPath.Replace('\\', '/');

			string[] pathArray = newPath.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();

			var index = Array.IndexOf(pathArray, '.');

			while (index != -1)
			{
				pathArray = new ArraySegment<string>(pathArray, index, 1).Array;

				index = Array.IndexOf(pathArray, '.');
			}

			index = Array.IndexOf(pathArray, "..");

			while (index != -1)
			{
				pathArray = new ArraySegment<string>(pathArray, index > 0 ? index - 1 : index, index > 0 ? 2 : 1).Array;

				index = Array.IndexOf(pathArray, "..");
			}

			return string.Join("/", pathArray);
		}
		public static GameObject GetOrCreateGameController()
		{
			GameObject controller = GameObject.Find("_GameController");

			if (!controller)
				controller = GameObject.FindGameObjectWithTag("GameController");

			if (!controller)
				controller = new GameObject("_GameController");

			controller.tag = "GameController";

			return controller;
		}

		#endregion

		#region Modules & Enumerators

		#region Enumerators

		public enum UnitType { Metric, Imperial }
		public enum Units { Distance, DistanceAccurate, DistanceLong, Force, Liquid, Power, Size, SizeAccurate, Speed, Time, TimeAccurate, Torque, Weight }

		#endregion

		#region Modules

		[Serializable]
		public class JsonArray<T>
		{
			#region Variables

			#region  Global Variables

			[SerializeField]
			private T[] items;

			#endregion

			#region Indexers

			public T this[int index]
			{
				get
				{
					return items[index];
				}
			}

			#endregion

			#endregion

			#region Methods

			public T[] ToArray()
			{
				return items;
			}

			#endregion

			#region Constructors

			public JsonArray(T[] array)
			{
				items = array;
			}
		
			#endregion
		}
		[Serializable]
		public struct SerializableVector2
		{
			public float x;
			public float y;

			public SerializableVector2(Vector2 vector)
			{
				x = vector.x;
				y = vector.y;
			}

			public static SerializableVector2 operator *(SerializableVector2 a, float b)
			{
				return new SerializableVector2(new Vector2(a.x, a.y) * b);
			}
			public static implicit operator Vector2(SerializableVector2 vector)
			{
				return new Vector2(vector.x, vector.y);
			}
			public static implicit operator SerializableVector2(Vector2 vector)
			{
				return new SerializableVector2(vector);
			}
		}
		[Serializable]
		public struct SerializableRect
		{
			public float x;
			public float y;
			public float width;
			public float height;
			public SerializableVector2 position;
			public SerializableVector2 size;

			public SerializableRect(Rect rect)
			{
				x = rect.x;
				y = rect.y;
				width = rect.width;
				height = rect.height;
				position = rect.position;
				size = rect.size;
			}
			public static implicit operator Rect(SerializableRect rect)
			{
				return new Rect(rect.x, rect.y, rect.width, rect.height);
			}
			public static implicit operator SerializableRect(Rect rect)
			{
				return new SerializableRect(rect);
			}

			public bool Contains(Vector2 point)
			{
				return new Rect(x, y, width, height).Contains(point);
			}
			public bool Contains(Vector3 point)
			{
				return new Rect(x, y, width, height).Contains(point);
			}
			public bool Contains(Vector3 point, bool allowInverse)
			{
				return new Rect(x, y, width, height).Contains(point, allowInverse);
			}
		}
		[Serializable]
		public struct ColorSheet
		{
			public string name;
			public SerializableColor color;
			public float metallic;
			public float smoothness;

			public ColorSheet(string name)
			{
				this.name = name;
				color = UnityEngine.Color.white;
				metallic = 0f;
				smoothness = .5f;
			}
			public ColorSheet(ColorSheet sheet)
			{
				name = sheet.name;
				color = sheet.color;
				metallic = sheet.metallic;
				smoothness = sheet.smoothness;
			}

			public void SetMaterial(Material material)
			{
				material.SetColor("_BaseColor", color);
				material.SetFloat("_Metallic", metallic);
				material.SetFloat("_Smoothness", smoothness);
				material.SetFloat("_SmoothnessRemapMin", smoothness);
				material.SetFloat("_SmoothnessRemapMax", smoothness);
			}

			public static implicit operator ColorSheet(UnityEngine.Color color)
			{
				return new ColorSheet() { color = color };
			}
			public static implicit operator ColorSheet(Material material)
			{
				if (material.shader != Shader.Find("HDRP/Lit"))
					return null;

				return new ColorSheet()
				{
					color = material.GetColor("_BaseColor"),
					metallic = material.GetFloat("_Metallic"),
					smoothness = material.GetFloat("_Smoothness")
				};
			}
			public static bool operator ==(ColorSheet sheetA, ColorSheet sheetB)
			{
				return sheetA.name == sheetB.name && sheetA.color == sheetB.color && sheetA.metallic == sheetB.metallic && sheetA.smoothness == sheetB.smoothness;
			}
			public static bool operator !=(ColorSheet sheetA, ColorSheet sheetB)
			{
				return !(sheetA == sheetB);
			}

			public override bool Equals(object obj)
			{
				return obj is ColorSheet sheet &&
					name == sheet.name &&
					EqualityComparer<SerializableColor>.Default.Equals(color, sheet.color) &&
					metallic == sheet.metallic &&
					smoothness == sheet.smoothness;
			}
			public override int GetHashCode()
			{
				int hashCode = -383408880;

				hashCode *= -1521134295 + name.GetHashCode();
				hashCode *= -1521134295 + EqualityComparer<SerializableColor>.Default.GetHashCode(color);
				hashCode *= -1521134295 + metallic.GetHashCode();
				hashCode *= -1521134295 + smoothness.GetHashCode();

				return hashCode;
			}
		}
		[Serializable]
		public struct SerializableColor
		{
			public float r;
			public float g;
			public float b;
			public float a;

			public SerializableColor(UnityEngine.Color color)
			{
				r = color.r;
				g = color.g;
				b = color.b;
				a = color.a;
			}

			public static implicit operator UnityEngine.Color(SerializableColor color)
			{
				return new UnityEngine.Color(color.r, color.g, color.b, color.a);
			}
			public static implicit operator SerializableColor(UnityEngine.Color color)
			{
				return new SerializableColor(color);
			}
			public static bool operator ==(SerializableColor colorA, SerializableColor colorB)
			{
				return colorA.r == colorB.r && colorA.g == colorB.g && colorA.b == colorB.b && colorA.a == colorB.a;
			}
			public static bool operator !=(SerializableColor colorA, SerializableColor colorB)
			{
				return !(colorA == colorB);
			}

			public override bool Equals(object obj)
			{
				return
					obj is SerializableColor color &&
					r == color.r &&
					g == color.g &&
					b == color.b &&
					a == color.a;
			}
			public override int GetHashCode()
			{
				var hashCode = -490236692;

				hashCode = hashCode * -1521134295 + r.GetHashCode();
				hashCode = hashCode * -1521134295 + g.GetHashCode();
				hashCode = hashCode * -1521134295 + b.GetHashCode();
				hashCode = hashCode * -1521134295 + a.GetHashCode();

				return hashCode;
			}
		}
		public static class Color
		{
			public static UnityEngine.Color darkGray = new UnityEngine.Color(.25f, .25f, .25f);
			public static UnityEngine.Color lightGray = new UnityEngine.Color(.67f, .67f, .67f);
			public static UnityEngine.Color orange = new UnityEngine.Color(1f, .5f, 0f);
			public static UnityEngine.Color purple = new UnityEngine.Color(.5f, 0f, 1f);
			public static UnityEngine.Color transparent = new UnityEngine.Color(0f, 0f, 0f, 0f);
		}

		#endregion
		
		#endregion
	}
}
