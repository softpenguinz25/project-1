using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallV2GFX : MonoBehaviour
{
	[SerializeField] GameObject currentWallGFX;
	[SerializeField] float noChangeChance;
    [SerializeField] List<TileWallSpawnChance> tileWallGFXPrefabs;

	private void Start()
	{
		float totalDenomination = 0;
		foreach(TileWallSpawnChance tileWallSpawnChance in tileWallGFXPrefabs)
			totalDenomination += tileWallSpawnChance.spawnChance;
		totalDenomination += noChangeChance;

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (TileWallSpawnChance tileWallSpawnChance in tileWallGFXPrefabs)
		{
			if (tileWallSpawnChance.spawnChance >= randomNumber)
			{
				SpawnWall(tileWallSpawnChance.wallGFX);
				break;
			}

			randomNumber -= tileWallSpawnChance.spawnChance;
		}
	}

	private void SpawnWall(GameObject wallGFX)
	{
		Instantiate(wallGFX, currentWallGFX.transform.position, currentWallGFX.transform.rotation, currentWallGFX.transform.parent);
		Destroy(currentWallGFX);
	}
}

[System.Serializable]
public class TileWallSpawnChance
{
    public GameObject wallGFX;
    public float spawnChance;
}
