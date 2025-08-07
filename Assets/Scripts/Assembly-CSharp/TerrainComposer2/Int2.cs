using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public struct Int2
	{
		public int x;

		public int y;

		public static Int2 One = new Int2(1, 1);

		public Int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Int2(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}

		public Int2(Vector2 v)
		{
			x = (int)v.x;
			y = (int)v.y;
		}

		public Vector2 ToVector2()
		{
			return new Vector2(x, y);
		}

		public override string ToString()
		{
			return x + "x" + y;
		}
	}
}
