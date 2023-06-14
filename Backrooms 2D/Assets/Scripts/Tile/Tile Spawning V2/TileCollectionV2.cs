using MyBox;
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
	[SerializeField] bool useCustomInitialRotation;
	[SerializeField] [ConditionalField(nameof(useCustomInitialRotation))] int initialRotation = 90;
	[SerializeField] private TileTypeSpawnChanceArrayDictionary tileSpawnChances;
	[Tooltip("Probability for dead end between two adjacent non-connecting tiles stays a dead end")] [Range(0, 1)] public float deadEndProbability = .8f;

	public GameObject WallGO { get => wallGO; set => wallGO = value; }
	public bool UseCustomInitialRotation { get => useCustomInitialRotation; set => useCustomInitialRotation = value; }
	public int InitialRotation { get => initialRotation; set => initialRotation = value; }
	public TileV2.TileType InitialTile { get => initialTile; set => initialTile = value; }
	public TileTypeSpawnChanceArrayDictionary TileSpawnChances { get => tileSpawnChances; set => tileSpawnChances = value; }

	public TileV2.TileType GetRandomTileType(bool mustBeRegularTile = false)
	{
		float totalDenomination = 0;

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance[]> tileSpawnChanceArray in TileSpawnChances)
		{
			if (IsGroupTile(tileSpawnChanceArray.Key) && mustBeRegularTile) continue;
			foreach (TileSpawnChance tileSpawnChance in tileSpawnChanceArray.Value)
			{
				totalDenomination += tileSpawnChance.GetSpawnChance(tileSpawnChanceArray.Key);
			}
		}

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance[]> tileSpawnChanceArray in TileSpawnChances)
		{
			if (IsGroupTile(tileSpawnChanceArray.Key) && mustBeRegularTile) continue;

			foreach (TileSpawnChance tileSpawnChance in tileSpawnChanceArray.Value)
			{
				if (tileSpawnChance.GetSpawnChance(tileSpawnChanceArray.Key) >= randomNumber)
				{
					return tileSpawnChanceArray.Key;
				}

				randomNumber -= tileSpawnChance.GetSpawnChance(tileSpawnChanceArray.Key);
			}
		}

		Debug.LogError("Could not choose tile!");
		return TileV2.TileType.Closed;
	}

	public Object GetRandomTileOnTileType(TileV2.TileType tileType)
	{
		float totalDenomination = 0;

		//Debug.Log(TileSpawnChances[tileType].Length);
		foreach (TileSpawnChance tileSpawnChance in TileSpawnChances[tileType])
		{
			totalDenomination += tileSpawnChance.GetSpawnChance(tileType);
		}

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (TileSpawnChance tileSpawnChance in TileSpawnChances[tileType])
		{
			if (tileSpawnChance.GetSpawnChance(tileType) >= randomNumber)
			{
				return tileSpawnChance.tileGO;
			}

			randomNumber -= tileSpawnChance.GetSpawnChance(tileType);
		}

		Debug.LogError("Could not choose tile on this tile type: " + tileType);
		return null;
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
		foreach (KeyValuePair<TileV2.TileType, TileSpawnChance[]> tileTypePairs in TileSpawnChances)
		{
			foreach (TileSpawnChance tileSpawnChance in tileTypePairs.Value)
			{
				if (!IsGroupTile(tileTypePairs.Key) && tileSpawnChance.GetSpawnChance(tileTypePairs.Key) > 0)
					return true;
			}
		}

		return false;
	}
}

[System.Serializable]
public class TileSpawnChance
{
	public Object tileGO;
	public float spawnChance;
	int numTiles => TileDataManagerV2.NumTiles;
	public int numTilesUntilCanBeSpawned;
	public int maxNumTilesOfType;

	public float GetSpawnChance(TileV2.TileType tileType)
	{
		if (maxNumTilesOfType > 0 && TileDataManagerV2.TilesPerType[tileType] >= maxNumTilesOfType)
			return 0;

		if (numTiles < numTilesUntilCanBeSpawned)
			return 0;

		return spawnChance;
	}
	public void SetSpawnChance(float value)
	{
		spawnChance = value;
	}
}