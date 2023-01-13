using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLAbandonedOfficeMiddleTileItemGenerator : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float spawnChance = .25f;
	[SerializeField] List<MiddleTileItem> items;

	private IEnumerator Start()
	{
		if (UnityEngine.Random.value > spawnChance) yield break;

		MiddleTileItem itemToSpawn = RandomSpawnChanceDecoration(items);
		GameObject itemToSpawnGO = Instantiate(itemToSpawn.gameObject, transform.position, itemToSpawn.randomizeRotation ? Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f))) : Quaternion.identity, transform);

		itemToSpawnGO.GetComponent<SpriteRenderer>().enabled = false;
		LayerMask oldLayer = itemToSpawnGO.gameObject.layer;
		itemToSpawnGO.gameObject.layer = LayerMask.NameToLayer("Invisible Tile");

		yield return new WaitForSeconds(.3f);

		itemToSpawnGO.gameObject.layer = oldLayer;
		itemToSpawnGO.GetComponent<SpriteRenderer>().enabled = true;
	}

	private MiddleTileItem RandomSpawnChanceDecoration(List<MiddleTileItem> middleTileItems)
	{
		float totalDenomination = 0;

		foreach (MiddleTileItem middleTileItem in middleTileItems)
		{
			totalDenomination += middleTileItem.spawnChance;
		}

		float randomNumber = UnityEngine.Random.Range(0, totalDenomination);

		foreach (MiddleTileItem middleTileItem in middleTileItems)
		{
			if (middleTileItem.spawnChance >= randomNumber)
			{
				return middleTileItem;
			}

			randomNumber -= middleTileItem.spawnChance;
		}

		Debug.LogError("Random Tile Generation Failed!");
		return null;
	}
}

[Serializable]
public class MiddleTileItem
{
	public GameObject gameObject;
	[Range(0, 1)] public float spawnChance = .5f;
	public bool randomizeRotation;
}
