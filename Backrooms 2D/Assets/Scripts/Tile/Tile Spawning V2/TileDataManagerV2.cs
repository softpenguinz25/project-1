using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileSpawnerV2))]
[RequireComponent(typeof(TileLoaderV2))]

public class TileDataManagerV2 : MonoBehaviour
{
	TileSpawnerV2 ts;
	TileLoaderV2 tl;

	public Dictionary<Vector3, TileV2> tileDict = new Dictionary<Vector3, TileV2>();
	public Dictionary<Vector3, TileV2> cpDict = new Dictionary<Vector3, TileV2>();

	public event Action<Vector3> CPAdded;
	private void Awake()
	{
		ts = GetComponent<TileSpawnerV2>();
		tl = GetComponent<TileLoaderV2>();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		foreach (KeyValuePair<Vector3, TileV2> tilePair in tileDict) tilePair.Value.DrawTile();
	}
#endif

	public void AddTile(TileV2 tile)
	{
		tileDict.Add(tile.tilePosition, tile);
		foreach (Vector3 cp in tile.cps) AddCP(cp, tile);
	}

	void AddCP(Vector3 cp, TileV2 cpOwner)
	{
		cpDict.Add(cp, cpOwner);

		CPAdded?.Invoke(cp);
	}
}