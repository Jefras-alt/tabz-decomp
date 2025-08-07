using UnityEngine;

public class PhysicsDamageChecker : MonoBehaviour
{
	public float multiplier = 1f;

	private HealthHandler healthHandler;

	private void Start()
	{
		healthHandler = GetComponentInParent<HealthHandler>();
	}

	private void OnCollisionEnter(Collision collision)
	{
	}
}
