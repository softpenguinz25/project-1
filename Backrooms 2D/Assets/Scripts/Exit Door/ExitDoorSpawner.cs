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

		/*#region Rotation Optimization (failed lol
			Vector3 targetPos = FindObjectOfType<PlayerMovement>().transform.position;
			Vector2 direction = targetPos - transform.position;

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			Vector3 eulerAngles = Quaternion.Euler(Vector2.up * (angle + angleOffset)).eulerAngles;
			Vector3 roundedEulerAngles = new Vector3
			(Mathf.Round(eulerAngles.x / 90) * 90,
			Mathf.Round(eulerAngles.y / 90) * 90,
			Mathf.Round(eulerAngles.z / 90) * 90);
			#endregion*/

			//Spawn Door
			GameObject exitDoorObj = Instantiate(exitDoor, tile.transform.position, tile.transform.rotation);
			ExitDoorSpawned?.Invoke(exitDoorObj);
			enabled = false;
		};
	}
}
