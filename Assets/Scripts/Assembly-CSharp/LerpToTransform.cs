using UnityEngine;

public class LerpToTransform : MonoBehaviour
{
	public Transform to;

	public float speed;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, to.position, Time.deltaTime * speed);
	}
}
