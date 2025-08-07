using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_SeedAnimate : MonoBehaviour
	{
		public float animateSpeed;

		private float time;

		private void Update()
		{
			MyUpdate();
		}

		private void MyUpdate()
		{
			if (!(TC_Settings.instance == null))
			{
				TC_Settings.instance.seed += (Time.realtimeSinceStartup - time) * animateSpeed;
				time = Time.realtimeSinceStartup;
				TC.AutoGenerate();
			}
		}
	}
}
