using UnityEngine;
using UnityEngine.PostProcessing;

public class QualitySettingsSetter : MonoBehaviour
{
	public enum QualitySetting
	{
		Lowest = 0,
		Low = 1,
		Medium = 2,
		Good = 3
	}

	[SerializeField]
	private PostProcessingProfile mMenuProfile;

	[SerializeField]
	private PostProcessingProfile mGameProfile;

	[SerializeField]
	private PostProcessingProfile[] mMenuPresets;

	[SerializeField]
	private PostProcessingProfile[] mGamePresets;

	private QualitySetting mCurrentQualitySetting;

	private void Start()
	{
		mCurrentQualitySetting = (QualitySetting)QualitySettings.GetQualityLevel();
		mCurrentQualitySetting = QualitySetting.Lowest;
		Debug.Log("Current quiality setting: " + mCurrentQualitySetting);
		ApplyQualitySettings();
	}

	private void ApplyQualitySettings()
	{
		mMenuProfile = mMenuPresets[(int)mCurrentQualitySetting];
		mGameProfile = mGamePresets[(int)mCurrentQualitySetting];
	}
}
