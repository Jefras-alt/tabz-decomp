using UnityEngine;

public class ZombieSpawnEvent : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prefab;

	[SerializeField]
	private bool m_ignoreChunkSystem;

	[SerializeField]
	private bool m_useRotation;

	private NetworkZombieSpawner m_spawner;

	private float cd;

	private void Start()
	{
		m_spawner = Object.FindObjectOfType<NetworkZombieSpawner>();
	}

	private void Update()
	{
		cd += Time.deltaTime;
	}

	public void Trigger()
	{
		if (!(cd < 1f))
		{
			cd = 0f;
			NetworkManager.SpawnZombies(new byte[1] { m_spawner.GetIndexForPrefab(m_prefab) }, new Vector3[1] { base.transform.position }, m_useRotation ? new Quaternion[1] { base.transform.rotation } : new Quaternion[1] { Quaternion.identity }, m_ignoreChunkSystem);
		}
	}
}
