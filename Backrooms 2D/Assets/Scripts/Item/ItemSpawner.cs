using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	TileLoader tl;
	TileDataManager tdm;
	GameObject player;

	[Header("Spawn Item")]
    [SerializeField] int spawnItemEveryXChunks = 2;
    [SerializeField] RangedFloat spawnDistanceAwayFromPlayer = new RangedFloat(25, 40);

	[Header("Items")]
	[SerializeField] List<GameObject> possibleItems;

	#region Cache Variables + Listen to Events
	private void Awake()
	{
		tl = FindObjectOfType<TileLoader>();
		tdm = FindObjectOfType<TileDataManager>();

		player = FindObjectOfType<PlayerMovement>().gameObject;
	}

	private void OnEnable()
	{
		tl.ChunkSpawned += AttemptToSpawnItem;
	}

	private void OnDisable()
	{
		tl.ChunkSpawned -= AttemptToSpawnItem;
	}
	#endregion

	#region Debugging
	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying) return;

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(player.transform.position, spawnDistanceAwayFromPlayer.Min);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(player.transform.position, spawnDistanceAwayFromPlayer.Max);
	}
	#endregion

	private void AttemptToSpawnItem(int chunksSpawned)
	{
		//Debug.Log("Chunks Spawned: " + chunksSpawned);
		if (chunksSpawned % spawnItemEveryXChunks != 0) return;

		SpawnItem();
	}

	[ContextMenu("Spawn Item")]
	public virtual void SpawnItem()
	{
		#region Old Attempts
		//1. Basic Spawning
		//Vector2 spawnPos = (Random.insideUnitCircle.normalized * spawnDistanceAwayFromPlayer.Min + (Vector2)player.transform.position) + normalizedDirectionVector * new Vector2(Random.Range(0, spawnDistanceAwayFromPlayer.Max - spawnDistanceAwayFromPlayer.Min), Random.Range(0, spawnDistanceAwayFromPlayer.Max - spawnDistanceAwayFromPlayer.Min));

		//2. Checking All Tiles
		/*List<Vector3> validSpawnTilePositions = new List<Vector3>();
		foreach(TilePrefab tile in tdm.tiles)
		{
			Vector2 tilePos = tile.transform.position;
			float distanceBetweenTileAndPlayer = Vector2.Distance(tilePos, player.transform.position);

			if (distanceBetweenTileAndPlayer >= spawnDistanceAwayFromPlayer.Min && distanceBetweenTileAndPlayer <= spawnDistanceAwayFromPlayer.Max)
			{
				validSpawnTilePositions.Add(tilePos);
			}
		}*/
		#endregion

		#region Detect Tiles in Spawn Radius
		var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;
		List<Collider2D> tilesDetectedInArea = Physics2D.OverlapCircleAll(player.transform.position, spawnDistanceAwayFromPlayer.Min).ToList();
		Physics2D.queriesHitTriggers = old;

		List<Collider2D> invalidTiles = new List<Collider2D>();
		foreach (Collider2D collider in tilesDetectedInArea)
		{
			TilePrefab colliderTilePrefab = collider.GetComponent<TilePrefab>();
			WallData colliderWallData = collider.GetComponent<WallData>();

			if(colliderWallData != null) { invalidTiles.Add(collider); continue; }
			else if (colliderTilePrefab == null) { invalidTiles.Add(collider); continue; }
			else if (colliderTilePrefab.isGroupTile) { invalidTiles.Add(collider); continue; }
		}

		foreach (Collider2D invalidTile in invalidTiles)
		{
			if (tilesDetectedInArea.Contains(invalidTile))
			{
				tilesDetectedInArea.Remove(invalidTile);
			}
		}
		#endregion

		Vector3 spawnPos = tilesDetectedInArea[Random.Range(0, tilesDetectedInArea.Count)].transform.position;

		Instantiate(possibleItems[Random.Range(0, possibleItems.Count)], spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
		//Debug.Log("Item Spawned!", );
	}
}
