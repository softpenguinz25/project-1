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
	TileCollectionV2 tc
	{
		get
		{
			return ts.Tc;
		}
	}

	[Header("Tile Pools")]
	[SerializeField] int defaultPoolSize;
	Dictionary<TileV2.TileType, int> poolSizes = new();
	Dictionary<TileV2.TileType, Queue<GameObject>> poolGOs = new();
	//Foreach tsp in TSP
	//if not group tile
	//for i in poolSize
	//Spawn tsp GO
	//Set inactive
	//Add GO to queue

	private void Awake()
	{
		ts = GetComponent<TileSpawnerV2>();

		poolGOs = new();
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
			GameObject queuedGO = Instantiate((GameObject)tsp.Value.tileGO, Vector3.zero, Quaternion.identity);
			queuedGO.transform.localScale = new Vector2(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
			queuedGO.SetActive(false);

			poolGOs[tsp.Key].Enqueue(queuedGO);
		}

		poolGOs.Add(tsp.Key, poolGOs[tsp.Key]);
	}

	public GameObject SpawnTileFromPool(TileV2.TileType tileType, Vector2 pos, Quaternion rot)
	{
		if (!poolGOs.ContainsKey(tileType))
		{
			Debug.LogError("Pool With Tag" + tileType + "Doesn't Exist!");
			return null;
		}

		GameObject tileToSpawn = poolGOs[tileType].Dequeue();

		tileToSpawn.SetActive(true);
		tileToSpawn.transform.SetPositionAndRotation(pos, rot);

		poolGOs[tileType].Enqueue(tileToSpawn);

		return tileToSpawn;
	}
}
