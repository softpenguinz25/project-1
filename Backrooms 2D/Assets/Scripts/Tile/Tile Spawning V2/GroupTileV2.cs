using Array2DEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroupTileV2 : TileV2
{
	//References
	GroupTileV2Data groupTileData;

	//Tiles
	List<TileV2> childTiles = new();
	TileV2 childTileOfConnectingCP;
	TileType groupTileType;

	//CPs
	Dictionary<TileV2, List<Vector2Int>> groupCPsSorted = new();


	//Debugging
	float testingSphereWidth = .15f;
	Color tileColor = new Color(1, .5f, 0), cpColor = Color.red;
	float wallDistFromTile = .45f;

	//Events
	event Action<TileV2> ChildTileAdded;
	void AddGroupCP(TileV2 cpOwner, Vector2Int cp)
	{
		cps.Add(cp);

		if (groupCPsSorted.Keys.Contains(cpOwner)) groupCPsSorted[cpOwner].Add(cp);
		else groupCPsSorted.Add(cpOwner, new List<Vector2Int> { cp });
	}

	protected override TileV2 Tile(TileV2 closestTile)
	{
		return GetClosestTile(closestTile);
	}

	public GroupTileV2(TileType tileType, GroupTileV2Data groupTileData) : base(tileType)
	{
		cps = new();
		groupTileType = tileType;
		this.groupTileData = groupTileData;

		ChildTileAdded += AddGroupCPFromTile;

		childTiles = DecodeGroupTileData(groupTileData);
	}

	List<TileV2> DecodeGroupTileData(GroupTileV2Data groupTileData)
	{
		List<TileV2> childTiles = new();

		//Loop Through All Tile Strings
		Array2DString tileStrings = groupTileData.tileLayout;
		for (int x = 0; x < tileStrings.GridSize.x; x++)
		{
			for (int y = 0; y < tileStrings.GridSize.y; y++)
			{
				string tileString = tileStrings.GetCell(x, y);

				//Walls
				string wallString = tileString.Substring(0, 4);
				if (!HelperMethods.isBinary(wallString))
				{
					Debug.LogError("Tile " + tileString + " at (" + x + ", " + y + ") has incorrect wall encoding.");
					break;
				}

				bool[] tileWalls = new bool[4];

				for (int wallCharIndex = 0; wallCharIndex < wallString.Length; wallCharIndex++)
					tileWalls[wallCharIndex] = wallString[wallCharIndex] == '0' ? false : true;

				//CPs
				string cpString = tileString.Substring(5, 4);
				if (!HelperMethods.isBinary(cpString))
				{
					Debug.LogError("Tile " + tileString + " at (" + x + ", " + y + ") has incorrect CP encoding.");
					break;
				}

				List<Vector2Int> tileCPs = new(4);

				for (int cpCharIndex = 0; cpCharIndex < cpString.Length; cpCharIndex++)
					if (cpString[cpCharIndex] == '1')
						tileCPs.Add(new Vector2Int(
							Mathf.RoundToInt(Mathf.Sin(cpCharIndex * Mathf.PI * .5f)) + x,
							Mathf.RoundToInt(Mathf.Cos(cpCharIndex * Mathf.PI * .5f)) - y));

				//GameObject
				string goString = tileString.Substring(10);
				string goPath = "lvl_" + groupTileData.tileCollection.levelName + "_" + goString;

				GameObject tileGO = Resources.Load<GameObject>(goPath);

				if (tileGO == null)
				{
					Debug.LogError("Tile " + tileString + " at (" + x + ", " + y + ") could not find " + goPath + " in a Resources folder.");
					break;
				}

				//Add to childTiles list
				TileV2 childTile = new(new Vector2Int(x, -y), tileWalls, tileCPs, tileGO);
				childTiles.Add(childTile);

				//Set other data
				ChildTileAdded?.Invoke(childTile);
			}
		}

		return childTiles;
	}

	void AddGroupCPFromTile(TileV2 childTile)
	{
		if (childTile.cps.Count <= 0) return;

		foreach (Vector2Int tileCP in childTile.cps)
		{
			AddGroupCP(childTile, tileCP);
		}
	}

	public override int GetConnectingCPIndex()
	{
		int tileIndex = Random.Range(0, groupCPsSorted.Keys.Count);
		int cpIndex = Random.Range(0, groupCPsSorted.ElementAt(tileIndex).Value.Count);
		childTileOfConnectingCP = groupCPsSorted.ElementAt(tileIndex).Key;

		tilePosition = groupCPsSorted.ElementAt(tileIndex).Key.tilePosition;
		return cps.IndexOf(groupCPsSorted.ElementAt(tileIndex).Value[cpIndex]);
	}

	protected override TileType GetTileType()
	{
		return groupTileType;
	}

	protected override List<Vector2Int> InitialCPs()
	{
		return cps;
	}

	public override bool IsOverlappingWithPosList(List<Vector2Int> posList)
	{
		foreach (TileV2 childTile in childTiles)
			if (posList.Contains(childTile.tilePosition)) return true;

		return false;
	}

	public override void MoveTileByDir(Vector2Int dir)
	{
		//Individual Child Tiles
		foreach (TileV2 childTile in childTiles)
		{
			childTile.MoveTileByDir(dir);

			if (childTile == childTileOfConnectingCP)
				tilePosition = childTile.tilePosition;

			//Overall Group Tile
			TryMoveSortedCPs(childTile, dir);
		}

		for (int cpIndex = 0; cpIndex < cps.Count; cpIndex++) cps[cpIndex] += dir;
	}

	private void TryMoveSortedCPs(TileV2 childTile, Vector2Int dir)
	{
		if (groupCPsSorted.ContainsKey(childTile))
			for (int groupCPIndex = 0; groupCPIndex < groupCPsSorted[childTile].Count; groupCPIndex++)
				groupCPsSorted[childTile][groupCPIndex] += dir;
	}

	public override void Rotate(int degrees)
	{
		if (degrees % 90 != 0)
		{
			Debug.LogError("Cannot rotate tile by a non-multiple of 90.");
			return;
		}

		float rotationsInRadians = degrees * Mathf.Deg2Rad;

		foreach (TileV2 childTile in childTiles)
		{
			//Rotate individual tile
			childTile.Rotate(degrees);

			//Move tiles in relation to pivot (0, 0)
			Vector2Int rotateMoveDir = new Vector2Int(
				Mathf.RoundToInt(childTile.tilePosition.x * Mathf.Cos(rotationsInRadians) + childTile.tilePosition.y * Mathf.Sin(rotationsInRadians)),
				Mathf.RoundToInt(-childTile.tilePosition.x * Mathf.Sin(rotationsInRadians) + childTile.tilePosition.y * Mathf.Cos(rotationsInRadians)))
				- childTile.tilePosition;

			childTile.MoveTileByDir(rotateMoveDir);

			//Overall Group Tile
			TryMoveSortedCPs(childTile, rotateMoveDir);
		}
	}

	public override void DrawTile(Color tileColor = default, Color cpColor = default, float sphereWidth = 0)
	{
		float testingSphereWidth = /*sphereWidth == 0 ? */this.testingSphereWidth/* : sphereWidth*/;
		Color testingTileColor = /*tileColor == Color.clear ? */this.tileColor/* : tileColor*/;
		Color testingCPColor = /*cpColor == Color.clear ? */this.cpColor/* : cpColor*/;

		foreach (TileV2 childTile in childTiles) 
			childTile.DrawTile(testingTileColor, testingCPColor, testingSphereWidth);
	}

	public override void RemoveCP(TileDataManagerV2 tdm, Vector2Int cp)
	{
		foreach (TileV2 cpOwner in GetCPOwnersFromCP(cp))
		{
			tdm.RemoveCP(cpOwner, cp);
			cps.Remove(cp);
		}
	}
	private List<TileV2> GetCPOwnersFromCP(Vector2Int cp)
	{
		List<TileV2> possibleCPOwners = new(4);

		foreach (KeyValuePair<TileV2, List<Vector2Int>> groupCPSorted in groupCPsSorted)
		{
			if (groupCPsSorted[groupCPSorted.Key].Contains(cp))
				possibleCPOwners.Add(groupCPSorted.Key);
		}

		return possibleCPOwners;
	}

	protected override int DirBetweenTiles(TileV2 tile, TileV2 otherTile)
	{
		TileV2 closestTile = GetClosestTile(otherTile);

		return base.DirBetweenTiles(closestTile, otherTile);
	}

	private TileV2 GetClosestTile(TileV2 otherTile)
	{
		List<float> tileDistances = new();
		int childTileIndex = 0;
		while (childTileIndex < childTiles.Count)
		{
			tileDistances.Add(Vector2.Distance(childTiles[childTileIndex].tilePosition, otherTile.tilePosition));
			childTileIndex++;
		}

		TileV2 closestTile = childTiles[0];
		float closestDistance = tileDistances[0];

		for (int tileDistanceIndex = 1; tileDistanceIndex < tileDistances.Count; tileDistanceIndex++)
		{
			if (tileDistances[tileDistanceIndex] < closestDistance)
			{
				closestTile = childTiles[tileDistanceIndex];
				closestDistance = tileDistances[tileDistanceIndex];
			}
		}

		return closestTile;
	}

	public override void Spawn(TileCollectionV2 tc)
	{
		foreach (TileV2 childTile in childTiles) childTile.Spawn(tc);
	}

	public override void ChangeLoadState(bool loadState)
	{
		foreach (TileV2 childTile in childTiles) childTile.ChangeLoadState(loadState);
	}

	public override void AddTile(TileDataManagerV2 tdm)
	{
		foreach (TileV2 childTile in childTiles) childTile.AddTile(tdm);
	}
}
