using MyBox;
using System;
using UnityEngine;

public class ExitDoorSpawnerTiles : ExitDoorSpawner
{
    [HideInInspector] public TileDataManagerV2 tdm;

	[SerializeField] private float tilesUntilDoorSpawns = 960;

	[Header("Spawn on Certain Tiles")]
	[SerializeField] private bool spawnOnCertainTiles = false;
	[ConditionalField(nameof(spawnOnCertainTiles))] [SerializeField] private TileCollection possibleTiles;

	private const float angleOffset = 0;
	public void Awake()
	{
		tdm = FindObjectOfType<TileDataManagerV2>();
	}

	public void Start()
	{
		SpawnDoorMechanism();
	}

	public void SpawnDoorMechanism()
	{
		tdm.TileAdded += (tile, numTiles) =>
		{
			if (!enabled) return;

			if (numTiles < tilesUntilDoorSpawns) return;

			SpawnDoor(tile);
			//Check if tile is in valid tiles list
			/*if (spawnOnCertainTiles)
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
			}*/
		};
	}
}
