using UnityEngine;

public class MovementHandler : MonoBehaviour
{
	public float forceToAdd;

	public Rigidbody crawlRig;

	private PhysicsAmimationController animController;

	private StandHandler standHandler;

	private Rigidbody[] allRigs;

	[HideInInspector]
	public float gravity;

	private PlayerHandler player;

	private StandHandler stand;

	private ZombieBlackboard m_blackboard;

	private NoiseSpawner m_walkNoise;

	private NoiseSpawner m_runNoise;

	private PhotonView view;

	private float airControl = 1f;

	private void Start()
	{
		m_blackboard = GetComponent<ZombieBlackboard>();
		view = GetComponent<PhotonView>();
		animController = GetComponent<PhysicsAmimationController>();
		standHandler = GetComponent<StandHandler>();
		allRigs = GetComponentsInChildren<Rigidbody>();
		player = GetComponent<PlayerHandler>();
		stand = GetComponent<StandHandler>();
		if (m_blackboard.Behaviour != BTType.DEFAULT)
		{
			return;
		}
		NoiseSpawner[] componentsInChildren = GetComponentsInChildren<NoiseSpawner>();
		NoiseSpawner[] array = componentsInChildren;
		foreach (NoiseSpawner noiseSpawner in array)
		{
			if (noiseSpawner.gameObject.name == "WalkNoise")
			{
				m_walkNoise = noiseSpawner;
			}
			else if (noiseSpawner.gameObject.name == "RunNoise")
			{
				m_runNoise = noiseSpawner;
			}
			else
			{
				Debug.LogWarning("Can't handle noise: " + noiseSpawner.gameObject.name, noiseSpawner.gameObject);
			}
		}
	}

	private void Update()
	{
		Gravity();
	}

	private void Gravity()
	{
		if (gravity > 0f && (m_blackboard.Grounded || (stand.sinceGrounded < 0.1f && player.mMovementState == 4)) && player.jumpCD > 0.2f)
		{
			gravity = 0f;
		}
		else
		{
			gravity += Time.deltaTime;
		}
		for (int i = 0; i < allRigs.Length; i++)
		{
			if (m_blackboard.Fallen < 0f)
			{
				allRigs[i].AddForce(Vector3.down * m_blackboard.GravityForce * gravity * Time.deltaTime, ForceMode.Acceleration);
			}
			else
			{
				allRigs[i].AddForce(Vector3.down * m_blackboard.GravityForce * gravity * 0.2f * Time.deltaTime, ForceMode.Acceleration);
			}
		}
	}

	public void Jump()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			for (int i = 0; i < allRigs.Length; i++)
			{
				allRigs[i].velocity = Vector3.up * m_blackboard.JumpForce / 60f;
			}
			gravity = -0.3f;
		}
	}

	public void Right()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			MoveBody(m_blackboard.SideMultiplier, true);
		}
		else
		{
			MoveBody(m_blackboard.SideMultiplier * airControl, true);
		}
	}

	public void Left()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			MoveBody(0f - m_blackboard.SideMultiplier, true);
		}
		else
		{
			MoveBody((0f - m_blackboard.SideMultiplier) * airControl, true);
		}
	}

	public void Run()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			MoveBody(m_blackboard.RunMultiplier);
			if (m_blackboard.Behaviour == BTType.DEFAULT)
			{
				m_runNoise.Trigger();
			}
		}
		else
		{
			MoveBody(m_blackboard.RunMultiplier * airControl);
		}
	}

	public void Walk()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			MoveBody(m_blackboard.WalkMultiplier);
			if (m_blackboard.Behaviour == BTType.DEFAULT)
			{
				m_walkNoise.Trigger();
			}
		}
		else
		{
			MoveBody(m_blackboard.WalkMultiplier * airControl);
		}
	}

	public void TriggerWalkNoise()
	{
		if (m_blackboard.ReasonablyGrounded && m_blackboard.Behaviour == BTType.DEFAULT)
		{
			m_walkNoise.Trigger();
		}
	}

	public void TriggerRunNoise()
	{
		if (m_blackboard.ReasonablyGrounded && m_blackboard.Behaviour == BTType.DEFAULT)
		{
			m_runNoise.Trigger();
		}
	}

	public void Back()
	{
		if (m_blackboard.ReasonablyGrounded)
		{
			MoveBody(0f - m_blackboard.WalkMultiplier);
			if (m_blackboard.Behaviour == BTType.DEFAULT)
			{
				m_walkNoise.Trigger();
			}
		}
		else
		{
			MoveBody((0f - m_blackboard.WalkMultiplier) * airControl);
		}
	}

	private void MoveBody(float multiplier)
	{
		if (m_blackboard.Fallen > 0f)
		{
			return;
		}
		Vector3 groundedCamForward = animController.groundedCamForward;
		if (stand.standMultiplier != 1)
		{
			crawlRig.AddForce(groundedCamForward * forceToAdd * multiplier * 3f * Time.deltaTime, ForceMode.Acceleration);
			return;
		}
		for (int i = 0; i < allRigs.Length; i++)
		{
			groundedCamForward = animController.groundedForward;
			if (allRigs[i].tag != "IgnoreForce")
			{
				allRigs[i].AddForce(groundedCamForward * forceToAdd * multiplier * Time.deltaTime, ForceMode.Acceleration);
			}
		}
	}

	private void MoveBody(float multiplier, bool right)
	{
		if (m_blackboard.Fallen > 0f)
		{
			return;
		}
		if (stand.standMultiplier != 1)
		{
			crawlRig.AddForce(-animController.groundedCamRight * forceToAdd * 3f * multiplier * Time.deltaTime, ForceMode.Acceleration);
			return;
		}
		for (int i = 0; i < allRigs.Length; i++)
		{
			allRigs[i].AddForce(-animController.groundedCamRight * forceToAdd * multiplier * Time.deltaTime, ForceMode.Acceleration);
		}
	}
}
