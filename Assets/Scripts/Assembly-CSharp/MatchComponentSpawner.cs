using UnityEngine;

public class MatchComponentSpawner : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public static void SpawnShit()
	{
		TerrainMaterialChecker terrainMaterialChecker = Object.FindObjectOfType<TerrainMaterialChecker>();
		if (!terrainMaterialChecker)
		{
			new GameObject("TerrainMaterialChecker").AddComponent<TerrainMaterialChecker>();
		}
		GameObject gameObject = null;
		SoundEventsManager soundEventsManager = Object.FindObjectOfType<SoundEventsManager>();
		if (!soundEventsManager)
		{
			gameObject = new GameObject("SoundEventManager");
			gameObject.AddComponent<SoundEventsManager>();
		}
		SoundEffectsManager soundEffectsManager = Object.FindObjectOfType<SoundEffectsManager>();
		if (!soundEffectsManager)
		{
			if (gameObject == null)
			{
				gameObject = new GameObject("SoundEventManager");
			}
			else
			{
				gameObject.AddComponent<SoundEffectsManager>();
			}
		}
		MusicHandler musicHandler = Object.FindObjectOfType<MusicHandler>();
		if (!musicHandler)
		{
			if (gameObject == null)
			{
				gameObject = new GameObject("SoundEventManager");
			}
			else
			{
				gameObject.AddComponent<MusicHandler>();
			}
		}
		ServiceLocator serviceLocator = Object.FindObjectOfType<ServiceLocator>();
		if (!serviceLocator)
		{
			new GameObject("ServiceLocator").AddComponent<ServiceLocator>();
		}
		NetworkManager networkManager = Object.FindObjectOfType<NetworkManager>();
		if (!networkManager)
		{
			Object.Instantiate(Resources.Load("NetworkStuff"));
		}
	}
}
