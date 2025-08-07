using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_AnimateNode : MonoBehaviour
	{
		public Vector3 moveSpeed;

		public float rotSpeed;

		public float scaleSpeed;

		public float opacitySpeed;

		private TC_ItemBehaviour item;

		private bool refresh;

		private void Start()
		{
			item = GetComponent<TC_ItemBehaviour>();
		}

		private void MyUpdate()
		{
			base.transform.Rotate(0f, rotSpeed, 0f);
			base.transform.Translate(moveSpeed * 90f);
			base.transform.localScale += new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);
			if (rotSpeed != 0f || moveSpeed.x != 0f || moveSpeed.y != 0f || moveSpeed.z != 0f || scaleSpeed != 0f)
			{
				refresh = true;
			}
			if (opacitySpeed != 0f && item != null)
			{
				item.opacity = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * opacitySpeed));
				refresh = true;
			}
			if (refresh)
			{
				TC.repaintNodeWindow = true;
				TC.AutoGenerate();
			}
		}
	}
}
