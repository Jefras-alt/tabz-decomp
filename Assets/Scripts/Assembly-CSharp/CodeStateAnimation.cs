using System.Collections;
using UnityEngine;

public class CodeStateAnimation : MonoBehaviour
{
	public enum AnimationType
	{
		Position = 0,
		Scale = 1
	}

	public AnimationType animationType;

	public bool state1 = true;

	private bool isAnimating;

	private bool isState1 = true;

	public bool useX = true;

	public bool useY = true;

	public bool useZ = true;

	[Header("State 1")]
	public AnimationCurve curve;

	public float duration = 1f;

	public float multiplier = 1f;

	[Header("")]
	[Header("State 2")]
	public AnimationCurve curve2;

	public float duration2 = 1f;

	public float multiplier2 = 1f;

	private float baseX;

	private float baseY;

	private float baseZ;

	private RectTransform rectTrans;

	private bool mStopped;

	public void Play(bool pState1)
	{
		state1 = pState1;
	}

	private void Start()
	{
		rectTrans = GetComponent<RectTransform>();
		if (animationType == AnimationType.Position)
		{
			if ((bool)rectTrans)
			{
				baseX = rectTrans.anchoredPosition.x;
				baseY = rectTrans.anchoredPosition.y;
				if (!state1)
				{
					float num = curve2.Evaluate(1f) * multiplier2;
					Vector2 anchoredPosition = rectTrans.anchoredPosition;
					if (useX)
					{
						anchoredPosition.x = num;
					}
					if (useY)
					{
						anchoredPosition.y = num;
					}
					rectTrans.anchoredPosition = anchoredPosition;
					isState1 = false;
				}
			}
			else
			{
				baseX = base.transform.localPosition.x;
				baseY = base.transform.localPosition.y;
				baseZ = base.transform.localPosition.z;
			}
		}
		if (animationType != AnimationType.Scale)
		{
			return;
		}
		baseX = base.transform.localScale.x;
		baseY = base.transform.localScale.y;
		baseZ = base.transform.localScale.z;
		if (!state1)
		{
			float num2 = curve2.Evaluate(1f);
			Vector3 localScale = base.transform.localScale;
			if (useX)
			{
				localScale.x = num2 * baseX;
			}
			if (useY)
			{
				localScale.y = num2 * baseY;
			}
			if (useZ)
			{
				localScale.z = num2 * baseZ;
			}
			base.transform.localScale = localScale;
			isState1 = false;
		}
	}

	private void Update()
	{
		if (state1 && !isState1 && !isAnimating)
		{
			StartCoroutine(Animation1());
			isState1 = true;
		}
		if (!state1 && isState1 && !isAnimating)
		{
			StartCoroutine(Animation2());
			isState1 = false;
		}
	}

	public void SetState(bool state)
	{
		state1 = state;
		isState1 = state;
		if (state)
		{
			float num = curve.Evaluate(1f) * multiplier;
			Vector3 localScale = base.transform.localScale;
			if (useX)
			{
				localScale.x = num * baseX;
			}
			if (useY)
			{
				localScale.y = num * baseY;
			}
			if (useZ)
			{
				localScale.z = num * baseZ;
			}
			base.transform.localScale = localScale;
		}
		else
		{
			float num2 = curve2.Evaluate(1f) * multiplier;
			Vector3 localScale2 = base.transform.localScale;
			if (useX)
			{
				localScale2.x = num2 * baseX;
			}
			if (useY)
			{
				localScale2.y = num2 * baseY;
			}
			if (useZ)
			{
				localScale2.z = num2 * baseZ;
			}
			base.transform.localScale = localScale2;
		}
	}

	private IEnumerator Animation1()
	{
		isAnimating = true;
		float t = 0f;
		while (t < duration)
		{
			t += Time.unscaledDeltaTime;
			float curveValue = curve.Evaluate(t / duration) * multiplier;
			if (mStopped)
			{
				t = duration;
				curveValue = curve.Evaluate(0f) * multiplier;
				mStopped = false;
			}
			if (animationType == AnimationType.Position)
			{
				if ((bool)rectTrans)
				{
					Vector2 anchoredPosition = rectTrans.anchoredPosition;
					if (useX)
					{
						anchoredPosition.x = curveValue + baseX;
					}
					if (useY)
					{
						anchoredPosition.y = curveValue + baseY;
					}
					rectTrans.anchoredPosition = anchoredPosition;
				}
				else
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
			}
			if (animationType == AnimationType.Scale)
			{
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
		isAnimating = false;
	}

	public void Stop()
	{
		mStopped = true;
	}

	private IEnumerator Animation2()
	{
		isAnimating = true;
		float t = 0f;
		while (t < duration2)
		{
			t += Time.unscaledDeltaTime;
			float curveValue = curve2.Evaluate(t / duration2) * multiplier2;
			if (mStopped)
			{
				t = duration2;
				curveValue = curve2.Evaluate(0f) * multiplier2;
				mStopped = false;
			}
			if (animationType == AnimationType.Position)
			{
				if ((bool)rectTrans)
				{
					Vector2 anchoredPosition = rectTrans.anchoredPosition;
					if (useX)
					{
						anchoredPosition.x = curveValue + baseX;
					}
					if (useY)
					{
						anchoredPosition.y = curveValue + baseY;
					}
					rectTrans.anchoredPosition = anchoredPosition;
				}
				else
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
			}
			if (animationType == AnimationType.Scale)
			{
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
		isAnimating = false;
	}
}
