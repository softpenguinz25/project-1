using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	TileLoaderV2 tl;
	TileDataManagerV2 tdm;
	GameObject player;

	[Header("Spawn Item")]
    [SerializeField] int spawnItemEveryXChunks = 2;
    [SerializeField] RangedFloat spawnDistanceAwayFromPlayer = new RangedFloat(25, 40);

	[Header("Items")]
	[SerializeField] List<GameObject> possibleItems;

	#region Cache Variables + Listen to Events
	private void Awake()
	{
		tl = FindObjectOfType<TileLoaderV2>();
		tdm = FindObjectOfType<TileDataManagerV2>();

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

		#region Old Attempt 2
		#region Detect Tiles in Spawn Radius
		/*var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;
		List<Collider2D> tilesDetectedInArea = Physics2D.OverlapCircleAll(player.transform.position, spawnDistanceAwayFromPlayer.Max).ToList();
		Physics2D.queriesHitTriggers = old;*/

		//Debug.Log("Before Filtering: TilesDetectedInArea Count: " + tilesDetectedInArea.Count);

		/*#region Invalid Colliders
		List<Collider2D> invalidTiles = new List<Collider2D>();
		foreach (Collider2D collider in tilesDetectedInArea)
		{
			//Debug.Log(collider.gameObject, collider);
			*//*TilePrefab parentTilePrefab = collider.GetComponentInParent<TilePrefab>();
			Collider2D parentTilePrefabCollider = parentTilePrefab.GetComponent<Collider2D>();*//*

			//if (!tilesDetectedInArea.Contains(parentTilePrefabCollider)) tilesDetectedInArea.Add(parentTilePrefabCollider);
			if (collider.GetComponent<TilePrefab>() == null) { *//*Debug.Log("TilePrefab Not Detected");*//* invalidTiles.Add(collider); continue; }
			else if (collider.GetComponent<TilePrefab>().isGroupTile) { invalidTiles.Add(collider); continue; }
			//Debug.Log("ColliderTilePrefab: " + colliderTilePrefab);
			*//*if (parentTilePrefab == null) { *//*Debug.Log("No Tile Prefab");*//* continue; }
			else if (invalidTiles.Contains(parentTilePrefabCollider)){ invalidTiles.Add(collider);*//*Debug.Log("Invalid Tile Already Added");*//* continue; }
			else if (parentTilePrefab.isGroupTile) { *//*Debug.Log("isGroupTile Tile Prefab Detected");*//* invalidTiles.Add(collider); continue; }*//*
		}

		foreach (Collider2D invalidTile in invalidTiles)
		{
			if (tilesDetectedInArea.Contains(invalidTile))
			{
				tilesDetectedInArea.Remove(invalidTile);
			}
		}
		#endregion*/

		//Debug.Log("Invalid Colliders Filter: " + tilesDetectedInArea.Count);

		#region Tiles Too Close
		/*List<Collider2D> tilesTooClose = new List<Collider2D>();
		foreach (Collider2D collider in tilesDetectedInArea)
		{
			if(Vector2.Distance(collider.transform.position, player.transform.position) < spawnDistanceAwayFromPlayer.Min)
			{
				tilesTooClose.Add(collider);
			}
		}

		foreach (Collider2D tileTooClose in tilesTooClose)
		{
			if (tilesDetectedInArea.Contains(tileTooClose))
			{
				tilesDetectedInArea.Remove(tileTooClose);
			}
		}*/
		#endregion

		//Debug.Log("Tiles Too Close Filter: " + tilesDetectedInArea.Count);

		#endregion

		//Transform chosenTransform = tilesDetectedInArea[Random.Range(0, tilesDetectedInArea.Count)].transform;
		//Debug.Log(chosenTransform, chosenTransform);
		//Vector3 spawnPos = chosenTransform.position;

		//Instantiate(possibleItems[Random.Range(0, possibleItems.Count)], spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
		//Debug.Log("Item Spawned!", );
		#endregion

		//1. Basic Spawning
		Vector2Int roundedSpawnPos = GetRoundedSpawnPos();
		int escapeWhileLoopIndex = 0;

		//2. Checking If Pos Is Valid
		while (!tdm.TileDict.Keys.ToList().Contains(roundedSpawnPos))
		{
			roundedSpawnPos = GetRoundedSpawnPos();
			escapeWhileLoopIndex++;

			if(escapeWhileLoopIndex == 30)
			{
				Debug.LogError("Item Could Not Find Valid Spawn Pos!");
				break;
			}
		}

		//3. Spawn Item
		Instantiate(possibleItems[Random.Range(0, possibleItems.Count)], (Vector3Int)roundedSpawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
	}

	private Vector2Int GetRoundedSpawnPos()
	{
		Vector2 spawnPos = Random.insideUnitCircle.normalized * Random.Range(spawnDistanceAwayFromPlayer.Min, spawnDistanceAwayFromPlayer.Max) + (Vector2)player.transform.position;
		Vector2Int roundedSpawnPos = new Vector2Int(Mathf.RoundToInt(spawnPos.x), Mathf.RoundToInt(spawnPos.y));
		return roundedSpawnPos;
	}
}
