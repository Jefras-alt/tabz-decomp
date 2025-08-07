using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	public static class Mathw
	{
		public static float Clamp01(float v)
		{
			if (v < 0f)
			{
				v = 0f;
			}
			else if (v > 1f)
			{
				v = 1f;
			}
			return v;
		}

		public static float Clamp(float v, float min, float max)
		{
			if (v < min)
			{
				v = min;
			}
			else if (v > max)
			{
				v = max;
			}
			return v;
		}

		public static int Clamp(int v, int min, int max)
		{
			if (v < min)
			{
				v = min;
			}
			else if (v > max)
			{
				v = max;
			}
			return v;
		}

		public static byte Clamp(byte v, byte min, byte max)
		{
			if (v < min)
			{
				v = min;
			}
			else if (v > max)
			{
				v = max;
			}
			return v;
		}

		public static float Abs(float v)
		{
			return (!(v < 0f)) ? v : (v * -1f);
		}

		public static int Abs(int v)
		{
			return (v >= 0) ? v : (v * -1);
		}

		public static double Abs(double v)
		{
			return (!(v < 0.0)) ? v : (v * -1.0);
		}

		public static float GetColorBrightness(Color color)
		{
			return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
		}

		public static float Frac(float v)
		{
			return v - Mathf.Floor(v);
		}

		public static bool ArrayContains<T>(T[] array, T obj) where T : class
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == obj)
				{
					return true;
				}
			}
			return false;
		}

		public static Object[] AddToArray(Object[] array, Object obj)
		{
			List<Object> list = new List<Object>();
			list.AddRange(array);
			list.Add(obj);
			return list.ToArray();
		}

		public static T[] AddToArray<T>(T[] array, T t)
		{
			T[] array2 = new T[array.Length + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			array2[array.Length] = t;
			return array2;
		}

		public static T[] ResizeArray<T>(T[] array, int length)
		{
			if (array == null)
			{
				return new T[length];
			}
			T[] array2 = new T[length];
			int num = ((array.Length <= length) ? array.Length : length);
			for (int i = 0; i < num; i++)
			{
				array2[i] = array[i];
			}
			return array2;
		}

		public static Object[] RemoveFromArray(Object[] array, Object obj)
		{
			List<Object> list = new List<Object>();
			list.AddRange(array);
			int num = list.IndexOf(obj);
			if (num != -1)
			{
				list.RemoveAt(num);
			}
			return list.ToArray();
		}

		public static AnimationCurve InvertCurve(AnimationCurve curve)
		{
			Keyframe[] array = new Keyframe[curve.keys.Length];
			for (int i = 0; i < curve.keys.Length; i++)
			{
				array[i] = new Keyframe(curve.keys[i].time, 1f - curve.keys[i].value, curve.keys[i].inTangent * -1f, curve.keys[i].outTangent * -1f);
			}
			return new AnimationCurve(array);
		}

		public static void EncapsulteRect(ref Rect rect, Rect rect2)
		{
			rect.xMin = Mathf.Min(rect.xMin, rect2.xMin);
			rect.yMin = Mathf.Min(rect.yMin, rect2.yMin);
			rect.xMax = Mathf.Max(rect.xMax, rect2.xMax);
			rect.yMax = Mathf.Max(rect.yMax, rect2.yMax);
		}

		public static Rect ClampRect(Rect baseRect, Rect clampRect)
		{
			return new Rect
			{
				xMin = Mathf.Max(baseRect.xMin, clampRect.xMin),
				xMax = Mathf.Min(baseRect.xMax, clampRect.xMax),
				yMin = Mathf.Max(baseRect.yMin, clampRect.yMin),
				yMax = Mathf.Min(baseRect.yMax, clampRect.yMax)
			};
		}

		public static bool OverlapRect(Rect baseRect, Rect testRect, out Rect overlapRect)
		{
			overlapRect = new Rect(0f, 0f, 0f, 0f);
			if (testRect.xMax > baseRect.xMin && testRect.xMin < baseRect.xMax && testRect.yMax > baseRect.yMin && testRect.yMin < baseRect.yMax)
			{
				overlapRect = ClampRect(baseRect, testRect);
				return true;
			}
			return false;
		}

		public static Rect UniformRectToResolution(Rect rect, Int2 targetRes, Int2 sampleRes, out Int2 samplePos)
		{
			Vector2 vector = new Vector2((float)targetRes.x / (float)sampleRes.x, (float)targetRes.y / (float)sampleRes.y);
			samplePos = new Int2(Mathf.FloorToInt(rect.x * (float)sampleRes.x), Mathf.FloorToInt(rect.y * (float)sampleRes.y));
			Vector2 size = new Vector2(Mathf.Ceil(rect.width * (float)sampleRes.x) * vector.x, Mathf.Ceil(rect.height * (float)sampleRes.y) * vector.y);
			Vector2 position = new Vector2((float)samplePos.x * vector.x, (float)samplePos.y * vector.y);
			return new Rect(position, size);
		}

		public static AnimationCurve SetAnimationCurveLinear(AnimationCurve curve)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			for (int i = 0; i < curve.keys.Length; i++)
			{
				float inTangent = 0f;
				float outTangent = 0f;
				bool flag = false;
				bool flag2 = false;
				Vector2 zero = Vector2.zero;
				Vector2 zero2 = Vector2.zero;
				Vector2 zero3 = Vector2.zero;
				Keyframe key = curve[i];
				if (i == 0)
				{
					inTangent = 0f;
					flag = true;
				}
				if (i == curve.keys.Length - 1)
				{
					outTangent = 0f;
					flag2 = true;
				}
				if (!flag)
				{
					zero.x = curve.keys[i - 1].time;
					zero.y = curve.keys[i - 1].value;
					zero2.x = curve.keys[i].time;
					zero2.y = curve.keys[i].value;
					zero3 = zero2 - zero;
					inTangent = zero3.y / zero3.x;
				}
				if (!flag2)
				{
					zero.x = curve.keys[i].time;
					zero.y = curve.keys[i].value;
					zero2.x = curve.keys[i + 1].time;
					zero2.y = curve.keys[i + 1].value;
					zero3 = zero2 - zero;
					outTangent = zero3.y / zero3.x;
				}
				key.inTangent = inTangent;
				key.outTangent = outTangent;
				animationCurve.AddKey(key);
			}
			return animationCurve;
		}

		public static Vector2 VectorMul(Vector2 p, float v)
		{
			return new Vector2(p.x * v, p.y * v);
		}

		public static Vector3 VectorMul(Vector3 p, float v)
		{
			return new Vector3(p.x * v, p.y * v, p.z * v);
		}

		public static Vector2 VectorDiv(Vector2 p, float v)
		{
			return new Vector2(p.x / v, p.y / v);
		}

		public static Vector3 VectorDiv(Vector3 p, float v)
		{
			return new Vector3(p.x / v, p.y / v, p.z / v);
		}

		public static Vector4[] ColorsToVector4(Color[] colors)
		{
			Vector4[] array = new Vector4[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				array[i] = colors[i];
			}
			return array;
		}

		public static float Snap(float v, float snapValue)
		{
			return (float)(int)(v / snapValue) * snapValue;
		}

		public static Vector2 SnapVector2(Vector2 v, float snapValue)
		{
			v.x = (float)(int)(v.x / snapValue) * snapValue;
			v.y = (float)(int)(v.y / snapValue) * snapValue;
			return v;
		}

		public static Vector3 SnapVector3(Vector3 v, float snapValue)
		{
			v.x = (float)(int)(v.x / snapValue) * snapValue;
			v.y = (float)(int)(v.y / snapValue) * snapValue;
			v.z = (float)(int)(v.z / snapValue) * snapValue;
			return v;
		}

		public static Vector3 SnapRoundVector3(Vector3 v, float snapValue)
		{
			v.x = Mathf.Round(v.x / snapValue) * snapValue;
			v.y = Mathf.Round(v.y / snapValue) * snapValue;
			v.z = Mathf.Round(v.z / snapValue) * snapValue;
			return v;
		}

		public static Vector3 SnapVector3xz(Vector3 v, float snapValue)
		{
			v.x = (float)(int)(v.x / snapValue) * snapValue;
			v.z = (float)(int)(v.z / snapValue) * snapValue;
			return v;
		}

		public static bool BitSwitch(int v, int index)
		{
			int num = (int)Mathf.Pow(2f, index);
			return (v & num) == num;
		}

		public static int SetBitSwitch(int v, int index)
		{
			return v & (int)Mathf.Pow(2f, index);
		}

		public static string CutString(string name, int length)
		{
			if (length > name.Length)
			{
				return name;
			}
			return name.Substring(0, length);
		}
	}
}
