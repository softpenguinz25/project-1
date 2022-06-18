using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	private TileDataManager tm;

	[Header("Spawning")]
	[SerializeField] public TileCollection tileCollection;
	[SerializeField] [Range(0.001f, 1)] private float timeBtwnSpawns = .001f;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
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
			Transform randomConnectionPoint = tm.connectionPoints[Random.Range(0, tm.connectionPoints.Count)];
			TilePrefab randomTileSelected = tileCollection.tiles[Random.Range(0, tileCollection.tiles.Count)].tilePrefab;
			TilePrefab tilePrefab = Instantiate(RandomSpawnChanceTile().tilePrefab, randomConnectionPoint.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90));

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

		float randomNumber = Random.Range(0, totalDenomination);	
		
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
