using Photon;
using UnityEngine;

public class NetworkChunk : PunBehaviour
{
	private PhotonView m_view;

	private ChunkManager m_chunkManager;

	private void Start()
	{
		m_view = GetComponent<PhotonView>();
		m_chunkManager = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ChunkManager>();
	}

	private void Update()
	{
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
	}

	[PunRPC]
	private void SyncChunks(byte[] chunksPos)
	{
	}
}
