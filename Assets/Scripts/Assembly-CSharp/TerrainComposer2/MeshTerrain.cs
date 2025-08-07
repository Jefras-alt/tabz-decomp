using System;
using UnityEngine;

namespace TerrainComposer2
{
	[Serializable]
	public class MeshTerrain : TC_Terrain
	{
		public MeshTerrain(Transform t)
		{
			base.t = t;
			Init();
		}
	}
}
