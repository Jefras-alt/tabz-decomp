using System.Collections;
using UnityEngine;

public class CodeAnimation : MonoBehaviour
{
	public enum AnimationType
	{
		Scale = 0,
		Position = 1
	}

	public AnimationType animationType;

	[Header("Animation")]
	public bool looping;

	public bool playOnAwake = true;

	public bool useX = true;

	public bool useY = true;

	public bool useZ = true;

	public AnimationCurve curve;

	public float duration = 1f;

	public float aditionalRandomDuration;

	public float multiplier = 1f;

	private float baseX;

	private float baseY;

	private float baseZ;

	public float firstDelay;

	private void Start()
	{
		duration += Random.Range(0f, aditionalRandomDuration);
		if (animationType == AnimationType.Position)
		{
			baseX = base.transform.localPosition.x;
			baseY = base.transform.localPosition.y;
			baseZ = base.transform.localPosition.z;
		}
		if (animationType == AnimationType.Scale)
		{
			baseX = base.transform.localScale.x;
			baseY = base.transform.localScale.y;
			baseZ = base.transform.localScale.z;
		}
		if (playOnAwake)
		{
			StartCoroutine(Animation());
		}
	}

	private void Update()
	{
	}

	public void Play()
	{
		StartCoroutine(Animation());
	}

	private IEnumerator Animation()
	{
		while (firstDelay > 0f)
		{
			firstDelay -= Time.deltaTime;
			yield return null;
		}
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float curveValue = curve.Evaluate(t / duration) * multiplier;
			if (animationType == AnimationType.Position)
			{
				Vector3 localPosition = base.transform.localPosition;
				if (useX)
				{
					localPosition.x = curveValue + baseX;
				}
				if (useY)
				{
					localPosition.y = curveValue + baseY;
				}
				if (useZ)
				{
					localPosition.z = curveValue + baseZ;
				}
				base.transform.localPosition = localPosition;
			}
			if (animationType == AnimationType.Scale)
			{
				if (curveValue == 0f)
				{
					curveValue = 0.001f;
				}
				Vector3 localScale = base.transform.localScale;
				if (useX)
				{
					localScale.x = curveValue * baseX;
				}
				if (useY)
				{
					localScale.y = curveValue * baseY;
				}
				if (useZ)
				{
					localScale.z = curveValue * baseZ;
				}
				base.transform.localScale = localScale;
			}
			yield return new WaitForEndOfFrame();
		}
		if (looping)
		{
			Play();
		}
	}
}
