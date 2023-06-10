using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class LVLAbandonedOfficeWallGenerator : MonoBehaviour
{
    [SerializeField] LVLAbandonedOfficeWallPrefabs[] wallPrefabs;
    [SerializeField] List<GameObject> wallSpots;
    List<GameObject> currentWallSpots = new List<GameObject>();
    [Tooltip("inclusive, inclusive")]
    [SerializeField] RangedInt numWallOpeningsRange = new RangedInt(1, 2);

	private void Awake()
	{
        currentWallSpots = wallSpots;

        int numWallOpenings = Random.Range(numWallOpeningsRange.Min, numWallOpeningsRange.Max + 1);
        int numWallOpeningsIndex = numWallOpenings;

        //Spawn "open" walls
        while(numWallOpeningsIndex > 0)
		{
            SpawnWall(GetWallTypes(true)[Random.Range(0, GetWallTypes(true).Count)].wall, currentWallSpots[Random.Range(0, currentWallSpots.Count)]);
            numWallOpeningsIndex--;
		}

        //Spawn "closed" walls (AKA rest of the walls)        
        while(currentWallSpots.Count > 0)
		{
            SpawnWall(GetWallTypes(false)[Random.Range(0, GetWallTypes(false).Count)].wall, currentWallSpots[Random.Range(0, currentWallSpots.Count)]);
        }
	}

    public void SpawnWall(GameObject wallPrefab, GameObject wallSpot, bool debug = false)
	{
        if(currentWallSpots.Contains(wallSpot)) currentWallSpots.Remove(wallSpot);
		GameObject wall = Instantiate(wallPrefab, wallSpot.transform.position, wallSpot.transform.rotation, wallSpot.transform);
        wall.transform.localScale = Vector3.one;
		//if(debug) Debug.Log("Wall Spawned: " + wall.name, wall);
	}

    public List<LVLAbandonedOfficeWallPrefabs> GetWallTypes(bool wallIsOpen)
	{
        List<LVLAbandonedOfficeWallPrefabs> result = new List<LVLAbandonedOfficeWallPrefabs>();
        foreach (LVLAbandonedOfficeWallPrefabs wallType in wallPrefabs) if (wallType.isOpening == wallIsOpen) result.Add(wallType);
        return result;
	}
}

[System.Serializable]
public struct LVLAbandonedOfficeWallPrefabs
{
    public GameObject wall;
    public bool isOpening;
}
