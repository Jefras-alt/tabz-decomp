using UnityEngine;
using UnityEngine.UI;

public class UIInventoryTooltip : MonoBehaviour
{
	[SerializeField]
	private Text m_textItemName;

	[SerializeField]
	private Text m_textAmmoType;

	[SerializeField]
	private Text m_textFlavour;

	private CodeStateAnimation m_stateAnimation;

	private float m_closedTimestamp;

	private bool m_isOpen;

	private void Awake()
	{
		m_stateAnimation = GetComponent<CodeStateAnimation>();
		m_closedTimestamp = Time.unscaledTime;
	}

	public void UpdateInfo(UIInventoryItem item)
	{
		if (item == null)
		{
			m_isOpen = false;
			m_closedTimestamp = Time.unscaledTime;
			return;
		}
		m_isOpen = true;
		m_stateAnimation.state1 = true;
		m_textItemName.text = item.CurrentItem.DisplayName;
		m_textFlavour.text = item.CurrentItem.FlavourText;
		if (item.CurrentItem.ItemType == InventoryService.ItemType.WEAPON)
		{
			m_textAmmoType.text = (item.CurrentItem as InventoryItemWeapon).AmmoType.ToString();
		}
		else
		{
			m_textAmmoType.text = string.Empty;
		}
	}

	private void Update()
	{
		if (!m_isOpen && Time.unscaledTime - m_closedTimestamp > 0.1f)
		{
			m_stateAnimation.state1 = false;
		}
	}
}
