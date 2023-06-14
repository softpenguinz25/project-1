using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExitDoorSpawner : MonoBehaviour
{
	[SerializeField] private GameObject exitDoor;
	public event Action ExitDoorSpawned;

	public void SpawnDoor(TileV2 tile)
	{
		//print("spawning door...");
		GameObject exitDoorObj = Instantiate(exitDoor, (Vector3Int)tile.TilePosition, tile.TileRotation);
		ExitDoorSpawned?.Invoke();
		enabled = false;
	}
}
