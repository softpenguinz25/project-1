using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileDataManagerV2))]
[RequireComponent(typeof(TileLoaderV2))]
public class TilePoolV2 : MonoBehaviour
{
	[Header("References")]
	TileSpawnerV2 ts;
	TileDataManagerV2 tdm;
	TileCollectionV2 tc
	{
		get
		{
			return ts.Tc;
		}
	}

	[Header("Tile Pools")]
	[SerializeField] int defaultPoolSize = 250;
	[SerializeField] TileTypeIntDictionary poolSizes = new();
	Dictionary<TileV2.TileType, Queue<TileGOV2>> poolGOs = new();
	//Foreach tsp in TSP
	//if not group tile
	//for i in poolSize
	//Spawn tsp GO
	//Set inactive
	//Add GO to queue

	private void Awake()
	{
		ts = GetComponent<TileSpawnerV2>();
		tdm = GetComponent<TileDataManagerV2>();

		poolGOs = new();
		foreach (TileV2.TileType tileType in tc.TileSpawnChances.Keys)
			poolGOs.Add(tileType, new Queue<TileGOV2>());
	}

	private void Start()
	{
		
	}

	public void AllocateTilePool()
	{
		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tsp in tc.TileSpawnChances)
		{
			if (tc.IsGroupTile(tsp.Key)) continue;

			SpawnQueuedGOs(tsp);
		}
	}

	private void SpawnQueuedGOs(KeyValuePair<TileV2.TileType, TileSpawnChance> tsp)
	{
		//If GO is a group tile
		if (tsp.Value.tileGO as GameObject == null)
		{
			Debug.LogError("Cannot Convert " + tsp.Value.tileGO + " to a GameObject!");
			return;
		}

		int poolSize = poolSizes.ContainsKey(tsp.Key) ? poolSizes[tsp.Key] : defaultPoolSize;

		for (int GOIndex = 0; GOIndex < poolSize; GOIndex++)
		{
			TileGOV2 queuedGO = Instantiate((GameObject)tsp.Value.tileGO, Vector3.zero, Quaternion.identity).GetComponent<TileGOV2>();
			queuedGO.transform.localScale = new Vector2(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
			queuedGO.gameObject.SetActive(false);

			poolGOs[tsp.Key].Enqueue(queuedGO);
		}

		//poolGOs.Add(tsp.Key, poolGOs[tsp.Key]);
	}

	public TileGOV2 SpawnTileFromPool(TileV2.TileType tileType, Vector2Int pos, Quaternion rot)
	{
		if (!poolGOs.ContainsKey(tileType))
		{
			Debug.LogError("Pool With Tag " + tileType + " Doesn't Exist!");
			return null;
		}

		TileGOV2 tileToSpawn = poolGOs[tileType].Dequeue();
		/*TileV2 oldTile = tdm.TileDict[(new Vector2Int((int)tileToSpawn.transform.position.x, (int)tileToSpawn.transform.position.y))];
		oldTile.HasSpawned = false;*/

		tileToSpawn.gameObject.SetActive(true);
		tileToSpawn.gameObject.transform.SetPositionAndRotation((Vector3Int)pos, rot);
		tileToSpawn.transform.parent = TileLoaderV2.chunkGOs[TileLoaderV2.GetChunkFromPos(pos, TileLoaderV2.ChunkSize)].transform;

		poolGOs[tileType].Enqueue(tileToSpawn);

		return tileToSpawn;
	}
}
