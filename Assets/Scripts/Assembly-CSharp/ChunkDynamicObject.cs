using UnityEngine;

public class ChunkDynamicObject : MonoBehaviour
{
	private ChunkManager m_chunkManager;

	private int m_chunkSizeX;

	private int m_chunkSizeY;

	private int m_chunkSizeZ;

	private ChunkPosition m_chunkPosition;

	private ChunkPosition m_lastChunkPosition;

	private PhotonView m_photonView;

	[SerializeField]
	private bool m_debug;

	public ChunkPosition[] ChunksToLoad = new ChunkPosition[4]
	{
		new ChunkPosition(-1, 0, 0),
		new ChunkPosition(1, 0, 0),
		new ChunkPosition(0, 0, 1),
		new ChunkPosition(0, 0, -1)
	};

	public ChunkPosition ChunkPosition
	{
		get
		{
			return m_chunkPosition;
		}
	}

	public ChunkPosition LastChunkPosition
	{
		get
		{
			return m_lastChunkPosition;
		}
	}

	public PhotonView PhotonView
	{
		get
		{
			return m_photonView;
		}
	}

	private void Awake()
	{
		m_photonView = GetComponentInParent<PhotonView>();
	}

	private void Start()
	{
		m_chunkManager = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
		m_chunkSizeX = m_chunkManager.ChunkSizeX;
		m_chunkSizeY = m_chunkManager.ChunkSizeY;
		m_chunkSizeZ = m_chunkManager.ChunkSizeZ;
		m_chunkPosition = GetChunkPosition();
		m_lastChunkPosition = m_chunkPosition;
		m_chunkManager.AddDynamicObject(this);
	}

	private void OnDestroy()
	{
		m_chunkManager.RemoveDynamicObject(this);
	}

	private void Update()
	{
		m_lastChunkPosition = ChunkPosition;
		m_chunkPosition = GetChunkPosition();
		if (!ChunkPosition.IsSame(m_lastChunkPosition, m_chunkPosition))
		{
			m_chunkManager.DynamicObjectMoved(this);
		}
	}

	private ChunkPosition GetChunkPosition()
	{
		return new ChunkPosition((int)base.transform.position.x / m_chunkSizeX, (int)base.transform.position.y / m_chunkSizeY, (int)base.transform.position.z / m_chunkSizeZ);
	}

	private void OnDrawGizmos()
	{
		if (m_debug)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(base.transform.position, 2f);
		}
	}
}
