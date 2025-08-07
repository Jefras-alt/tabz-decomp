using UnityEngine;

public class SetRandomRotation : MonoBehaviour
{
	public float spread;

	private void Awake()
	{
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x + Random.Range(0f - spread, spread), base.transform.rotation.eulerAngles.y + Random.Range(0f - spread, spread), base.transform.rotation.eulerAngles.z + Random.Range(0f - spread, spread));
	}
}
