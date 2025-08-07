using UnityEngine;

public class DespawnOnClick : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnMouseDown()
	{
		SmartPool.Despawn(base.gameObject);
	}
}
