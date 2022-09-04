using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLObstacleSpawner : MonoBehaviour
{
	[SerializeField] private Vector2 boxSize = new Vector2(8, 8);
	[SerializeField] private RangedInt obstaclesInTile = new RangedInt(1, 2);
	private int currentObstaclesInTile;
    [Range(0, 1)] [SerializeField] private float spawnChance = .15f;
	private static GameObject lastObstacleType;

	private void Start()
	{
		for (int i = 0; i < obstaclesInTile.Min; i++)
		{
			SpawnObject();
		}

		while(Random.value < spawnChance && currentObstaclesInTile < obstaclesInTile.Max)
		{
			SpawnObject();			
		}
	}

	private void SpawnObject()
	{
		#region Unique Obstacle Type
		List<GameObject> possibleObstacles = FindObjectOfType<LVLRFYLObstacleSpawnerManager>().obstacles;
		GameObject obstacleType = possibleObstacles[Random.Range(0, possibleObstacles.Count)];
		if (lastObstacleType == null) lastObstacleType = obstacleType;
		else
		{
			while(obstacleType == lastObstacleType)
			{
				obstacleType = possibleObstacles[Random.Range(0, possibleObstacles.Count)];
			}
			lastObstacleType = obstacleType;
		}
		#endregion

		#region Correct Instantiation Data
		GameObject obstacle = Instantiate(obstacleType, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))), transform);
		Vector2 point = new Vector2(Random.Range(-boxSize.x * .5f, boxSize.x * .5f), Random.Range(-boxSize.y * .5f, boxSize.y * .5f));
		obstacle.transform.localPosition = point;
		obstacle.transform.localScale = Vector3.Scale(new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z), obstacleType.transform.localScale);
		#endregion

		currentObstaclesInTile++;
	}
}
