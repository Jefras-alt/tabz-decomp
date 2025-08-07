using System.IO;
using Photon;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class NetworkConnector : Photon.MonoBehaviour
{
	private class UpdateMarkerPos : UnityEngine.MonoBehaviour
	{
		public GameObject toFollow;

		private void Update()
		{
			if (toFollow != null)
			{
				base.gameObject.transform.position = toFollow.transform.position;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private class UpdateMarkerPosByCamera : UnityEngine.MonoBehaviour
	{
		private void Update()
		{
			if (Camera.main != null)
			{
				base.gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
				base.gameObject.transform.rotation = Quaternion.Euler(90f, Camera.main.transform.rotation.eulerAngles.y, 0f);
			}
		}
	}

	private bool attachedToCam;

	private GameObject mapMarker;

	private Canvas mapCanvas;

	public static RawImage backgroundImage;

	public static RawImage markerImage;

	private Camera camera;

	private RenderTexture renderTexture;

	private Texture2D texture;

	private bool setCameraStuff;

	private bool grassToggle;

	public static int[,] origNormalGrassMap;

	public static int[,] origTallGrassMap;

	public static Terrain terrain;

	private void Awake()
	{
		Time.timeScale = 1f;
	}

	private void Start()
	{
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings(MainMenuHandler.VERSION_NUMBER);
			return;
		}
		MusicHandler.OnGameStarted();
		SpawnPlayer();
	}

	public virtual void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = MainMenuHandler.MaxPlayers,
			CleanupCacheOnLeave = false
		}, null);
	}

	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		MusicHandler.OnGameStarted();
		SpawnPlayer();
	}

	public void SpawnPlayer()
	{
		Vector3 randomSpawnPoint = SpawnPointManager.GetRandomSpawnPoint();
		bool iS_RED = MainMenuHandler.IS_RED;
		Color[] colors = MainMenuHandler.colors;
		int colorIndex = MainMenuHandler.colorIndex;
		Color color = colors[colorIndex];
		float r = color.r;
		float g = color.g;
		float b = color.b;
		float a = color.a;
		GameObject gameObject = PhotonNetwork.Instantiate("TABS_Player_Network", randomSpawnPoint, Quaternion.identity, 0, new object[5] { iS_RED, r, g, b, a });
		NetworkManager.OnPlayerSpawned(gameObject.GetPhotonView());
		gameObject.GetComponent<NetworkPlayerActivator>().Activate(true);
		if (!gameObject.GetPhotonView().isMine)
		{
			return;
		}
		Camera.main.cullingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Ignore")) | (1 << LayerMask.NameToLayer("Raycast")) | (1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("IngoredInEditor")) | (1 << LayerMask.NameToLayer("PlayerCollider")) | (1 << LayerMask.NameToLayer("HearSensor")) | (1 << LayerMask.NameToLayer("TerrainBox")) | (1 << LayerMask.NameToLayer("Item")) | (1 << LayerMask.NameToLayer("Tree")) | (1 << LayerMask.NameToLayer("PlayerColliderOther")) | (1 << LayerMask.NameToLayer("GiantZombie"));
		Camera componentInChildren = gameObject.transform.Find("BigSniper").GetComponentInChildren<Camera>();
		componentInChildren.cullingMask = Camera.main.cullingMask;
		componentInChildren.GetComponent<PostProcessingBehaviour>().enabled = false;
		componentInChildren.farClipPlane = 1500f;
		Camera componentInChildren2 = gameObject.transform.Find("HuntingSniper").GetComponentInChildren<Camera>();
		componentInChildren2.cullingMask = Camera.main.cullingMask;
		componentInChildren2.GetComponent<PostProcessingBehaviour>().enabled = false;
		componentInChildren2.farClipPlane = 1500f;
		if (!terrain)
		{
			terrain = Resources.FindObjectsOfTypeAll<Terrain>()[0];
		}
		if ((origNormalGrassMap == null || origTallGrassMap == null) && (bool)terrain)
		{
			origNormalGrassMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);
			origTallGrassMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 1);
		}
		if (!camera)
		{
			camera = new GameObject("minimap camera object").AddComponent<Camera>();
			camera.farClipPlane = 1500f;
			camera.nearClipPlane = 1000f;
			camera.cullingMask = 1 << LayerMask.NameToLayer("Default");
			camera.aspect = terrain.terrainData.size.x / terrain.terrainData.size.z;
			Vector3 position = terrain.GetPosition();
			position = new Vector3(position.x + terrain.terrainData.size.x / 2f, position.y + terrain.terrainData.size.y / 2f, position.z + terrain.terrainData.size.z / 2f);
			camera.transform.position = position + terrain.transform.up * 1300f;
			camera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		}
		int num = 1000;
		if (!renderTexture)
		{
			renderTexture = new RenderTexture(Mathf.RoundToInt((float)num * camera.aspect), num, 24);
			camera.targetTexture = renderTexture;
			camera.Render();
			RenderTexture.active = renderTexture;
			string path = "map.png";
			if (!texture)
			{
				texture = new Texture2D(renderTexture.width, renderTexture.height);
			}
			if (File.Exists(path))
			{
				byte[] data = File.ReadAllBytes(path);
				texture = new Texture2D(2, 2);
				texture.LoadImage(data);
			}
			else
			{
				texture.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				texture.Apply();
				File.WriteAllBytes(path, texture.EncodeToPNG());
			}
			RenderTexture.active = null;
		}
		if (!mapCanvas)
		{
			mapCanvas = new GameObject("canvas").AddComponent<Canvas>();
			mapCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			mapCanvas.enabled = true;
			mapCanvas.gameObject.SetActive(true);
		}
		string path2 = "coords.txt";
		float num2;
		float num3;
		if (File.Exists(path2))
		{
			string[] array = File.ReadAllLines(path2);
			num2 = float.Parse(array[0]);
			num3 = float.Parse(array[1]);
		}
		else
		{
			num2 = 0.12f;
			num3 = 0.88f;
			File.WriteAllLines(path2, new string[2]
			{
				string.Concat(num2),
				string.Concat(num3)
			});
		}
		float num4 = 0.25f;
		string path3 = "size.txt";
		float num5;
		if (File.Exists(path3))
		{
			string[] array2 = File.ReadAllLines(path3);
			num5 = float.Parse(array2[0]);
			num4 = float.Parse(array2[1]);
		}
		else
		{
			num5 = 0.25f;
			num4 = 0.25f;
			File.WriteAllLines(path3, new string[2]
			{
				string.Concat(num5),
				string.Concat(num4)
			});
		}
		if (!backgroundImage)
		{
			backgroundImage = new GameObject("raw back image").AddComponent<RawImage>();
			backgroundImage.transform.parent = mapCanvas.transform;
			backgroundImage.rectTransform.position = new Vector3((float)Screen.width * num2, (float)Screen.height * num3, 0f);
			backgroundImage.rectTransform.sizeDelta = new Vector2((float)Screen.width * num5, (float)Screen.height * num4);
			backgroundImage.texture = texture;
		}
		if (!markerImage)
		{
			markerImage = new GameObject("raw image").AddComponent<RawImage>();
			markerImage.transform.parent = mapCanvas.transform;
			markerImage.rectTransform.position = new Vector3((float)Screen.width * num2, (float)Screen.height * num3, 0f);
			markerImage.rectTransform.sizeDelta = new Vector2((float)Screen.width * num5, (float)Screen.height * num4);
			markerImage.texture = renderTexture;
			camera.cullingMask = 1 << LayerMask.NameToLayer("ZombeCollider");
		}
		if (!mapMarker)
		{
			mapMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			mapMarker.transform.position = randomSpawnPoint;
			mapMarker.AddComponent<UpdateMarkerPosByCamera>();
			mapMarker.layer = LayerMask.NameToLayer("ZombeCollider");
			mapMarker.transform.localScale *= 40f;
			mapMarker.GetComponent<MeshRenderer>().material.color = Color.red;
			float y = 2f;
			float num6 = 1f;
			Mesh mesh = new Mesh();
			float num7 = 1f * 0.5f;
			float num8 = num6 * 0.5f;
			Vector3[] array3 = new Vector3[5]
			{
				new Vector3(0f - num7, 0f, 0f - num8),
				new Vector3(num7, 0f, 0f - num8),
				new Vector3(num7, 0f, num8),
				new Vector3(0f - num7, 0f, num8),
				new Vector3(0f, y, 0f)
			};
			mesh.vertices = new Vector3[18]
			{
				array3[0],
				array3[1],
				array3[2],
				array3[0],
				array3[2],
				array3[3],
				array3[0],
				array3[1],
				array3[4],
				array3[1],
				array3[2],
				array3[4],
				array3[2],
				array3[3],
				array3[4],
				array3[3],
				array3[0],
				array3[4]
			};
			mesh.triangles = new int[18]
			{
				0, 1, 2, 3, 4, 5, 8, 7, 6, 11,
				10, 9, 14, 13, 12, 17, 16, 15
			};
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mapMarker.GetComponent<MeshFilter>().mesh = mesh;
			Object.Destroy(mapMarker.GetComponent<SphereCollider>());
		}
	}
}
