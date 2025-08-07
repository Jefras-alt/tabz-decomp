using System;
using UnityEngine;

[Serializable]
public class CurvimationTemplate
{
	public enum AnimationType
	{
		Scale = 0,
		Position = 1
	}

	public bool mLoop;

	public bool mUseX;

	public bool mUseY;

	public bool mUseZ;

	public float mDuration = 1f;

	public float mMultiplier = 1f;

	public AnimationCurve mCurve;

	public AnimationType mType;

	private void Start()
	{
	}

	private void Update()
	{
	}
}
