using UnityEngine;

public class AddForceTowards : MonoBehaviour
{
	private Rigidbody rig;

	public Transform to;

	public float force;

	private void Start()
	{
		rig = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		rig.AddForce((to.position - base.transform.position) * force * Time.deltaTime, ForceMode.Acceleration);
	}
}
