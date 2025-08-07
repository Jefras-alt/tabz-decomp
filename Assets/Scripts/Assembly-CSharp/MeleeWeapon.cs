using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
	[HideInInspector]
	public float multiplier;

	public float weaponMultiplier = 1f;

	public float forceMultiplier = 1f;

	private PhotonView mPhotonView;

	private EventInstance mHitSoundEvent;

	private float cd;

	private void Awake()
	{
		mPhotonView = GetComponentInParent<PhotonView>();
		SoundEventsManager soundEventsManager = Object.FindObjectOfType<SoundEventsManager>();
		mHitSoundEvent = RuntimeManager.CreateInstance(soundEventsManager.PunchHitEvent);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (multiplier > 0f)
		{
			multiplier -= Time.deltaTime * 3f;
		}
		else
		{
			multiplier = 0f;
		}
		cd += Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (mPhotonView.isMine && !(collision.transform.root == base.transform.root) && (bool)collision.rigidbody && !(multiplier <= 0f))
		{
			float num = (1f + collision.relativeVelocity.magnitude * 0.3f) * multiplier * weaponMultiplier * 2f;
			PhotonView componentInParent = collision.transform.GetComponentInParent<PhotonView>();
			RigidBodyIndexHolder component = collision.rigidbody.GetComponent<RigidBodyIndexHolder>();
			if ((bool)component)
			{
				Vector3 vector = Camera.main.transform.forward * forceMultiplier * multiplier * (1f + collision.relativeVelocity.magnitude * 0.3f);
				componentInParent.RPC("AddForce", PhotonTargets.All, component.Index, vector, 1);
			}
			else
			{
				Debug.LogError("Rigidbody: " + collision.rigidbody.name + " Has no RigidbodyIndexHolder!, Cannot be sent to the network", collision.rigidbody);
			}
			componentInParent.RPC("TakeDamage", PhotonTargets.All, num, PhotonNetwork.player, componentInParent.GetComponent<HealthHandler>().currentHealth <= num);
			screenShake.AddShake(0.2f, 0.2f);
			if (multiplier > 0.5f && cd > 0.1f)
			{
				cd = 0f;
				PlayHitSound();
			}
		}
	}

	private void PlayHitSound()
	{
		mHitSoundEvent.set3DAttributes(base.transform.To3DAttributes());
		mHitSoundEvent.start();
	}
}
