using UnityEngine;

public class NetworkPlayerActivator : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] m_ScriptsToHandle;

	[SerializeField]
	private GameObject[] m_ObjectsToHandle;

	[SerializeField]
	private Camera mCamera;

	public void Activate(bool active)
	{
		for (byte b = 0; b < m_ScriptsToHandle.Length; b++)
		{
			m_ScriptsToHandle[b].enabled = active;
		}
		for (byte b2 = 0; b2 < m_ObjectsToHandle.Length; b2++)
		{
			m_ObjectsToHandle[b2].SetActive(active);
		}
		mCamera.enabled = active;
		mCamera.cullingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Ignore")) | (1 << LayerMask.NameToLayer("Raycast")) | (1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("IngoredInEditor")) | (1 << LayerMask.NameToLayer("PlayerCollider")) | (1 << LayerMask.NameToLayer("HearSensor")) | (1 << LayerMask.NameToLayer("TerrainBox")) | (1 << LayerMask.NameToLayer("Item")) | (1 << LayerMask.NameToLayer("Tree")) | (1 << LayerMask.NameToLayer("PlayerColliderOther")) | (1 << LayerMask.NameToLayer("GiantZombie"));
	}
}
