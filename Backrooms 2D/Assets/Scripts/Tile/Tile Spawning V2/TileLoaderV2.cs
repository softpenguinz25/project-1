using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileDataManagerV2))]
[RequireComponent(typeof(TilePoolV2))]
public class TileLoaderV2 : MonoBehaviour
{
	TileDataManagerV2 tdm;

	[Header("Chunk")]
	[Tooltip("HAS TO BE EVEN | Unit: Tiles")] [SerializeField] int chunkSize = 16;
	public static int ChunkSize = 8;

	[Header("Player")]
	[SerializeField] GameObject player;
	Vector2Int playerChunk;

	[Header("Debugging")]
	[SerializeField] bool unloadChunks = true;
	public Vector2Int PlayerChunk
	{
		get
		{
			return playerChunk;
		}
		set
		{
			if (playerChunk != value)
			{
				Vector2Int oldPlayerChunk = playerChunk;
				playerChunk = value;

				PlayerChunkChange(oldPlayerChunk);
				PlayerChunkChanged?.Invoke();
			}
		}
	}

	public static Vector2Int NULL_CP = new(int.MinValue, int.MinValue);

	//Sort CPs by Chunk => Tile => CPs
	public Dictionary<Vector2Int, Dictionary<TileV2, List<Vector2Int>>> chunkTileCPData = new();

	//Other Data
	public List<TileV2> loadTiles = new();
	public Dictionary<TileV2, List<Vector2Int>> loadCPs = new();
	public Dictionary<Vector2Int, bool> chunkLoadStates = new();
	public static Dictionary<Vector2Int, GameObject> chunkGOs = new();

	public event Action PlayerChunkChanged;
	private void OnDrawGizmos()
	{
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying) return;
		#endif

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(new Vector2(playerChunk.x - chunkSize * .5f, playerChunk.y + chunkSize * .5f), new Vector2(playerChunk.x + chunkSize * .5f, playerChunk.y + chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x + chunkSize * .5f, playerChunk.y + chunkSize * .5f), new Vector2(playerChunk.x + chunkSize * .5f, playerChunk.y - chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x + chunkSize * .5f, playerChunk.y - chunkSize * .5f), new Vector2(playerChunk.x - chunkSize * .5f, playerChunk.y - chunkSize * .5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x - chunkSize * .5f, playerChunk.y - chunkSize * .5f), new Vector2(playerChunk.x - chunkSize * .5f, playerChunk.y + chunkSize * .5f));

