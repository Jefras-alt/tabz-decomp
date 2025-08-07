using System;
using FMODUnity;
using UnityEngine;

public class SoundEventsManager : MonoBehaviour
{
	[Serializable]
	public struct WeaponSound
	{
		public GameObject WeaponName;

		[EventRef]
		public string WeaponEvent;

		[EventRef]
		public string HitEvent;

		[EventRef]
		public string ReloadEvent;
	}

	[Serializable]
	public struct ScreamSound
	{
		public GameObject ZombiePrefab;

		[EventRef]
		public string ScreamEvent;
	}

	public struct WeaponSoundWrapper
	{
		public string WeaponEvent;

		public string ReloadEvent;

		public string HitEvent;
	}

	[SerializeField]
	[EventRef]
	[Header("Music")]
	private string m_MusicEvent;

	[SerializeField]
	[EventRef]
	private string m_GangstaEvent;

	[SerializeField]
	[EventRef]
	[Header("Menu Sounds")]
	private string m_MenuEvent;

	[SerializeField]
	[EventRef]
	[Header("Ambience Sounds")]
	private string m_AmbienceEvent;

	[SerializeField]
	[EventRef]
	[Header("Interface Sounds")]
	private string m_PickUpItemSound;

	[SerializeField]
	[EventRef]
	private string m_InventoryClickSound;

	[SerializeField]
	[EventRef]
	private string m_MenuClickSound;

	[SerializeField]
	[EventRef]
	[Header("Player Sounds")]
	private string m_WalkingEvent;

	[SerializeField]
	[EventRef]
	private string m_JumpEvent;

	[SerializeField]
	[EventRef]
	private string m_ProneEvent;

	[SerializeField]
	[EventRef]
	private string m_TalkQuietEvent;

	[SerializeField]
	[EventRef]
	private string m_TalkLoudEvent;

	[SerializeField]
	[EventRef]
	private string m_BreathEvent;

	[SerializeField]
	private WeaponSound[] m_Weapons;

	[SerializeField]
	[EventRef]
	private string m_HolsterWeaponEvent;

	[SerializeField]
	[EventRef]
	private string m_RaiseWeaponEvent;

	[SerializeField]
	[EventRef]
	private string m_PickUpWeaponEvent;

	[SerializeField]
	[EventRef]
	private string m_PunchSwingEvent;

	[SerializeField]
	[EventRef]
	private string m_PunchHitEvent;

	[SerializeField]
	[EventRef]
	private string m_DamageEvent;

	[SerializeField]
	[EventRef]
	[Header("Zombie Sounds")]
	private string m_IdleEvent;

	[SerializeField]
	private ScreamSound[] m_ScreamEvents;

	[SerializeField]
	[EventRef]
	private string m_DefaultZombieScreams;

	[SerializeField]
	[EventRef]
	private string m_DeathEvent;

	[SerializeField]
	[EventRef]
	private string m_ZombieDamageEvent;

	private static GameObject _Instance;

	public string MusicEvent
	{
		get
		{
			return m_MusicEvent;
		}
	}

	public string GangstaEvent
	{
		get
		{
			return m_GangstaEvent;
		}
	}

	public string MenuEvent
	{
		get
		{
			return m_MenuEvent;
		}
	}

	public string AmbienceEvent
	{
		get
		{
			return m_AmbienceEvent;
		}
	}

	public string PickUpItem
	{
		get
		{
			return m_PickUpItemSound;
		}
	}

	public string InventoryClick
	{
		get
		{
			return m_InventoryClickSound;
		}
	}

	public string MenuClick
	{
		get
		{
			return m_MenuClickSound;
		}
	}

	public string WalkingEvent
	{
		get
		{
			return m_WalkingEvent;
		}
	}

	public string JumpEvent
	{
		get
		{
			return m_JumpEvent;
		}
	}

	public string ProneEvent
	{
		get
		{
			return m_ProneEvent;
		}
	}

	public string TalkLoudEvent
	{
		get
		{
			return m_TalkLoudEvent;
		}
	}

	public string TalkQuiet
	{
		get
		{
			return m_TalkQuietEvent;
		}
	}

	public string BreathEvent
	{
		get
		{
			return m_BreathEvent;
		}
	}

	public string HolsterEvent
	{
		get
		{
			return m_HolsterWeaponEvent;
		}
	}

	public string RaiseEvent
	{
		get
		{
			return m_RaiseWeaponEvent;
		}
	}

	public string PickUpEvent
	{
		get
		{
			return m_PickUpWeaponEvent;
		}
	}

	public string PunchSwingEvent
	{
		get
		{
			return m_PunchSwingEvent;
		}
	}

	public string PunchHitEvent
	{
		get
		{
			return m_PunchHitEvent;
		}
	}

	public string DamageEvent
	{
		get
		{
			return m_DamageEvent;
		}
	}

	public string ZombieDeathEvent
	{
		get
		{
			return m_DeathEvent;
		}
	}

	public string ZombieDamageEvent
	{
		get
		{
			return m_ZombieDamageEvent;
		}
	}

	public WeaponSoundWrapper GetWeaponSoundEvent(string name)
	{
		for (ushort num = 0; num < m_Weapons.Length; num++)
		{
			WeaponSound wSound = m_Weapons[num];
			if (wSound.WeaponName.name == name)
			{
				return ToWrapper(wSound);
			}
		}
		Debug.LogError("Could not find Sound For Weapon: " + name);
		return default(WeaponSoundWrapper);
	}

	public string GetScreamEvent(GameObject prefab)
	{
		for (ushort num = 0; num < m_ScreamEvents.Length; num++)
		{
			ScreamSound screamSound = m_ScreamEvents[num];
			if (screamSound.ZombiePrefab.name == prefab.name.Split('(')[0])
			{
				return screamSound.ScreamEvent;
			}
		}
		return m_DefaultZombieScreams;
	}

	private void Awake()
	{
		if (_Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		_Instance = base.gameObject;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public WeaponSoundWrapper ToWrapper(WeaponSound wSound)
	{
		WeaponSoundWrapper result = default(WeaponSoundWrapper);
		result.WeaponEvent = wSound.WeaponEvent;
		result.ReloadEvent = wSound.ReloadEvent;
		result.HitEvent = wSound.HitEvent;
		return result;
	}
}
