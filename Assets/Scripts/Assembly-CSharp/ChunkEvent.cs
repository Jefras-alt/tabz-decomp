using UnityEngine;
using UnityEngine.Events;

public class ChunkEvent : MonoBehaviour
{
	public UnityEvent ChunkEnableEvent;

	public UnityEvent ChunkDisableEvent;

	private int m_chunkSizeX;

	private int m_chunkSizeY;

	private int m_chunkSizeZ;

	private ChunkPosition m_chunkPosition;

	private ChunkManager m_chunkManager;

	private void Start()
	{
		m_chunkManager = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
		m_chunkSizeX = m_chunkManager.ChunkSizeX;
		m_chunkSizeY = m_chunkManager.ChunkSizeY;
		m_chunkSizeZ = m_chunkManager.ChunkSizeZ;
		m_chunkPosition = GetChunkPosition();
		m_chunkManager.AddChunkEvent(m_chunkPosition.x, m_chunkPosition.y, m_chunkPosition.z, this);
	}

	private ChunkPosition GetChunkPosition()
	{
		return new ChunkPosition((int)base.transform.position.x / m_chunkSizeX, (int)base.transform.position.y / m_chunkSizeY, (int)base.transform.position.z / m_chunkSizeZ);
	}

	private void Update()
	{
	}

	public void TriggerEnableEvent()
	{
		Debug.Log("triggering Chunk event!!!");
		ChunkEnableEvent.Invoke();
	}

	public void TriggerDisableEvent()
	{
		ChunkDisableEvent.Invoke();
	}
}
