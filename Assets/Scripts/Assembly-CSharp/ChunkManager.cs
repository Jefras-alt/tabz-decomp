using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	[SerializeField]
	private int m_chunksX = 5;

	[SerializeField]
	private int m_chunksY = 1;

	[SerializeField]
	private int m_chunksZ = 5;

	[SerializeField]
	private int m_chunkSizeX = 15;

	[SerializeField]
	private int m_chunkSizeY = 30;

	[SerializeField]
	private int m_chunkSizeZ = 15;

	[SerializeField]
	private bool m_debug;

	private Chunk[,,] m_chunks;

	private List<ChunkActivator> m_activators = new List<ChunkActivator>();

	private List<Chunk> m_loadedChunks = new List<Chunk>();

	private List<Chunk> m_loadChunks = new List<Chunk>();

	private List<Chunk> m_removeChunks = new List<Chunk>();

	private List<Chunk> m_chunksToProcess = new List<Chunk>();

	private List<ChunkItem> m_items = new List<ChunkItem>();

	private NetworkZombieSpawner m_zombieSpawner;

	private Stopwatch m_watch = new Stopwatch();

	[SerializeField]
	private float m_period = 0.1f;

	private float m_counter;

	[SerializeField]
	private int m_maxUpdates = 20;

	private bool m_startedUpdate;

	private int m_updateIndex;

	private float m_lastUpdate;

	public int ChunkSizeX
	{
		get
		{
			return m_chunkSizeX;
		}
	}

	public int ChunkSizeY
	{
		get
		{
			return m_chunkSizeY;
		}
	}

	public int ChunkSizeZ
	{
		get
		{
			return m_chunkSizeZ;
		}
	}

	private void Awake()
	{
		m_zombieSpawner = Object.FindObjectOfType<NetworkZombieSpawner>();
		CreateChunks();
		UnityEngine.Debug.Log("Chunks created!");
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!DeleteChunks())
		{
			LoadChunks();
		}
		foreach (ChunkActivator activator in m_activators)
		{
			if (ChunkPosition.IsSame(activator.ChunkPosition, activator.LastChunkPosition))
			{
				continue;
			}
			if (InRange(activator.LastChunkPosition))
			{
				Chunk chunk = m_chunks[activator.LastChunkPosition.x, activator.LastChunkPosition.y, activator.LastChunkPosition.z];
				chunk.RemoveActivator(activator);
				chunk.DecrementChunkLoad();
				chunk.DecideChunkActive();
				chunk.Requestee = activator;
				if (activator.PhotonView.isMine)
				{
					chunk.Action = RequesteeAction.LEFT_CHUNK;
				}
				else
				{
					chunk.Action = RequesteeAction.NONE;
				}
				m_chunksToProcess.Add(chunk);
			}
			for (int i = 0; i < activator.ChunksToLoad.Length; i++)
			{
				ChunkPosition pos = new ChunkPosition(activator.LastChunkPosition.x + activator.ChunksToLoad[i].x, activator.LastChunkPosition.y + activator.ChunksToLoad[i].y, activator.LastChunkPosition.z + activator.ChunksToLoad[i].z);
				if (InRange(pos))
				{
					Chunk chunk2 = m_chunks[pos.x, pos.y, pos.z];
					chunk2.DecrementChunkLoad();
					chunk2.DecideChunkActive();
					chunk2.Requestee = activator;
					if (activator.PhotonView.isMine)
					{
						chunk2.Action = RequesteeAction.LEFT_CHUNK;
					}
					else
					{
						chunk2.Action = RequesteeAction.NONE;
					}
					m_chunksToProcess.Add(chunk2);
				}
			}
			if (InRange(activator.ChunkPosition))
			{
				Chunk chunk3 = m_chunks[activator.ChunkPosition.x, activator.ChunkPosition.y, activator.ChunkPosition.z];
				chunk3.AddActivator(activator);
				chunk3.IncrementChunkLoad();
				chunk3.DecideChunkActive();
				chunk3.Requestee = activator;
				if (activator.PhotonView.isMine)
				{
					chunk3.Action = RequesteeAction.ENTERED_CHUNK;
				}
				else
				{
					chunk3.Action = RequesteeAction.NONE;
				}
				m_chunksToProcess.Add(chunk3);
			}
			for (int j = 0; j < activator.ChunksToLoad.Length; j++)
			{
				ChunkPosition pos2 = new ChunkPosition(activator.ChunkPosition.x + activator.ChunksToLoad[j].x, activator.ChunkPosition.y + activator.ChunksToLoad[j].y, activator.ChunkPosition.z + activator.ChunksToLoad[j].z);
				if (InRange(pos2))
				{
					Chunk chunk4 = m_chunks[pos2.x, pos2.y, pos2.z];
					chunk4.IncrementChunkLoad();
					chunk4.DecideChunkActive();
					chunk4.Requestee = activator;
					if (activator.PhotonView.isMine)
					{
						chunk4.Action = RequesteeAction.ENTERED_CHUNK;
					}
					else
					{
						chunk4.Action = RequesteeAction.NONE;
					}
					if (!m_chunksToProcess.Contains(chunk4))
					{
						m_chunksToProcess.Add(chunk4);
					}
				}
			}
		}
		ProcessChunks();
		m_watch.Start();
		UpdateItemSpawns();
		m_watch.Stop();
		m_watch.Reset();
	}

	private void UpdateItemSpawns()
	{
		if (m_items.Count <= 0)
		{
			return;
		}
		m_counter += Time.deltaTime;
		m_watch.Reset();
		m_watch.Start();
		if (m_counter >= m_period || m_startedUpdate)
		{
			int updateIndex = m_updateIndex;
			m_lastUpdate = Time.realtimeSinceStartup;
			for (int i = updateIndex; i < updateIndex + m_maxUpdates; i++)
			{
				m_updateIndex = i;
				if (i < m_items.Count)
				{
					ChunkItem chunkItem = m_items[i];
					chunkItem.UpdateTime(PhotonNetwork.ServerTimestamp);
					if (i == updateIndex + m_maxUpdates - 1)
					{
						m_startedUpdate = true;
						m_updateIndex = i + 1;
						break;
					}
				}
				else if (i >= m_items.Count)
				{
					m_startedUpdate = false;
					m_counter = 0f;
					m_updateIndex = 0;
					break;
				}
			}
		}
		m_watch.Stop();
	}

	public void AddItemSpawn(ChunkItem item)
	{
		item.RefreshTime(PhotonNetwork.ServerTimestamp);
		item.Index = m_items.Count;
		m_items.Add(item);
	}

	public void PickedUpItem(int index)
	{
		m_items[index].RefreshTime(PhotonNetwork.ServerTimestamp);
		m_items[index].IsSpawned = false;
		m_items[index].TriggerEvents();
	}

	public void SuccessSpawnItem(int index)
	{
		m_items[index].IsSpawned = true;
	}

	public void RemoveItemSpawn(ChunkItem item)
	{
		m_items.Remove(item);
	}

	private void ProcessChunks()
	{
		for (int i = 0; i < m_chunksToProcess.Count; i++)
		{
			Chunk chunk = m_chunksToProcess[i];
			if (chunk.Requestee.PhotonView.isMine)
			{
				if (chunk.Action == RequesteeAction.LEFT_CHUNK)
				{
					chunk.DisableAllDynamicObjects();
				}
				else if (chunk.Action == RequesteeAction.ENTERED_CHUNK)
				{
					chunk.EnableAllDynamicObjects();
				}
			}
			if (!chunk.IsSpawned && !chunk.PrimaryLoaded && chunk.LoadValue > 0)
			{
				m_loadChunks.Add(chunk);
			}
			if (!chunk.SecondaryLoaded && !chunk.PrimaryLoaded && chunk.IsSpawned)
			{
				m_removeChunks.Add(chunk);
			}
		}
		m_chunksToProcess.Clear();
	}

	private bool LoadChunks()
	{
		if (m_loadChunks.Count <= 0)
		{
			return false;
		}
		Chunk chunk = m_loadChunks[0];
		m_loadChunks.RemoveAt(0);
		ChunkPosition position = chunk.Position;
		Vector3 vector = new Vector3(position.x * ChunkSizeX, position.y * ChunkSizeY, position.z * ChunkSizeZ);
		Vector3 vector2 = new Vector3((position.x + 1) * ChunkSizeX, (position.y + 1) * ChunkSizeY, (position.z + 1) * ChunkSizeZ);
		Chunk.Settings chunkSettings = chunk.ChunkSettings;
		int num = Random.Range(chunkSettings.MinZombies, chunkSettings.MaxZombies + 1);
		Vector3[] array = new Vector3[num];
		byte[] array2 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			Vector3 origin = new Vector3(Random.Range(vector.x, vector2.x), 200f, Random.Range(vector.z, vector2.z));
			RaycastHit hitInfo;
			if (Physics.Raycast(origin, -Vector3.up, out hitInfo))
			{
				List<int> list = new List<int>();
				int num2 = Random.Range(0, 101);
				if (chunkSettings.Zombies != null && chunkSettings.Zombies.Length > 0)
				{
					for (int j = 0; j < chunkSettings.Zombies.Length; j++)
					{
						if (num2 >= chunkSettings.Zombies[j].Rarity)
						{
							list.Add(m_zombieSpawner.GetIndexForPrefab(chunkSettings.Zombies[j].Prefab));
						}
					}
				}
				else
				{
					for (int k = 0; k < m_zombieSpawner.SpawnChances.Length; k++)
					{
						if (num2 >= m_zombieSpawner.SpawnChances[k])
						{
							list.Add(k);
						}
					}
				}
				array[i] = hitInfo.point;
				array2[i] = (byte)list[Random.Range(0, list.Count)];
			}
			else
			{
				UnityEngine.Debug.LogError("Invalid zombie spawn point, Ignoring spawn!");
			}
		}
		if (chunk.Requestee == null)
		{
			UnityEngine.Debug.LogError("Chunk requestee is null! SOMETHING IS WRONG! Cannot spawn zombies");
			return false;
		}
		if (chunk.Requestee.PhotonView.isMine)
		{
			NetworkManager.SpawnZombies(array2, array);
		}
		chunk.Requestee = null;
		chunk.IsSpawned = true;
		m_loadedChunks.Add(chunk);
		return true;
	}

	private bool DeleteChunks()
	{
		if (m_removeChunks.Count <= 0)
		{
			return false;
		}
		Chunk chunk = m_removeChunks[0];
		m_removeChunks.RemoveAt(0);
		if (chunk.Requestee == null)
		{
			UnityEngine.Debug.LogError("Chunk requestee is null!! Something went wrong!");
		}
		chunk.DestroyChunk();
		chunk.Requestee = null;
		chunk.IsSpawned = false;
		m_loadedChunks.Remove(chunk);
		return true;
	}

	public List<Chunk> GetLoadedChunks()
	{
		return m_loadedChunks;
	}

	public void ForceSetChunksLoaded(ChunkLoadPackage[] chunkPackages)
	{
		for (int i = 0; i < chunkPackages.Length; i++)
		{
			ChunkLoadPackage chunkLoadPackage = chunkPackages[i];
			Chunk chunk = m_chunks[chunkLoadPackage.x, chunkLoadPackage.y, chunkLoadPackage.z];
			chunk.IsSpawned = true;
			chunk.LoadValue = chunkLoadPackage.loadValue;
			chunk.DecideChunkActive();
			m_loadedChunks.Add(chunk);
		}
	}

	public void SetSettings(ChunkPosition pos, Chunk.Settings settings)
	{
		if (!InRange(pos))
		{
			UnityEngine.Debug.LogError("Cannot set chunk settings. Position " + pos.x + " " + pos.y + " " + pos.z + " is out of range!");
		}
		else
		{
			m_chunks[pos.x, pos.y, pos.z].ChunkSettings = settings;
		}
	}

	public void CreateChunks()
	{
		if (m_chunks != null)
		{
			m_chunks = null;
		}
		m_chunks = new Chunk[m_chunksX, m_chunksY, m_chunksZ];
		for (int i = 0; i < m_chunksX; i++)
		{
			for (int j = 0; j < m_chunksY; j++)
			{
				for (int k = 0; k < m_chunksZ; k++)
				{
					m_chunks[i, j, k] = new Chunk(i, j, k);
				}
			}
		}
	}

	public void AddActivator(ChunkActivator activator)
	{
		UnityEngine.Debug.Log("Adding Activator", activator.gameObject);
		m_activators.Add(activator);
		if (InRange(activator.ChunkPosition))
		{
			Chunk chunk = m_chunks[activator.ChunkPosition.x, activator.ChunkPosition.y, activator.ChunkPosition.z];
			chunk.AddActivator(activator);
			chunk.IncrementChunkLoad();
			chunk.DecideChunkActive();
			if (activator.PhotonView.isMine)
			{
				chunk.EnableAllDynamicObjects();
			}
		}
		for (int i = 0; i < activator.ChunksToLoad.Length; i++)
		{
			ChunkPosition pos = new ChunkPosition(activator.ChunkPosition.x + activator.ChunksToLoad[i].x, activator.ChunkPosition.y + activator.ChunksToLoad[i].y, activator.ChunkPosition.z + activator.ChunksToLoad[i].z);
			if (InRange(pos))
			{
				Chunk chunk2 = m_chunks[pos.x, pos.y, pos.z];
				chunk2.IncrementChunkLoad();
				chunk2.DecideChunkActive();
				if (activator.PhotonView.isMine)
				{
					chunk2.EnableAllDynamicObjects();
				}
			}
		}
	}

	public void DynamicObjectMoved(ChunkDynamicObject obj)
	{
		if (InRange(obj.LastChunkPosition))
		{
			Chunk chunk = m_chunks[obj.LastChunkPosition.x, obj.LastChunkPosition.y, obj.LastChunkPosition.z];
			chunk.RemoveDynamicObject(obj);
		}
		if (InRange(obj.ChunkPosition))
		{
			Chunk chunk2 = m_chunks[obj.ChunkPosition.x, obj.ChunkPosition.y, obj.ChunkPosition.z];
			if (chunk2.PrimaryLoaded || chunk2.SecondaryLoaded)
			{
				chunk2.AddDynamicObject(obj);
				chunk2.DecideChunkActive();
			}
			else
			{
				NetworkManager.DestroyZombie(new PhotonView[1] { obj.PhotonView });
			}
		}
		else
		{
			NetworkManager.DestroyZombie(new PhotonView[1] { obj.PhotonView });
		}
	}

	public void RemoveActivator(ChunkActivator activator)
	{
		m_activators.Remove(activator);
		if (InRange(activator.ChunkPosition))
		{
			Chunk chunk = m_chunks[activator.ChunkPosition.x, activator.ChunkPosition.y, activator.ChunkPosition.z];
			chunk.RemoveActivator(activator);
			chunk.DecrementChunkLoad();
			chunk.DecideChunkActive();
			if (activator.PhotonView.isMine)
			{
				chunk.DisableAllDynamicObjects();
			}
			if (!chunk.SecondaryLoaded && !chunk.PrimaryLoaded && chunk.IsSpawned)
			{
				m_removeChunks.Add(chunk);
				chunk.Requestee = activator;
			}
		}
		for (int i = 0; i < activator.ChunksToLoad.Length; i++)
		{
			ChunkPosition pos = new ChunkPosition(activator.ChunkPosition.x + activator.ChunksToLoad[i].x, activator.ChunkPosition.y + activator.ChunksToLoad[i].y, activator.ChunkPosition.z + activator.ChunksToLoad[i].z);
			if (InRange(pos))
			{
				Chunk chunk2 = m_chunks[pos.x, pos.y, pos.z];
				chunk2.DecrementChunkLoad();
				chunk2.DecideChunkActive();
				if (activator.PhotonView.isMine)
				{
					chunk2.DisableAllDynamicObjects();
				}
				if (!chunk2.SecondaryLoaded && !chunk2.PrimaryLoaded && chunk2.IsSpawned)
				{
					m_removeChunks.Add(chunk2);
					chunk2.Requestee = activator;
				}
			}
		}
	}

	public void AddDynamicObject(ChunkDynamicObject obj)
	{
		Chunk chunk = m_chunks[obj.ChunkPosition.x, obj.ChunkPosition.y, obj.ChunkPosition.z];
		if (!chunk.PrimaryLoaded && !chunk.SecondaryLoaded && chunk.LoadValue <= 0)
		{
			NetworkManager.DestroyZombie(new PhotonView[1] { obj.PhotonView });
		}
		else
		{
			chunk.AddDynamicObject(obj);
		}
	}

	public void RemoveDynamicObject(ChunkDynamicObject obj)
	{
		if (!InRange(obj.ChunkPosition))
		{
			UnityEngine.Debug.LogError("Not in range!!", obj);
		}
		else
		{
			m_chunks[obj.ChunkPosition.x, obj.ChunkPosition.y, obj.ChunkPosition.z].RemoveDynamicObject(obj);
		}
	}

	public void AddChunkEvent(int x, int y, int z, ChunkEvent evt)
	{
		if (!InRange(x, y, z))
		{
			UnityEngine.Debug.LogError("Not in range!!", evt.gameObject);
		}
		else
		{
			m_chunks[x, y, z].AddChunkEvent(evt);
		}
	}

	public void RemoveChunkEvent(int x, int y, int z, ChunkEvent evt)
	{
		if (!InRange(x, y, z))
		{
			UnityEngine.Debug.LogError("Not in range!!", evt.gameObject);
		}
		else
		{
			m_chunks[x, y, z].RemoveChunkEvent(evt);
		}
	}

	private void OnDrawGizmos()
	{
		if (!m_debug)
		{
			return;
		}
		Gizmos.color = Color.blue;
		if (Application.isPlaying)
		{
			for (int i = 0; i < m_chunksX; i++)
			{
				for (int j = 0; j < m_chunksY; j++)
				{
					for (int k = 0; k < m_chunksZ; k++)
					{
						Chunk chunk = m_chunks[i, j, k];
						if (chunk.LocallyActive)
						{
							Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.2f);
							Gizmos.DrawCube(new Vector3((float)(i * m_chunkSizeX) + (float)m_chunkSizeX / 2f, (float)(j * m_chunkSizeY) + (float)m_chunkSizeY / 2f, (float)(k * m_chunkSizeZ) + (float)m_chunkSizeZ / 2f), new Vector3(m_chunkSizeX, m_chunkSizeY, m_chunkSizeZ));
							Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.1f);
							Gizmos.DrawWireCube(new Vector3((float)(i * m_chunkSizeX) + (float)m_chunkSizeX / 2f, (float)(j * m_chunkSizeY) + (float)m_chunkSizeY / 2f, (float)(k * m_chunkSizeZ) + (float)m_chunkSizeZ / 2f), new Vector3(m_chunkSizeX, m_chunkSizeY, m_chunkSizeZ));
						}
						else if (chunk.ChunkSettings.Zombies != null && chunk.ChunkSettings.Zombies.Length > 0)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawWireCube(new Vector3((float)(i * m_chunkSizeX) + (float)m_chunkSizeX / 2f, (float)(j * m_chunkSizeY) + (float)m_chunkSizeY / 2f, (float)(k * m_chunkSizeZ) + (float)m_chunkSizeZ / 2f), new Vector3(m_chunkSizeX, m_chunkSizeY, m_chunkSizeZ));
						}
						else
						{
							Gizmos.color = Color.blue;
							Gizmos.DrawWireCube(new Vector3((float)(i * m_chunkSizeX) + (float)m_chunkSizeX / 2f, (float)(j * m_chunkSizeY) + (float)m_chunkSizeY / 2f, (float)(k * m_chunkSizeZ) + (float)m_chunkSizeZ / 2f), new Vector3(m_chunkSizeX, m_chunkSizeY, m_chunkSizeZ));
						}
					}
				}
			}
			return;
		}
		for (int l = 0; l < m_chunksX; l++)
		{
			for (int m = 0; m < m_chunksY; m++)
			{
				for (int n = 0; n < m_chunksZ; n++)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawWireCube(new Vector3((float)(l * m_chunkSizeX) + (float)m_chunkSizeX / 2f, (float)(m * m_chunkSizeY) + (float)m_chunkSizeY / 2f, (float)(n * m_chunkSizeZ) + (float)m_chunkSizeZ / 2f), new Vector3(m_chunkSizeX, m_chunkSizeY, m_chunkSizeZ));
				}
			}
		}
	}

	private bool InRange(int x, int y, int z)
	{
		if (x >= 0 && x < m_chunksX && y >= 0 && y < m_chunksY && z >= 0 && z < m_chunksZ)
		{
			return true;
		}
		return false;
	}

	private bool InRange(ChunkPosition pos)
	{
		if (pos.x >= 0 && pos.x < m_chunksX && pos.y >= 0 && pos.y < m_chunksY && pos.z >= 0 && pos.z < m_chunksZ)
		{
			return true;
		}
		return false;
	}
}
