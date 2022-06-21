using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	private TileDataManager tm;
	private TileLoader tl;

	[Header("Spawning")]
	public TileCollection tileCollection;
	[SerializeField] [Range(0.001f, 1)] private float timeBtwnSpawns = .001f;

	public event Action<TilePrefab> TileSpawned;

	private bool canSpawnTilesEvent = false;
	public event Action<bool> CanSpawnTiles;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
		tl = FindObjectOfType<TileLoader>();
		if (!tl.enabled) tl = null;
	}

	private void Start()
	{
		SpawnFirstTile();
	}

	private void SpawnFirstTile()
	{
		TilePrefab firstTile = Instantiate(tileCollection.firstTile.tilePrefab, Vector3.zero, Quaternion.identity);
		tm.AddTilePosition(firstTile);
		foreach(Transform connectionPoint in firstTile.connectionPoints) tm.AddConnectionPoints(connectionPoint);

		StartCoroutine(SpawnTilesCoroutine());
	}

	private IEnumerator SpawnTilesCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeBtwnSpawns);

			Transform randomConnectionPoint = null;
			//1. Pick Connection Point
			if (tl != null)
			{
				List<Transform> validConnectionPoints = new List<Transform>();
				while (validConnectionPoints.Count <= 0)
				{
					//Debug.Log("(Spawn Tile) Valid Connection Points Count: " + validConnectionPoints.Count);
					//Debug.Log("(Spawn Tile) TileManager Connection Points Count: " + tm.connectionPoints.Count);
					foreach (Transform connectionPoint in tm.connectionPoints)
					{
						Vector2 roundedConnectionPointPos = new Vector2(Mathf.Round(connectionPoint.transform.position.x / tl.chunkSize), Mathf.Round(connectionPoint.transform.position.y / tl.chunkSize)) * tl.chunkSize;
						//Debug.Log("(Spawn Tile) Rounded Connection Pos: " + roundedConnectionPointPos);
						//Debug.Log("(Spawn Tile) Current Player Chunk: " + tl.CurrentPlayerChunk());
						if (Vector2.Distance(roundedConnectionPointPos, tl.CurrentPlayerChunk().transform.position) < Mathf.Sqrt(tl.chunkSize * tl.chunkSize + tl.chunkSize * tl.chunkSize) + .01f)//Pythagorean theorem!
						{
							validConnectionPoints.Add(connectionPoint);
							//Debug.Log("(Spawn Tile) Added Valid Connection Point: " + connectionPoint);
						}
						else
						{
							tl.AddChunk(roundedConnectionPointPos);
							//Debug.Log("(Spawn Tile) Chunk Added at Position: " + roundedConnectionPointPos);
						}
					}

					if (validConnectionPoints.Count <= 0 && canSpawnTilesEvent)
					{
						CanSpawnTiles?.Invoke(false);

						canSpawnTilesEvent = false;
					}
					else if (validConnectionPoints.Count > 0) break;

					yield return null;
				}
				//yield return new WaitUntil(() => validConnectionPoints.Count > 0);

				randomConnectionPoint = validConnectionPoints[UnityEngine.Random.Range(0, validConnectionPoints.Count)];
				//Debug.Log("Random Connection Point Found: " + randomConnectionPoint);
			}
			else
			{
				randomConnectionPoint = tm.connectionPoints[UnityEngine.Random.Range(0, tm.connectionPoints.Count)];
			}

			//2. Spawn Tile
			TilePrefab tilePrefab = Instantiate(RandomSpawnChanceTile().tilePrefab, randomConnectionPoint.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90));

			//2.5. Apply Group Tile Settings (If Applicable)
			if (tilePrefab.isGroupTile)
			{
				//Correct Rotation
				tilePrefab.transform.eulerAngles = Vector3.zero;

				//Correct Position
				float epsilon = .01f;
				Vector2 diffBetweenConnectionAndTile = randomConnectionPoint.position - randomConnectionPoint.parent.position;

				bool connectionPointIsUp = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y > epsilon;
				bool connectionPointIsRight = diffBetweenConnectionAndTile.x > epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;
				bool connectionPointIsDown = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y < -epsilon;
				bool connectionPointIsLeft = diffBetweenConnectionAndTile.x < -epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;

				/*Debug.Log("Up: " + connectionPointIsUp);
				Debug.Log("Right: " + connectionPointIsRight);
				Debug.Log("Down: " + connectionPointIsDown);
				Debug.Log("Left: " + connectionPointIsLeft);*/

				if (!((connectionPointIsUp && tilePrefab.canConnectDown) || (connectionPointIsRight && tilePrefab.canConnectLeft) || (connectionPointIsDown && tilePrefab.canConnectUp) || (connectionPointIsLeft && tilePrefab.canConnectRight)))
				{
					Destroy(tilePrefab.gameObject);
					continue;
				}
				else if (connectionPointIsUp && tilePrefab.canConnectDown) { Vector2 newPos = (Vector2)tilePrefab.transform.position + tilePrefab.positionOffsetValueDown; tilePrefab.transform.position = newPos; }
				else if (connectionPointIsRight && tilePrefab.canConnectLeft) { Vector2 newPos = (Vector2)tilePrefab.transform.position + tilePrefab.positionOffsetValueLeft; tilePrefab.transform.position = newPos; }
				else if (connectionPointIsDown && tilePrefab.canConnectUp) { Vector2 newPos = (Vector2)tilePrefab.transform.position + tilePrefab.positionOffsetValueUp; tilePrefab.transform.position = newPos; }
				else if (connectionPointIsLeft && tilePrefab.canConnectRight) { Vector2 newPos = (Vector2)tilePrefab.transform.position + tilePrefab.positionOffsetValueRight; tilePrefab.transform.position = newPos; }

				//Check if tiles are obstructing
				List<TilePrefab> areaTiles = tilePrefab.GetComponentsInChildren<TilePrefab>().ToList();
				areaTiles.Remove(tilePrefab);

				bool groupTileInIllegalPosition = false;
				foreach (TilePrefab areaTile in areaTiles)
				{
					foreach (TilePrefab existingTile in tm.tiles)
					{
						if (Vector2.Distance(areaTile.transform.position, existingTile.transform.position) < epsilon)
						{							
							groupTileInIllegalPosition = true;
							break;
						}
					}
					if (groupTileInIllegalPosition) break;
				}

				if (groupTileInIllegalPosition)
				{
					Destroy(tilePrefab.gameObject);
					continue;
				}

				foreach (TilePrefab areaTile in areaTiles)
				{
					AddTile(areaTile);
				}

				continue;
			}

			//3. Rotate Tile
			Transform spawnedTileReference = randomConnectionPoint.parent;
			bool tileHasValidRotation = false;
			Transform connectedPoint;
			while (!tileHasValidRotation)
			{
				foreach (Transform connectionPoint in tilePrefab.connectionPoints)
				{
					if (Vector3.Distance(connectionPoint.position, spawnedTileReference.position) < .01f)
					{
						connectedPoint = connectionPoint;
						tileHasValidRotation = true;
					}
				}

				if (tileHasValidRotation) break;

				tilePrefab.transform.eulerAngles = new Vector3(0, 0, tilePrefab.transform.eulerAngles.z + 90);
				yield return null;
			}

			//4. Update Manager Lists
			AddTile(tilePrefab);
		}
	}

	private void AddTile(TilePrefab tilePrefab)
	{
		tm.AddTilePosition(tilePrefab);
		foreach (Transform connectionPoint in tilePrefab.connectionPoints) tm.AddConnectionPoints(connectionPoint);
		tm.CheckConnectionPoints();

		TileSpawned?.Invoke(tilePrefab);

		if (!canSpawnTilesEvent)
		{
			CanSpawnTiles?.Invoke(true);
			canSpawnTilesEvent = true;
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
