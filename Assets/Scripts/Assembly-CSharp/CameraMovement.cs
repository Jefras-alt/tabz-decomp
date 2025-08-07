using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public Transform cameraTarget;

	public Transform cameraRotatonTarget;

	public float rotationLerpSpeed = 10f;

	public Transform mimicLocalRotation;

	public Transform mimicTarget;

	private Transform cameraRotation;

	private StandHandler standHandler;

	private HealthHandler healthHandler;

	private bool mHasNetworkControl;

	private Quaternion mLastRotation;

	private ZombieBlackboard m_blackboard;

	public Transform realHeadRotation;

	private Transform ADSHolder;

	private Transform cameraForceTransform;

	private PhotonView view;

	public AnimationCurve forceCurve;

	public Weapon currentWeapon;

	public Transform CameraRotationTransform
	{
		get
		{
			return cameraRotation;
		}
	}

	public Vector3 CameraRotation
	{
		get
		{
			return cameraRotation.rotation.eulerAngles;
		}
	}

	private void Awake()
	{
		mHasNetworkControl = GetComponentInParent<PhotonView>().isMine;
		cameraRotation = base.transform.GetChild(0);
		cameraForceTransform = base.transform.GetChild(0).GetChild(0);
		standHandler = base.transform.root.GetComponent<StandHandler>();
		healthHandler = base.transform.root.GetComponent<HealthHandler>();
		m_blackboard = GetComponentInParent<ZombieBlackboard>();
		ADSHolder = GetComponentInChildren<ADSHolder>().transform;
		view = GetComponentInParent<PhotonView>();
	}

	public void AddCameraForce(Vector3 force, float time)
	{
		StartCoroutine(RotateForceCamera(-force, time));
	}

	private IEnumerator RotateForceCamera(Vector3 force, float time)
	{
		float nextPosition = 0f;
		float thisPosition = 0f;
		while (nextPosition < time)
		{
			nextPosition += Time.deltaTime;
			float change = forceCurve.Evaluate(nextPosition / time) - forceCurve.Evaluate(thisPosition / time);
			cameraForceTransform.Rotate(force * change, Space.Self);
			thisPosition += Time.deltaTime;
			yield return null;
		}
	}

	private void LateUpdate()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, cameraTarget.position, Time.deltaTime * 80f);
		if (PauseMenuHandler.IsPaused)
		{
			return;
		}
		InventoryService service = ServiceLocator.GetService<InventoryService>();
		if (service != null && !(service.CurrentUI == null) && (service == null || !(service.CurrentUI != null) || service.CurrentUI.IsOpen))
		{
			return;
		}
		if (mHasNetworkControl)
		{
			if (Input.GetKey(KeyCode.Mouse1) && mHasNetworkControl)
			{
				if ((bool)currentWeapon && (bool)currentWeapon.currentADS)
				{
					Vector3 position = currentWeapon.currentADS.position;
					ADSHolder.position = Vector3.Lerp(ADSHolder.position, position, Time.deltaTime * 20f);
					Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, currentWeapon.fov, Time.deltaTime * 20f);
				}
				else
				{
					Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, Time.deltaTime * 20f);
				}
			}
			else
			{
				ADSHolder.localPosition = Vector3.Lerp(ADSHolder.localPosition, Vector3.zero, Time.deltaTime * 20f);
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90f, Time.deltaTime * 20f);
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			if (m_blackboard.Fallen < -0.5f && !healthHandler.dead)
			{
				mimicTarget.localRotation = Quaternion.Lerp(mimicTarget.localRotation, mimicLocalRotation.localRotation, Time.deltaTime * rotationLerpSpeed);
				cameraRotation.Rotate(Vector3.up * Input.GetAxis("Mouse X"), Space.World);
				cameraRotation.Rotate(cameraRotation.right * (0f - Input.GetAxis("Mouse Y")), Space.World);
			}
			else
			{
				mimicTarget.rotation = Quaternion.Lerp(mimicTarget.rotation, realHeadRotation.rotation, Time.deltaTime * 5f);
			}
		}
		else
		{
			cameraRotation.rotation = mLastRotation;
		}
	}

	public void ApplyClientCameraRotation(Vector3 newRotation)
	{
		mLastRotation = Quaternion.Euler(newRotation);
	}
}
