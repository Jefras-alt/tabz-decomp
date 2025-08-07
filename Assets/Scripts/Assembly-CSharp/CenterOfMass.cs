using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
	public Vector3 newCenterOfMass;

	private void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = newCenterOfMass;
	}

	private void Update()
	{
	}
}
