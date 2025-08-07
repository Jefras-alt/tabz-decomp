using System.Collections.Generic;
using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_TerrainMeshArea : MonoBehaviour
	{
		public List<MeshTerrain> terrains = new List<MeshTerrain>();

		public bool refresh;

		private void OnEnable()
		{
		}

		private void Update()
		{
			if (refresh)
			{
				refresh = false;
				GetTerrains();
			}
		}

		private void GetTerrains()
		{
			terrains.Clear();
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				Debug.Log(componentsInChildren[i].name);
				terrains.Add(new MeshTerrain(componentsInChildren[i]));
			}
		}
	}
}
