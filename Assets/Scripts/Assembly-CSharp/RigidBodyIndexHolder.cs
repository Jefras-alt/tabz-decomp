using UnityEngine;

public class RigidBodyIndexHolder : MonoBehaviour
{
	private byte mIndex;

	public byte Index
	{
		get
		{
			return mIndex;
		}
	}

	public void Init(byte index)
	{
		mIndex = index;
	}
}
