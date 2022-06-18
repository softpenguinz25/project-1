using System.Collections;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	private TileManager tm;

	[Header("Spawning")]
	[SerializeField] public TileCollection tileCollection;
	[SerializeField] [Range(0, 1)] private float spawnRate = .5f;

	private void Awake()
	{
		tm = FindObjectOfType<TileManager>();
	}

	private void Start()
	{
		SpawnFirstTile();
	}

	private void SpawnFirstTile()
	{
		TilePrefab firstTile = Instantiate(tileCollection.firstTile.tilePrefab, Vector3.zero, Quaternion.identity);
		tm.UpdateConnectionPoints(firstTile.startingConnectionPoints, null);

		StartCoroutine(SpawnTilesCoroutine());
	}

	private IEnumerator SpawnTilesCoroutine()
	{
		/*while (true)
		{*/
			Debug.Log("Spawn Tile!");
			yield return new WaitForSeconds(.5f);
		/*}*/
	}
}
