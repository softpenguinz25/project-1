using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
	private TileSpawner ts;

	public List<TilePrefab> tiles;
    public List<Transform> connectionPoints;

	public event Action<TilePrefab, int> TileAdded;

	private void Awake()
	{
		ts = FindObjectOfType<TileSpawner>();
	}

	/*private void OnEnable()
	{
		ts.CanSpawnTiles += CheckTiles;
	}

	private void OnDisable()
	{
		ts.CanSpawnTiles -= CheckTiles;
	}*/

	public void AddTilePosition(TilePrefab tileToAdd)
	{
		tiles.Add(tileToAdd);
		TileAdded?.Invoke(tileToAdd, tiles.Count);
	}

	public void AddConnectionPoints(Transform connectionPointToAdd)
	{
		connectionPoints.Add(connectionPointToAdd);
	}

	public void CheckConnectionPoints()
	{
		//int i = 0;
		List<Transform> impossibleConnectionPoints = new List<Transform>();
		foreach (TilePrefab tile in tiles)
		{
			Vector2 tilePosition = tile.transform.position;
			foreach (Transform connectionPoint in connectionPoints)
			{
				if(Vector3.Distance(connectionPoint.position, tilePosition) < .01f)//If connection point overlaps with a tile...
				{
					impossibleConnectionPoints.Add(connectionPoint);
				}				
			}
			//i++;
			//if(i % 10 == 0)Debug.Log("Checking connection points: " + i, tile);
		}

		foreach(Transform impossibleConnectionPoint in impossibleConnectionPoints)
		{
			connectionPoints.Remove(impossibleConnectionPoint);
			impossibleConnectionPoint.transform.parent.GetComponent<TilePrefab>().connectionPoints.Remove(impossibleConnectionPoint);
			Destroy(impossibleConnectionPoint.gameObject);
		}

		if(connectionPoints.Count <= 0)
		{
			FindObjectOfType<TileRestarter>().RestartTileGeneration();
		}
	}

	/*public void CheckTiles(bool canSpawnTiles)
	{
		Debug.Log(canSpawnTiles);
		if (canSpawnTiles) return;//Only check if we cant spawn tiles (signify that tile spawning is done)

		List<TilePrefab> impossibleTiles = new List<TilePrefab>();
		foreach (TilePrefab tileToCompareTo in tiles)
		{
			Vector2 tileToCompareToPosition = tileToCompareTo.transform.position;
			foreach(TilePrefab tile in tiles)
			{
				Vector2 tilePosition = tile.transform.position;
				if(tile != tileToCompareTo && Vector2.Distance(tilePosition, tileToCompareToPosition) < .01f)
				{
					impossibleTiles.Add(tile);
				}
			}
		}

		foreach(TilePrefab impossibleTile in impossibleTiles)
		{
			tiles.Remove(impossibleTile);
		}
	}*/
}
