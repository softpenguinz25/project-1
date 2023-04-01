using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileDataManagerV2))]
public class TileLoaderV2 : MonoBehaviour
{
	TileDataManagerV2 tdm;

	[Header("Chunk")]
	[Tooltip("HAS TO BE EVEN | Unit: Tiles")] public int chunkSize = 16;
	public static int ChunkSize;

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

	//Sort CPs by Chunk => Tile => CPs
	public Dictionary<Vector2Int, Dictionary<TileV2, List<Vector2Int>>> sortedCPs = new();
	public Dictionary<Vector2Int, bool> chunkLoadStates = new();
	public static Dictionary<Vector2Int, GameObject> chunkGOs = new();
	public Dictionary<TileV2, List<Vector2Int>> LoadCPs = new();

	public event Action PlayerChunkChanged;
	private void OnDrawGizmos()
	{
		if (!UnityEditor.EditorApplication.isPlaying) return;

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
	}

	#region Event Subbing
	private void OnEnable()
	{
		tdm.CPAdded += AddCP;
		tdm.CPRemoved += RemoveCP;
	}

	private void OnDisable()
	{
		tdm.CPAdded -= AddCP;
		tdm.CPRemoved -= RemoveCP;
	}
	#endregion

	private void Start()
	{
		playerChunk = Vector2Int.zero;
	}

	void AddCP(TileV2 cpOwner, Vector2Int cp)
	{
		//Add CP to sortedCPs Dictionary Based On What Data is Already There
		if (sortedCPs.Keys.Contains(GetChunkFromPos(cpOwner.tilePosition, chunkSize)))
			if (sortedCPs[GetChunkFromPos(cpOwner.tilePosition, chunkSize)].Keys.Contains(cpOwner))
				sortedCPs[GetChunkFromPos(cpOwner.tilePosition, chunkSize)][cpOwner].Add(cp);
			else
				sortedCPs[GetChunkFromPos(cpOwner.tilePosition, chunkSize)].Add(cpOwner, new List<Vector2Int> { cp });
		else
		{
			sortedCPs.Add(GetChunkFromPos(cpOwner.tilePosition, chunkSize), new Dictionary<TileV2, List<Vector2Int>> { [cpOwner] = new List<Vector2Int> { cp } });
			
			//Other data aside from sortedCPs
			chunkLoadStates.Add(GetChunkFromPos(cpOwner.tilePosition, chunkSize), true);
			CreateChunkGO(cpOwner);
		}

		//Add Load CPs Based On Surrounding Chunks
		if (GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(cpOwner.tilePosition, chunkSize)))
			if (LoadCPs.ContainsKey(cpOwner)) LoadCPs[cpOwner].Add(cp);
			else LoadCPs.Add(cpOwner, new List<Vector2Int> {cp});
	}

	private void CreateChunkGO(TileV2 cpOwner)
	{
		GameObject chunk = new(GetChunkFromPos(cpOwner.tilePosition, chunkSize).ToString());
		chunk.transform.parent = transform;

		if (chunkGOs.ContainsKey(GetChunkFromPos(cpOwner.tilePosition, chunkSize)))
			chunkGOs.Remove(GetChunkFromPos(cpOwner.tilePosition, chunkSize));

		chunkGOs.Add(GetChunkFromPos(cpOwner.tilePosition, chunkSize), chunk);
	}

	private void RemoveCP(TileV2 cpOwner, Vector2Int cp)
	{
		sortedCPs[GetChunkFromPos(cpOwner.tilePosition, chunkSize)][cpOwner].Remove(cp);

		if (GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(cpOwner.tilePosition, chunkSize)))
		{
			LoadCPs[cpOwner].Remove(cp);
			if (LoadCPs[cpOwner].Count <= 0)
				LoadCPs.Remove(cpOwner);
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
				if(sortedCPs.ContainsKey(Chunk)) surroundingChunks.Add(Chunk);
			}
		}

		return surroundingChunks;
	}

	void PlayerChunkChange(Vector2Int oldPlayerChunk)
	{
		ChunkChangeLoadCPs();
		UnloadChunks(oldPlayerChunk);
		LoadChunks();
	}

	void ChunkChangeLoadCPs()
	{
		//1. Remove Load CPs Outside of Surrounding Chunk
		foreach (TileV2 loadTile in LoadCPs.Keys)
			if (!GetSurroundingChunks(playerChunk).Contains(GetChunkFromPos(loadTile.tilePosition, chunkSize)))
				LoadCPs.Remove(loadTile);

		//2. Add Load CPs in Surrounding Chunks Not Already in Load CPs Dictionary
		foreach (Vector2Int surroundingChunk in GetSurroundingChunks(playerChunk))
		{
			foreach (KeyValuePair<TileV2, List<Vector2Int>> tileCPs in sortedCPs[surroundingChunk])
			{
				if (!LoadCPs.ContainsKey(tileCPs.Key) && tileCPs.Value.Count > 0)
				{
					LoadCPs.Add(tileCPs.Key, tileCPs.Value);
				}
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
