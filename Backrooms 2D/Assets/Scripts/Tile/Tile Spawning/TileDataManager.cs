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

	public void AddTile(TilePrefab tileToAdd)
	{
		tiles.Add(tileToAdd);
		TileAdded?.Invoke(tileToAdd, tiles.Count);
	}

	public void AddConnectionPoint(Transform connectionPointToAdd) => connectionPoints.Add(connectionPointToAdd);
	public void DestroyConnectionPoint(Transform connectionPointToRemove)
	{
		for(int i = 0; i < connectionPoints.Count; i++)
		{
			if (connectionPoints[i] == connectionPointToRemove)
			{
				connectionPoints.Remove(connectionPointToRemove);//destroy duplicates as well
				i = 0;
			}
		}
		Destroy(connectionPointToRemove.gameObject);
	}

	public void CheckConnectionPoints()
	{
		for (int i = connectionPoints.Count - 1; i >= 0; i--)
		{
			if (connectionPoints[i] == null)
			{
				connectionPoints.RemoveAt(i);
			}
		}
		//Debug.Log("Checking " + connectionPoints.Count + " CPs in " + tiles.Count + " tiles...");
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

		/*foreach (Transform impossibleConnectionPoint in impossibleConnectionPoints)
		{
			connectionPoints.Remove(impossibleConnectionPoint);
			impossibleConnectionPoint.transform.parent.GetComponent<TilePrefab>().connectionPoints.Remove(impossibleConnectionPoint);
			Destroy(impossibleConnectionPoint.gameObject);
		}*/

		foreach (Transform impossibleConnectionPoint in impossibleConnectionPoints)
		{
			TilePrefab impossibleConnectionPointParent = impossibleConnectionPoint.gameObject.GetComponentInParent<TilePrefab>(true);
			if(impossibleConnectionPointParent == null)
			{
				Debug.Log("Could not find impossible connection point parent in " + impossibleConnectionPoint.gameObject.name, impossibleConnectionPoint);
				//break;
			}
			//Debug.Log(impossibleConnectionPointParent.name, impossibleConnectionPointParent);
			/*TilePrefab impossibleConnectionPointParent = impossibleConnectionPoint.transform.parent.GetComponent<TilePrefab>();
			if(impossibleConnectionPointParent == null) impossibleConnectionPointParent = impossibleConnectionPoint.transform.root.GetComponent<TilePrefab>();*/
			//Debug.Log(impossibleConnectionPoint, impossibleConnectionPoint);
			//Debug.Log(impossibleConnectionPointParent, impossibleConnectionPointParent);
			connectionPoints.Remove(impossibleConnectionPoint);
			if (impossibleConnectionPointParent.connectionPoints.Contains(impossibleConnectionPoint)) impossibleConnectionPointParent.connectionPoints.Remove(impossibleConnectionPoint);
			else if (impossibleConnectionPointParent.specialCPs.Contains(impossibleConnectionPoint)) impossibleConnectionPointParent.specialCPs.Remove(impossibleConnectionPoint);
			//Debug.Log(impossibleConnectionPoint.gameObject, impossibleConnectionPoint.gameObject);
			Destroy(impossibleConnectionPoint.gameObject);
		}

		if (connectionPoints.Count <= 0)
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
