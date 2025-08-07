using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
	[SerializeField]
	private LayerMask m_itemLayers;

	[SerializeField]
	private Text m_interactionInfo;

	[SerializeField]
	private CurvimationTemplate mFadeInAnim;

	[SerializeField]
	private CurvimationTemplate mFadeOutAnim;

	[SerializeField]
	private CurvimationTemplate mPickUpAnim;

	private Curvimate mCurvimate;

	private bool m_ToggleConnectedText;

	private Curvimate P_Curvimate
	{
		get
		{
			return mCurvimate ?? (mCurvimate = m_interactionInfo.gameObject.GetComponent<Curvimate>());
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		string empty = string.Empty;
		bool flag = false;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f));
		RaycastHit hitInfo;
		if (Physics.SphereCast(ray, 0.5f, out hitInfo, 2f, m_itemLayers))
		{
			InventoryItem componentInParent = hitInfo.collider.GetComponentInParent<InventoryItem>();
			if (componentInParent != null)
			{
				if (P_Curvimate.P_ActiveTemplate != mFadeInAnim)
				{
					P_Curvimate.PlayCurvimation(mFadeInAnim);
					m_interactionInfo.text = "Press E to pick up " + componentInParent.DisplayName;
				}
				flag = true;
				if (Input.GetKeyDown(KeyCode.E) && componentInParent.TryPickup())
				{
					P_Curvimate.PlayCurvimation(mPickUpAnim);
				}
			}
		}
		if (!flag && P_Curvimate != null && P_Curvimate.P_ActiveTemplate != mFadeOutAnim && P_Curvimate.P_ActiveTemplate != mPickUpAnim)
		{
			P_Curvimate.PlayCurvimation(mFadeOutAnim);
		}
		if (base.transform.localScale.magnitude < 0.1f && !flag)
		{
			m_interactionInfo.text = string.Empty;
			Debug.Log("remove text");
		}
	}
}
