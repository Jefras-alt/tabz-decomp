using Photon;
using UnityEngine;

public class WorldItemAdder : PunBehaviour
{
	private void Start()
	{
		if (PhotonNetwork.connected)
		{
			Init();
		}
	}

	public override void OnJoinedRoom()
	{
		Init();
		base.OnJoinedRoom();
	}

	private void Init()
	{
		ChunkManager component = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			WorldItemSpawn component2 = base.transform.GetChild(i).GetComponent<WorldItemSpawn>();
			component2.ChunkItem.Position = component2.transform.position;
			component.AddItemSpawn(component2.ChunkItem);
			CustomEventTrigger[] componentsInChildren = component2.GetComponentsInChildren<CustomEventTrigger>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				component2.ChunkItem.AddTrigger(componentsInChildren[j]);
				componentsInChildren[j].gameObject.transform.SetParent(null, true);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
