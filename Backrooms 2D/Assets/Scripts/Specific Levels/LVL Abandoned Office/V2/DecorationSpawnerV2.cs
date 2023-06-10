using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileGOV2))]
public class DecorationSpawnerV2 : MonoBehaviour
{
	[SerializeField] [Range(0, 1)] float chanceToSpawn;
	[SerializeField] List<DecorationSpawnChance> decorationSpawnChances;
    public DecorationObjectV2 GetDecorationGO()
	{
		float totalDenomination = 0;
		foreach (DecorationSpawnChance decorationSpawnChance in decorationSpawnChances)
			totalDenomination += decorationSpawnChance.spawnChance;

		float randomNumber = Random.Range(0, totalDenomination);

		foreach (DecorationSpawnChance deocrationSpawnChance in decorationSpawnChances)
		{
			if (deocrationSpawnChance.spawnChance >= randomNumber)
			{
				return deocrationSpawnChance.decoration;
			}

			randomNumber -= deocrationSpawnChance.spawnChance;
		}

		Debug.LogError("Could not get decoration GO!");
		return null;
	}

	private void Start()
	{
		if (chanceToSpawn <= Random.value) return;
		
		GetDecorationGO().SpawnDecoration(GetComponent<TileGOV2>());
	}
}

[System.Serializable]
public class DecorationSpawnChance
{
    public DecorationObjectV2 decoration;
    public float spawnChance;
}
