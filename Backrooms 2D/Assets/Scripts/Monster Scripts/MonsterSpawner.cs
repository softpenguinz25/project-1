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

	[Header("Spawn")]
	[SerializeField] private GameObject monster;
	[Space]
	[SerializeField] private GameObject player;
	[SerializeField] [MinMaxRange(0, 50)] private RangedFloat spawnRadiusAwayFromPlayer;
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
			if (numChunks >= chunksBeforeSpawn)
			{
				StopCoroutine(SpawnMonsterAfterDelayCoroutine());
				SpawnMonster();
			}
		};
	}

	private IEnumerator SpawnMonsterAfterDelayCoroutine()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(maxSpawnTimeDelay.Min, maxSpawnTimeDelay.Max));
		SpawnMonster();
	}

	private void SpawnMonster()
	{
		if (!canSpawnMonster) return;

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
		Instantiate(monster, tileDistances[0].tile.transform.position, Quaternion.identity);

		canSpawnMonster = false;
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
