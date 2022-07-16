using MyBox;
using System;
using UnityEngine;

public class ExitDoorSpawner : MonoBehaviour
{
    private TileDataManager tdm;

	[SerializeField] private GameObject exitDoor;
	[SerializeField] private float tilesUntilDoorSpawns = 960;

	[Header("Spawn on Certain Tiles")]
	[SerializeField] private bool spawnOnCertainTiles = false;
	[ConditionalField(nameof(spawnOnCertainTiles))] [SerializeField] private TileCollection possibleTiles;

	public event Action<GameObject> ExitDoorSpawned;
	private const float angleOffset = 0;
	private void Awake()
	{
		tdm = FindObjectOfType<TileDataManager>();
	}

	private void OnEnable()
	{
		tdm.TileAdded += (tile, numTiles) =>
		{
			if (!enabled) return;

			if (numTiles < tilesUntilDoorSpawns) return;
			//Check if tile is in valid tiles list
			if (spawnOnCertainTiles)
			{
				foreach(TileTemplate possibleTile in possibleTiles.tiles)
				{
					if(tile.name.Contains(possibleTile.tilePrefab.name))
					{
						SpawnDoor(tile);
						break;
					}
				}
				//if (enabled) Debug.Log(tile.name + " is not a possible tile name for door to spawn on!");
			}
			else
			{
				SpawnDoor(tile);
			}
		};
	}

	private void SpawnDoor(TilePrefab tile)
	{
		GameObject exitDoorObj = Instantiate(exitDoor, tile.transform.position, tile.transform.rotation);
		ExitDoorSpawned?.Invoke(exitDoorObj);
		enabled = false;
	}
}
