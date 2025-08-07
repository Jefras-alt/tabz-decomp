using DeepSky.Haze;
using Photon;
using UnityEngine;

public class DayNightCycle : Photon.MonoBehaviour
{
	[Range(0f, 1f)]
	public float time;

	public AnimationCurve sunCurve;

	public AnimationCurve ambientSunCurve;

	public AnimationCurve ambientReflectionsSunCurve;

	public Light sun;

	public DS_HazeCore skyCore;

	public Gradient sunColor;

	[SerializeField]
	private bool mUseDynamicTime = true;

	private float secondsPerDay = 3600f;

	private void Start()
	{
		time = 0.5f;
	}

	private void Update()
	{
		sun.intensity = 2.01145f;
		sun.color = sunColor.Evaluate(time);
		sun.shadows = LightShadows.None;
		RenderSettings.ambientIntensity = 1.499999f;
		if (TABZChat.fogToggle)
		{
			sun.shadows = LightShadows.Soft;
			RenderSettings.ambientIntensity = ambientSunCurve.Evaluate(time);
			sun.intensity = sunCurve.Evaluate(time);
		}
		RenderSettings.reflectionIntensity = ambientReflectionsSunCurve.Evaluate(time);
		skyCore.Time = time;
		sun.transform.localRotation = Quaternion.Euler(time * 120f + 40f, 0f, 0f);
		time = (float)PhotonNetwork.time % secondsPerDay / secondsPerDay;
	}
}
