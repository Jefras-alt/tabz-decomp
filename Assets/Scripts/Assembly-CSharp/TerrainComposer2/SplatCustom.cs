using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public struct SplatCustom
	{
		public Vector4 select;

		public Vector4 map0;

		public Vector4 map1;

		public SplatCustom(Vector4 select, Vector4 map0, Vector4 map1)
		{
			this.select = select;
			this.map0 = map0;
			this.map1 = map1;
		}
	}
}
