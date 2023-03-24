using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileDataManagerV2))]
public class TileLoaderV2 : MonoBehaviour
{
	TileDataManagerV2 tdm;

    [Tooltip("HAS TO BE EVEN | Unit: Tiles")]
    public int chunkSize = 16;
    public Chunk playerChunk; public void SetPlayerChunk(Chunk pc)
	{
		playerChunk = pc;
		PlayerChunkChanged?.Invoke();
	}

	public event Action PlayerChunkChanged;
	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();
	}

	#region Event Subbing
	private void OnEnable()
	{
		tdm.CPAdded += AddCP;
	}
	
	private void OnDisable()
	{
		tdm.CPAdded -= AddCP;
	}
	#endregion

	void AddCP(Vector3 cp)
	{
        if (cp.x > playerChunk.pos.x - chunkSize * .5f * 3 && cp.x < playerChunk.pos.x + chunkSize * .5f * 3 && cp.y > playerChunk.pos.y - chunkSize * .5f * 3 && cp.y < playerChunk.pos.y + chunkSize * .5f * 3) loadCPs.Add(cp);
        if (cp.x > playerChunk.pos.x - chunkSize * .5f && cp.x < playerChunk.pos.x + chunkSize * .5f && cp.y > playerChunk.pos.y - chunkSize * .5f && cp.y < playerChunk.pos.y + chunkSize * .5f) chunkCPs.Add(cp);
    }
    public List<Vector3> loadCPs; public void AddLoadCP(Vector3 loadCP) { loadCPs.Add(loadCP); }
    public List<Vector3> chunkCPs; public void AddChunkCP(Vector3 chunkCP) { chunkCPs.Add(chunkCP); }
}

public class Chunk
{
    public Vector3 pos = Vector3.one;
    public List<TileV2> tiles;
}
