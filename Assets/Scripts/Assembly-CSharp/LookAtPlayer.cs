using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
	private float mLerpSpeed = 5f;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 eulerAngles = Camera.main.transform.eulerAngles;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(eulerAngles), Time.deltaTime * mLerpSpeed);
	}
}
