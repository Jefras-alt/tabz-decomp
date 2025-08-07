using FMOD.Studio;
using FMODUnity;
using Photon;
using UnityEngine;

public class ProjectileHit : Photon.MonoBehaviour
{
	public enum HitTypes
	{
		Ground = 0,
		Dude = 1
	}

	private const string HIT_TYPE_PARAM = "hitType";

	public float damage;

	public float force;

	public float fall = 1f;

	public GameObject effect;

	private EventInstance mSoundEvent;

	private static SoundEffectsManager mSoundEffectManager;

	private void Awake()
	{
	}

	public void InitHitSoundEvent(EventInstance soundEvent)
	{
		mSoundEvent = soundEvent;
	}

	public void Hit(Rigidbody rigidbody, Vector3 projectileHitPoint, Vector3 projectileeHitNormal, Vector3 projectileHitDirection, PhotonPlayer sender)
	{
		GameObject gameObject = Object.Instantiate(effect, projectileHitPoint, Quaternion.LookRotation(projectileeHitNormal));
		if ((bool)rigidbody)
		{
			HealthHandler componentInParent = rigidbody.transform.GetComponentInParent<HealthHandler>();
			if ((bool)componentInParent)
			{
				if (sender == PhotonNetwork.player)
				{
					float num = 1f;
					DamageMultplier component = rigidbody.GetComponent<DamageMultplier>();
					if ((bool)component)
					{
						num = component.multiplier;
					}
					componentInParent.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damage * num, PhotonNetwork.player, componentInParent.currentHealth <= damage * num);
					if (fall > 0f)
					{
						componentInParent.GetComponent<PhotonView>().RPC("Fall", PhotonTargets.All, fall, true);
					}
					RigidBodyIndexHolder component2 = rigidbody.GetComponent<RigidBodyIndexHolder>();
					if ((bool)component2)
					{
						byte index = component2.Index;
						componentInParent.GetComponent<PhotonView>().RPC("AddForce", PhotonTargets.All, index, projectileHitDirection * force, 1);
					}
				}
				PlayHitSound(HitTypes.Dude, projectileHitPoint);
				return;
			}
		}
		PlayHitSound(HitTypes.Ground, projectileHitPoint);
	}

	private void PlayHitSound(HitTypes type, Vector3 pos)
	{
		if (!(mSoundEvent == null))
		{
			mSoundEvent.setParameterValue("hitType", (float)type);
			mSoundEvent.set3DAttributes(pos.To3DAttributes());
			mSoundEvent.start();
		}
	}
}
