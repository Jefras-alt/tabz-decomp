using Photon;
using UnityEngine;

public class WalkStyleRandomizer : PunBehaviour
{
	private float mStep;

	private float mOutforce;

	private float mLiftForce;

	private float mSwitchAngle;

	private PhotonView myView;

	private void Awake()
	{
		myView = GetComponent<PhotonView>();
	}

	private void Start()
	{
		PhysicsAmimationController component = GetComponent<PhysicsAmimationController>();
		mStep = Random.Range(0.1f, 0.4f);
		mOutforce = Random.Range(0.9f, 1.1f);
		mLiftForce = 1f;
		mSwitchAngle = Random.Range(30f, 100f);
		component.stepLengthRun = mStep;
		component.stepLengthWalk = mStep;
		component.switchAngle = mSwitchAngle;
		JointAnimation[] componentsInChildren = GetComponentsInChildren<JointAnimation>();
		foreach (JointAnimation jointAnimation in componentsInChildren)
		{
			jointAnimation.torqueOut *= mOutforce;
			jointAnimation.TorqueIn *= mOutforce;
			jointAnimation.TorqueInWalk *= mOutforce;
			jointAnimation.torqueOutWalk *= mOutforce;
			jointAnimation.TorqueInBack *= mOutforce;
			jointAnimation.torqueOutBack *= mOutforce;
		}
		myView.RPC("RecieveSillyWalk", PhotonTargets.Others, mStep, mSwitchAngle, mOutforce, mLiftForce, myView.viewID);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected(newPlayer);
		myView.RPC("RecieveSillyWalk", newPlayer, mStep, mSwitchAngle, mOutforce, mLiftForce, myView.viewID);
	}

	[PunRPC]
	public void RecieveSillyWalk(float step, float switchA, float outForce, float lift, int viewID)
	{
		if (GetComponent<PhotonView>().viewID == viewID)
		{
			PhysicsAmimationController component = GetComponent<PhysicsAmimationController>();
			component.stepLengthRun = step;
			component.stepLengthWalk = step;
			component.switchAngle = switchA;
			GetComponent<StandHandler>().liftForces[0] *= lift;
			JointAnimation[] componentsInChildren = GetComponentsInChildren<JointAnimation>();
			foreach (JointAnimation jointAnimation in componentsInChildren)
			{
				jointAnimation.torqueOut *= outForce;
				jointAnimation.TorqueInWalk *= outForce;
				jointAnimation.TorqueInBack *= outForce;
			}
		}
	}
}
