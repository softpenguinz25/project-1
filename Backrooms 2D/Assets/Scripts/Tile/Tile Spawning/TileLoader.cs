using System;
using System.Collections.Generic;
using UnityEngine;

public class TileLoader : MonoBehaviour
{
	private TileSpawner ts;
	private TileDataManager tdm;

	[SerializeField] private List<GameObject> chunks = new List<GameObject>();
	public float chunkSize = 16;
	private float chunkSizeHypotenuse;

	private GameObject newChunk;

	[SerializeField] private GameObject player;

	private GameObject currentChunkComparison;

	public event Action<int> ChunkSpawned;
	private void Awake()
	{
		ts = FindObjectOfType<TileSpawner>();
		tdm = FindObjectOfType<TileDataManager>();
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
		chunkSizeHypotenuse = Mathf.Sqrt(chunkSize * chunkSize + chunkSize * chunkSize) + .01f;

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

		//LoadChunks();

		//if(tile.isGroupTile) Debug.Log("Tile Added to Chunk");
	}

	private void Update()
	{
		if (currentChunkComparison != CurrentPlayerChunk())
		{
			LoadChunks();
			//Debug.Log(currentChunkComparison + " vs. " + CurrentPlayerChunk());
			currentChunkComparison = CurrentPlayerChunk();
		}
	}

	private void LoadChunks()
	{
		/*List<GameObject> chunksToEnable = new List<GameObject>();
		List<GameObject> chunksToDisable = new List<GameObject>();*/
		foreach (GameObject chunk in chunks) {
			//Debug.Log("Checking chunks...");
			//Pythagorean theorem!
			if (Vector2.Distance(chunk.transform.position, CurrentPlayerChunk().transform.position) > chunkSizeHypotenuse && chunk.activeSelf)/* chunksToDisable.Add(chunk);*/chunk.gameObject.SetActive(false);
			else if (Vector2.Distance(chunk.transform.position, CurrentPlayerChunk().transform.position) <= chunkSizeHypotenuse && !chunk.activeSelf)/* chunksToEnable.Add(chunk);*/chunk.gameObject.SetActive(true);

			/*foreach (GameObject chunkToEnable in chunksToEnable) chunkToEnable.SetActive(true);
			foreach (GameObject chunkToDisable in chunksToDisable) chunkToDisable.SetActive(false);*/
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

		AddChunk(roundedPlayerPos);
		Debug.Log("Player went to undiscovered chunk; creating new chunk.");
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
