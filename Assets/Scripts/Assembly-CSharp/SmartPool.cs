using System.Collections.Generic;
using UnityEngine;

public class SmartPool : MonoBehaviour
{
	public const string Version = "1.02";

	private static Dictionary<string, SmartPool> _Pools = new Dictionary<string, SmartPool>();

	public string PoolName;

	public GameObject Prefab;

	public bool DontDestroy;

	public bool PrepareAtStart = true;

	public int AllocationBlockSize = 1;

	public int MinPoolSize = 1;

	public int MaxPoolSize = 1;

	public PoolExceededMode OnMaxPoolSize;

	public bool AutoCull = true;

	public float CullingSpeed = 1f;

	public bool DebugLog;

	private Stack<GameObject> mStock = new Stack<GameObject>();

	private List<GameObject> mSpawned = new List<GameObject>();

	private float mLastCullingTime;

	public int InStock
	{
		get
		{
			return mStock.Count;
		}
	}

	public int Spawned
	{
		get
		{
			return mSpawned.Count;
		}
	}

	private void Awake()
	{
		if (PoolName.Length == 0)
		{
			Debug.LogWarning("SmartPool: Missing PoolName for pool belonging to '" + base.gameObject.name + "'!");
		}
		if (DontDestroy)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (Prefab != null)
		{
			if (GetPoolByName(PoolName) == null)
			{
				_Pools.Add(PoolName, this);
				if (DebugLog)
				{
					Debug.Log("SmartPool: Adding '" + PoolName + "' to the pool dictionary!");
				}
			}
		}
		else
		{
			Debug.LogError("SmartPool: Pool '" + PoolName + "' is missing it's prefab!");
		}
	}

	private void Start()
	{
		if (PrepareAtStart)
		{
			Prepare();
		}
	}

	private void LateUpdate()
	{
		if (AutoCull && Time.time - mLastCullingTime > CullingSpeed)
		{
			mLastCullingTime = Time.time;
			Cull(true);
		}
	}

	private void OnDisable()
	{
		if (!DontDestroy)
		{
			Clear();
			if (_Pools.Remove(PoolName) && DebugLog)
			{
				Debug.Log("SmartPool: Removing " + PoolName + " from the pool dictionary!");
			}
		}
	}

	private void Reset()
	{
		PoolName = string.Empty;
		Prefab = null;
		DontDestroy = false;
		AllocationBlockSize = 1;
		MinPoolSize = 1;
		MaxPoolSize = 1;
		OnMaxPoolSize = PoolExceededMode.Ignore;
		DebugLog = false;
		AutoCull = true;
		CullingSpeed = 1f;
		mLastCullingTime = 0f;
	}

	private void Clear()
	{
		if (DebugLog)
		{
			Debug.Log("SmartPool (" + PoolName + "): Clearing all instances of " + Prefab.name);
		}
		foreach (GameObject item in mSpawned)
		{
			Object.Destroy(item);
		}
		mSpawned.Clear();
		foreach (GameObject item2 in mStock)
		{
			Object.Destroy(item2);
		}
		mStock.Clear();
	}

	public void Cull()
	{
		Cull(false);
	}

	public void Cull(bool smartCull)
	{
		int num = ((!smartCull) ? (mStock.Count - MaxPoolSize) : Mathf.Min(AllocationBlockSize, mStock.Count - MaxPoolSize));
		if (DebugLog && num > 0)
		{
			Debug.Log("SmartPool (" + PoolName + "): Culling " + num + " items");
		}
		while (num-- > 0)
		{
			GameObject obj = mStock.Pop();
			Object.Destroy(obj);
		}
	}

	public void DespawnItem(GameObject item)
	{
		if (!item)
		{
			if (DebugLog)
			{
				Debug.LogWarning("SmartPool (" + PoolName + ").DespawnItem: item is null!");
			}
			return;
		}
		if (IsSpawned(item))
		{
			item.SetActive(false);
			item.name = Prefab.name + "_stock";
			mSpawned.Remove(item);
			mStock.Push(item);
			if (DebugLog)
			{
				Debug.Log("SmartPool (" + PoolName + "): Despawning '" + item.name);
			}
			return;
		}
		Object.Destroy(item);
		if (DebugLog)
		{
			Debug.LogWarning("SmartPool (" + PoolName + "): Cant Despawn" + item.name + "' because it's not managed by this pool! However, SmartPool destroyed it!");
		}
	}

	public void DespawnAllItems()
	{
		while (mSpawned.Count > 0)
		{
			DespawnItem(mSpawned[0]);
		}
	}

