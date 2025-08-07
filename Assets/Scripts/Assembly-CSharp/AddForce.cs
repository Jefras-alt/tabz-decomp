using UnityEngine;

public class AddForce : MonoBehaviour
{
	private Rigidbody rig;

	public float force;

	public float upForce;

	private void Start()
	{
		rig = GetComponent<Rigidbody>();
		rig.AddForce(base.transform.forward * force + Vector3.up * upForce, ForceMode.VelocityChange);
	}
}
