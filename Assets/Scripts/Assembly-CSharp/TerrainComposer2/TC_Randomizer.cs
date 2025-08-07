using UnityEngine;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_Randomizer : MonoBehaviour
	{
		public TC_ItemBehaviour item;

		public TC_RandomSettings r;

		public bool randomize;

		private void Awake()
		{
			item = GetComponent<TC_ItemBehaviour>();
		}

		private void Update()
		{
			if (randomize)
			{
				randomize = false;
				Randomize();
			}
		}

		private void Randomize()
		{
			if (!(r == null) && !(item == null))
			{
				int num = Random.Range(r.amount.x, r.amount.y);
				for (int i = 0; i < num; i++)
				{
					Vector3 position = new Vector3(Random.Range(r.posX.x, r.posX.y), 0f, Random.Range(r.posZ.x, r.posZ.y));
					float y = Random.Range(r.rotY.x, r.rotY.y);
					float num2 = Random.Range(r.scaleX.x, r.scaleX.y);
					Vector3 localScale = new Vector3(num2, Random.Range(r.scaleY.x, r.scaleY.y), num2);
					TC_ItemBehaviour tC_ItemBehaviour = item.Duplicate(item.t.parent);
					tC_ItemBehaviour.t.position = position;
					tC_ItemBehaviour.t.rotation = Quaternion.Euler(0f, y, 0f);
					tC_ItemBehaviour.t.localScale = localScale;
					tC_ItemBehaviour.method = Method.Max;
				}
				TC.AutoGenerate();
			}
		}
	}
}
