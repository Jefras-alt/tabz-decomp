using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_AutoGenerate : MonoBehaviour
	{
		[HideInInspector]
		public CachedTransform cT = new CachedTransform();

		public bool generateOnEnable = true;

		public bool generateOnDisable = true;

		public bool instantGenerate;

		public bool waitForEndOfFrame;

		private bool generate;

		private Transform t;

		private void Start()
		{
			t = base.transform;
			cT.Copy(t);
		}

		private void Update()
		{
			MyUpdate();
		}

		private void MyUpdate()
		{
			if (cT.hasChanged(t))
			{
				cT.Copy(t);
				if (waitForEndOfFrame)
				{
					generate = true;
				}
				else
				{
					Generate();
				}
			}
		}

		private void LateUpdate()
		{
			if (generate)
			{
				Generate();
			}
		}

		private void Generate()
		{
			generate = false;
			if (instantGenerate)
			{
				TC_Generate.instance.Generate(true);
			}
			else
			{
				TC.AutoGenerate();
			}
		}

		private void OnEnable()
		{
			if (generateOnEnable)
			{
				TC.AutoGenerate();
			}
		}

		private void OnDisable()
		{
			if (generateOnDisable)
			{
				TC.AutoGenerate();
			}
		}

		private void OnDestroy()
		{
		}
	}
}
