using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToMove : MonoBehaviour
{
	public float speed = 5f;

	public Transform[] cubes;

	public List<Vector3> PositionsQueue = new List<Vector3>(20);

	private Vector3[] cubeStartPositions;

	private int nextPosIndex;

	private float lerpTime;

	private bool recording;

	public void Start()
	{
		cubeStartPositions = new Vector3[cubes.Length];
		for (int i = 0; i < cubes.Length; i++)
		{
			Transform transform = cubes[i];
			cubeStartPositions[i] = transform.position;
		}
	}

	public void Update()
	{
		if (!PhotonNetwork.isMasterClient || recording)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
		{
			StartCoroutine("RecordMouse");
		}
		else if (PositionsQueue.Count != 0)
		{
			Vector3 vector = PositionsQueue[nextPosIndex];
			int index = ((nextPosIndex <= 0) ? (PositionsQueue.Count - 1) : (nextPosIndex - 1));
			Vector3 vector2 = PositionsQueue[index];
			lerpTime += Time.deltaTime * speed;
			for (int i = 0; i < cubes.Length; i++)
			{
				Transform transform = cubes[i];
				Vector3 b = vector + cubeStartPositions[i];
				Vector3 a = vector2 + cubeStartPositions[i];
				transform.transform.position = Vector3.Lerp(a, b, lerpTime);
			}
			if (lerpTime > 1f)
			{
				nextPosIndex = (nextPosIndex + 1) % PositionsQueue.Count;
				lerpTime = 0f;
			}
		}
	}

	public IEnumerator RecordMouse()
	{
		recording = true;
		PositionsQueue.Clear();
		while (Input.GetMouseButton(0) || Input.touchCount > 0)
		{
			yield return new WaitForSeconds(0.1f);
			Vector3 v3 = Input.mousePosition;
			if (Input.touchCount > 0)
			{
				v3 = Input.GetTouch(0).position;
			}
			Ray inputRay = Camera.main.ScreenPointToRay(v3);
			RaycastHit hit;
			if (Physics.Raycast(inputRay, out hit))
			{
				PositionsQueue.Add(hit.point);
			}
		}
		nextPosIndex = 0;
		recording = false;
	}
}
