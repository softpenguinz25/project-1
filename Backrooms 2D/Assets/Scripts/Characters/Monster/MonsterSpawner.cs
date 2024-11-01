using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
	private TileLoaderV2 tl;

	[Header("Initial Spawn")]
	[SerializeField] [Range(0, 250)] private int chunksBeforeSpawn = 25;
	[SerializeField] [MinMaxRange(0, 60)] private RangedFloat maxSpawnTimeDelay = new RangedFloat(20, 40);

	[Header("Reocurring Spawns")]
	[SerializeField] private bool canReocurringlySpawn = true;

	public void Disable()
	{
		canSpawnMonster = false;
		enabled = false;
	}

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
		tl = FindObjectOfType<TileLoaderV2>();
	}

	private void Start()
	{
		//canReocurringlySpawn = !!canReocurringlySpawn;//this is not a typo, just wanted unity to stop giving me the "this variable is not in use" warning message lol
		//Debug.Log("SpawnMonster - Start");
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
					//Debug.Log("SpawnMonster - First Chunk Threshold Met");
					StopCoroutine(SpawnMonsterAfterDelayCoroutine());//*IMPORTANT* THIS MAY NOT STOP THE COROUTINE, COULD CAUSE FUTURE ERRORS
					StartCoroutine(SpawnMonsterAfterDelayCoroutine(spawnInstantly : true));
				}
			}
			else if(canReocurringlySpawn)
			{
				if ((numChunks + chunksBeforeSpawn) % reoccurringChunksBeforeSpawn == 0)
				{
					//Debug.Log("SpawnMonster - Reoccuring Chunks Threshold Met");
					StopCoroutine(SpawnMonsterAfterDelayCoroutine());//*IMPORTANT* THIS MAY NOT STOP THE COROUTINE, COULD CAUSE FUTURE ERRORS
					StartCoroutine(SpawnMonsterAfterDelayCoroutine(spawnInstantly : true));
				}
			}
		};
	}

	public virtual IEnumerator SpawnMonsterAfterDelayCoroutine(GameObject monsterToSpawn = null, bool spawnInstantly = false)
	{
		//Debug.Log("Monster Passed as Parameter (Base Coroutine): " + monsterToSpawn);
		//Debug.Log(monsterToSpawn);
		if (!spawnInstantly)
		{
			float spawnDelay = !hasSpawnedOnce ? UnityEngine.Random.Range(maxSpawnTimeDelay.Min, maxSpawnTimeDelay.Max) : UnityEngine.Random.Range(reoccurringMaxSpawnTimeDelay.Min, reoccurringMaxSpawnTimeDelay.Max);
			//Debug.Log(spawnDelay);
			yield return new WaitForSeconds(spawnDelay);
			//Debug.Log("Spawn Delay: " + spawnDelay);
			/*while(spawnDelay > 0)
			{
				spawnDelay -= Time.deltaTime;
				yield return null;
			}*/
		}

		//Debug.Log("Spawning Monster (Base Coroutine): " + monsterToSpawn);
			SpawnMonster(monsterToSpawn);
	}

	public bool IsMonsterChasingPlayer()
	{
		foreach(MonsterClose mc in FindObjectsOfType<MonsterClose>())
		{
			if (mc.IsClose && mc.GetComponent<MonsterInfo>().isDeadly)
			{
				return true;
			}
		}

		return false;
	}
	
	[ContextMenu("Spawn Monster")]
	public void SpawnMonster(GameObject monsterToSpawn = null)
	{
		//Debug.Log("Monster Passed as Parameter (Function): " + monsterToSpawn);

		if (!canSpawnMonster) return;
		hasSpawnedOnce = true;

		Vector2 randomEdgePos = UnityEngine.Random.insideUnitCircle.normalized * spawnRadiusAwayFromPlayer.Max + (Vector2)player.transform.position;
		Vector2 randomEdgeOffsetPos = new Vector2(UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min), UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min));
		Vector2 randomPos = (new Vector2(Mathf.Abs(randomEdgePos.x), Mathf.Abs(randomEdgePos.y)) - randomEdgeOffsetPos) * new Vector2(Mathf.Sign(randomEdgePos.x), Mathf.Sign(randomEdgePos.y));

		List<TileDistance> tileDistances = new List<TileDistance>();
		foreach(TileV2 tile in FindObjectOfType<TileDataManagerV2>().TileDict.Values)
		{
			tileDistances.Add(new TileDistance(tile, Vector2.Distance(randomPos, tile.TilePosition)));
		}
		//THANKS iwaldrop! https://answers.unity.com/questions/476940/how-to-sort-a-list-by-a-class-paramater.html
		tileDistances = tileDistances.OrderBy(x => x.distance).ToList();

		List<TileDistance> tooClose = new List<TileDistance>();
		foreach(TileDistance tileDistance in tileDistances)
		{
			if(Vector2.Distance(tileDistance.tile.TilePosition, player.transform.position) < minDistanceAwayFromPlayer)
			{
				tooClose.Add(tileDistance);
			}
		}		

		foreach(TileDistance closeTile in tooClose)
		{
			tileDistances.Remove(closeTile);
		}

		GameObject chosenMonster;
		if(monsterToSpawn != null)
		{
			if (possibleMonsters.Contains(monsterToSpawn))
			{
				chosenMonster = monsterToSpawn;
			}
			else
			{
				Debug.LogError("Monster " + monsterToSpawn + " is not in " + nameof(possibleMonsters));
				chosenMonster = possibleMonsters[UnityEngine.Random.Range(0, possibleMonsters.Count)];
			}
		}
		else
		{
			chosenMonster = possibleMonsters[UnityEngine.Random.Range(0, possibleMonsters.Count)];
		}
		/*MonsterSpawnTiles monsterSpawnTiles = chosenMonster.GetComponent<MonsterSpawnTiles>();
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

			//Check if there's a "rogue go" on the tile

			foreach (TileDistance invalidSpawnTile in invalidSpawnTiles) tileDistances.Remove(invalidSpawnTile);
		}*/
		//Debug.Log(tileDistances.Count);
		//Debug.Log("Spawning monster: " + chosenMonster);
		GameObject monsterObj = Instantiate(chosenMonster, (Vector3Int)tileDistances[0].tile.TilePosition, Quaternion.identity);

		canSpawnMonster = false;
		totalMonsters.Add(monsterObj);

		if(totalMonsters.Count > maxNumMonsters)
		{
			Destroy(totalMonsters[0]);
			totalMonsters.RemoveAt(0);
		}

		StartCoroutine(RestartSystem());
	}

	public IEnumerator RestartSystem()
	{
		yield return new WaitUntil(() => !IsMonsterChasingPlayer());
		//Debug.Log("SpawnMonster - Restart System");
		canSpawnMonster = true;
		StartCoroutine(SpawnMonsterAfterDelayCoroutine());
	}
}

public struct TileDistance
{
	public TileV2 tile;
	public float distance;

	public TileDistance(TileV2 _tile, float _distance)
	{
		tile = _tile;
		distance = _distance;
	}
}
