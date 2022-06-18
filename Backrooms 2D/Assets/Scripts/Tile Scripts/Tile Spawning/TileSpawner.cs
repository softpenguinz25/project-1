using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	private TileDataManager tm;
	private TileLoader tl;

	[Header("Spawning")]
	[SerializeField] public TileCollection tileCollection;
	[SerializeField] [Range(0.001f, 1)] private float timeBtwnSpawns = .001f;

	public event Action<TilePrefab> TileSpawned;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
		tl = FindObjectOfType<TileLoader>();
	}

	private void Start()
	{
		SpawnFirstTile();
	}

	private void SpawnFirstTile()
	{
		TilePrefab firstTile = Instantiate(tileCollection.firstTile.tilePrefab, Vector3.zero, Quaternion.identity);
		tm.AddTilePosition(firstTile.transform.position);
		foreach(Transform connectionPoint in firstTile.connectionPoints) tm.AddConnectionPoints(connectionPoint);

		StartCoroutine(SpawnTilesCoroutine());
	}

	private IEnumerator SpawnTilesCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeBtwnSpawns);

			//1. Spawn Tile
			List<Transform> validConnectionPoints = new List<Transform>();
			while (validConnectionPoints.Count <= 0)
			{
				foreach (Transform connectionPoint in tm.connectionPoints)
				{
					Vector2 roundedConnectionPointPos = new Vector2(Mathf.Round(connectionPoint.transform.position.x / tl.chunkSize), Mathf.Round(connectionPoint.transform.position.y / tl.chunkSize)) * tl.chunkSize;
					//Debug.Log(roundedConnectionPointPos + "" + tl.CurrentPlayerChunk().transform.position);
					if (Vector2.Distance(roundedConnectionPointPos, tl.CurrentPlayerChunk().transform.position) < Mathf.Sqrt(tl.chunkSize*tl.chunkSize+ tl.chunkSize * tl.chunkSize) + .01f)//Pythagorean theorem!
					{
						validConnectionPoints.Add(connectionPoint);
					}
					else
					{
						tl.AddChunk(roundedConnectionPointPos);
					}
				}
				
				yield return null;
			}

			yield return new WaitUntil(() => validConnectionPoints.Count > 0);

			Transform randomConnectionPoint = validConnectionPoints[UnityEngine.Random.Range(0, validConnectionPoints.Count)];
			TilePrefab tilePrefab = Instantiate(RandomSpawnChanceTile().tilePrefab, randomConnectionPoint.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90));

			//2. Rotate Tile
			Transform spawnedTileReference = randomConnectionPoint.parent;
			bool tileConnected = false;
			Transform connectedPoint;
			while (!tileConnected)
			{
				foreach (Transform connectionPoint in tilePrefab.connectionPoints)
				{
					if (Vector3.Distance(connectionPoint.position, spawnedTileReference.position) < .01f)
					{
						connectedPoint = connectionPoint;
						tileConnected = true;
					}
				}

				if (tileConnected) break;

				tilePrefab.transform.eulerAngles = new Vector3(0, 0, tilePrefab.transform.eulerAngles.z + 90);
			}

			//3. Update Manager Lists
			tm.AddTilePosition(tilePrefab.transform.position);
			foreach (Transform connectionPoint in tilePrefab.connectionPoints) tm.AddConnectionPoints(connectionPoint);
			tm.CheckConnectionPoints();

			TileSpawned?.Invoke(tilePrefab);
		}
	}

	//THANKS https://www.youtube.com/watch?v=Gj7UU5IU3-E
	private TileTemplate RandomSpawnChanceTile()
	{
		float totalDenomination = 0;

		foreach (TileTemplate tile in tileCollection.tiles)
		{
			totalDenomination += tile.spawnChance;
		}

		float randomNumber = UnityEngine.Random.Range(0, totalDenomination);	
		
		foreach(TileTemplate tile in tileCollection.tiles)
		{
			if(tile.spawnChance >= randomNumber)
			{
				return tile;
			}

			randomNumber -= tile.spawnChance;
		}

		Debug.LogError("Random Tile Generation Failed!");
		return null;
	}
}
