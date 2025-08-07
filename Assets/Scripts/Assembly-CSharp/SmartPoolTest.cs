using UnityEngine;

public class SmartPoolTest : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Spawn brick (click them to despawn)"))
		{
			GameObject gameObject = SmartPool.Spawn("Brick");
			if ((bool)gameObject)
			{
				gameObject.transform.localPosition = Random.insideUnitSphere * 10f;
			}
		}
		if (GUILayout.Button("Despawn all bricks (and see them cull automatically)"))
		{
			SmartPool.DespawnAllItems("Brick");
		}
		if (GUILayout.Button("Spawn bullet (click them to despawn)"))
		{
			GameObject gameObject2 = SmartPool.Spawn("Bullet");
			if ((bool)gameObject2)
			{
				gameObject2.transform.localPosition = Random.insideUnitSphere * 10f;
			}
		}
		if (GUILayout.Button("Despawn all bullets"))
		{
			SmartPool.DespawnAllItems("Bullet");
		}
		GUILayout.Label("Please add Example and Example2ndScene as levels to the build settings!");
		if (GUILayout.Button("Switch Scene"))
		{
			Application.LoadLevel(1);
		}
	}
}
