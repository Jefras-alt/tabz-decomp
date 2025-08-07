using UnityEngine;

public class Explosion : MonoBehaviour
{
	public float dmg;

	public float force;

	public float range;

	public LayerMask mask;

	private void Start()
	{
		Explode();
	}

	private void Explode()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, range, mask);
		foreach (Collider collider in array)
		{
			PhotonView componentInParent = collider.GetComponentInParent<PhotonView>();
			if (!componentInParent.isMine)
			{
				break;
			}
			componentInParent.RPC("TakeDamage", PhotonTargets.All, dmg, null, componentInParent.GetComponent<HealthHandler>().currentHealth <= dmg);
			Rigidbody componentInParent2 = collider.GetComponentInParent<Rigidbody>();
			RigidBodyIndexHolder component = componentInParent2.GetComponent<RigidBodyIndexHolder>();
			if (component == null)
			{
				Debug.LogError("KUKEN HAR INGEN RIGIDBODYU INDEX, KAN INTE SKICKA FORCE ÖVER NÄTET");
				break;
			}
			byte index = component.Index;
			Vector3 vector = (range - Vector3.Distance(base.transform.position, collider.transform.position) / range) * force * (collider.transform.position - base.transform.position).normalized;
			collider.GetComponentInParent<PhotonView>().RPC("AddForce", PhotonTargets.All, index, vector, 2);
		}
	}
}
