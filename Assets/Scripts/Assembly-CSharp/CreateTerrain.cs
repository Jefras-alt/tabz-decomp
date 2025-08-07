using TerrainComposer2;
using UnityEngine;

[ExecuteInEditMode]
public class CreateTerrain : MonoBehaviour
{
	public bool createTerrain;

	private void Update()
	{
		if (createTerrain)
		{
			createTerrain = false;
			CreateTerrains();
		}
	}

	private void CreateTerrains()
	{
		TC_Area2D current = TC_Area2D.current;
		if (!(current == null))
		{
			current.terrainAreas[0].CreateTerrains();
		}
	}
}
