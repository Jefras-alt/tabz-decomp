using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class Curve
	{
		public bool active;

		public Vector2 range = new Vector2(0f, 1f);

		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Vector4[] c;

		public float[] curveKeys;

		public int length;

		public float Calc(float v)
		{
			return curve.Evaluate(v) * v;
		}

		public void ConvertCurve(float scale = 1f)
		{
			if (curve.keys.Length < 2 || !active)
			{
				length = 0;
				return;
			}
			length = curve.keys.Length;
			c = new Vector4[curve.length - 1];
			curveKeys = new float[curve.length];
			for (int i = 0; i < c.Length; i++)
			{
				float time = curve.keys[i].time;
				float num = curve.keys[i].value * scale;
				float outTangent = curve.keys[i].outTangent;
				float time2 = curve.keys[i + 1].time;
				float num2 = curve.keys[i + 1].value * scale;
				float inTangent = curve.keys[i + 1].inTangent;
				c[i].x = (time * outTangent + time * inTangent - time2 * outTangent - time2 * inTangent - 2f * num + 2f * num2) / (time * time * time - time2 * time2 * time2 + 3f * time * time2 * time2 - 3f * time * time * time2);
				c[i].y = ((0f - time) * time * outTangent - 2f * time * time * inTangent + 2f * time2 * time2 * outTangent + time2 * time2 * inTangent - time * time2 * outTangent + time * time2 * inTangent + 3f * time * num - 3f * time * num2 + 3f * num * time2 - 3f * time2 * num2) / (time * time * time - time2 * time2 * time2 + 3f * time * time2 * time2 - 3f * time * time * time2);
				c[i].z = (time * time * time * inTangent - time2 * time2 * time2 * outTangent - time * time2 * time2 * outTangent - 2f * time * time2 * time2 * inTangent + 2f * time * time * time2 * outTangent + time * time * time2 * inTangent - 6f * time * num * time2 + 6f * time * time2 * num2) / (time * time * time - time2 * time2 * time2 + 3f * time * time2 * time2 - 3f * time * time * time2);
				c[i].w = (time * time2 * time2 * time2 * outTangent - time * time * time2 * time2 * outTangent + time * time * time2 * time2 * inTangent - time * time * time * time2 * inTangent - num * time2 * time2 * time2 + time * time * time * num2 + 3f * time * num * time2 * time2 - 3f * time * time * time2 * num2) / (time * time * time - time2 * time2 * time2 + 3f * time * time2 * time2 - 3f * time * time * time2);
			}
			for (int j = 0; j < curveKeys.Length; j++)
			{
				curveKeys[j] = curve.keys[j].time;
			}
		}
	}
}
