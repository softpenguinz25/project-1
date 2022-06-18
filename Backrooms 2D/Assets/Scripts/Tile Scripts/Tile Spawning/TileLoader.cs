using System.Collections.Generic;
using UnityEngine;

public class TileLoader : MonoBehaviour
{
	private TileSpawner ts;

	[SerializeField] private List<GameObject> chunks = new List<GameObject>();
	[SerializeField] private float chunkSize = 16;

	private GameObject newChunk;

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
		SpawnChunk(Vector2.zero);
	}

	private void SpawnChunk(Vector2 pos)
	{
		GameObject chunk = new GameObject(pos.ToString());
		chunk.transform.position = pos;
		chunks.Add(chunk);
		newChunk = chunk;
	}

	private void AddTileToChunk(TilePrefab tile)
	{
		Vector3 tilePos = tile.transform.position;
		Vector3 roundedTilePos = new Vector3(Mathf.Round(tilePos.x / chunkSize), Mathf.Round(tilePos.y / chunkSize), Mathf.Round(tilePos.z / chunkSize)) * chunkSize;

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
			SpawnChunk(roundedTilePos);
			tile.transform.parent = newChunk.transform;
		}
	}
}
