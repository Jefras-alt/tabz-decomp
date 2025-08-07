using System.Collections;
using UnityEngine;

public class MenuTimeHandler : MonoBehaviour
{
	public bool done;

	private float speed;

	public AnimationCurve curve;

	public CodeStateAnimation anim;

	public CodeStateAnimation anim2;

	private bool mStarted;

	public void Done()
	{
		mStarted = true;
		StartCoroutine(MenuTime());
	}

	private void Update()
	{
		if (mStarted)
		{
			if (Input.anyKey)
			{
				anim.state1 = true;
				anim2.state1 = true;
			}
			if (done)
			{
				base.transform.Rotate(Vector3.up * speed * -10f * Time.unscaledDeltaTime);
			}
		}
	}

	private IEnumerator StartSpinning()
	{
		float t = 0f;
		while (t < 1f)
		{
			speed = curve.Evaluate(t);
			t += Time.unscaledDeltaTime * 0.3f;
			yield return null;
		}
		speed = 1f;
	}

	private IEnumerator MenuTime()
	{
		Time.timeScale = 0f;
		yield return new WaitForEndOfFrame();
		yield return new WaitForSecondsRealtime(1f);
		Time.timeScale = 1f;
		Object.FindObjectOfType<MusicHandler>().StartMusic();
		yield return new WaitForSecondsRealtime(3.8f);
		while ((double)Time.timeScale > 0.0051)
		{
			Time.timeScale = Mathf.Lerp(Time.timeScale, 0.005f, Time.unscaledDeltaTime * 20f);
			yield return null;
		}
		Time.timeScale = 0.005f;
		yield return new WaitForSecondsRealtime(1.4f);
		Time.timeScale = 1f;
		anim.state1 = true;
		anim2.state1 = true;
		yield return new WaitForSecondsRealtime(0.4f);
		while ((double)Time.timeScale > 0.0051)
		{
			Time.timeScale = Mathf.Lerp(Time.timeScale, 0.005f, Time.unscaledDeltaTime * 10f);
			yield return null;
		}
		Time.timeScale = 0.005f;
		yield return new WaitForSecondsRealtime(4.4f);
		yield return null;
		StartCoroutine(StartSpinning());
		done = true;
	}
}
