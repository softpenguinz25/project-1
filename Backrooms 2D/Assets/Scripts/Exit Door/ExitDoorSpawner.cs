using System;
using UnityEngine;

public class ExitDoorSpawner : MonoBehaviour
{
    private TileDataManager tdm;

	[SerializeField] private GameObject exitDoor;
	[SerializeField] private float tilesUntilDoorSpawns = 960;

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
			//Spawn Door
			GameObject exitDoorObj = Instantiate(exitDoor, tile.transform.position, tile.transform.rotation);
			ExitDoorSpawned?.Invoke(exitDoorObj);
			enabled = false;
		};
	}
}
