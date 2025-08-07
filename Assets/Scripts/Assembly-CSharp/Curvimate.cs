using System;
using UnityEngine;

public class Curvimate : MonoBehaviour
{
	public bool mPlaying;

	public bool mPlayOnAwake;

	public bool mLoop;

	[SerializeField]
	private CurvimationTemplate mTemplate = new CurvimationTemplate();

	[SerializeField]
	private bool mPlay;

	private Vector3 mBasePos;

	private Vector3 mBaseScale;

	private Vector3 mBaseRectPos;

	private float mElapsed = float.MaxValue;

	private RectTransform mRectTransform;

	private float mMultiplier = 1f;

	public float P_Multiplier
	{
		get
		{
			return mMultiplier;
		}
		set
		{
			mMultiplier = value;
		}
	}

	public CurvimationTemplate P_ActiveTemplate
	{
		get
		{
			return mTemplate;
		}
		set
		{
			mTemplate = value;
		}
	}

	private void Awake()
	{
		mRectTransform = GetComponent<RectTransform>();
		mBasePos = base.transform.localPosition;
		mBaseScale = base.transform.localScale;
	}

	private void Start()
	{
	}

	public void PlayCurvimation(CurvimationTemplate template)
	{
		mTemplate = template;
		mElapsed = 0f;
		mPlaying = true;
		Animate(mElapsed);
	}

	private Vector3 GetVecToAnim()
	{
		switch (mTemplate.mType)
		{
		case CurvimationTemplate.AnimationType.Scale:
			return base.transform.localScale;
		case CurvimationTemplate.AnimationType.Position:
			if (mRectTransform != null)
			{
				return mRectTransform.anchoredPosition;
			}
			return base.transform.localPosition;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void ApplyVec(Vector3 vec)
	{
		switch (mTemplate.mType)
		{
		case CurvimationTemplate.AnimationType.Scale:
			base.transform.localScale = vec;
			break;
		case CurvimationTemplate.AnimationType.Position:
			if (mRectTransform != null)
			{
				mRectTransform.anchoredPosition = vec;
			}
			else
			{
				base.transform.localPosition = vec;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private Vector3 GetBaseVec()
	{
		switch (mTemplate.mType)
		{
		case CurvimationTemplate.AnimationType.Scale:
			return mBaseScale;
		case CurvimationTemplate.AnimationType.Position:
			if (mRectTransform != null)
			{
				return mBaseRectPos;
			}
			return mBasePos;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	internal void PlayCurvimation(object mUpdateTextAnim)
	{
		throw new NotImplementedException();
	}

	private void Animate(float elapsedTime)
	{
		if (mPlaying)
		{
			float num = mTemplate.mCurve.Evaluate(elapsedTime / mTemplate.mDuration) * mTemplate.mMultiplier * P_Multiplier;
			Vector3 vecToAnim = GetVecToAnim();
			Vector3 vec = GetBaseVec() + num * Vector3.one;
			vec.x = ((!mTemplate.mUseX) ? vecToAnim.x : vec.x);
			vec.y = ((!mTemplate.mUseY) ? vecToAnim.y : vec.y);
			vec.z = ((!mTemplate.mUseZ) ? vecToAnim.z : vec.z);
			ApplyVec(vec);
		}
	}

	public void FinishAnim()
	{
		Animate(mTemplate.mDuration);
	}

	public void StopAnim()
	{
		mElapsed = 0f;
		mPlaying = false;
	}

	private void Update()
	{
		if (mPlay)
		{
			PlayCurvimation(mTemplate);
			mPlay = false;
		}
		if (mElapsed >= mTemplate.mDuration)
		{
			FinishAnim();
			StopAnim();
			if (mTemplate.mLoop)
			{
				PlayCurvimation(mTemplate);
			}
		}
		else
		{
			Animate(mElapsed);
		}
		mElapsed += Time.unscaledDeltaTime;
	}
}
