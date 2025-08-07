using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkItem
{
	public Item[] Item;

	public float Timer = 10f;

	private int m_millisecondsTimer;

	private int m_millisecondCounter;

	private bool m_isSpawned;

	private int m_index = -1;

	private bool m_initialSpawn;

	private List<CustomEventTrigger> m_triggers = new List<CustomEventTrigger>();

	public Vector3 Position { get; set; }

	public int MillsecondsTimer
	{
		get
		{
			return m_millisecondsTimer;
		}
		set
		{
			m_millisecondsTimer = value;
		}
	}

	public int MillisecondCounter
	{
		get
		{
			return m_millisecondCounter;
		}
		set
		{
			m_millisecondCounter = value;
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

	public int Index
	{
		get
		{
			return m_index;
		}
		set
		{
			m_index = value;
		}
	}

	public void RefreshTime(int time)
	{
		if (time < m_millisecondCounter)
		{
			HandleOverflow(time);
		}
		m_millisecondsTimer = time + (int)(Timer * 1000f);
		m_millisecondCounter = time;
	}

	public void UpdateTime(int time)
	{
		if (!m_initialSpawn)
		{
			m_initialSpawn = true;
			TrySpawn(time);
		}
		if (time < m_millisecondCounter)
		{
			HandleOverflow(time);
		}
		int num = time - m_millisecondCounter;
		m_millisecondCounter += num;
		if (m_millisecondCounter >= m_millisecondsTimer && !m_isSpawned)
		{
			TrySpawn(time);
		}
	}

	private void TrySpawn(int time)
	{
		if (PhotonNetwork.isMasterClient)
		{
			m_isSpawned = true;
			PickItemToSpawn(time);
		}
	}

	private void PickItemToSpawn(int time)
	{
		int num = UnityEngine.Random.Range(0, 101);
		if (UnityEngine.Random.value < 0.3f)
		{
			num = 1;
		}
		List<Item> list = new List<Item>();
		for (int i = 0; i < Item.Length; i++)
		{
			if (num >= Item[i].Rarity)
			{
				list.Add(Item[i]);
			}
		}
		if (list.Count == 0)
		{
			m_isSpawned = false;
			RefreshTime(time);
		}
		else
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			NetworkManager.SpawnLoot(list[index].ItemPrefab, Position, Index);
		}
	}

	private void HandleOverflow(int time)
	{
		int millisecondsTimer = m_millisecondsTimer - m_millisecondCounter;
		m_millisecondCounter = time;
		m_millisecondsTimer = millisecondsTimer;
	}

	public void TriggerEvents()
	{
		foreach (CustomEventTrigger trigger in m_triggers)
		{
			trigger.Trigger();
		}
	}

	public void AddTrigger(CustomEventTrigger trigger)
	{
		Debug.Log("Adding trigger!", trigger.gameObject);
		m_triggers.Add(trigger);
	}
}
