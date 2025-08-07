using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_TerrainSettings : MonoBehaviour
	{
		public float heightmapPixelError = 5f;

		public int heightmapMaximumLOD;

		public bool drawTreesAndFoliage = true;

		public float treeDistance = 2000f;

		public float detailObjectDistance = 80f;

		public float detailObjectDensity = 1f;

		public float treeBillboardDistance = 50f;

		public int treeMaximumFullLODCount = 50;

		public float basemapDistance = 20000f;

		public void Start()
		{
			SetTerrainSettings();
		}

		public void SetTerrainSettings()
		{
			Terrain component = GetComponent<Terrain>();
			if (!(component == null))
			{
				component.heightmapPixelError = heightmapPixelError;
				component.heightmapMaximumLOD = heightmapMaximumLOD;
				if (drawTreesAndFoliage)
				{
					component.treeDistance = treeDistance;
					component.detailObjectDistance = detailObjectDistance;
				}
				else
				{
					component.treeDistance = 0f;
					component.detailObjectDistance = 0f;
				}
				component.detailObjectDensity = detailObjectDensity;
				component.treeMaximumFullLODCount = treeMaximumFullLODCount;
				component.treeBillboardDistance = treeBillboardDistance;
				component.treeMaximumFullLODCount = treeMaximumFullLODCount;
				component.basemapDistance = basemapDistance;
				component.terrainData.wavingGrassAmount = 0.25f;
			}
		}
	}
}
