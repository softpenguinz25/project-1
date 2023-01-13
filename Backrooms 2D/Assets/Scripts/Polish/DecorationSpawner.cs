using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationSpawner : MonoBehaviour
{
	[SerializeField] RangedInt tilesUntilDecorationSpawnRange = new RangedInt();
	int tilesUntilDecorationSpawn;
	[SerializeField] List<DecorationData> decorations = new List<DecorationData>();

	TileDataManager tdm;

	private void Awake()
	{
		tdm = FindObjectOfType<TileDataManager>();
	}

	private void Start()
	{
		tilesUntilDecorationSpawn = UnityEngine.Random.Range(tilesUntilDecorationSpawnRange.Min, tilesUntilDecorationSpawnRange.Max);
	}

	private void OnEnable()
	{
		tdm.TileAdded += AttemptToSpawnDecoration;
	}

	private void OnDisable()
	{
		tdm.TileAdded -= AttemptToSpawnDecoration;
	}

	private void AttemptToSpawnDecoration(TilePrefab tilePrefab, int numTiles)
	{
		if(tilesUntilDecorationSpawn > 0)
		{
			tilesUntilDecorationSpawn--;
			return;
		}

		DecorationData randomSpawnChanceDecoration = RandomSpawnChanceDecoration(decorations);
		GameObject decoration = Instantiate(randomSpawnChanceDecoration.decoration, tilePrefab.transform.position, Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f))), tilePrefab.transform);
		decoration.transform.localScale = randomSpawnChanceDecoration.localScale;
		decoration.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(EnableSpriteRenderer(decoration.GetComponent<SpriteRenderer>()));
		tilesUntilDecorationSpawn = UnityEngine.Random.Range(tilesUntilDecorationSpawnRange.Min, tilesUntilDecorationSpawnRange.Max);
	}

	IEnumerator EnableSpriteRenderer(SpriteRenderer spriteRendererToEnable)
	{
		yield return new WaitForSeconds(.3f);

		spriteRendererToEnable.enabled = true;
	}

	//THANKS Dev Leonardo! https://www.youtube.com/watch?v=Gj7UU5IU3-E
	private DecorationData RandomSpawnChanceDecoration(List<DecorationData> decorationDatas)
	{
		float totalDenomination = 0;

		foreach (DecorationData decorationData in decorationDatas)
		{
			totalDenomination += decorationData.spawnChance;
		}

		float randomNumber = UnityEngine.Random.Range(0, totalDenomination);

		foreach (DecorationData decorationData in decorationDatas)
		{
			if (decorationData.spawnChance >= randomNumber)
			{
				return decorationData;
			}

			randomNumber -= decorationData.spawnChance;
		}

		Debug.LogError("Random Tile Generation Failed!");
		return null;
	}
}

[Serializable]
public class DecorationData
{
	public GameObject decoration;
	public Vector3 localScale;
	[Range(0, 1)] public float spawnChance = .5f;
}
