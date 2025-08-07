using TerrainComposer2;
using UnityEngine;

public class LoadTerrainLayer : MonoBehaviour
{
	public GameObject terrainLayerPrefab;

	public bool generateOnStart;

	public bool instantGenerate;

	private void Start()
	{
		InstantiateTerrainLayer();
		if (generateOnStart)
		{
			TC_Generate.instance.Generate(instantGenerate);
		}
	}

	public void InstantiateTerrainLayer()
	{
		if (terrainLayerPrefab == null || terrainLayerPrefab.GetComponent<TC_TerrainLayer>() == null)
		{
			return;
		}
		TC_Area2D current = TC_Area2D.current;
		if (!(current == null))
		{
			if (current.terrainLayer != null)
			{
				Object.Destroy(current.terrainLayer.gameObject);
			}
			GameObject gameObject = Object.Instantiate(terrainLayerPrefab);
			current.terrainLayer = gameObject.GetComponent<TC_TerrainLayer>();
			current.terrainLayer.GetItems(false);
		}
	}
}
