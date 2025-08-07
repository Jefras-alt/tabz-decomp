using UnityEngine;

public class ChunkSettings : MonoBehaviour
{
	[Header("Spawning")]
	[SerializeField]
	[Range(0f, 30f)]
	private int m_minZombies;

	[SerializeField]
	[Range(0f, 30f)]
	private int m_maxZombies = 3;

	[SerializeField]
	private ZombieRarity[] m_zombieOverrides;

	private int m_chunkSizeX;

	private int m_chunkSizeY;

	private int m_chunkSizeZ;

	public ZombieRarity[] ZombieOverrides
	{
		get
		{
			return m_zombieOverrides;
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("ChunkManager");
		if (gameObject == null)
		{
			Debug.LogError("Couldn't find game object tagged ChunkManager, can not apply ChunkSettings!");
			return;
		}
		ChunkManager component = gameObject.GetComponent<ChunkManager>();
		m_chunkSizeX = component.ChunkSizeX;
		m_chunkSizeY = component.ChunkSizeY;
		m_chunkSizeZ = component.ChunkSizeZ;
		component.SetSettings(GetChunkPosition(), new Chunk.Settings(m_minZombies, m_maxZombies, m_zombieOverrides));
		Object.Destroy(base.gameObject);
	}

	private ChunkPosition GetChunkPosition()
	{
		return new ChunkPosition((int)base.transform.position.x / m_chunkSizeX, (int)base.transform.position.y / m_chunkSizeY, (int)base.transform.position.z / m_chunkSizeZ);
	}
}
