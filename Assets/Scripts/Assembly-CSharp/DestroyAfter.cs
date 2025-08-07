using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
	[SerializeField]
	private float mDestroyTimer = 1f;

	private float mTimer;

	private void Start()
	{
	}

	private void Update()
	{
		mTimer += Time.deltaTime;
		if (mTimer > mDestroyTimer)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
