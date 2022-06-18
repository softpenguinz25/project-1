using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
	[HideInInspector] public List<Vector2> tilePositions;
    [HideInInspector] public List<Transform> connectionPoints;

	public void AddTilePosition(Vector2 tilePositionToAdd)
	{
		tilePositions.Add(tilePositionToAdd);
	}

	public void AddConnectionPoints(Transform connectionPointToAdd)
	{
		connectionPoints.Add(connectionPointToAdd);
	}

	public void CheckConnectionPoints()
	{
		List<Transform> impossibleConnectionPoints = new List<Transform>();
		foreach (Vector2 tilePosition in tilePositions)
		{
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
			FindObjectOfType<TileDisabler>().DisableScripts();
		}
	}
}
