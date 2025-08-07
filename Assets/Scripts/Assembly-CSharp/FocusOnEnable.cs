using UnityEngine;
using UnityEngine.EventSystems;

public class FocusOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		Debug.Log("Focus me", base.gameObject);
		EventSystem.current.SetSelectedGameObject(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
