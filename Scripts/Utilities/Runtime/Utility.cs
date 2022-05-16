#region Namespaces

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

#endregion

namespace Utilities
{
	public static class Extensions
	{
		public static Transform Find(this Transform transform, string name, bool caseSensitive = true)
		{
			for (int i = 0; i < transform.childCount; i++)
				if (!caseSensitive && transform.GetChild(i).name.ToUpper() == name.ToUpper() || transform.GetChild(i).name == name)
					return transform.GetChild(i);

			return null;
		}
		public static Transform FindStartsWith(this Transform transform, string name, bool caseSensitive = true)
		{
			for (int i = 0; i < transform.childCount; i++)
				if (!caseSensitive && transform.GetChild(i).name.ToUpper().StartsWith(name.ToUpper()) || transform.GetChild(i).name.StartsWith(name))
					return transform.GetChild(i);

			return null;
		}
		public static Transform FindEndsWith(this Transform transform, string name, bool caseSensitive = true)
		{
			for (int i = 0; i < transform.childCount; i++)
				if (!caseSensitive && transform.GetChild(i).name.ToUpper().EndsWith(name.ToUpper()) || transform.GetChild(i).name.EndsWith(name))
					return transform.GetChild(i);

			return null;
		}
		public static Transform FindContains(this Transform transform, string name, bool caseSensitive = true)
		{
			for (int i = 0; i < transform.childCount; i++)
				if (!caseSensitive && transform.GetChild(i).name.ToUpper().Contains(name.ToUpper()) || transform.GetChild(i).name.Contains(name))
					return transform.GetChild(i);

			return null;
		}
		public static AnimationCurve Clamp01(this AnimationCurve curve)
		{
			return curve.Clamp(0f, 1f, 0f, 1f);
		}
		public static AnimationCurve Clamp(this AnimationCurve curve, Keyframe min, Keyframe max)
		{
			return curve.Clamp(min.time, max.time, min.value, max.value);
		}
		public static AnimationCurve Clamp(this AnimationCurve curve, float timeMin, float timeMax, float valueMin, float valueMax)
		{
			AnimationCurve newCurve;

			if (curve.length < 2)
			{
				newCurve = curve.Clone();

				if (newCurve.length == 1)
					newCurve.MoveKey(0, new Keyframe(timeMin, valueMin));

				return newCurve;
			}

			float curveTimeMin = curve.keys.Min(key => key.time);
			float curveValueMin = curve.keys.Min(key => key.value);
			float curveTimeMax = curve.keys.Max(key => key.time);
			float curveValueMax = curve.keys.Max(key => key.value);

			if (curveTimeMin == timeMin && curveValueMin == valueMin && curveTimeMax == timeMax && curveValueMax == valueMax)
				return curve;

			newCurve = curve.Clone();

			for (int i = 0; i < curve.length; i++)
			{
				float newKeyTime = Mathf.Lerp(timeMin, timeMax, Mathf.InverseLerp(curveTimeMin, curveTimeMax, curve[i].time));
				float newKeyValue = Mathf.Lerp(valueMin, valueMax, Mathf.InverseLerp(curveValueMin, curveValueMax, curve[i].value));

				newCurve.MoveKey(i, new Keyframe(newKeyTime, newKeyValue));
			}

			return newCurve;
		}
		public static AnimationCurve Clone(this AnimationCurve curve)
		{
			Keyframe[] newKeys = new Keyframe[curve.length];

			curve.keys.CopyTo(newKeys, 0);

			return new AnimationCurve(newKeys)
			{
				postWrapMode = curve.postWrapMode,
				preWrapMode = curve.preWrapMode
			};
		}
		public static T[] ToArray<T>(this Array array)
		{
			T[] newArray = new T[array.Length];

			for (int i = 0; i < array.Length; i++)
				newArray[i] = (T)array.GetValue(i);

			return newArray;
		}
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}
		public static string Join(this string[] strings, string separator)
		{
			return string.Join(separator, strings);
		}
	}
	public static class Utility
	{
		#region Modules & Enumerators

		#region Enumerators

		public enum Precision { Simple, Advanced }
		public enum UnitType { Metric, Imperial }
		public enum Units { Area, AreaAccurate, AreaLarge, ElectricConsumption, Density, Distance, DistanceAccurate, DistanceLong, ElectricCapacity, Force, Frequency, FuelConsumption, Liquid, Power, Pressure, Size, SizeAccurate, Speed, Time, TimeAccurate, Torque, Velocity, Volume, VolumeAccurate, VolumeLarge, Weight }
		public enum RenderPipeline { Standard, URP, HDRP, Custom }
		public enum TextureEncodingType { EXR, JPG, PNG, TGA }
		public enum WorldSide { Left = -1, Center, Right }
		public enum WorldSurface { XY, XZ, YZ }
		public enum Axis2 { X, Y }
		public enum Axis3 { X, Y, Z }
		public enum Axis4 { X, Y, Z, W }

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
				items = array.Distinct().ToArray();
			}

			#endregion
		}
		[Serializable]
		public struct Interval
		{
			#region Variables

			public float Min
			{
				get
				{
					return min;
				}
				set
				{
					min = Mathf.Clamp(value, Mathf.NegativeInfinity, OverrideBorders ? Mathf.Infinity : Max);
				}
			}
			public float Max
			{
				get
				{
					return max;
				}
				set
				{
					max = Mathf.Clamp(value, OverrideBorders ? Mathf.NegativeInfinity : Min, Mathf.Infinity);
				}
			}
			public bool OverrideBorders
			{
				get
				{
					return overrideBorders;
				}
				set
				{
					if (!value)
					{
						min = Mathf.Clamp(min, ClampToZero ? 0f : Mathf.NegativeInfinity, max);
						max = Mathf.Clamp(max, min, Mathf.Infinity);
					}

					overrideBorders = value;
				}
			}
			public bool ClampToZero
			{
				get
				{
					return OverrideBorders && clampToZero;
				}
				set
				{
					clampToZero = value;
					OverrideBorders = overrideBorders;
				}
			}

			[SerializeField]
			private float min;
			[SerializeField]
			private float max;
			[SerializeField]
			private bool overrideBorders;
			[SerializeField]
			private bool clampToZero;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				return obj is Interval interval &&
					   min == interval.min &&
					   max == interval.max &&
					   overrideBorders == interval.overrideBorders &&
					   clampToZero == interval.clampToZero;
			}
			public override int GetHashCode()
			{
				int hashCode = -897720056;

				hashCode = hashCode * -1521134295 + min.GetHashCode();
				hashCode = hashCode * -1521134295 + max.GetHashCode();
				hashCode = hashCode * -1521134295 + overrideBorders.GetHashCode();
				hashCode = hashCode * -1521134295 + clampToZero.GetHashCode();

				return hashCode;
			}

			#endregion

			#region Global Methods

			public bool InRange(float value)
			{
				return value >= min && value <= max;
			}
			public float Lerp(float time, bool clamped = true)
			{
				return clamped ? Mathf.Lerp(Min, Max, time) : Mathf.LerpUnclamped(Min, Max, time);
			}
			public float InverseLerp(float value, bool clamped = true)
			{
				return clamped ? Utility.InverseLerp(Min, Max, value) : InverseLerpUnclamped(Min, Max, value);
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public Interval(float min, float max, bool overrideBorders = false, bool clampToZero = false)
			{
				this.min = Mathf.Clamp(min, clampToZero ? 0f : Mathf.NegativeInfinity, overrideBorders ? Mathf.Infinity : max);
				this.max = Mathf.Clamp(max, overrideBorders ? Mathf.NegativeInfinity : min, Mathf.Infinity);
				this.overrideBorders = overrideBorders;
				this.clampToZero = clampToZero;
			}
			public Interval(Interval interval)
			{
				min = interval.Min;
				max = interval.Max;
				overrideBorders = interval.OverrideBorders;
				clampToZero = interval.clampToZero;
			}

			#endregion

			#region Operators

			public static Interval operator +(Interval a, float b)
			{
				return new Interval(a.min + b, a.max + b);
			}
			public static Interval operator +(Interval a, Interval b)
			{
				return new Interval(a.min + b.min, a.max + b.max);
			}
			public static Interval operator -(Interval a, float b)
			{
				return new Interval(a.min - b, a.max - b);
			}
			public static Interval operator -(Interval a, Interval b)
			{
				return new Interval(a.min - b.min, a.max - b.max);
			}
			public static Interval operator *(Interval a, float b)
			{
				return new Interval(a.min * b, a.max * b);
			}
			public static Interval operator *(Interval a, Interval b)
			{
				return new Interval(a.min * b.min, a.max * b.max);
			}
			public static Interval operator /(Interval a, float b)
			{
				return new Interval(a.min / b, a.max / b);
			}
			public static Interval operator /(Interval a, Interval b)
			{
				return new Interval(a.min / b.min, a.max / b.max);
			}
			public static bool operator ==(Interval a, Interval b)
			{
				return a.Equals(b);
			}
			public static bool operator !=(Interval a, Interval b)
			{
				return !(a == b);
			}

			#endregion

			#endregion

		}
		[Serializable]
		public struct Interval2
		{
			#region Variables

			public Interval x;
			public Interval y;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				return obj is Interval2 interval &&
					   EqualityComparer<Interval>.Default.Equals(x, interval.x) &&
					   EqualityComparer<Interval>.Default.Equals(y, interval.y);
			}
			public override int GetHashCode()
			{
				int hashCode = 1502939027;

				hashCode = hashCode * -1521134295 + x.GetHashCode();
				hashCode = hashCode * -1521134295 + y.GetHashCode();

				return hashCode;
			}

			#endregion

			#region Global Methods

			public bool InRange(float value)
			{
				return x.InRange(value) && y.InRange(value);
			}
			public bool InRange(Vector2 value)
			{
				return x.InRange(value.x) && y.InRange(value.y);
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public Interval2(float xMin, float xMax, float yMin, float yMax)
			{
				x = new Interval(xMin, xMax);
				y = new Interval(yMin, yMax);
			}
			public Interval2(Interval x, Interval y)
			{
				this.x = x;
				this.y = y;
			}
			public Interval2(Interval2 interval)
			{
				x = new Interval(interval.x);
				y = new Interval(interval.y);
			}

			#endregion

			#region Operators

			public static bool operator ==(Interval2 a, Interval2 b)
			{
				return a.Equals(b);
			}
			public static bool operator !=(Interval2 a, Interval2 b)
			{
				return !(a == b);
			}
			public static implicit operator Interval(Interval2 interval)
			{
				return new Interval(interval.x);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public struct Interval3
		{
			#region Variables

			public Interval x;
			public Interval y;
			public Interval z;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				return obj is Interval3 interval &&
					   EqualityComparer<Interval>.Default.Equals(x, interval.x) &&
					   EqualityComparer<Interval>.Default.Equals(y, interval.y) &&
					   EqualityComparer<Interval>.Default.Equals(z, interval.z);
			}
			public override int GetHashCode()
			{
				int hashCode = 373119288;

				hashCode = hashCode * -1521134295 + x.GetHashCode();
				hashCode = hashCode * -1521134295 + y.GetHashCode();
				hashCode = hashCode * -1521134295 + z.GetHashCode();

				return hashCode;
			}

			#endregion

			#region Global Methods

			public bool InRange(float value)
			{
				return x.InRange(value) && y.InRange(value) && z.InRange(value);
			}
			public bool InRange(Vector3 value)
			{
				return x.InRange(value.x) && y.InRange(value.y) && z.InRange(value.z);
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public Interval3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
			{
				x = new Interval(xMin, xMax);
				y = new Interval(yMin, yMax);
				z = new Interval(zMin, zMax);
			}
			public Interval3(Interval x, Interval y, Interval z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
			public Interval3(Interval3 interval)
			{
				x = new Interval(interval.x);
				y = new Interval(interval.y);
				z = new Interval(interval.z);
			}

			#endregion

			#region Operators

			public static bool operator ==(Interval3 a, Interval3 b)
			{
				return a.Equals(b);
			}
			public static bool operator !=(Interval3 a, Interval3 b)
			{
				return !(a == b);
			}
			public static implicit operator Interval2(Interval3 interval)
			{
				return new Interval2(interval.x, interval.y);
			}
			public static implicit operator Interval(Interval3 interval)
			{
				return new Interval(interval.x);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public struct SerializableVector2
		{
			#region Variables

			public float x;
			public float y;

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableVector2(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
			public SerializableVector2(Vector2 vector)
			{
				x = vector.x;
				y = vector.y;
			}

			#endregion

			#region Operators

			public static SerializableVector2 operator *(SerializableVector2 a, float b)
			{
				return new SerializableVector2(new Vector2(a.x, a.y) * b);
			}
			public static SerializableVector2 operator +(SerializableVector2 a, float b)
			{
				return new SerializableVector2(new Vector2(a.x + b, a.y + b));
			}
			public static SerializableVector2 operator *(SerializableVector2 a, SerializableVector2 b)
			{
				return new SerializableVector2(a.x * b.x, a.y * b.y);
			}
			public static SerializableVector2 operator +(SerializableVector2 a, SerializableVector2 b)
			{
				return new SerializableVector2(a.x + b.x, a.y + b.y);
			}
			public static implicit operator Vector2(SerializableVector2 vector)
			{
				return new Vector2(vector.x, vector.y);
			}
			public static implicit operator SerializableVector2(Vector2 vector)
			{
				return new SerializableVector2(vector);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public struct SerializableRect
		{
			#region Variables

			public float x;
			public float y;
			public float width;
			public float height;
			public SerializableVector2 position;
			public SerializableVector2 size;

			#endregion

			#region Methods

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

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableRect(Rect rect)
			{
				x = rect.x;
				y = rect.y;
				width = rect.width;
				height = rect.height;
				position = rect.position;
				size = rect.size;
			}

			#endregion

			#region Operators

			public static implicit operator Rect(SerializableRect rect)
			{
				return new Rect(rect.x, rect.y, rect.width, rect.height);
			}
			public static implicit operator SerializableRect(Rect rect)
			{
				return new SerializableRect(rect);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public struct ColorSheet
		{
			#region Variables

			public string name;
			public SerializableColor color;
			public float metallic;
			public float smoothness;

			#endregion

			#region Methods

			public void SetMaterial(Material material)
			{
				material.SetColor("_BaseColor", color);
				material.SetFloat("_Metallic", metallic);
				material.SetFloat("_Smoothness", smoothness);
				material.SetFloat("_SmoothnessRemapMin", smoothness);
				material.SetFloat("_SmoothnessRemapMax", smoothness);
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

				hashCode = hashCode * -1521134295 + name.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<SerializableColor>.Default.GetHashCode(color);
				hashCode = hashCode * -1521134295 + metallic.GetHashCode();
				hashCode = hashCode * -1521134295 + smoothness.GetHashCode();

				return hashCode;
			}

			#endregion

			#region Constructors & Operators

			#region Constructors

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

			#endregion

			#region Operators

			public static implicit operator ColorSheet(UnityEngine.Color color)
			{
				return new ColorSheet()
				{
					color = color
				};
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

			#endregion

			#endregion
		}
		[Serializable]
		public struct SerializableColor
		{
			#region Variables

			public float r;
			public float g;
			public float b;
			public float a;

			#endregion

			#region Methods

			public override bool Equals(object obj)
			{
				bool equalsColor = obj is SerializableColor color && r == color.r && g == color.g && b == color.b && a == color.a;
				bool equalsUColor = obj is UnityEngine.Color uColor && r == uColor.r && g == uColor.g && b == uColor.b && a == uColor.a;

				return equalsColor || equalsUColor;
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

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableColor(UnityEngine.Color color)
			{
				r = color.r;
				g = color.g;
				b = color.b;
				a = color.a;
			}

			#endregion

			#region Operators

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
				return colorA.Equals(colorB);
			}
			public static bool operator !=(SerializableColor colorA, SerializableColor colorB)
			{
				return !(colorA == colorB);
			}
			public static bool operator ==(SerializableColor colorA, UnityEngine.Color colorB)
			{
				return colorA.Equals(colorB);
			}
			public static bool operator !=(SerializableColor colorA, UnityEngine.Color colorB)
			{
				return !(colorA == colorB);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public class SerializableAudioClip
		{
			#region Variables

			public string resourcePath;
			public AudioClip Clip
			{
				get
				{
					if (!clip || resourcePath != path)
						Reload();

					return clip;
				}
			}

			private AudioClip clip;
			private string path;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				bool equalsClip = obj is SerializableAudioClip clip && clip.Clip == Clip;
				bool equalsUClip = obj is AudioClip uClip && uClip == Clip;

				return equalsClip || equalsUClip;
			}
			public override int GetHashCode()
			{
				return -2053173677 + EqualityComparer<AudioClip>.Default.GetHashCode(Clip);
			}

			#endregion

			#region Global Methods

			private void Reload()
			{
				path = resourcePath;
				clip = Resources.Load(path) as AudioClip;
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableAudioClip(string path)
			{
				resourcePath = path;

				Reload();
			}

			#endregion

			#region Operators

			public static implicit operator bool(SerializableAudioClip audioClip) => audioClip != null;
			public static implicit operator AudioClip(SerializableAudioClip audioClip) => audioClip.Clip;
			public static bool operator ==(SerializableAudioClip clipA, SerializableAudioClip clipB)
			{
				return clipA.Equals(clipB);
			}
			public static bool operator !=(SerializableAudioClip clipA, SerializableAudioClip clipB)
			{
				return !(clipA == clipB);
			}
			public static bool operator ==(SerializableAudioClip clipA, AudioClip clipB)
			{
				return clipA.Equals(clipB);
			}
			public static bool operator !=(SerializableAudioClip clipA, AudioClip clipB)
			{
				return !(clipA == clipB);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public class SerializableParticleSystem
		{
			#region Variables

			public string resourcePath;
			public ParticleSystem Particle
			{
				get
				{
					if (!particle || resourcePath != path)
						Reload();

					return particle;
				}
			}

			private ParticleSystem particle;
			private string path;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				bool equalsParticle = obj is SerializableParticleSystem particle && particle.Particle == Particle;
				bool equalsUParticle = obj is ParticleSystem uParticle && uParticle == Particle;

				return equalsParticle || equalsUParticle;
			}
			public override int GetHashCode()
			{
				return 1500868535 + EqualityComparer<ParticleSystem>.Default.GetHashCode(Particle);
			}

			#endregion

			#region Global Methods

			private void Reload()
			{
				path = resourcePath;
				particle = Resources.Load(path) as ParticleSystem;
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableParticleSystem(string path)
			{
				resourcePath = path;

				Reload();
			}

			#endregion

			#region Operators

			public static implicit operator bool(SerializableParticleSystem particleSystem) => particleSystem != null;
			public static implicit operator ParticleSystem(SerializableParticleSystem particleSystem) => particleSystem.Particle;
			public static bool operator ==(SerializableParticleSystem particleA, SerializableParticleSystem particleB)
			{
				return particleA.Equals(particleB);
			}
			public static bool operator !=(SerializableParticleSystem particleA, SerializableParticleSystem particleB)
			{
				return !(particleA == particleB);
			}
			public static bool operator ==(SerializableParticleSystem particleA, ParticleSystem particleB)
			{
				return particleA.Equals(particleB);
			}
			public static bool operator !=(SerializableParticleSystem particleA, ParticleSystem particleB)
			{
				return !(particleA == particleB);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public class SerializableMaterial
		{
			#region Variables

			public string resourcePath;
			public Material Material
			{
				get
				{
					if (!material || resourcePath != path)
						Reload();

					return material;
				}
			}

			private Material material;
			private string path;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				bool equalsMaterial = obj is SerializableMaterial material && material.Material == Material;
				bool equalsUMaterial = obj is Material uMaterial && uMaterial == Material;

				return equalsMaterial || equalsUMaterial;
			}
			public override int GetHashCode()
			{
				return 1578056576 + EqualityComparer<Material>.Default.GetHashCode(Material);
			}

			#endregion

			#region Global Methods

			private void Reload()
			{
				path = resourcePath;
				material = Resources.Load(path) as Material;
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableMaterial(string path)
			{
				resourcePath = path;

				Reload();
			}

			#endregion

			#region Operators

			public static implicit operator bool(SerializableMaterial material) => material != null;
			public static implicit operator Material(SerializableMaterial material) => material.Material;
			public static bool operator ==(SerializableMaterial materialA, SerializableMaterial materialB)
			{
				return materialA.Equals(materialB);
			}
			public static bool operator !=(SerializableMaterial materialA, SerializableMaterial materialB)
			{
				return !(materialA == materialB);
			}
			public static bool operator ==(SerializableMaterial materialA, Material materialB)
			{
				return materialA.Equals(materialB);
			}
			public static bool operator !=(SerializableMaterial materialA, Material materialB)
			{
				return !(materialA == materialB);
			}

			#endregion

			#endregion
		}
		[Serializable]
		public class SerializableLight
		{
			#region Variables

			public string resourcePath;
			public Light Light
			{
				get
				{
					if (!light || resourcePath != path)
						Reload();

					return light;
				}
			}

			private Light light;
			private string path;

			#endregion

			#region Methods

			#region Virtual Methods

			public override bool Equals(object obj)
			{
				bool equalsLight = obj is SerializableLight light && light.Light == Light;
				bool equalsULight = obj is Light uLight && uLight == Light;

				return equalsLight || equalsULight;
			}
			public override int GetHashCode()
			{
				return 1344377895 + EqualityComparer<Light>.Default.GetHashCode(Light);
			}

			#endregion

			#region Global Methods

			private void Reload()
			{
				path = resourcePath;
				light = Resources.Load(path) as Light;
			}

			#endregion

			#endregion

			#region Constructors & Operators

			#region Constructors

			public SerializableLight(string path)
			{
				resourcePath = path;

				Reload();
			}

			#endregion

			#region Operators

			public static implicit operator bool(SerializableLight light) => light != null;
			public static implicit operator Light(SerializableLight light) => light.Light;
			public static bool operator ==(SerializableLight lightA, SerializableLight lightB)
			{
				return lightA.Equals(lightB);
			}
			public static bool operator !=(SerializableLight lightA, SerializableLight lightB)
			{
				return !(lightA == lightB);
			}
			public static bool operator ==(SerializableLight lightA, Light lightB)
			{
				return lightA.Equals(lightB);
			}
			public static bool operator !=(SerializableLight lightA, Light lightB)
			{
				return !(lightA == lightB);
			}

			#endregion

			#endregion
		}
		public static class Color
		{
			public static UnityEngine.Color darkGray = new UnityEngine.Color(.25f, .25f, .25f);
			public static UnityEngine.Color lightGray = new UnityEngine.Color(.67f, .67f, .67f);
			public static UnityEngine.Color orange = new UnityEngine.Color(1f, .5f, 0f);
			public static UnityEngine.Color purple = new UnityEngine.Color(.5f, 0f, 1f);
			public static UnityEngine.Color transparent = new UnityEngine.Color(0f, 0f, 0f, 0f);
		}
		public static class FormulaInterpolation
		{
			public static float Linear(float t)
			{
				return Mathf.Clamp01(t);
			}
			public static float CircularLowToHigh(float t)
			{
				return Mathf.Clamp01(1f - Mathf.Pow(Mathf.Cos(Mathf.PI * Mathf.Rad2Deg * Mathf.Clamp01(t) * .5f), .5f));
			}
			public static float CircularHighToLow(float t)
			{
				return Mathf.Clamp01(Mathf.Pow(Mathf.Abs(Mathf.Sin(Mathf.PI * Mathf.Rad2Deg * Mathf.Clamp01(t) * .5f)), .5f));
			}
		}

		#endregion

		#endregion

		#region Constants

		public const float airDensity = 1.29f;
		public const string emptyString = "";

		#endregion

		#region Methods

		public static void Dummy()
		{

		}
		public static float UnitMultiplier(Units unit, UnitType unitType)
		{
			switch (unit)
			{
				case Units.Area:
					return unitType == UnitType.Metric ? 1f : 10.7639f;

				case Units.AreaAccurate:
					return unitType == UnitType.Metric ? 1f : 0.155f;

				case Units.AreaLarge:
					return unitType == UnitType.Metric ? 1f : 1f / 2.59f;

				case Units.ElectricConsumption:
					return unitType == UnitType.Metric ? 1f : 1f / 1.609f;

				case Units.Density:
					return unitType == UnitType.Metric ? 1f : 0.06242796f;

				case Units.Distance:
					return unitType == UnitType.Metric ? 1f : 3.28084f;

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? 1f : 1.09361f;

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? 1f : 0.621371f;

				case Units.FuelConsumption:
					return unitType == UnitType.Metric ? 1f : 235.215f;

				case Units.Liquid:
					return unitType == UnitType.Metric ? 1f : 1f / 4.546f;

				case Units.Power:
					return unitType == UnitType.Metric ? 1f : 0.7457f;

				case Units.Pressure:
					return unitType == UnitType.Metric ? 1f : 14.503773773f;

				case Units.Size:
					return unitType == UnitType.Metric ? 1f : 1f / 2.54f;

				case Units.Speed:
					return unitType == UnitType.Metric ? 1f : 0.621371f;

				case Units.Torque:
					return unitType == UnitType.Metric ? 1f : 0.73756f;

				case Units.Velocity:
					return unitType == UnitType.Metric ? 1f : 3.28084f;

				case Units.Volume:
					return unitType == UnitType.Metric ? 1f : 35.3147f;

				case Units.VolumeAccurate:
					return unitType == UnitType.Metric ? 1f : 1f / 16.3871f;

				case Units.VolumeLarge:
					return unitType == UnitType.Metric ? 1f : 1f / 4.16818f;

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
				case Units.Area:
					return unitType == UnitType.Metric ? "m²" : "ft²";

				case Units.AreaAccurate:
					return unitType == UnitType.Metric ? "cm²" : "in²";

				case Units.AreaLarge:
					return unitType == UnitType.Metric ? "km²" : "mi²";

				case Units.ElectricConsumption:
					return unitType == UnitType.Metric ? "kW⋅h/100km" : "kW⋅h/100m";

				case Units.Density:
					return unitType == UnitType.Metric ? "kg/m³" : "lbᵐ/ft³";

				case Units.Distance:
					return unitType == UnitType.Metric ? "m" : "ft";

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? "m" : "yd";

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? "km" : "mi";

				case Units.ElectricCapacity:
					return unitType == UnitType.Metric ? "kW⋅h" : "kW⋅h";

				case Units.Force:
					return unitType == UnitType.Metric ? "N" : "N";

				case Units.Frequency:
					return unitType == UnitType.Metric ? "Hz" : "Hz";

				case Units.FuelConsumption:
					return unitType == UnitType.Metric ? "L/100km" : "mpg";

				case Units.Liquid:
					return unitType == UnitType.Metric ? "L" : "gal";

				case Units.Power:
					return unitType == UnitType.Metric ? "hp" : "kW";

				case Units.Pressure:
					return unitType == UnitType.Metric ? "bar" : "psi";

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
					return unitType == UnitType.Metric ? "N⋅m" : "ft-lb";

				case Units.Velocity:
					return unitType == UnitType.Metric ? "m/s" : "ft/s";

				case Units.Volume:
					return unitType == UnitType.Metric ? "m³" : "ft³";

				case Units.VolumeAccurate:
					return unitType == UnitType.Metric ? "cm³" : "in³";

				case Units.VolumeLarge:
					return unitType == UnitType.Metric ? "km³" : "mi³";

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
				case Units.Area:
					return unitType == UnitType.Metric ? "Square Metre" : "Square Feet";

				case Units.AreaAccurate:
					return unitType == UnitType.Metric ? "Square Centimetre" : "Square Inch";

				case Units.AreaLarge:
					return unitType == UnitType.Metric ? "Square Kilometre" : "Square Mile";

				case Units.ElectricConsumption:
					return unitType == UnitType.Metric ? "KiloWatt Hour per 100 Kilometres" : "KiloWatt Hour per 100 Miles";

				case Units.Density:
					return unitType == UnitType.Metric ? "Kilogram per Cubic Metre" : "Pound-mass per Cubic Foot";

				case Units.Distance:
					return unitType == UnitType.Metric ? "Metre" : "Foot";

				case Units.DistanceAccurate:
					return unitType == UnitType.Metric ? "Metre" : "Yard";

				case Units.DistanceLong:
					return unitType == UnitType.Metric ? "Kilometre" : "Mile";

				case Units.ElectricCapacity:
					return unitType == UnitType.Metric ? "KiloWatt Hour" : "KiloWatt Hour";

				case Units.Force:
					return unitType == UnitType.Metric ? "Newton" : "Newton";

				case Units.Frequency:
					return unitType == UnitType.Metric ? "Hertz" : "Hertz";

				case Units.FuelConsumption:
					return unitType == UnitType.Metric ? "Liters per 100 Kilometre" : "Miles per Galon";

				case Units.Liquid:
					return unitType == UnitType.Metric ? "Litre" : "Gallon";

				case Units.Power:
					return unitType == UnitType.Metric ? "Horsepower" : "KiloWatt";

				case Units.Pressure:
					return unitType == UnitType.Metric ? "Bar" : "Pounds per Square Inch";

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
					return unitType == UnitType.Metric ? "Newton⋅Metre" : "Pound⋅Feet";

				case Units.Velocity:
					return unitType == UnitType.Metric ? "Metres per Second" : "Feet per Second";

				case Units.Volume:
					return unitType == UnitType.Metric ? "Cubic Metre" : "Cubic Foot";

				case Units.VolumeAccurate:
					return unitType == UnitType.Metric ? "Cubic Centimetre" : "Cubic Inch";

				case Units.VolumeLarge:
					return unitType == UnitType.Metric ? "Cubic Kilometre" : "Cubic Mile";

				case Units.Weight:
					return unitType == UnitType.Metric ? "Kilogram" : "Pound";

				default:
					return default;
			}
		}
		public static float ValueWithUnitToNumber(string value)
		{
			string[] valueArray = value.Split(' ');

			value = "";

			for (int i = 0; i < valueArray.Length; i++)
				if (float.TryParse(valueArray[i], out float number))
					value += number + (i < valueArray.Length - 1 ? " " : "");

			return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && float.TryParse(value, out float result) ? result : 0f;
		}
		public static float ValueWithUnitToNumber(string value, Units unit, UnitType unitType)
		{
			string[] valueArray = value.Split(' ');

			value = "";

			for (int i = 0; i < valueArray.Length; i++)
				if (float.TryParse(valueArray[i], out float number))
					value += number + (i < valueArray.Length - 1 ? " " : "");

			return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && float.TryParse(value, out float result) ? (IsDeviderUnit(unit) && unitType != UnitType.Metric ? UnitMultiplier(unit, unitType) / result : result / UnitMultiplier(unit, unitType)) : 0f;
		}
		public static string NumberToValueWithUnit(float number, string unit, bool rounded)
		{
			if (number == Mathf.Infinity)
				return "Infinity";
			else if (number == Mathf.NegativeInfinity || number == -Mathf.Infinity)
				return "-Infinity";

			return $"{(rounded ? Mathf.Round(number) : number)} {unit}";
		}
		public static string NumberToValueWithUnit(float number, string unit, uint decimals)
		{
			if (number == Mathf.Infinity)
				return "Infinity";
			else if (number == Mathf.NegativeInfinity || number == -Mathf.Infinity)
				return "-Infinity";

			return $"{Round(number, decimals)} {unit}";
		}
		public static string NumberToValueWithUnit(float number, Units unit, UnitType unitType, bool rounded)
		{
			if (number == Mathf.Infinity)
				return "Infinity";
			else if (number == Mathf.NegativeInfinity)
				return "-Infinity";

			if (IsDeviderUnit(unit) && unitType != UnitType.Metric)
				number = UnitMultiplier(unit, unitType) / number;
			else
				number *= UnitMultiplier(unit, unitType);

			return $"{(rounded ? Mathf.Round(number) : number)} {Unit(unit, unitType)}";
		}
		public static string NumberToValueWithUnit(float number, Units unit, UnitType unitType, uint decimals)
		{
			if (number == Mathf.Infinity)
				return "Infinity";
			else if (number == Mathf.NegativeInfinity || number == -Mathf.Infinity)
				return "-Infinity";

			if (IsDeviderUnit(unit) && unitType != UnitType.Metric)
				number = UnitMultiplier(unit, unitType) / number;
			else
				number *= UnitMultiplier(unit, unitType);

			return $"{Round(number, decimals)} {Unit(unit, unitType)}";
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
					return Round(slip, 2) != 0f ? (refMaterial.dynamicFriction + material.dynamicFriction) * .5f : (refMaterial.staticFriction + material.staticFriction) * .5f;

				case PhysicMaterialCombine.Multiply:
					return Round(slip, 2) != 0f ? refMaterial.dynamicFriction * material.dynamicFriction : refMaterial.staticFriction * material.staticFriction;

				case PhysicMaterialCombine.Minimum:
					return Round(slip, 2) != 0f ? Mathf.Min(refMaterial.dynamicFriction, material.dynamicFriction) : Mathf.Max(refMaterial.staticFriction, material.staticFriction);

				case PhysicMaterialCombine.Maximum:
					return Round(slip, 2) != 0f ? Mathf.Max(refMaterial.dynamicFriction, material.dynamicFriction) : Mathf.Max(refMaterial.staticFriction, material.staticFriction);

				default:
					return 0f;
			}
		}
		public static float EvaluateFriction(float slip, PhysicMaterial refMaterial, float stiffness)
		{
			switch (refMaterial.frictionCombine)
			{
				case PhysicMaterialCombine.Average:
					return Round(slip, 2) != 0f ? (refMaterial.dynamicFriction + stiffness) * .5f : (refMaterial.staticFriction + stiffness) * .5f;

				case PhysicMaterialCombine.Multiply:
					return Round(slip, 2) != 0f ? refMaterial.dynamicFriction * stiffness : refMaterial.staticFriction * stiffness;

				case PhysicMaterialCombine.Minimum:
					return Round(slip, 2) != 0f ? Mathf.Min(refMaterial.dynamicFriction, stiffness) : Mathf.Max(refMaterial.staticFriction, stiffness);

				case PhysicMaterialCombine.Maximum:
					return Round(slip, 2) != 0f ? Mathf.Max(refMaterial.dynamicFriction, stiffness) : Mathf.Max(refMaterial.staticFriction, stiffness);

				default:
					return 0f;
			}
		}
		public static float BrakingDistance(float speed, float friction)
		{
			friction = InverseLerpUnclamped(1f, 1.5f, friction);

			return ClampInfinity(Mathf.LerpUnclamped(Mathf.LerpUnclamped(30f, 26f, friction), Mathf.LerpUnclamped(143f, 113f, friction), InverseLerpUnclamped(40f, 110f, speed)));
		}
		public static float RPMToSpeed(float rpm, float radius)
		{
			return radius * .377f * rpm;
		}
		public static float SpeedToRPM(float speed, float radius)
		{
			if (radius <= 0f)
				return speed / .377f;

			return speed / radius / .377f;
		}
		public static bool MaskHasLayer(LayerMask mask, int layer)
		{
			return MaskHasLayer(mask.value, layer);
		}
		public static bool MaskHasLayer(int mask, int layer)
		{
			return (mask & 1 << layer) != 0;
		}
		public static bool MaskHasLayer(LayerMask mask, string layer)
		{
			return MaskHasLayer(mask.value, layer);
		}
		public static bool MaskHasLayer(int mask, string layer)
		{
			return MaskHasLayer(mask, LayerMask.NameToLayer(layer));
		}
		public static int ExclusiveMask(string name)
		{
			return ExclusiveMask(new string[] { name });
		}
		public static int ExclusiveMask(int layer)
		{
			return ExclusiveMask(new int[] { layer });
		}
		public static int ExclusiveMask(params string[] layers)
		{
			return ~LayerMask.GetMask(layers);
		}
		public static int ExclusiveMask(params int[] layers)
		{
			return ExclusiveMask(layers.Select(layer => LayerMask.LayerToName(layer)).ToArray());
		}
		public static int BoolToNumber(bool condition)
		{
			return condition ? 1 : 0;
		}
		public static float BoolToNumber(float source, bool condition, float damping = 2.5f)
		{
			return Mathf.MoveTowards(source, BoolToNumber(condition), Time.deltaTime * damping);
		}
		public static int InvertSign(bool invert)
		{
			return invert ? -1 : 1;
		}
		public static bool NumberToBool(float number)
		{
			return Mathf.Clamp01(Mathf.RoundToInt(number)) != 0f;
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
		public static bool IsAlphabet(char c)
		{
			c = char.ToUpper(c);

			return c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F' || c == 'G' || c == 'H' || c == 'I' || c == 'J' || c == 'K' || c == 'L'
				|| c == 'M' || c == 'N' || c == 'O' || c == 'P' || c == 'Q' || c == 'R' || c == 'S' || c == 'T' || c == 'U' || c == 'V' || c == 'W' || c == 'X'
				|| c == 'Y' || c == 'Z';
		}
		public static bool IsNumber(char c)
		{
			return c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9';
		}
		public static bool IsSymbol(char c)
		{
			return !IsAlphabet(c) && !IsNumber(c);
		}
		public static string RandomString(int length, bool upperChars = true, bool lowerChars = true, bool numbers = true, bool symbols = true)
		{
			string chars = "";

			chars += upperChars ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : "";
			chars += lowerChars ? "abcdefghijklmnopqrstuvwxyz" : "";
			chars += numbers ? "0123456789" : "";
			chars += symbols ? "!@#$%^&()_+-{}[],.;" : "";

			return new string(Enumerable.Repeat(chars, length).Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
		}
		public static string GetReadableSize(long length)
		{
			string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB" };
			float size = length;
			int index = 0;

			while (size >= 1024f && index < sizes.Length - 1)
			{
				size /= 1024f;

				index++;
			}

			return $"{Round(size, 2):0.00} {sizes[index]}";
		}
		public static void DrawArrowForGizmos(Vector3 pos, Vector3 direction, float arrowHeadLength = .25f, float arrowHeadAngle = 20f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * Vector3.forward;
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * Vector3.forward;

			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}
		public static void DrawArrowForGizmos(Vector3 pos, Vector3 direction, UnityEngine.Color color, float arrowHeadLength = .25f, float arrowHeadAngle = 20f)
		{
			UnityEngine.Color orgColor = Gizmos.color;

			Gizmos.color = color;

			DrawArrowForGizmos(pos, direction, arrowHeadLength, arrowHeadAngle);

			Gizmos.color = orgColor;
		}
		public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = .25f, float arrowHeadAngle = 20f)
		{
			DrawArrowForDebug(pos, direction, UnityEngine.Color.white, arrowHeadLength, arrowHeadAngle);
		}
		public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, UnityEngine.Color color, float arrowHeadLength = .25f, float arrowHeadAngle = 20f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * Vector3.forward;
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * Vector3.forward;

			Debug.DrawRay(pos, direction, color);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
		public static void DrawBoundsForDebug(Bounds bounds)
		{
			// bottom
			Vector3 p1 = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
			Vector3 p2 = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
			Vector3 p3 = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
			Vector3 p4 = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

			Debug.DrawLine(p1, p2, UnityEngine.Color.blue);
			Debug.DrawLine(p2, p3, UnityEngine.Color.red);
			Debug.DrawLine(p3, p4, UnityEngine.Color.yellow);
			Debug.DrawLine(p4, p1, UnityEngine.Color.magenta);

			// top
			Vector3 p5 = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
			Vector3 p6 = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
			Vector3 p7 = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
			Vector3 p8 = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);

			Debug.DrawLine(p5, p6, UnityEngine.Color.blue);
			Debug.DrawLine(p6, p7, UnityEngine.Color.red);
			Debug.DrawLine(p7, p8, UnityEngine.Color.yellow);
			Debug.DrawLine(p8, p5, UnityEngine.Color.magenta);

			// sides
			Debug.DrawLine(p1, p5, UnityEngine.Color.white);
			Debug.DrawLine(p2, p6, UnityEngine.Color.gray);
			Debug.DrawLine(p3, p7, UnityEngine.Color.green);
			Debug.DrawLine(p4, p8, UnityEngine.Color.cyan);
		}
		public static bool PointInTriangle(Vector2 point, Vector2 point1, Vector2 point2, Vector2 point3)
		{
			float d1, d2, d3;
			bool isNegative, isPositive;

			d1 = PointSign(point, point1, point2);
			d2 = PointSign(point, point2, point3);
			d3 = PointSign(point, point3, point1);

			isNegative = (d1 < 0) || (d2 < 0) || (d3 < 0);
			isPositive = (d1 > 0) || (d2 > 0) || (d3 > 0);

			return !(isNegative && isPositive);
		}
		public static Vector3 PointFromCircle(Vector3 center, float radius, float angle, WorldSurface surface, Quaternion rotation)
		{
			Vector3 newPosition = Vector3.zero;

			switch (surface)
			{
				case WorldSurface.XY:
					newPosition.x += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
					newPosition.y += radius * Mathf.Cos(angle * Mathf.Deg2Rad);
					break;

				case WorldSurface.XZ:
					newPosition.x += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
					newPosition.z += radius * Mathf.Cos(angle * Mathf.Deg2Rad);
					break;

				case WorldSurface.YZ:
					newPosition.y += radius * Mathf.Cos(angle * Mathf.Deg2Rad);
					newPosition.z += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
					break;
			}

			return center + rotation * newPosition;
		}
		public static Vector3 PointFromLine(Vector3 start, Vector3 direction, float length, float position)
		{
			return start + length * position * direction;
		}
		public static Vector3 Abs(Vector3 vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}
		public static Vector3 VectorsOffset(Vector3 a, Vector3 b, bool abs = false)
		{
			Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

			return abs ? Abs(vector) : vector;
		}
		public static Vector3 Round(Vector3 vector)
		{
			return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
		}
		public static Vector3 Round(Vector3 vector, uint decimals)
		{
			return new Vector3(Round(vector.x, decimals), Round(vector.y, decimals), Round(vector.z, decimals));
		}
		public static Vector2 Round(Vector2 vector)
		{
			return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
		}
		public static Vector2 Round(Vector2 vector, uint decimals)
		{
			return new Vector2(Round(vector.x, decimals), Round(vector.y, decimals));
		}
		public static Vector3Int RoundToInt(Vector3 vector)
		{
			return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
		}
		public static Vector2Int RoundToInt(Vector2 vector)
		{
			return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
		}
		public static float Round(float number, uint decimals)
		{
			float multiplier = Mathf.Pow(10, decimals);

			return Mathf.Round(number * multiplier) / multiplier;
		}
		public static Vector3 Direction(Vector3 origin, Vector3 destination)
		{
			return (destination - origin).normalized;
		}
		public static Vector3 DirectionUnNormalized(Vector3 origin, Vector3 destination)
		{
			return destination - origin;
		}
		public static Vector3 DirectionRight(Vector3 forward, Vector3 up)
		{
			return Quaternion.AngleAxis(90f, up) * forward;
		}
		public static WorldSide GetPointSideCompared(Vector3 point, Vector3 comparingPoint, Vector3 comparingForward, Vector3 comparingUp)
		{
			Vector3 pointForward = Direction(comparingPoint, point);
			float compareAngle = Vector3.SignedAngle(comparingForward, pointForward, comparingUp);

			if (compareAngle > 0f)
				return WorldSide.Right;
			else if (compareAngle < 0f)
				return WorldSide.Left;
			else
				return WorldSide.Center;
		}
		public static float AngleAroundAxis(Vector3 direction, Vector3 axis, Vector3 forward)
		{
			Vector3 right = Vector3.Cross(axis, forward).normalized;

			forward = Vector3.Cross(right, axis).normalized;

			return Mathf.Atan2(Vector3.Dot(direction, right), Vector3.Dot(direction, forward)) * Mathf.Rad2Deg;
		}
		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 t)
		{
			return Mathf.Clamp01(InverseLerpUnclamped(a, b, t));
		}
		public static float InverseLerpUnclamped(Vector3 a, Vector3 b, Vector3 t)
		{
			Vector3 AB = b - a;
			Vector3 AT = t - a;

			return Vector3.Dot(AT, AB) / Vector3.Dot(AB, AB);
		}
		public static float InverseLerp(float a, float b, float t)
		{
			return Mathf.Clamp01(InverseLerpUnclamped(a, b, t));
		}
		public static float InverseLerpUnclamped(float a, float b, float t)
		{
			return (t - a) / (b - a);
		}
		public static UnityEngine.Color MoveTowards(UnityEngine.Color a, UnityEngine.Color b, float maxDelta)
		{
			return new UnityEngine.Color()
			{
				r = Mathf.MoveTowards(a.r, b.r, maxDelta),
				g = Mathf.MoveTowards(a.g, b.g, maxDelta),
				b = Mathf.MoveTowards(a.b, b.b, maxDelta),
				a = Mathf.MoveTowards(a.a, b.a, maxDelta)
			};
		}
		public static UnityEngine.Color MoveTowards(UnityEngine.Color a, UnityEngine.Color b, UnityEngine.Color maxDelta)
		{
			return new UnityEngine.Color()
			{
				r = Mathf.MoveTowards(a.r, b.r, maxDelta.r),
				g = Mathf.MoveTowards(a.g, b.g, maxDelta.g),
				b = Mathf.MoveTowards(a.b, b.b, maxDelta.b),
				a = Mathf.MoveTowards(a.a, b.a, maxDelta.a)
			};
		}
		public static float Distance(params Vector3[] vectors)
		{
			float distance = 0f;

			for (int i = 0; i < vectors.Length - 1; i++)
				distance += Distance(vectors[i], vectors[i + 1]);

			return distance;
		}
		public static float Distance(Vector3 a, Vector3 b)
		{
			return (a - b).magnitude;
		}
		public static float DistanceSqr(Vector3 a, Vector3 b)
		{
			return (a - b).sqrMagnitude;
		}
		public static float Distance(float a, float b)
		{
			return Mathf.Max(a, b) - Mathf.Min(a, b);
		}
		public static float Velocity(float current, float last, float deltaTime)
		{
			return (last - current) / deltaTime;
		}
		public static Vector3 Velocity(Vector3 current, Vector3 last, float deltaTime)
		{
			return Devide(last - current, deltaTime);
		}
		public static float LoopNumber(float number, float after)
		{
			while (number >= after)
				number -= after;

			while (number < 0)
				number += after;

			return number;
		}
		public static bool IsDownFromLastState(float current, float last)
		{
			return IsDownFromLastState(NumberToBool(current), NumberToBool(last));
		}
		public static bool IsDownFromLastState(int current, int last)
		{
			return IsDownFromLastState(NumberToBool(current), NumberToBool(last));
		}
		public static bool IsDownFromLastState(bool current, bool last)
		{
			return !last && current;
		}
		public static bool IsUpFromLastState(float current, float last)
		{
			return IsUpFromLastState(NumberToBool(current), NumberToBool(last));
		}
		public static bool IsUpFromLastState(int current, int last)
		{
			return IsUpFromLastState(NumberToBool(current), NumberToBool(last));
		}
		public static bool IsUpFromLastState(bool current, bool last)
		{
			return last && !current;
		}
		public static Vector3 Devide(params Vector3[] vectors)
		{
			return Devide(vectors as IEnumerable<Vector3>);
		}
		public static Vector3 Devide(IEnumerable<Vector3> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			Vector3 result = vectors.FirstOrDefault();

			for (int i = 1; i < vectors.Count(); i++)
			{
				result.x /= vectors.ElementAt(i).x != 0f ? vectors.ElementAt(i).x : 1f;
				result.y /= vectors.ElementAt(i).y != 0f ? vectors.ElementAt(i).y : 1f;
				result.z /= vectors.ElementAt(i).z != 0f ? vectors.ElementAt(i).z : 1f;
			}

			return result;
		}
		public static Vector3 Devide(Vector3 vector, float devider)
		{
			if (devider == 0f)
				return default;

			return new Vector3(vector.x / devider, vector.y / devider, vector.z / devider);
		}
		public static Vector3 Multiply(params Vector3[] vectors)
		{
			return Multiply(vectors as IEnumerable<Vector3>);
		}
		public static Vector3 Multiply(IEnumerable<Vector3> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			Vector3 result = vectors.FirstOrDefault();

			for (int i = 1; i < vectors.Count(); i++)
			{
				result.x *= vectors.ElementAt(i).x;
				result.y *= vectors.ElementAt(i).y;
				result.z *= vectors.ElementAt(i).z;
			}

			return result;
		}
		public static Vector3 Average(params Vector3[] vectors)
		{
			return Average(vectors as IEnumerable<Vector3>);
		}
		public static Vector3 Average(IEnumerable<Vector3> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			return new Vector3()
			{
				x = vectors.Average(vector => vector.x),
				y = vectors.Average(vector => vector.y),
				z = vectors.Average(vector => vector.z)
			};
		}
		public static Vector2 Average(params Vector2[] vectors)
		{
			return Average(vectors as IEnumerable<Vector2>);
		}
		public static Vector2 Average(IEnumerable<Vector2> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			return new Vector2()
			{
				x = vectors.Average(vector => vector.x),
				y = vectors.Average(vector => vector.y)
			};
		}
		public static Vector3Int Average(params Vector3Int[] vectors)
		{
			return Average(vectors as IEnumerable<Vector3Int>);
		}
		public static Vector3Int Average(IEnumerable<Vector3Int> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			return RoundToInt(new Vector3()
			{
				x = (float)vectors.Average(vector => vector.x),
				y = (float)vectors.Average(vector => vector.y),
				z = (float)vectors.Average(vector => vector.z)
			});
		}
		public static Vector2Int Average(params Vector2Int[] vectors)
		{
			return Average(vectors as IEnumerable<Vector2Int>);
		}
		public static Vector2Int Average(IEnumerable<Vector2Int> vectors)
		{
			if (vectors.Count() < 1)
				return default;

			return RoundToInt(new Vector2()
			{
				x = (float)vectors.Average(vector => vector.x),
				y = (float)vectors.Average(vector => vector.y)
			});
		}
		public static Quaternion Average(params Quaternion[] quaternions)
		{
			return Average(quaternions as IEnumerable<Quaternion>);
		}
		public static Quaternion Average(IEnumerable<Quaternion> quaternions)
		{
			if (quaternions.Count() < 1)
				return default;

			Quaternion average = quaternions.FirstOrDefault();
			float weight;

			for (int i = 1; i < quaternions.Count(); i++)
			{
				weight = 1f / (i + 1);
				average = Quaternion.Slerp(average, quaternions.ElementAt(i), weight);
			}

			return average;
		}
		public static float Average(params float[] floats)
		{
			if (floats.Length < 1)
				return 0f;

			return floats.Average();
		}
		public static byte Average(params byte[] bytes)
		{
			if (bytes.Length < 1)
				return 0;

			return (byte)Mathf.RoundToInt(bytes.Select(@byte => (float)@byte).Average());
		}
		public static int Average(params int[] ints)
		{
			if (ints.Length < 1)
				return 0;

			return Mathf.RoundToInt((float)ints.Average());
		}
		public static int Square(int number)
		{
			return number * number;
		}
		public static float Square(float number)
		{
			return number * number;
		}
		public static byte Max(params byte[] numbers)
		{
			if (numbers.Length < 1)
				return default;

			byte max = numbers[0];

			for (int i = 1; i < numbers.Length; i++)
				if (numbers[i] > max)
					max = numbers[i];

			return max;
		}
		public static byte Min(params byte[] numbers)
		{
			if (numbers.Length < 1)
				return default;

			byte min = numbers[0];

			for (int i = 1; i < numbers.Length; i++)
				if (min > numbers[i])
					min = numbers[i];

			return min;
		}
		public static float ClampInfinity(float number, float min = 0f)
		{
			return min >= 0f ? Mathf.Max(number, min) : Mathf.Min(number, min);
		}
		public static int ClampInfinity(int number, int min = 0)
		{
			return min >= 0 ? Mathf.Max(number, min) : Mathf.Min(number, min);
		}
		public static Vector3 ClampInfinity(Vector3 vector, float min = 0f)
		{
			return min >= 0f ? new Vector3(Mathf.Max(vector.x, min), Mathf.Max(vector.y, min), Mathf.Max(vector.z, min)) : new Vector3(Mathf.Min(vector.x, min), Mathf.Min(vector.y, min), Mathf.Min(vector.z, min));
		}
		public static Vector3 ClampInfinity(Vector3 vector, Vector3 min)
		{
			return Utility.Average(min.x, min.y, min.z) >= 0f ? new Vector3(Mathf.Max(vector.x, min.x), Mathf.Max(vector.y, min.y), Mathf.Max(vector.z, min.z)) : new Vector3(Mathf.Min(vector.x, min.x), Mathf.Min(vector.y, min.y), Mathf.Min(vector.z, min.z));
		}
		public static bool Approximately(Vector3 vector1, Vector3 vector2)
		{
			return Mathf.Approximately(vector1.x, vector2.x) && Mathf.Approximately(vector1.y, vector2.y) && Mathf.Approximately(vector1.z, vector2.z);
		}
		public static long GetTimestamp(DateTime dateTime)
		{
			return long.Parse(dateTime.ToString("yyyyMMddHHmmssffff"));
		}
		public static long GetTimestamp(bool UTC = false)
		{
			return GetTimestamp(UTC ? DateTime.UtcNow : DateTime.Now);
		}
		public static bool FindIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
		{
			intersection = Vector3.zero;

			var d = (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);

			if (d == 0f)
				return false;

			var u = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / d;
			var v = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / d;

			if (u < 0f || u > 1f || v < 0f || v > 1f)
				return false;

			intersection.x = p1.x + u * (p2.x - p1.x);
			intersection.z = p1.z + u * (p2.z - p1.z);

			return true;
		}
		public static void AddTorqueAtPosition(Rigidbody rigid, Vector3 torque, Vector3 point, ForceMode mode)
		{
			rigid.AddForceAtPosition(.5f * torque.y * Vector3.forward, point + Vector3.left, mode);
			rigid.AddForceAtPosition(.5f * torque.y * Vector3.back, point + Vector3.right, mode);
			rigid.AddForceAtPosition(.5f * torque.x * Vector3.forward, point + Vector3.up, mode);
			rigid.AddForceAtPosition(.5f * torque.x * Vector3.back, point + Vector3.down, mode);
			rigid.AddForceAtPosition(.5f * torque.z * Vector3.right, point + Vector3.up, mode);
			rigid.AddForceAtPosition(.5f * torque.z * Vector3.left, point + Vector3.down, mode);
		}
		public static string ParsePath(params string[] path)
		{
			string newPath = string.Join(Path.AltDirectorySeparatorChar.ToString(), path);

			while (newPath.IndexOf(Path.AltDirectorySeparatorChar) > -1)
				newPath = newPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			string[] pathArray = newPath.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)).ToArray();

			var index = Array.IndexOf(pathArray, '.');

			while (index > -1)
			{
				pathArray = new ArraySegment<string>(pathArray, index, 1).Array;

				index = Array.IndexOf(pathArray, '.');
			}

			index = Array.IndexOf(pathArray, "..");

			while (index > -1)
			{
				pathArray = new ArraySegment<string>(pathArray, index > 0 ? index - 1 : index, index > 0 ? 2 : 1).Array;

				index = Array.IndexOf(pathArray, "..");
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(), pathArray);
		}
		public static bool IsDriectoryEmpty(string path)
		{
			IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);

#pragma warning disable IDE0063 // Use simple 'using' statement
			using (IEnumerator<string> entry = items.GetEnumerator())
				return !entry.MoveNext();
#pragma warning restore IDE0063 // Use simple 'using' statement
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
					Destroy(source.gameObject, clip.length * 5f);
				else
					Destroy(false, source.gameObject);
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
		public static T CloneComponent<T>(T original, GameObject destination) where T : Component
		{
			Type type = typeof(T);
			T target = destination.AddComponent<T>();
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			PropertyInfo[] pinfos = type.GetProperties(flags);

			foreach (var pinfo in pinfos)
				if (pinfo.CanWrite)
					try
					{
						pinfo.SetValue(target, pinfo.GetValue(original, null), null);
					}
					catch { }

			FieldInfo[] finfos = type.GetFields(flags);

			foreach (var finfo in finfos)
				try
				{
					finfo.SetValue(target, finfo.GetValue(original));
				}
				catch { }

			return target as T;
		}
		public static Texture2D GetTextureArrayItem(Texture2DArray array, int index)
		{
			bool isReadable = array.isReadable;

			if (!isReadable)
				array.Apply(false, false);

			Texture2D texture = new Texture2D(array.width, array.height, array.format, array.mipMapBias > 0f);

			texture.SetPixels32(array.GetPixels32(index));
			texture.Apply();

			if (!isReadable)
				array.Apply(false, true);

			return texture;
		}
		public static Texture2D[] GetTextureArrayItems(Texture2DArray array)
		{
			bool isReadable = array.isReadable;

			if (!isReadable)
				array.Apply(false, false);

			Texture2D[] textures = new Texture2D[array.depth];

			for (int i = 0; i < array.depth; i++)
			{
				textures[i] = new Texture2D(array.width, array.height, array.format, array.mipMapBias > 0f);

				textures[i].SetPixels32(array.GetPixels32(i));
				textures[i].Apply(true, false);
			}

			if (!isReadable)
				array.Apply(false, true);

			return textures;
		}
		public static void SaveTexture2D(Texture2D texture, TextureEncodingType type, string path)
		{
			if (!texture)
				return;

			byte[] bytes;

			switch (type)
			{
				case TextureEncodingType.EXR:
					bytes = texture.EncodeToEXR();
					break;

				case TextureEncodingType.JPG:
					bytes = texture.EncodeToJPG();
					break;

				case TextureEncodingType.TGA:
					bytes = texture.EncodeToTGA();
					break;

				default:
					bytes = texture.EncodeToPNG();
					break;
			}

			string fileName = Path.GetFileNameWithoutExtension(path);

			path = Path.GetDirectoryName(path);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			File.WriteAllBytes($"{path}/{fileName}.{type.ToString().ToLower()}", bytes);
		}
		public static Texture2D TakeScreenshot(UnityEngine.Camera camera, Vector2Int size, int depth = 72)
		{
			RenderTexture renderTexture = new RenderTexture(size.x, size.y, depth);

			camera.targetTexture = renderTexture;
			RenderTexture.active = renderTexture;

			Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);

			camera.Render();
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
		public static RenderPipeline GetCurrentRenderPipeline()
		{
#if UNITY_2019_3_OR_NEWER
			if (!GraphicsSettings.currentRenderPipeline)
#else
			if (!GraphicsSettings.renderPipelineAsset)
#endif
				return RenderPipeline.Standard;
#if UNITY_2019_3_OR_NEWER
			else if (GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("HDRenderPipelineAsset"))
#else
			else if (GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("HDRenderPipelineAsset"))
#endif
				return RenderPipeline.HDRP;
#if UNITY_2019_3_OR_NEWER
			else if (GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("LightweightRenderPipelineAsset") || GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("UniversalRenderPipelineAsset"))
#else
			else if (GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("LightweightRenderPipelineAsset") || GraphicsSettings.renderPipelineAsset.GetType().Name.Contains("UniversalRenderPipelineAsset"))
#endif
				return RenderPipeline.URP;
			else
				return RenderPipeline.Custom;
		}
		public static GameObject[] FindGameObjectsWithLayerMask(string[] layers, bool includeInactive = true)
		{
			return FindGameObjectsWithLayerMask(LayerMask.GetMask(layers), includeInactive);
		}
		public static GameObject[] FindGameObjectsWithLayerMask(LayerMask layerMask, bool includeInactive = true)
		{
			return FindGameObjectsWithLayerMask(layerMask.value, includeInactive);
		}
		public static GameObject[] FindGameObjectsWithLayerMask(int layerMask, bool includeInactive = true)
		{
			List<GameObject> gameObjects = new List<GameObject>();
			IEnumerable<GameObject> rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

			foreach (GameObject rootGameObject in rootGameObjects)
			{
				IEnumerable<GameObject> children = rootGameObject.GetComponentsInChildren<Transform>().Select(transform => transform.gameObject);

				if (children.Count() > 0)
					gameObjects.AddRange(children.Where(gameObject => (includeInactive || gameObject.activeInHierarchy) && MaskHasLayer(layerMask, gameObject.layer)));
			}

			return gameObjects.ToArray();
		}
		public static Bounds GetObjectPhysicsBounds(GameObject gameObject, bool includeTriggers, bool keepRotation = false, bool keepScale = true)
		{
			IEnumerable<Collider> colliders = gameObject.GetComponentsInChildren<Collider>();

			if (colliders.Count() > 0)
				colliders = colliders.Where(collider => includeTriggers || !collider.isTrigger);

			Bounds bounds = default;
			Quaternion orgRotation = gameObject.transform.rotation;
			Vector3 orgScale = gameObject.transform.localScale;

			if (!keepScale)
				gameObject.transform.localScale = Vector3.one;

			if (!keepRotation)
				gameObject.transform.rotation = Quaternion.identity;

			for (int i = 0; i < colliders.Count(); i++)
				if (bounds.size == Vector3.zero)
					bounds = colliders.ElementAt(i).bounds;
				else
					bounds.Encapsulate(colliders.ElementAt(i).bounds);

			if (!keepScale)
				gameObject.transform.localScale = orgScale;

			if (!keepRotation)
				gameObject.transform.rotation = orgRotation;

			return bounds;
		}
		public static Bounds GetObjectBounds(GameObject gameObject, bool keepRotation = false, bool keepScale = true)
		{
			IEnumerable<Renderer> renderers = gameObject.GetComponentsInChildren<Renderer>();

			if (renderers.Count() > 0)
				renderers = renderers.Where(renderer => !(renderer is TrailRenderer || renderer is ParticleSystemRenderer));

			Bounds bounds = default;
			Quaternion orgRotation = gameObject.transform.rotation;
			Vector3 orgScale = gameObject.transform.localScale;

			if (!keepScale)
				gameObject.transform.localScale = Vector3.one;

			if (!keepRotation)
				gameObject.transform.rotation = Quaternion.identity;

			for (int i = 0; i < renderers.Count(); i++)
				if (bounds.size == Vector3.zero)
					bounds = renderers.ElementAt(i).bounds;
				else
					bounds.Encapsulate(renderers.ElementAt(i).bounds);

			if (!keepScale)
				gameObject.transform.localScale = orgScale;

			if (!keepRotation)
				gameObject.transform.rotation = orgRotation;

			return bounds;
		}
		public static bool CheckPointInCollider(Collider collider, Vector3 point)
		{
			return CheckPointInCollider(collider, point, out _);
		}
		public static bool CheckPointInCollider(Collider collider, Vector3 point, out Vector3 closestPoint)
		{
			closestPoint = collider.ClosestPoint(point);

			return closestPoint == point;
		}
		public static void Destroy(bool immediate, UnityEngine.Object obj)
		{
			if (immediate)
				UnityEngine.Object.DestroyImmediate(obj);
			else
				UnityEngine.Object.Destroy(obj);
		}
		public static void Destroy(UnityEngine.Object obj, float time)
		{
			UnityEngine.Object.Destroy(obj, time);
		}
		public static void Destroy(UnityEngine.Object obj, bool allowDestroyingAssets)
		{
			UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets);
		}

		private static bool IsDeviderUnit(Units unit)
		{
			return unit == Units.FuelConsumption;
		}
		private static float PointSign(Vector2 point1, Vector2 point2, Vector2 point3)
		{
			return (point1.x - point3.x) * (point2.y - point3.y) - (point2.x - point3.x) * (point1.y - point3.y);
		}

		#endregion
	}
}
