using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BTService : IService
{
	[SerializeField]
	private float m_period = 0.1f;

	private float m_counter;

	[SerializeField]
	private int m_maxUpdates = 3;

	private List<ZombieBlackboard> m_agents = new List<ZombieBlackboard>();

	private Dictionary<BTType, BehaviorTree<ZombieBlackboard>> m_behaviorTrees = new Dictionary<BTType, BehaviorTree<ZombieBlackboard>>();

	private bool m_startedUpdate;

	private int m_updateIndex;

	private float m_lastUpdate;

	private Stopwatch m_watch = new Stopwatch();

	private float m_debugTime;

	public void Initialize()
	{
		UnityEngine.Debug.Log("Initialized BTService");
		m_behaviorTrees.Add(BTType.DEFAULT, BTFactory.CreateDefaultBT());
		m_behaviorTrees.Add(BTType.ZOMBIE, BTFactory.CreateZombieBT());
	}

	public void Update()
	{
		if (m_agents.Count <= 0)
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
				if (i < m_agents.Count)
				{
					ZombieBlackboard zombieBlackboard = m_agents[i];
					m_behaviorTrees[zombieBlackboard.Behaviour].Update(m_counter, zombieBlackboard);
					if (i == updateIndex + m_maxUpdates - 1)
					{
						m_startedUpdate = true;
						m_updateIndex = i + 1;
						break;
					}
				}
				else if (i >= m_agents.Count)
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

	public void AddZombieBlackboard(ZombieBlackboard bb)
	{
		m_agents.Add(bb);
	}

	public void RemoveZombieBlackboard(ZombieBlackboard bb)
	{
		m_agents.Remove(bb);
	}

	public void LateUpdate()
	{
	}

	public void FixedUpdate()
	{
	}

	public void Destroy()
	{
	}

	public float UpdateTime()
	{
		return m_watch.ElapsedMilliseconds;
	}

	public float LateUpdateTime()
	{
		return -1f;
	}

	public float FixedUpdateTime()
	{
		return -1f;
	}
}