		Gizmos.color = new Color(.5f, 0, .5f);
		Gizmos.DrawLine(new Vector2(playerChunk.x - chunkSize * 1.5f, playerChunk.y + chunkSize * 1.5f), new Vector2(playerChunk.x + chunkSize * 1.5f, playerChunk.y + chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x + chunkSize * 1.5f, playerChunk.y + chunkSize * 1.5f), new Vector2(playerChunk.x + chunkSize * 1.5f, playerChunk.y - chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x + chunkSize * 1.5f, playerChunk.y - chunkSize * 1.5f), new Vector2(playerChunk.x - chunkSize * 1.5f, playerChunk.y - chunkSize * 1.5f));
		Gizmos.DrawLine(new Vector2(playerChunk.x - chunkSize * 1.5f, playerChunk.y - chunkSize * 1.5f), new Vector2(playerChunk.x - chunkSize * 1.5f, playerChunk.y + chunkSize * 1.5f));
	}

	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();

		chunkGOs = new();
	}

	#region Event Subbing
	private void OnEnable()
	{
		tdm.TileCPAdded += AddChunkTileCP;
		tdm.TileCPRemoved += RemoveCP;
	}

	private void OnDisable()
	{
		tdm.TileCPAdded -= AddChunkTileCP;
		tdm.TileCPRemoved -= RemoveCP;
	}
	#endregion

	private void Start()
	{
		chunkSize *= TileSpawnerV2.TileSize;
		playerChunk = Vector2Int.zero;
	}

	void AddChunkTileCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (cp != NULL_CP)
		{
			//Add CP to sortedCPs Dictionary Based On What Data is Already There
			if (chunkTileCPData.Keys.Contains(GetChunkFromPos(cpOwner.TilePosition, chunkSize)))
				if (chunkTileCPData[GetChunkFromPos(cpOwner.TilePosition, chunkSize)].Keys.Contains(cpOwner))
					chunkTileCPData[GetChunkFromPos(cpOwner.TilePosition, chunkSize)][cpOwner].Add(cp);
				else
					chunkTileCPData[GetChunkFromPos(cpOwner.TilePosition, chunkSize)].Add(cpOwner, new List<Vector2Int> { cp });
			else
			{
				chunkTileCPData.Add(GetChunkFromPos(cpOwner.TilePosition, chunkSize), new Dictionary<TileV2, List<Vector2Int>> { [cpOwner] = new List<Vector2Int> { cp } });

				//Other data aside from sortedCPs
				chunkLoadStates.Add(GetChunkFromPos(cpOwner.TilePosition, chunkSize), true);
			}
		}

		CreateChunkGO(cpOwner);
		AddLoadLists(cpOwner, cp);
	}

	private void AddLoadLists(TileV2 cpOwner, Vector2Int cp)
	{
		//Add Load Tiles, Will Always Be In Surrounding Chunks bc Load CPs Are Only In Surrounding Chunks
		if (!loadTiles.Contains(cpOwner))
			loadTiles.Add(cpOwner);

		if (cp != NULL_CP)
		{
			//Add Load CPs Based On Surrounding Chunks
			if (GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(cpOwner.TilePosition, chunkSize)))
				if (loadCPs.ContainsKey(cpOwner)) loadCPs[cpOwner].Add(cp);
				else loadCPs.Add(cpOwner, new List<Vector2Int> { cp });
		}
	}

	private void CreateChunkGO(TileV2 cpOwner)
	{
		Vector2Int chunk = GetChunkFromPos(cpOwner.TilePosition, chunkSize);

		if (chunkGOs.ContainsKey(chunk)) return;

		GameObject chunkGO = new(chunk.ToString());
		chunkGO.transform.parent = transform;
		chunkGOs.Add(chunk, chunkGO);
	}

	private void RemoveCP(TileV2 cpOwner, Vector2Int cp)
	{
		chunkTileCPData[GetChunkFromPos(cpOwner.TilePosition, chunkSize)][cpOwner].Remove(cp);

		if (GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(cpOwner.TilePosition, chunkSize)))
		{
			loadCPs[cpOwner].Remove(cp);
			if (loadCPs[cpOwner].Count <= 0)
				loadCPs.Remove(cpOwner);
		}
	}

	public static Vector2Int GetChunkFromPos(Vector2Int pos, int chunkSize) { return new Vector2Int(Mathf.RoundToInt((float)pos.x / chunkSize), Mathf.RoundToInt((float)pos.y / chunkSize)) * chunkSize; }
	public static Vector2Int GetChunkFromPos(Vector3 pos, int chunkSize) { return new Vector2Int(Mathf.RoundToInt(pos.x / chunkSize), Mathf.RoundToInt(pos.y / chunkSize)) * chunkSize; }

	public List<Vector2Int> GetSurroundingChunks(Vector2Int middleChunk)
	{
		List<Vector2Int> surroundingChunks = new();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				Vector2Int Chunk = new(middleChunk.x + chunkSize * x, middleChunk.y + chunkSize * y);
				if(chunkTileCPData.ContainsKey(Chunk)) surroundingChunks.Add(Chunk);
			}
		}

		return surroundingChunks;
	}

	void PlayerChunkChange(Vector2Int oldPlayerChunk)
	{
		ChunkChangeLoadLists();
		UnloadChunks(oldPlayerChunk);
		LoadChunks();
	}

	void ChunkChangeLoadLists()
	{
		//1. Remove Load Tiles + CPs Outside of Surrounding Chunk
		for (int loadTileIndex = loadTiles.Count - 1; loadTileIndex >= 0; loadTileIndex--)
		{
			TileV2 loadTile = loadTiles[loadTileIndex];
			if (!GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(loadTile.TilePosition, chunkSize)))
				loadTiles.Remove(loadTile);
		}

		for(int loadTileCPIndex = loadCPs.Keys.Count - 1;  loadTileCPIndex >= 0; loadTileCPIndex--)
		{
			if (!GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(loadCPs.Keys.ElementAt(loadTileCPIndex).TilePosition, chunkSize)))
				loadCPs.Remove(loadCPs.Keys.ElementAt(loadTileCPIndex));
		}
			

		//2. Add Load Tiles + CPs in Surrounding Chunks Not Already in Load CPs Dictionary
		foreach (Vector2Int surroundingChunk in GetSurroundingChunks(playerChunk))
		{
			foreach (KeyValuePair<TileV2, List<Vector2Int>> tileCPs in chunkTileCPData[surroundingChunk])
			{
				if (!loadTiles.Contains(tileCPs.Key))
					loadTiles.Add(tileCPs.Key);

				if (!loadCPs.ContainsKey(tileCPs.Key) && tileCPs.Value.Count > 0)
					loadCPs.Add(tileCPs.Key, tileCPs.Value);
			}
		}
	}

	void UnloadChunks(Vector2Int oldPlayerChunk)
	{
		if (!unloadChunks) return;

		List<Vector2Int> chunksToUnload = new();

		foreach (Vector2Int oldSurroundingPlayerChunk in GetSurroundingChunks(oldPlayerChunk))
			if (!GetSurroundingChunks(playerChunk).Contains(oldSurroundingPlayerChunk))
				chunksToUnload.Add(oldSurroundingPlayerChunk);

		foreach (Vector2Int chunkToUnload in chunksToUnload)
		{
			chunkLoadStates[chunkToUnload] = false;
			chunkGOs[chunkToUnload].SetActive(false);
		}
	}

	void LoadChunks()
	{
		foreach(Vector2Int surroundingChunk in GetSurroundingChunks(playerChunk))
			if(!chunkLoadStates[surroundingChunk])
				chunkGOs[surroundingChunk].SetActive(true);
	}

	private void Update()
	{
		PlayerChunk = GetChunkFromPos(player.transform.position, chunkSize);
		ChunkSize = chunkSize;
	}
}
