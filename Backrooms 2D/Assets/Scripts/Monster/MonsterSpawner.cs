using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
	private TileLoader tl;

	[Header("Initial Spawn")]
	[SerializeField] [Range(0, 250)] private int chunksBeforeSpawn = 25;
	[SerializeField] [MinMaxRange(0, 60)] private RangedFloat maxSpawnTimeDelay = new RangedFloat(20, 40);

	[Header("Reocurring Spawns")]
	[SerializeField] private bool canReocurringlySpawn = true;
	[ConditionalField(nameof(canReocurringlySpawn))][SerializeField] [Range(0, 250)] private int reoccurringChunksBeforeSpawn = 25;
	[ConditionalField(nameof(canReocurringlySpawn))] [SerializeField] [MinMaxRange(0, 60)] private RangedFloat reoccurringMaxSpawnTimeDelay = new RangedFloat(20, 40);
	private bool hasSpawnedOnce;
	[ConditionalField(nameof(canReocurringlySpawn))] [SerializeField] private int maxNumMonsters = 3;
	private List<GameObject> totalMonsters = new List<GameObject>();

	[Header("Spawn")]
	[SerializeField] private List<GameObject> possibleMonsters = new List<GameObject>();
	[Space]
	[SerializeField] private GameObject player;
	[SerializeField] [MinMaxRange(0, 50)] private RangedFloat spawnRadiusAwayFromPlayer;
	[SerializeField] private float minDistanceAwayFromPlayer = 7;
	private bool canSpawnMonster = true;
	private void Awake()
	{
		tl = FindObjectOfType<TileLoader>();
	}

	private void Start()
	{
		StartCoroutine(SpawnMonsterAfterDelayCoroutine());
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(player.transform.position, spawnRadiusAwayFromPlayer.Min);

		Gizmos.color = Color.gray;
		Gizmos.DrawWireSphere(player.transform.position, spawnRadiusAwayFromPlayer.Max);
	}

	private void OnEnable()
	{
		tl.ChunkSpawned += (numChunks) =>
		{
			if (!hasSpawnedOnce)
			{
				if (numChunks % chunksBeforeSpawn == 0)
				{
					StopCoroutine(SpawnMonsterAfterDelayCoroutine());
					SpawnMonster();
				}
			}
			else
			{
				if ((numChunks + chunksBeforeSpawn) % reoccurringChunksBeforeSpawn == 0)
				{
					StopCoroutine(SpawnMonsterAfterDelayCoroutine());
					SpawnMonster();
				}
			}
		};
	}

	private IEnumerator SpawnMonsterAfterDelayCoroutine()
	{
		if(!hasSpawnedOnce)
			yield return new WaitForSeconds(UnityEngine.Random.Range(maxSpawnTimeDelay.Min, maxSpawnTimeDelay.Max));
		else
			yield return new WaitForSeconds(UnityEngine.Random.Range(reoccurringMaxSpawnTimeDelay.Min, reoccurringMaxSpawnTimeDelay.Max));
		SpawnMonster();
	}
	
	[ContextMenu("Spawn Monster")]
	public void SpawnMonster()
	{
		if (!canSpawnMonster) return;
		hasSpawnedOnce = true;

		Vector2 randomEdgePos = UnityEngine.Random.insideUnitCircle.normalized * spawnRadiusAwayFromPlayer.Max + (Vector2)player.transform.position;
		Vector2 randomEdgeOffsetPos = new Vector2(UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min), UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min));
		Vector2 randomPos = (new Vector2(Mathf.Abs(randomEdgePos.x), Mathf.Abs(randomEdgePos.y)) - randomEdgeOffsetPos) * new Vector2(Mathf.Sign(randomEdgePos.x), Mathf.Sign(randomEdgePos.y));

		List<TileDistance> tileDistances = new List<TileDistance>();
		foreach(TilePrefab tile in FindObjectOfType<TileDataManager>().tiles)
		{
			tileDistances.Add(new TileDistance(tile, Vector2.Distance(randomPos, tile.transform.position)));
		}
		//THANKS iwaldrop! https://answers.unity.com/questions/476940/how-to-sort-a-list-by-a-class-paramater.html
		tileDistances = tileDistances.OrderBy(x => x.distance).ToList();

		List<TileDistance> tooClose = new List<TileDistance>();
		foreach(TileDistance tileDistance in tileDistances)
		{
			if(Vector2.Distance(tileDistance.tile.transform.position, player.transform.position) < minDistanceAwayFromPlayer)
			{
				tooClose.Add(tileDistance);
			}
		}		

		foreach(TileDistance closeTile in tooClose)
		{
			tileDistances.Remove(closeTile);
		}

		GameObject chosenMonster = possibleMonsters[UnityEngine.Random.Range(0, possibleMonsters.Count)];
		MonsterSpawnTiles monsterSpawnTiles = chosenMonster.GetComponent<MonsterSpawnTiles>();
		if (monsterSpawnTiles != null)
		{
			List<string> validSpawnTileNames = new List<string>();
			foreach(TilePrefab validSpawnTile in monsterSpawnTiles.possibleSpawnTiles)
			{
				string validSpawnTileName = validSpawnTile.gameObject.name;
				validSpawnTileName.Replace("(Clone)", "");
				//Debug.Log(validSpawnTileName);
				validSpawnTileNames.Add(validSpawnTile.gameObject.name);
			}
			
			List<TileDistance> invalidSpawnTiles = new List<TileDistance>();
			foreach (TileDistance td in tileDistances)
			{
				string tileName = td.tile.gameObject.name;
				//tileName.Replace("(Clone)", "");
				//tileName = tileName.Substring(0, tileName.Length - 7);
				//Debug.Log(tileName);
				bool invalidTileName = true;
				foreach(string validTileName in validSpawnTileNames)
				{
					if (tileName.Contains(validTileName)) invalidTileName = false;
				}
				if(invalidTileName) invalidSpawnTiles.Add(td);
				//if (!validSpawnTileNames.Contains(tileName)) invalidSpawnTiles.Add(td);
			}

			foreach (TileDistance invalidSpawnTile in invalidSpawnTiles) tileDistances.Remove(invalidSpawnTile);
		}
		//Debug.Log(tileDistances.Count);
		GameObject monsterObj = Instantiate(chosenMonster, tileDistances[0].tile.transform.position, Quaternion.identity);

		canSpawnMonster = false;
		totalMonsters.Add(monsterObj);

		if(totalMonsters.Count > maxNumMonsters)
		{
			Destroy(totalMonsters[0]);
			totalMonsters.RemoveAt(0);
		}

		StartCoroutine(RestartSystem());
	}

	private IEnumerator RestartSystem()
	{
		yield return new WaitForSeconds(reoccurringMaxSpawnTimeDelay.Min * .01f);

		canSpawnMonster = true;
		StartCoroutine(SpawnMonsterAfterDelayCoroutine());
	}
}

public struct TileDistance
{
	public TilePrefab tile;
	public float distance;

	public TileDistance(TilePrefab _tile, float _distance)
	{
		tile = _tile;
		distance = _distance;
	}
}
