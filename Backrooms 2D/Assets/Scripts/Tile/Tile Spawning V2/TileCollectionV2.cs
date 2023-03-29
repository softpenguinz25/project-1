using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Collection", menuName = "V2/Tile/Tile Collection", order = 1)]
public class TileCollectionV2 : ScriptableObject
{
	[Header("Tile Types")]
	public TileV2.TileType initialTile;
    public List<TileSpawnChance> tileSpawnChances;

	[Header("Dead Ends")]
	[Tooltip("Probability for dead end between two adjacent non-connecting tiles stays a dead end")][Range(0, 1)] public float deadEndProbability = .8f;

	public TileV2.TileType GetRandomTile()
	{
		float totalDenomination = 0;

		foreach (TileSpawnChance tileSpawnChance in tileSpawnChances)
		{
			totalDenomination += tileSpawnChance.spawnChance;
		}

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (TileSpawnChance tsp in tileSpawnChances)
		{
			if (tsp.spawnChance >= randomNumber)
			{
				return tsp.tileType;
			}

			randomNumber -= tsp.spawnChance;
		}

		Debug.LogError("Random Tile Generation Failed!");
		return TileV2.TileType.Closed;
	}

	//TODO: List<GroupTile>
}

[System.Serializable]
public class TileSpawnChance
{
    public TileV2.TileType tileType;
    public float spawnChance;
}