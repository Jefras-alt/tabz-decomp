using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_FollowTarget : MonoBehaviour
	{
		public Transform target;

		public Vector3 offset;

		public bool refresh;

		private void MyUpdate()
		{
			if (!(target == null))
			{
				base.transform.position = target.position + offset;
				if (refresh)
				{
					TC.repaintNodeWindow = true;
					TC.AutoGenerate();
				}
			}
		}
	}
}
