using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
	private static Transform m_playerTransform;

	[SerializeField]
	private GameObject m_canvasPrefab;

	[SerializeField]
	private Transform m_canvasOriginTransform;

	private PhotonView m_photonView;

	private Transform m_canvasInstance;

	private Text m_textPlayerName;

	private CodeStateAnimation m_panelAnimation;

	private void Awake()
	{
		m_photonView = GetComponent<PhotonView>();
		if (m_photonView.isMine)
		{
			m_playerTransform = m_canvasOriginTransform;
			return;
		}
		GameObject gameObject = Object.Instantiate(m_canvasPrefab, base.transform.root);
		m_textPlayerName = gameObject.GetComponentInChildren<Text>();
		m_panelAnimation = gameObject.GetComponentInChildren<CodeStateAnimation>();
		m_textPlayerName.text = m_photonView.owner.NickName;
		m_canvasInstance = gameObject.transform;
	}

	private void LateUpdate()
	{
		if (!m_photonView.isMine && m_playerTransform != null)
		{
			m_panelAnimation.state1 = Vector3.SqrMagnitude(m_playerTransform.position - m_canvasOriginTransform.position) < 25f;
			m_canvasInstance.position = m_canvasOriginTransform.position;
		}
	}

	private void OnDestroy()
	{
		if (m_photonView.isMine)
		{
			m_playerTransform = null;
		}
	}
}
