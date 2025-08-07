using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
	private Dictionary<string, Stack<GameObject>> m_pools = new Dictionary<string, Stack<GameObject>>();

	public GameObject FetchGO(string prefabID)
	{
		return m_pools[prefabID].Pop();
	}

	public void ReturnGO(string prefabID, GameObject go)
	{
		if (!m_pools.ContainsKey(prefabID))
		{
			m_pools.Add(prefabID, new Stack<GameObject>());
		}
		m_pools[prefabID].Push(go);
	}
}
