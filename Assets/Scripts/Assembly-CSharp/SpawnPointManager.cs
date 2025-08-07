using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
	private static SpawnPointTAG[] mSpawnPoints;

	private void Awake()
	{
	}

	public static Vector3 GetRandomSpawnPoint()
	{
		mSpawnPoints = Object.FindObjectsOfType<SpawnPointTAG>();
		int num = Random.Range(0, mSpawnPoints.Length);
		return mSpawnPoints[num].transform.position;
	}
}
