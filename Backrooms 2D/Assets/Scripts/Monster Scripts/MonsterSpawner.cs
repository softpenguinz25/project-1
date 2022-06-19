using MyBox;
using System;
using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
	private TileLoader tl;

	[Header("Initial Spawn")]
	[SerializeField] [Range(0, 35)] private int chunksBeforeSpawn = 25;
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

		Debug.Log("Spawn Monter");

		Vector2 randomEdgePos = UnityEngine.Random.insideUnitCircle.normalized * spawnRadiusAwayFromPlayer.Max + (Vector2)player.transform.position;
		Vector2 randomEdgeOffsetPos = new Vector2(UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min), UnityEngine.Random.Range(0, spawnRadiusAwayFromPlayer.Max - spawnRadiusAwayFromPlayer.Min));
		Vector2 randomPos = (new Vector2(Mathf.Abs(randomEdgePos.x), Mathf.Abs(randomEdgePos.y)) - randomEdgeOffsetPos) * new Vector2(Mathf.Sign(randomEdgePos.x), Mathf.Sign(randomEdgePos.y));

		foreach(Vector2 tilePos in FindObjectOfType<TileDataManager>().tilePositions)
		{
			if (Vector2.Distance(randomPos, tilePos) < 5)
			{
				Instantiate(monster, tilePos, Quaternion.identity);
				break;
			}
		}

		canSpawnMonster = false;
	}
}
