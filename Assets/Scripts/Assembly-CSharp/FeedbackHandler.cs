using UnityEngine;
using UnityStandardAssets.CinematicEffects;

public class FeedbackHandler : MonoBehaviour
{
	private LensAberrations aber;

	private void Awake()
	{
		if (!GetComponent<PhotonView>().isMine)
		{
			base.enabled = false;
		}
		else
		{
			aber = GetComponentInChildren<LensAberrations>(true);
		}
	}

	private void Update()
	{
		UpdateDistortion();
		UpdateAber();
		UpdateVignette();
	}

	public void DamageFeedback(float dmg)
	{
		dmg *= 4f;
		AddAber(dmg * 0.2f);
		AddVignette(Mathf.Clamp(dmg / 10f, 0f, 2f));
		screenShake.AddShake(dmg * 0.05f, 0.2f);
	}

	public void AddVignette(float amount)
	{
		if (aber.vignette.intensity < amount)
		{
			aber.vignette.intensity = amount;
		}
	}

	private void UpdateVignette()
	{
		if (aber.vignette.intensity > 0f)
		{
			aber.vignette.intensity = Mathf.Lerp(aber.vignette.intensity, 0f, Time.deltaTime * 1f);
		}
		else
		{
			aber.vignette.intensity = 0f;
		}
	}

	public void AddAber(float amount)
	{
		if (aber.chromaticAberration.amount < amount)
		{
			aber.chromaticAberration.amount = amount;
		}
	}

	private void UpdateAber()
	{
		if (aber.chromaticAberration.amount > 0f)
		{
			aber.chromaticAberration.amount = Mathf.Lerp(aber.chromaticAberration.amount, 0f, Time.deltaTime * 1f);
		}
		else
		{
			aber.chromaticAberration.amount = 0f;
		}
	}

	public void AddDistortion(float amount)
	{
		if (aber.distortion.amount < amount)
		{
			aber.distortion.amount = amount;
		}
	}

	private void UpdateDistortion()
	{
		if (aber.distortion.amount > 0f)
		{
			aber.distortion.amount = Mathf.Lerp(aber.distortion.amount, 0f, Time.deltaTime * 5f);
		}
		else
		{
			aber.distortion.amount = 0f;
		}
	}
}
