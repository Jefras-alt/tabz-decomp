using System;
using System.Collections.Generic;
using UnityEngine;

public class screenShake : MonoBehaviour
{
	[Serializable]
	public class shake
	{
		[SerializeField]
		private float _amount = -1f;

		[SerializeField]
		private float _time = -1f;

		[SerializeField]
		private float _timeLeft = -1f;

		[SerializeField]
		private float _startAmount = -1f;

		private int mIndex;

		public int Index
		{
			get
			{
				return mIndex;
			}
		}

		public float timeLeft
		{
			get
			{
				return _timeLeft;
			}
		}

		public float strength
		{
			get
			{
				return _amount;
			}
		}

		public shake(float amount, float time, int index)
		{
			_amount = amount;
			_time = time;
			_timeLeft = time;
			_startAmount = amount;
			mIndex = index;
		}

		public void shakeMe()
		{
			_amount -= Time.deltaTime * (_startAmount / _time) * 2f;
		}

		public void tick()
		{
			_timeLeft -= Time.deltaTime * 0.5f;
		}
	}

	[SerializeField]
	public static List<shake> _shakes = new List<shake>();

	private void Start()
	{
	}

	public void shakeThis(shake toShake)
	{
		toShake.shakeMe();
		base.transform.localRotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f - toShake.strength, toShake.strength) * 2f, base.transform.localRotation.eulerAngles.y + UnityEngine.Random.Range(0f - toShake.strength, toShake.strength), 0f));
	}

	public void tickSHake(shake toShake)
	{
		toShake.tick();
	}

	private void Update()
	{
		int num = -1;
		float num2 = 0f;
		for (int i = 0; i < _shakes.Count; i++)
		{
			tickSHake(_shakes[i]);
			if (_shakes[i].timeLeft <= 0f)
			{
				_shakes.RemoveThingyYao(_shakes[i]);
			}
			else if (_shakes[i].strength > num2)
			{
				num = i;
				num2 = _shakes[i].strength;
			}
		}
		if (num != -1)
		{
			shakeThis(_shakes[num]);
		}
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
	}

	public static void AddShake(float amount, float time)
	{
		_shakes.AddSorted(new shake(amount, time, _shakes.Count - 1));
	}

	public static void AddShake(float amount, float time, float distance, Transform caller)
	{
		float value = (distance - Vector3.Distance(Camera.main.transform.position, caller.position)) / distance;
		value = Mathf.Clamp(value, 0f, 1f);
		_shakes.AddSorted(new shake(amount * value, time * value, _shakes.Count - 1));
	}
}
