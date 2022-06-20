using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
	public List<TilePrefab> tiles;
    public List<Transform> connectionPoints;

	public event Action<TilePrefab, int> TileAdded;

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
}
