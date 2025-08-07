using UnityEngine;

namespace TerrainComposer2
{
	public class TC_RandomSpawner : MonoBehaviour
	{
		public GameObject spawnObject;

		public float posOffsetY = 1f;

		public Vector2 posRangeX = new Vector2(-1000f, 1000f);

		public Vector2 posRangeZ = new Vector2(-1000f, 1000f);

		public Vector2 rotRangeY = new Vector2(-180f, 180f);

		public bool spawnOnStart;

		private void Start()
		{
			if (spawnOnStart)
			{
				Spawn();
			}
		}

		public GameObject Spawn()
		{
			if (spawnObject == null)
			{
				return null;
			}
			Vector3 position = base.transform.position;
			position.x += Random.Range(posRangeX.x, posRangeX.y) * base.transform.localScale.x;
			position.z += Random.Range(posRangeZ.x, posRangeZ.y) * base.transform.localScale.z;
			position.y = SampleTerrainHeight(position) + posOffsetY;
			return Object.Instantiate(rotation: Quaternion.Euler(new Vector3(0f, Random.Range(rotRangeY.x, rotRangeY.y), 0f)), original: spawnObject, position: position);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireCube(base.transform.position, new Vector3((posRangeX.y - posRangeX.x) * base.transform.localScale.x * 2f, 100f, posRangeZ.y - posRangeZ.x) * base.transform.localScale.z * 2f);
		}

		private float SampleTerrainHeight(Vector3 pos)
		{
			TC_TerrainArea tC_TerrainArea = TC_Generate.instance.area2D.terrainAreas[0];
			for (int i = 0; i < tC_TerrainArea.terrains.Count; i++)
			{
				TCUnityTerrain tCUnityTerrain = tC_TerrainArea.terrains[i];
				if (!(tCUnityTerrain.terrain == null) && !(tCUnityTerrain.terrain.terrainData == null))
				{
					Vector3 position = tCUnityTerrain.terrain.transform.position;
					Vector3 size = tCUnityTerrain.terrain.terrainData.size;
					if (new Rect(position.x, position.z, size.x, size.z).Contains(pos))
					{
						return tCUnityTerrain.terrain.SampleHeight(pos);
					}
				}
			}
			return -1f;
		}
	}
}
