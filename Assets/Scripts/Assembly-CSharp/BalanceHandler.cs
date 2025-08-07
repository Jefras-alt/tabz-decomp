using UnityEngine;

public class BalanceHandler : MonoBehaviour
{
	private Transform legDirectionLeft;

	private Transform legDirectionRight;

	public float legAngle;

	private PhysicsAmimationController animController;

	private ZombieBlackboard mBlackboard;

	public float torsoFallAngle = 360f;

	public float legFallAngle = 360f;

	private StandHandler stand;

	private PhotonView mPhotonView;

	private float cantFall;

	private void Awake()
	{
		mPhotonView = GetComponent<PhotonView>();
	}

	private void Start()
	{
		legDirectionLeft = GetComponentInChildren<LegDirectionLeft>().transform;
		legDirectionRight = GetComponentInChildren<LegDirectionRight>().transform;
		animController = GetComponent<PhysicsAmimationController>();
		stand = GetComponent<StandHandler>();
		mBlackboard = GetComponent<ZombieBlackboard>();
	}

	private void Update()
	{
		legAngle = Vector3.Angle(legDirectionRight.forward, legDirectionLeft.forward);
		if (!mPhotonView.isMine)
		{
			return;
		}
		if (stand.standMultiplier != 1)
		{
			cantFall = 1f;
			return;
		}
		cantFall -= Time.deltaTime;
		if (!(cantFall < 0f) || !(mBlackboard.Fallen < -1f))
		{
			return;
		}
		if (legFallAngle < 360f)
		{
			if (Vector3.Angle(-Vector3.up, legDirectionRight.forward + legDirectionLeft.forward) > legFallAngle)
			{
				float num = Random.Range(1f, 2f);
				mPhotonView.RPC("Fall", PhotonTargets.All, num);
			}
		}
		else if (torsoFallAngle < 360f)
		{
			if (Vector3.Angle(Vector3.up, animController.up.forward) > torsoFallAngle)
			{
				float num2 = Random.Range(1f, 2f);
				mPhotonView.RPC("Fall", PhotonTargets.All, num2);
			}
		}
		else if (stand.sinceGrounded > 2f)
		{
			mPhotonView.RPC("Fall", PhotonTargets.All, 1f, true);
		}
	}
}
