using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	public class Settings
	{
		public int MinZombies;

		public int MaxZombies;

		public ZombieRarity[] Zombies;

		public Settings(int minZombies, int maxZombies, ZombieRarity[] zombies)
		{
			MinZombies = minZombies;
			MaxZombies = maxZombies;
			Zombies = zombies;
		}

		public Settings()
		{
			MinZombies = 1;
			MaxZombies = 3;
		}
	}

	private bool m_primaryLoaded;

	private bool m_secondaryLoaded;

	private bool m_isSpawned;

	private int m_chunkLoadValue;

	private List<ChunkActivator> m_activators = new List<ChunkActivator>();

	private List<ChunkDynamicObject> m_dynamicObjects = new List<ChunkDynamicObject>();

	private ChunkPosition m_position;

	private ChunkActivator m_requestee;

	private Settings m_settings = new Settings();

	private bool m_locallyActive;

	private double m_disabledTime;

	private double m_enabledTime;

	private List<ChunkEvent> m_chunkEvents = new List<ChunkEvent>();

	public bool PrimaryLoaded
	{
		get
		{
			return m_primaryLoaded;
		}
	}

	public bool SecondaryLoaded
	{
		get
		{
			return m_secondaryLoaded;
		}
	}

	public bool IsSpawned
	{
		get
		{
			return m_isSpawned;
		}
		set
		{
			m_isSpawned = value;
		}
	}

	public int LoadValue
	{
		get
		{
			return m_chunkLoadValue;
		}
		set
		{
			m_chunkLoadValue = value;
		}
	}

	public ChunkPosition Position
	{
		get
		{
			return m_position;
		}
	}

	public ChunkActivator Requestee
	{
		get
		{
			return m_requestee;
		}
		set
		{
			m_requestee = value;
		}
	}

	public Settings ChunkSettings
	{
		get
		{
			return m_settings;
		}
		set
		{
			m_settings = value;
		}
	}

	public RequesteeAction Action { get; set; }

	public bool LocallyActive
	{
		get
		{
			return m_locallyActive;
		}
	}

	public Chunk(int x, int y, int z)
	{
		m_position = new ChunkPosition(x, y, z);
		Action = RequesteeAction.NONE;
		DisableAllDynamicObjects();
		if (Random.Range(0, 101) >= 100)
		{
			NetworkZombieSpawner networkZombieSpawner = Object.FindObjectOfType<NetworkZombieSpawner>();
			m_settings.MinZombies = 3;
			m_settings.MaxZombies = 5;
			m_settings.Zombies = new ZombieRarity[1];
			m_settings.Zombies[0] = new ZombieRarity();
			m_settings.Zombies[0].Rarity = 0;
			m_settings.Zombies[0].Prefab = networkZombieSpawner.GetPrefabfromIndex(networkZombieSpawner.GetRandomZombieIndex());
		}
	}

	public void DecideChunkActive()
	{
		if (m_activators.Count > 0)
		{
			m_primaryLoaded = true;
			m_secondaryLoaded = true;
		}
		else if (m_chunkLoadValue > 0)
		{
			m_primaryLoaded = false;
			m_secondaryLoaded = true;
		}
		else
		{
			m_primaryLoaded = false;
			m_secondaryLoaded = false;
		}
	}

	public void IncrementChunkLoad()
	{
		m_chunkLoadValue++;
	}

	public void DecrementChunkLoad()
	{
		m_chunkLoadValue--;
	}

	public void AddActivator(ChunkActivator activator)
	{
		if (!m_activators.Contains(activator))
		{
			m_activators.Add(activator);
		}
	}

	public void RemoveActivator(ChunkActivator activator)
	{
		if (m_activators.Contains(activator))
		{
			m_activators.Remove(activator);
		}
	}

	public void AddChunkEvent(ChunkEvent evt)
	{
		m_chunkEvents.Add(evt);
	}

	public void RemoveChunkEvent(ChunkEvent evt)
	{
		m_chunkEvents.Remove(evt);
	}

	public void AddDynamicObject(ChunkDynamicObject obj)
	{
		if (!m_dynamicObjects.Contains(obj))
		{
			m_dynamicObjects.Add(obj);
			if (!m_locallyActive && !obj.PhotonView.isMine)
			{
				obj.PhotonView.gameObject.SetActive(false);
			}
			else
			{
				obj.PhotonView.gameObject.SetActive(true);
			}
		}
	}

	public void RemoveDynamicObject(ChunkDynamicObject obj)
	{
		if (m_dynamicObjects.Contains(obj))
		{
			m_dynamicObjects.Remove(obj);
		}
	}

	public Vector3 GetVectorPosition()
	{
		return new Vector3(m_position.x, m_position.y, m_position.z);
	}

	public void DisableAllDynamicObjects()
	{
		if (!m_locallyActive)
		{
			return;
		}
		for (int i = 0; i < m_dynamicObjects.Count; i++)
		{
			ChunkDynamicObject chunkDynamicObject = m_dynamicObjects[i];
			if (!chunkDynamicObject.PhotonView.isMine)
			{
				chunkDynamicObject.PhotonView.gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < m_chunkEvents.Count; j++)
		{
			m_chunkEvents[j].TriggerDisableEvent();
		}
		m_locallyActive = false;
		m_disabledTime = PhotonNetwork.time;
	}

	public void EnableAllDynamicObjects()
	{
		if (!m_locallyActive)
		{
			for (int i = 0; i < m_dynamicObjects.Count; i++)
			{
				ChunkDynamicObject chunkDynamicObject = m_dynamicObjects[i];
				chunkDynamicObject.PhotonView.gameObject.SetActive(true);
			}
			for (int j = 0; j < m_chunkEvents.Count; j++)
			{
				m_chunkEvents[j].TriggerEnableEvent();
			}
			m_locallyActive = true;
			m_enabledTime = PhotonNetwork.time;
		}
	}

	public void DestroyChunk()
	{
		PhotonView[] array = new PhotonView[m_dynamicObjects.Count];
		for (int i = 0; i < m_dynamicObjects.Count; i++)
		{
			ChunkDynamicObject chunkDynamicObject = m_dynamicObjects[i];
			array[i] = chunkDynamicObject.PhotonView;
		}
		NetworkManager.DestroyZombie(array);
		m_dynamicObjects.Clear();
	}
}
