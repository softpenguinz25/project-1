using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationObjectV2 : MonoBehaviour
{
	[SerializeField] [Range(0, 1)] float spawnDeviationAroundTile;
	[SerializeField] bool randomizeRotation = true;

	public virtual void SpawnDecoration(TileGOV2 tileGO)
	{
		Instantiate(GetGOToSpawn(tileGO), GetPosition(tileGO), GetRotation(tileGO), tileGO.transform);
	}

	public virtual Vector2 GetPosition(TileGOV2 tileGO)
	{
		float x = Random.Range(tileGO.transform.position.x - TileSpawnerV2.TileSize * .5f * spawnDeviationAroundTile, tileGO.transform.position.x + TileSpawnerV2.TileSize * .5f * spawnDeviationAroundTile);
		float y = Random.Range(tileGO.transform.position.y - TileSpawnerV2.TileSize * .5f * spawnDeviationAroundTile, tileGO.transform.position.y + TileSpawnerV2.TileSize * .5f * spawnDeviationAroundTile);
		return new Vector2(x, y);
	}

	public virtual Quaternion GetRotation(TileGOV2 tileGO)
	{
		if (!randomizeRotation) return Quaternion.identity;

		return Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
	}

	public virtual GameObject GetGOToSpawn(TileGOV2 tileGO)
	{
		return gameObject;
	}
}
