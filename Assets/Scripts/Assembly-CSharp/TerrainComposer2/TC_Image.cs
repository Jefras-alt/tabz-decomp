using UnityEngine;

namespace TerrainComposer2
{
	public class TC_Image : MonoBehaviour
	{
		public RenderTexture rt;

		public int referenceCount;

		public bool isDestroyed;

		public bool callDestroy;

		private void Awake()
		{
			if (isDestroyed)
			{
				TC_Settings.instance.imageList.Add(this);
			}
			if (!callDestroy)
			{
				TC.RefreshOutputReferences(6);
				referenceCount = 0;
			}
			else
			{
				callDestroy = false;
			}
		}

		private void OnDestroy()
		{
			if (!callDestroy)
			{
				TC.RefreshOutputReferences(6);
			}
		}

		private void DestroyMe()
		{
			TC_Settings instance = TC_Settings.instance;
			if (!(instance == null) && instance.imageList != null)
			{
				int num = instance.imageList.IndexOf(this);
				if (num != -1)
				{
					instance.imageList.RemoveAt(num);
				}
				TC_Compute.DisposeRenderTexture(ref rt);
				Object.Destroy(base.gameObject);
			}
		}

		public void UnregisterReference()
		{
			referenceCount--;
			if (referenceCount <= 0)
			{
				isDestroyed = true;
				callDestroy = true;
				DestroyMe();
			}
		}
	}
}
