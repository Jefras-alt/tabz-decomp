using UnityEngine;

namespace TerrainComposer2
{
	public class TC_MeasureTerrains : MonoBehaviour
	{
		public class GrassLayer
		{
			public int[,] grass;
		}

		public bool locked;

		public RaycastHit hit;

		public Terrain terrain;

		public MeshRenderer mr;

		public float normalizedHeight;

		public float height;

		public float angle;

		public int textureSize = 50;

		public float[,,] splat;

		public Vector3 size;

		public int splatResolution;

		public Vector2 splatConversion;

		public Vector2 localPos;

		public GrassLayer[] grassLayer;

		public int grassResolution;

		public Vector2 grassConversion;

		public Vector2 grassLocalPos;

		public bool drawSplat;

		public bool drawGrass;

		public void ReadTerrain()
		{
			size = terrain.terrainData.size;
			height = hit.point.y - terrain.transform.position.y;
			normalizedHeight = height / size.y;
			localPos = new Vector2(hit.point.x - terrain.transform.position.x, hit.point.z - terrain.transform.position.z);
			if (drawSplat)
			{
				splatResolution = terrain.terrainData.alphamapResolution;
				splatConversion = new Vector2((float)(splatResolution - 1) / size.x, (float)(splatResolution - 1) / size.z);
				splat = terrain.terrainData.GetAlphamaps(Mathf.RoundToInt(localPos.x * splatConversion.x), Mathf.RoundToInt(localPos.y * splatConversion.y), 1, 1);
			}
			if (!drawGrass)
			{
				return;
			}
			grassResolution = terrain.terrainData.detailResolution;
			grassConversion = new Vector2((float)grassResolution / size.x, (float)grassResolution / size.z);
			int num = terrain.terrainData.detailPrototypes.Length;
			if (grassLayer == null)
			{
				grassLayer = new GrassLayer[num];
			}
			else if (grassLayer.Length != num)
			{
				grassLayer = new GrassLayer[num];
			}
			for (int i = 0; i < num; i++)
			{
				if (grassLayer[i] == null)
				{
					grassLayer[i] = new GrassLayer();
				}
				grassLayer[i].grass = terrain.terrainData.GetDetailLayer(Mathf.RoundToInt(localPos.x * grassConversion.x), Mathf.RoundToInt(localPos.y * grassConversion.y), 1, 1, i);
			}
		}
	}
}
