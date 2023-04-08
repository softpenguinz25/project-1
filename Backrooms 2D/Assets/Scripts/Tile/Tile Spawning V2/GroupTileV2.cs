using Array2DEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroupTileV2 : TileV2
{
	//References
	GroupTileV2Data groupTileData;

	//Tiles
	List<TileV2> childTiles = new();
	TileV2 connectingCPTile;
	TileType groupTileType;

	//CPs
	List<TileV2> childTilesWithCPs = new();
	public override List<Vector2Int> Cps { 
		get {
			List<Vector2Int> cpResult = new();
			foreach (TileV2 childTileWithCP in childTilesWithCPs)
				foreach (Vector2Int childCP in childTileWithCP.Cps) cpResult.Add(childCP);

			return cpResult;
		}
		set => base.Cps = value; 
	}

	//Debugging
	float testingSphereWidth = .15f * TileSpawnerV2.TileSize;
	Color tileColor = new Color(1, .5f, 0), cpColor = Color.red;
	float wallDistFromTile = .45f * TileSpawnerV2.TileSize;

	//Events
	event Action<TileV2> ChildTileAdded;
	event Action<int> ConnectingCPGenerated;

	public GroupTileV2(TileType tileType, GroupTileV2Data groupTileData) : base(tileType)
	{
		groupTileType = tileType;
		this.groupTileData = groupTileData;

		ChildTileAdded += AddGroupCPFromTile;
		ConnectingCPGenerated += (int connectingCPTileIndex) => {
			connectingCPTile = childTilesWithCPs[connectingCPTileIndex];
			TilePosition = connectingCPTile.TilePosition;
		};

		childTiles = DecodeGroupTileData(groupTileData);

		foreach (TileV2 childTile in childTiles) childTile.isPartOfGroupTile = true;
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
				int xPos = x * TileSpawnerV2.TileSize, yPos = y * TileSpawnerV2.TileSize;

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
							Mathf.RoundToInt((Mathf.Sin(cpCharIndex * Mathf.PI * .5f)) + x) * TileSpawnerV2.TileSize ,
							Mathf.RoundToInt((Mathf.Cos(cpCharIndex * Mathf.PI * .5f)) - y) * TileSpawnerV2.TileSize ));

				//GameObject
				GameObject tileGO = null;
				if (tileString.Length > 10)
				{
					string goString = tileString.Substring(10);
					string goPath = "lvl_" + groupTileData.tileCollection.levelName + "_" + goString;

					tileGO = Resources.Load<GameObject>(goPath);

					if (tileGO == null)
					{
						Debug.LogError("Tile " + tileString + " at (" + xPos + ", " + yPos + ") could not find " + goPath + " in a Resources folder.");
						break;
					}
				}

				//Add to childTiles list
				TileV2 childTile = new(new Vector2Int(xPos, -yPos), tileWalls, tileCPs, tileGO);
				childTiles.Add(childTile);

				//Set other data
				ChildTileAdded?.Invoke(childTile);
			}
		}

		return childTiles;
	}

	void AddGroupCPFromTile(TileV2 childTile)
	{
		if (childTile.Cps.Count <= 0) return;

		childTilesWithCPs.Add(childTile);
	}

	public override int GetConnectingCPIndex()
	{
		int tileIndex = Random.Range(0, childTilesWithCPs.Count);
		int cpIndex = Random.Range(0, childTilesWithCPs[tileIndex].Cps.Count);
		ConnectingCPGenerated?.Invoke(tileIndex);
		return Cps.IndexOf(childTilesWithCPs[tileIndex].Cps[cpIndex]);
	}

	protected override TileType GetTileType(bool[] walls)
	{
		return groupTileType;
	}

	protected override List<Vector2Int> InitialCPs()
	{
		return Cps;
	}

	public override bool IsOverlappingWithPosList(HashSet<Vector2Int> posList)
	{
		foreach (TileV2 childTile in childTiles)
			if (posList.Contains(childTile.TilePosition)) return true;

		return false;
	}

	public override void MoveTileByDir(Vector2Int dir)
	{
		//Individual Child Tiles
		foreach (TileV2 childTile in childTiles)
		{
			childTile.MoveTileByDir(dir);

			if (childTile == connectingCPTile)
				TilePosition = childTile.TilePosition;
		}
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
				Mathf.RoundToInt(childTile.TilePosition.x * Mathf.Cos(rotationsInRadians) + childTile.TilePosition.y * Mathf.Sin(rotationsInRadians)),
				Mathf.RoundToInt(-childTile.TilePosition.x * Mathf.Sin(rotationsInRadians) + childTile.TilePosition.y * Mathf.Cos(rotationsInRadians)))
				- childTile.TilePosition;

			childTile.MoveTileByDir(rotateMoveDir);

		}
	}

	public override void DrawTile(Color tileColor = default, Color cpColor = default, float sphereWidth = 0)
	{
		float testingSphereWidth = sphereWidth == 0 ? this.testingSphereWidth : sphereWidth;
		Color testingTileColor = tileColor == Color.clear ? this.tileColor : tileColor;
		Color testingCPColor = cpColor == Color.clear ? this.cpColor : cpColor;

		//Individual Tiles
		foreach (TileV2 childTile in childTiles)
			childTile.DrawTile(testingTileColor, testingCPColor, testingSphereWidth);
	}

	public override void RemoveCP(TileCollectionV2 tc, TileDataManagerV2 tdm, Vector2Int cp, bool replaceWithWall = false)
	{
		//Overall Tiles
		foreach (TileV2 cpOwner in GetCPOwnersFromCP(cp))
		{
			childTilesWithCPs.Remove(cpOwner);
			cpOwner.RemoveCP(tc, tdm, cp);
		}
	}

	private List<TileV2> GetCPOwnersFromCP(Vector2Int cp)
	{
		List<TileV2> possibleCPOwners = new(4);

		foreach (TileV2 childTileWithCP in childTilesWithCPs)
		{
			if (childTileWithCP.Cps.Contains(cp))
				possibleCPOwners.Add(childTileWithCP);
		}

		return possibleCPOwners;
	}

	public override Dictionary<Vector2Int, TileV2> GetSurroundingTiles(TileDataManagerV2 tdm)
	{
		Dictionary<Vector2Int, TileV2> surroundingTiles = new();
		foreach (TileV2 childTile in childTiles)
		{
			Dictionary<Vector2Int, TileV2> childSurroundingTiles = tdm.GetSurroundingTiles(childTile.TilePosition);
			foreach(KeyValuePair<Vector2Int, TileV2> childSurroundingPair in childSurroundingTiles)
			{
				if (!childTiles.Contains(childSurroundingPair.Value))
					surroundingTiles.Add(childSurroundingPair.Key, childSurroundingPair.Value);
			}
		}

		return surroundingTiles;
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
			tileDistances.Add(Vector2.Distance(childTiles[childTileIndex].TilePosition, otherTile.TilePosition));
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

	public override int NumWallsBetweenTiles(TileV2 thisTile, TileV2 otherTile)
	{
		TileV2 closestTile = GetClosestTile(otherTile);
		return base.NumWallsBetweenTiles(closestTile, otherTile);
	}

	public override void RemoveWallsBetweenTiles(TileV2 thisTile, TileV2 otherTile)
	{
		TileV2 closestTile = GetClosestTile(otherTile);
		base.RemoveWallsBetweenTiles(closestTile, otherTile);
	}

	public override bool PosOverlaps(Vector2Int pos)
	{
		List<Vector2Int> childTilePos = new();
		foreach (TileV2 childTile in childTiles) childTilePos.Add(childTile.TilePosition);
		return childTilePos.Contains(pos);
	}

	public override void Spawn(TileCollectionV2 tc)
	{
		Debug.Log("Group Tile Spawn, Spawning " + childTiles.Count + " tiles.");
		foreach (TileV2 childTile in childTiles) childTile.Spawn(tc);
	}

	public override void ChangeLoadState(bool loadState)
	{
		foreach (TileV2 childTile in childTiles) childTile.ChangeLoadState(loadState);
	}

	public override void AddTile(TileDataManagerV2 tdm)
	{
		foreach (TileV2 childTile in childTiles)
		{
			childTile.AddTile(tdm);
		}
	}
}
