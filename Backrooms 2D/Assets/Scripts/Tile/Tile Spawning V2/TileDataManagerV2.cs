using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileLoaderV2))]
[RequireComponent(typeof(TilePoolV2))]
public class TileDataManagerV2 : MonoBehaviour
{
	TileSpawnerV2 ts;
	TileLoaderV2 tl;

	private Dictionary<Vector2Int, TileV2> tileDict = new();
	private Dictionary<TileV2, List<Vector2Int>> cpDict = new();
	private static Dictionary<TileV2.TileType, int> tilesPerType = new();

	public event Action<TileV2, int> TileAdded;
	public event Action<TileV2, Vector2Int> TileCPAdded;
	public event Action<TileV2, Vector2Int> TileCPRemoved;

	public static int NumTiles;

	public Dictionary<Vector2Int, TileV2> TileDict { get => tileDict; set => tileDict = value; }
	public Dictionary<TileV2, List<Vector2Int>> CpDict { get => cpDict; set => cpDict = value; }
	//TODO: Make this per OBJECT, not TILE TYPE
	public static Dictionary<TileV2.TileType, int> TilesPerType { get => tilesPerType; set => tilesPerType = value; }

	private void Awake()
	{
		ts = GetComponent<TileSpawnerV2>();
		tl = GetComponent<TileLoaderV2>();
	}

	private void Start()
	{
		foreach (TileV2.TileType tileType in ts.Tc.TileSpawnChances.Keys)
		{
			if (!TilesPerType.Keys.Contains(tileType)) TilesPerType.Add(tileType, 0);
			else TilesPerType[tileType] = 0;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		foreach (KeyValuePair<Vector2Int, TileV2> tilePair in TileDict) tilePair.Value.DrawTile();
	}
#endif

	public void AddTile(TileV2 tile)
	{
		TileDict.Add(tile.TilePosition, tile);
		TilesPerType[tile.TileType_]++;
		//Debug.Log(tile.TilePosition);
		TileAdded?.Invoke(tile, tileDict.Count);

		NumTiles = tileDict.Count;

		AddCP(tile);
	}

	void AddCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (CpDict.ContainsKey(cpOwner)) CpDict[cpOwner].Add(cp);
		else CpDict.Add(cpOwner, new List<Vector2Int> { cp});

		TileCPAdded?.Invoke(cpOwner, cp);
	}

	void AddCP(TileV2 cpOwner)
	{
		if (cpOwner.Cps.Count <= 0)
		{
			TileCPAdded?.Invoke(cpOwner, TileLoaderV2.NULL_CP);
			return;
		}

		if (!CpDict.ContainsKey(cpOwner)) CpDict.Add(cpOwner, new List<Vector2Int>());

		foreach (Vector2Int cp in cpOwner.Cps)
		{
			CpDict[cpOwner].Add(cp);

			TileCPAdded?.Invoke(cpOwner, cp);
		}
	}

	public void RemoveCP(TileV2 cpOwner, Vector2Int cp)
	{
		if (!cpOwner.Cps.Contains(cp))
		{
			Debug.LogError("Cannot Remove CP bc parent tile does not contain cp! (TDM)");
			return;
		}

		//Remove CP from cpDict
		CpDict[cpOwner].Remove(cp);
		if (CpDict[cpOwner].Count <= 0) CpDict.Remove(cpOwner);

		TileCPRemoved?.Invoke(cpOwner, cp);
	}

	public Dictionary<Vector2Int, TileV2> GetSurroundingTiles(Vector2Int tilePos)
	{
		Dictionary<Vector2Int, TileV2> surroundingTiles = new();
		for(int i = 0; i < 360; i += 90)
		{
			Vector2Int surroundingTilePos = new Vector2Int(
				Mathf.RoundToInt(tilePos.x + Mathf.Sin(i * Mathf.Deg2Rad) * TileSpawnerV2.TileSize), 
				Mathf.RoundToInt(tilePos.y + Mathf.Cos(i * Mathf.Deg2Rad) * TileSpawnerV2.TileSize));

			if (TileDict.ContainsKey(surroundingTilePos))
				surroundingTiles.Add(TileDict[surroundingTilePos].TilePosition, TileDict[surroundingTilePos]);
		}
		return surroundingTiles;
	}

#if UNITY_EDITOR
	public List<Vector2Int> tilePos = new();
	public List<DebugCPDataV2> cps = new();

	private void Update()
	{
		tilePos.Clear();
		cps.Clear();

		tilePos = TileDict.Keys.ToList();
		foreach(KeyValuePair<TileV2, List<Vector2Int>> cpData in CpDict)
		{
			cps.Add(new DebugCPDataV2(cpData.Key, cpData.Value));
		}
	}
#endif
}

[Serializable]
public class DebugCPDataV2
{
	[HideInInspector] public TileV2 tile;
	public Vector2Int tilePos;
	public List<Vector2Int> cps;

	public DebugCPDataV2(TileV2 t, List<Vector2Int> cp)
	{
		tile = t;
		tilePos = t.TilePosition;
		cps = cp;
	}
}