using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Collection", menuName = "Tile/Tile Collection", order = 1)]
public class TileCollectionV2 : ScriptableObject
{
	[Header("Level")]
	public string levelName;

	[Header("Tile Types")]
	public GameObject wallGO;
	public TileV2.TileType initialTile;
	public TileTypeSpawnChanceDictionary tileSpawnChances;

	[Header("Dead Ends")]
	[Tooltip("Probability for dead end between two adjacent non-connecting tiles stays a dead end")][Range(0, 1)] public float deadEndProbability = .8f;
	public TileV2.TileType GetRandomTileType(bool mustBeRegularTile = false)
	{
		float totalDenomination = 0;

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tileSpawnChance in tileSpawnChances)
		{
			if (IsGroupTile(tileSpawnChance.Key) && mustBeRegularTile) continue;

			totalDenomination += tileSpawnChance.Value.spawnChance;
		}

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tsp in tileSpawnChances)
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
		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance> tileTypePairs in tileSpawnChances)
			if (!IsGroupTile(tileTypePairs.Key) && tileSpawnChances[tileTypePairs.Key].spawnChance > 0)
				return true;

		return false;
	}
}

[System.Serializable]
public class TileSpawnChance
{
    public float spawnChance;
	public Object tileGO;
}