using System;
using System.Collections.Generic;
using UnityEngine;

public class TileLoader : MonoBehaviour
{
	private TileSpawner ts;

	private List<GameObject> chunks = new List<GameObject>();
	public float chunkSize = 16;

	private GameObject newChunk;

	[SerializeField] private GameObject player;

	private GameObject currentChunkComparison;

	public event Action<int> ChunkSpawned;
	private void Awake()
	{
		ts = FindObjectOfType<TileSpawner>();
	}

	private void OnEnable()
	{
		ts.TileSpawned += AddTileToChunk;
	}

	private void OnDisable()
	{
		ts.TileSpawned -= AddTileToChunk;
	}

	private void Start()
	{
		AddChunk(new Vector2(-chunkSize, chunkSize));
		AddChunk(new Vector2(0, chunkSize));
		AddChunk(new Vector2(chunkSize, chunkSize));
		AddChunk(new Vector2(-chunkSize, 0));
		AddChunk(Vector2.zero);
		AddChunk(new Vector2(chunkSize, 0));
		AddChunk(new Vector2(-chunkSize, -chunkSize));
		AddChunk(new Vector2(0, -chunkSize));
		AddChunk(new Vector2(chunkSize, -chunkSize));

		currentChunkComparison = CurrentPlayerChunk();
	}

	public void AddChunk(Vector2 pos)
	{
		if (ChunkExists(pos)) return;
		
		GameObject chunk = new GameObject(pos.ToString());
		chunk.transform.position = pos;
		chunks.Add(chunk);
		newChunk = chunk;

		ChunkSpawned?.Invoke(chunks.Count);
	}

	private void AddTileToChunk(TilePrefab tile)
	{
		Vector2 tilePos = tile.transform.position;
		Vector2 roundedTilePos = new Vector2(Mathf.Round(tilePos.x / chunkSize), Mathf.Round(tilePos.y / chunkSize)) * chunkSize;

		bool assignedToChunk = false;
		foreach(GameObject chunk in chunks)
		{
			if (Vector2.Distance(roundedTilePos, chunk.transform.position) < .01f)
			{
				tile.transform.parent = chunk.transform;
				assignedToChunk = true;
			}
		}

		if (!assignedToChunk)
		{
			AddChunk(roundedTilePos);
			tile.transform.parent = newChunk.transform;
		}

		//if(tile.isGroupTile) Debug.Log("Tile Added to Chunk");
	}

	private void Update()
	{
		if (currentChunkComparison != CurrentPlayerChunk())
		{
			LoadChunks();
			currentChunkComparison = CurrentPlayerChunk();
		}		
	}

	private void LoadChunks()
	{
		foreach (GameObject chunk in chunks) {
			//Pythagorean theorem!
			if (Vector2.Distance(chunk.transform.position, CurrentPlayerChunk().transform.position) > Mathf.Sqrt(chunkSize * chunkSize + chunkSize * chunkSize) + .01f) chunk.gameObject.SetActive(false);
			else chunk.gameObject.SetActive(true);
		}
	}

	public GameObject CurrentPlayerChunk()
	{
		Vector2 playerPos = player.transform.position;
		Vector2 roundedPlayerPos = new Vector2(Mathf.Round(playerPos.x / chunkSize), Mathf.Round(playerPos.y / chunkSize)) * chunkSize;

		foreach (GameObject chunk in chunks)
		{
			if (Vector2.Distance(roundedPlayerPos, chunk.transform.position) < .01f)
			{
				return chunk;
			}
		}

		Debug.LogError("Could Not Detect Current Player Chunk.");
		return null;
	}

	private bool ChunkExists(Vector2 pos)
	{
		foreach (GameObject chunkCheck in chunks)
		{
			if (Vector2.Distance(pos, chunkCheck.transform.position) < .01f) return true;
		}
		return false;
	}
}
