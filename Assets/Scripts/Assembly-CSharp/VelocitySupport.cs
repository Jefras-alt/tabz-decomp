using UnityEngine;

public class VelocitySupport : MonoBehaviour
{
	private Rigidbody mainRig;

	private Rigidbody rig;

	public float amount;

	private void Start()
	{
		mainRig = base.transform.root.GetComponent<PhysicsAmimationController>().mainRig;
		rig = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		Vector3 vector = new Vector3(mainRig.velocity.x, 0f, mainRig.velocity.z);
		Vector3 normalized = vector.normalized;
		Vector3 to = base.transform.position - mainRig.transform.position;
		to.y = 0f;
		if (Vector3.Angle(normalized, to) < 90f)
		{
			rig.AddForce(Vector3.up * amount * Time.deltaTime, ForceMode.Acceleration);
		}
	}
}