	public void KillItem(GameObject item)
	{
		if (!item)
		{
			if (DebugLog)
			{
				Debug.LogWarning("SmartPool (" + PoolName + ").KillItem: item is null!");
			}
		}
		else
		{
			mSpawned.Remove(item);
			Object.Destroy(item);
		}
	}

	public bool IsManagedObject(GameObject item)
	{
		if (!item)
		{
			if (DebugLog)
			{
				Debug.LogWarning("SmartPool (" + PoolName + ").IsManagedObject: item is null!");
			}
			return false;
		}
		if (mSpawned.Contains(item) || mStock.Contains(item))
		{
			return true;
		}
		return false;
	}

	public bool IsSpawned(GameObject item)
	{
		if (!item)
		{
			if (DebugLog)
			{
				Debug.LogWarning("SmartPool (" + PoolName + ").IsSpawned: item is null!");
			}
			return false;
		}
		return mSpawned.Contains(item);
	}

	private void Populate(int no)
	{
		while (no > 0)
		{
			GameObject gameObject = Object.Instantiate(Prefab);
			gameObject.SetActive(false);
			gameObject.transform.parent = base.transform;
			gameObject.name = Prefab.name + "_stock";
			mStock.Push(gameObject);
			no--;
		}
		if (DebugLog)
		{
			Debug.Log("SmartPool (" + PoolName + "): Instantiated " + mStock.Count + " instances of " + Prefab.name);
		}
	}

	public void Prepare()
	{
		Clear();
		mStock = new Stack<GameObject>(MinPoolSize);
		Populate(MinPoolSize);
	}

	public GameObject SpawnItem()
	{
		GameObject gameObject = null;
		if (InStock == 0 && (Spawned < MaxPoolSize || OnMaxPoolSize == PoolExceededMode.Ignore))
		{
			Populate(AllocationBlockSize);
		}
		if (InStock > 0)
		{
			gameObject = mStock.Pop();
			if (DebugLog)
			{
				Debug.Log("SmartPool (" + PoolName + "): Spawning item, taking it from the stock!");
			}
		}
		else if (OnMaxPoolSize == PoolExceededMode.ReUse)
		{
			gameObject = mSpawned[0];
			mSpawned.RemoveAt(0);
			if (DebugLog)
			{
				Debug.Log("SmartPool (" + PoolName + "): Spawning item, reusing an existing item!");
			}
		}
		else if (DebugLog)
		{
			Debug.Log("SmartPool (" + PoolName + "): MaxPoolSize exceeded, nothing was spawned!");
		}
		if (gameObject != null)
		{
			mSpawned.Add(gameObject);
			gameObject.SetActive(true);
			gameObject.name = Prefab.name + "_clone";
			gameObject.transform.localPosition = Vector3.zero;
		}
		return gameObject;
	}

	public static void Cull(string poolName)
	{
		Cull(poolName, false);
	}

	public static void Cull(string poolName, bool smartCull)
	{
		SmartPool poolByName = GetPoolByName(poolName);
		if ((bool)poolByName)
		{
			poolByName.Cull();
		}
	}

	public static void Despawn(GameObject item)
	{
		if ((bool)item)
		{
			SmartPool poolByItem = GetPoolByItem(item);
			if (poolByItem != null)
			{
				poolByItem.DespawnItem(item);
			}
			else
			{
				Object.Destroy(item);
			}
		}
	}

	public static void DespawnAllItems(string poolName)
	{
		SmartPool poolByName = GetPoolByName(poolName);
		if (poolByName != null)
		{
			poolByName.DespawnAllItems();
		}
	}

	public static SmartPool GetPoolByItem(GameObject item)
	{
		foreach (SmartPool value in _Pools.Values)
		{
			if (value.IsManagedObject(item))
			{
				return value;
			}
		}
		return null;
	}

	public static SmartPool GetPoolByName(string poolName)
	{
		SmartPool value;
		_Pools.TryGetValue(poolName, out value);
		return value;
	}

	public static void Kill(GameObject item)
	{
		if ((bool)item)
		{
			SmartPool poolByItem = GetPoolByItem(item);
			if (poolByItem != null)
			{
				poolByItem.KillItem(item);
			}
			else
			{
				Object.Destroy(item);
			}
		}
	}

	public static void Prepare(string poolName)
	{
		SmartPool poolByName = GetPoolByName(poolName);
		if (poolByName != null)
		{
			poolByName.Prepare();
		}
	}

	public static GameObject Spawn(string poolName)
	{
		SmartPool value;
		if (_Pools.TryGetValue(poolName, out value))
		{
			return value.SpawnItem();
		}
		Debug.LogWarning("SmartPool: No pool with name '" + poolName + "' found!");
		return null;
	}
}
