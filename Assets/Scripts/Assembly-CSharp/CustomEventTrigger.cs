using UnityEngine;
using UnityEngine.Events;

public class CustomEventTrigger : MonoBehaviour
{
	public UnityEvent Event;

	public bool TriggerOnDisable;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Trigger()
	{
		Event.Invoke();
	}

	private void OnDisable()
	{
		if (TriggerOnDisable)
		{
			Event.Invoke();
		}
	}
}
