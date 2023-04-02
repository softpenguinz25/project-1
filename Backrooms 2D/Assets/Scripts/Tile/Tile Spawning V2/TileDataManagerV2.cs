using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileLoaderV2))]

public class TileDataManagerV2 : MonoBehaviour
{
	TileSpawnerV2 ts;
	TileLoaderV2 tl;

	public Dictionary<Vector2Int, TileV2> tileDict = new();
	public Dictionary<TileV2, List<Vector2Int>> cpDict = new();

	public event Action<TileV2, Vector2Int> TileCPAdded;
	public event Action<TileV2, Vector2Int> TileCPRemoved;
	private void Awake()
	{
		ts = GetComponent<TileSpawnerV2>();
		tl = GetComponent<TileLoaderV2>();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		foreach (KeyValuePair<Vector2Int, TileV2> tilePair in tileDict) tilePair.Value.DrawTile();
	}
#endif

	public void AddTile(TileV2 tile)
	{
		tileDict.Add(tile.tilePosition, tile);

		AddCP(tile);
	}

	void AddCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (cpDict.ContainsKey(cpOwner)) cpDict[cpOwner].Add(cp);
		else cpDict.Add(cpOwner, new List<Vector2Int> { cp});

		TileCPAdded?.Invoke(cpOwner, cp);
	}

	void AddCP(TileV2 cpOwner)
	{
		if (cpOwner.cps.Count <= 0)
		{
			TileCPAdded?.Invoke(cpOwner, TileLoaderV2.NULL_CP);
			return;
		}

		if (!cpDict.ContainsKey(cpOwner)) cpDict.Add(cpOwner, new List<Vector2Int>());

		foreach (Vector2Int cp in cpOwner.cps)
		{
			cpDict[cpOwner].Add(cp);

			TileCPAdded?.Invoke(cpOwner, cp);
		}
	}

	public void RemoveCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (!cpOwner.cps.Contains(cp))
		{
			Debug.LogError("Cannot Remove CP bc parent tile does not contain cp! (TDM)");
			return;
		}

		//Remove CP from cpDict
		cpDict[cpOwner].Remove(cp);
		if (cpDict[cpOwner].Count <= 0) cpDict.Remove(cpOwner);

		TileCPRemoved?.Invoke(cpOwner, cp);
	}

	public Dictionary<Vector2Int, TileV2> GetSurroundingTiles(Vector2Int tilePos)
	{
		Dictionary<Vector2Int, TileV2> surroundingTiles = new();
		for(int i = 0; i < 360; i += 90)
		{
			Vector2Int surroundingTilePos = new Vector2Int(
				Mathf.RoundToInt(tilePos.x + Mathf.Sin(i * Mathf.Deg2Rad)), 
				Mathf.RoundToInt(tilePos.y + Mathf.Cos(i * Mathf.Deg2Rad)));

			if (tileDict.ContainsKey(surroundingTilePos))
				surroundingTiles.Add(tileDict[surroundingTilePos].tilePosition, tileDict[surroundingTilePos]);
		}
		return surroundingTiles;
	}

#if UNITY_EDITOR
	public List<Vector2Int> tilePos = new();
	public List<CPDataV2> cps = new();

	private void Update()
	{
		tilePos.Clear();
		cps.Clear();

		tilePos = tileDict.Keys.ToList();
		foreach(KeyValuePair<TileV2, List<Vector2Int>> cpData in cpDict)
		{
			cps.Add(new CPDataV2(cpData.Key, cpData.Value));
		}
	}
}
#endif

[Serializable]
public class CPDataV2
{
	[HideInInspector] public TileV2 tile;
	public Vector2Int tilePos;
	public List<Vector2Int> cps;

	public CPDataV2(TileV2 t, List<Vector2Int> cp)
	{
		tile = t;
		tilePos = t.tilePosition;
		cps = cp;
	}
}