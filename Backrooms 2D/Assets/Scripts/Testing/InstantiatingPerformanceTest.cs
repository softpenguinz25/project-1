using System.Collections;
using UnityEngine;

public class InstantiatingPerformanceTest : MonoBehaviour
{
	[Header("Tile")]
	[SerializeField] GameObject testTile;

	[Header("Tile Spawn Region")]
    [SerializeField] Vector2 tileSize;
    Vector2 center;

	[Header("Tile Spawn Test")]
	[SerializeField] int waitFrameEveryXTiles;

	private void Start()
	{
		center = transform.position;
		StartCoroutine(SpawnTiles(waitFrameEveryXTiles));
	}

	private IEnumerator SpawnTiles(int waitFrameEveryXTiles)
	{
		Vector2 firstTilePos = center + new Vector2(-tileSize.x * .5f, tileSize.y * .5f);

		for (int y = 0; y < tileSize.y; y++)
		{
			for (int x = 0; x < tileSize.x; x++)
			{
				Instantiate(testTile, firstTilePos + new Vector2(x, -y), Quaternion.identity);
				
				if ((x + 1) % waitFrameEveryXTiles == 0) yield return null;
			}
		}
	}
}
