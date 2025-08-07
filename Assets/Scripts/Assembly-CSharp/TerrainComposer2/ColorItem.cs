using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public struct ColorItem
	{
		public Vector3 select;

		public Vector4 color;

		public ColorItem(Vector3 select, Vector4 color)
		{
			this.select = select;
			this.color = color;
		}
	}
}
