using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Collection", menuName = "Tile/Tile Collection", order = 1)]
public class TileCollectionV2 : ScriptableObject
{
	[Header("Level")]
	public string levelName;

	[Header("Tile Info")]
	[SerializeField] private GameObject wallGO;
	[SerializeField] private TileV2.TileType initialTile;
	[SerializeField] private TileTypeSpawnChanceDictionary tileSpawnChances;
	[Tooltip("Probability for dead end between two adjacent non-connecting tiles stays a dead end")] [Range(0, 1)] public float deadEndProbability = .8f;

	[Header("Tile Pooling")]
	[SerializeField] float tilePoolSize;
	Dictionary<TileV2.TileType, Queue<GameObject>> poolGOs = new();
	//Foreach tsp in TSP
	//if not group tile
	//for i in poolSize
	//Spawn tsp GO
	//Set inactive
	//Add GO to queue

	public GameObject WallGO { get => wallGO; set => wallGO = value; }
	public TileV2.TileType InitialTile { get => initialTile; set => initialTile = value; }
	public TileTypeSpawnChanceDictionary TileSpawnChances { get => tileSpawnChances; set => tileSpawnChances = value; }

	public TileV2.TileType GetRandomTileType(bool mustBeRegularTile = false)
	{
		float totalDenomination = 0;

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tileSpawnChance in TileSpawnChances)
		{
			if (IsGroupTile(tileSpawnChance.Key) && mustBeRegularTile) continue;

			totalDenomination += tileSpawnChance.Value.spawnChance;
		}

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tsp in TileSpawnChances)
		{
			if (IsGroupTile(tsp.Key) && mustBeRegularTile) continue;

			if (tsp.Value.spawnChance >= randomNumber)
			{
				return tsp.Key;
			}

			randomNumber -= tsp.Value.spawnChance;
		}

		Debug.LogError("Could not choose tile!");
		return TileV2.TileType.Closed;
	}

	public bool IsGroupTile(TileV2.TileType tileType)
	{
		return
			!(tileType == TileV2.TileType.Open ||
			tileType == TileV2.TileType.Hall ||
			tileType == TileV2.TileType.Split ||
			tileType == TileV2.TileType.Corner ||
			tileType == TileV2.TileType.End ||
			tileType == TileV2.TileType.Closed);
	}

	public bool HasRegTiles()
	{
		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tileTypePairs in TileSpawnChances)
			if (!IsGroupTile(tileTypePairs.Key) && TileSpawnChances[tileTypePairs.Key].spawnChance > 0)
				return true;

		return false;
	}

	private void Awake()
	{
		poolGOs = new();
	}

	public void AllocateTilePool()
	{
		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tsp in TileSpawnChances)
		{
			if (IsGroupTile(tsp.Key)) continue;

			SpawnQueuedGOs(tsp);
		}
	}

	private void SpawnQueuedGOs(KeyValuePair<TileV2.TileType, TileSpawnChance> tsp)
	{
		if (tsp.Value.tileGO as GameObject == null)
		{
			Debug.LogError("Cannot Convert " + tsp.Value.tileGO + " to a GameObject!");
			return;
		}

		Queue<GameObject> tilePool = new();

		for (int GOIndex = 0; GOIndex < tilePoolSize; GOIndex++)
		{
			GameObject queuedGO = Instantiate((GameObject)tsp.Value.tileGO, Vector3.zero, Quaternion.identity);
			queuedGO.transform.localScale = new Vector2(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
			queuedGO.SetActive(false);

			tilePool.Enqueue(queuedGO);
		}
		
		poolGOs.Add(tsp.Key, tilePool);
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

[System.Serializable]
public class TileSpawnChance
{
	public float spawnChance;
	public Object tileGO;
}