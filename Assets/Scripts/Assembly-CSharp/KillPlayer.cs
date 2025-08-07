using UnityEngine;

public class KillPlayer : MonoBehaviour
{
	private float cd;

	private void Start()
	{
	}

	private void Update()
	{
		cd += Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		HealthHandler component = other.transform.root.GetComponent<HealthHandler>();
		if ((bool)component && cd > 1f)
		{
			cd = 0f;
			component.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, 500f, null, true);
		}
	}
}
