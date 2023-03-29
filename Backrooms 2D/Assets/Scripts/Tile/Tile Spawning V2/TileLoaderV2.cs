using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileDataManagerV2))]
public class TileLoaderV2 : MonoBehaviour
{
	TileDataManagerV2 tdm;

    [Tooltip("HAS TO BE EVEN | Unit: Tiles")]
    public int chunkSize = 16;
    
	public Chunk playerChunk;

	//Sort CPs by ChunkPos => Tile => CPs
	//public SerializableDictionary<Vector2Int, SerializableDictionary<TileV2, List<Vector2Int>>> chunkTiles;
	public SerializableDictionary<TileV2, List<Vector2Int>> loadCPs = new(); 
    public SerializableDictionary<TileV2, List<Vector2Int>> chunkCPs = new(); 

	public event Action PlayerChunkChanged;
	private void OnDrawGizmos()
	{
		if (!UnityEditor.EditorApplication.isPlaying) return;

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x - chunkSize * .5f, playerChunk.pos.y + chunkSize * .5f), new Vector2(playerChunk.pos.x + chunkSize * .5f, playerChunk.pos.y + chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x + chunkSize * .5f, playerChunk.pos.y + chunkSize * .5f), new Vector2(playerChunk.pos.x + chunkSize * .5f, playerChunk.pos.y - chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x + chunkSize * .5f, playerChunk.pos.y - chunkSize * .5f), new Vector2(playerChunk.pos.x - chunkSize * .5f, playerChunk.pos.y - chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x - chunkSize * .5f, playerChunk.pos.y - chunkSize * .5f), new Vector2(playerChunk.pos.x - chunkSize * .5f, playerChunk.pos.y + chunkSize * .5f));

		Gizmos.color = new Color(.5f, 0, .5f);
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x - chunkSize * 1.5f, playerChunk.pos.y + chunkSize * 1.5f), new Vector2(playerChunk.pos.x + chunkSize * 1.5f, playerChunk.pos.y + chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x + chunkSize * 1.5f, playerChunk.pos.y + chunkSize * 1.5f), new Vector2(playerChunk.pos.x + chunkSize * 1.5f, playerChunk.pos.y - chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x + chunkSize * 1.5f, playerChunk.pos.y - chunkSize * 1.5f), new Vector2(playerChunk.pos.x - chunkSize * 1.5f, playerChunk.pos.y - chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.pos.x - chunkSize * 1.5f, playerChunk.pos.y - chunkSize * 1.5f), new Vector2(playerChunk.pos.x - chunkSize * 1.5f, playerChunk.pos.y + chunkSize * 1.5f));
	}

	public void SetPlayerChunk(Chunk pc)
	{
		playerChunk = pc;
		PlayerChunkChanged?.Invoke();
	}

	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();
	}

	#region Event Subbing
	private void OnEnable()
	{
		tdm.CPAdded += AddCP;
		tdm.TileMoved += MoveTile;
		tdm.CPRemoved += RemoveCP;
	}

	private void OnDisable()
	{
		tdm.CPAdded -= AddCP;
		tdm.TileMoved -= MoveTile;
		tdm.CPRemoved -= RemoveCP;
	}
	#endregion
	public void AddLoadCP(TileV2 owner, Vector2Int loadCP) { if (loadCPs.Keys.Contains(owner)) loadCPs[owner].Add(loadCP); else loadCPs.Add(owner, new List<Vector2Int> { loadCP }); }
	public void AddChunkCP(TileV2 owner, Vector2Int chunkCP) { if (chunkCPs.Keys.Contains(owner)) chunkCPs[owner].Add(chunkCP); else chunkCPs.Add(owner, new List<Vector2Int> { chunkCP }); }

	private void Start()
	{
		playerChunk = new Chunk(Vector2Int.zero, tdm.tileDict.Values.ToList());
	}

	void AddCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (cp.x > playerChunk.pos.x - chunkSize * 1.5f && cp.x < playerChunk.pos.x + chunkSize * 1.5f && cp.y > playerChunk.pos.y - chunkSize * 1.5f && cp.y < playerChunk.pos.y + chunkSize * 1.5f) AddLoadCP(cpOwner, cp);
		if (cp.x > playerChunk.pos.x - chunkSize * .5f && cp.x < playerChunk.pos.x + chunkSize * .5f && cp.y > playerChunk.pos.y - chunkSize * .5f && cp.y < playerChunk.pos.y + chunkSize * .5f) AddChunkCP(cpOwner, cp);

		/*Vector2Int chunkPos = new Vector2Int(Mathf.RoundToInt(cp.x / chunkSize), Mathf.RoundToInt(cp.y / chunkSize)) * chunkSize;
		if (chunkTiles.Keys.Contains(chunkPos))
		{
			if (chunkTiles[chunkPos].Contains(cpOwner))
			{
				chunkCPs.Add();
			}
		}*/
    }

	private void MoveTile(TileV2 tile, Vector2Int dir)
	{
		if (loadCPs.ContainsKey(tile)) {
			for (int i = 0; i < loadCPs[tile].Count; i++)
			{
				loadCPs[tile][i] += dir;
			}
		}

		if (chunkCPs.ContainsKey(tile)) {
			for (int i = 0; i < chunkCPs[tile].Count; i++)
			{
				chunkCPs[tile][i] += dir;
				Debug.Log("New Chunk CP: " + chunkCPs[tile][i]);
			}
		}
	}

	private void RemoveCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (loadCPs.ContainsKey(cpOwner))
		{
			loadCPs[cpOwner].Remove(cp);
			if (loadCPs[cpOwner].Count <= 0) loadCPs.Remove(cpOwner);
		}

		if (chunkCPs.ContainsKey(cpOwner))
		{
			chunkCPs[cpOwner].Remove(cp);
			if (chunkCPs[cpOwner].Count <= 0) chunkCPs.Remove(cpOwner);
		}
	}
}

public class Chunk
{
    public Vector2Int pos = Vector2Int.zero;
    public List<TileV2> tiles;

	public Chunk(Vector2Int p, List<TileV2> t)
	{
		pos = p;
		tiles = t;
	}
}
