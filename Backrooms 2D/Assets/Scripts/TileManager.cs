using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public List<Transform> connectionPoints;

    public void UpdateConnectionPoints(List<Transform> connectionPointsToAdd, List<Transform> connectionPointsToRemove = null)
	{
		foreach (Transform connectionPoint in connectionPointsToAdd) connectionPoints.Add(connectionPoint);
		if(connectionPointsToRemove != null) foreach (Transform connectionPoint in connectionPointsToRemove) connectionPoints.Remove(connectionPoint);
	}
}
