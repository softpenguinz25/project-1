using Array2DEditor;
using System;
using System.Collections.Generic;
using System.Linq;
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

	//Custom GO
	public GameObject customGO;

	//Debugging
	float testingSphereWidth = .15f * TileSpawnerV2.TileSize;
	Color tileColor = new Color(1, .5f, 0), cpColor = Color.red;
	float wallDistFromTile = .45f * TileSpawnerV2.TileSize;

	//Events
	event Action<TileV2> ChildTileAdded;
	event Action<int> ConnectingCPGenerated;

	public GroupTileV2(TileType tileType, GroupTileV2Data groupTileData) : base(tileType)
	{
		//TilePosition = new Vector2Int(int.MaxValue, int.MaxValue);

		groupTileType = tileType;
		this.groupTileData = groupTileData;

		ChildTileAdded += AddGroupCPFromTile;
		ConnectingCPGenerated += (int connectingCPTileIndex) => {
			connectingCPTile = childTilesWithCPs[connectingCPTileIndex];
			TilePosition = connectingCPTile.TilePosition;
		};

		//Get Child Tiles
		childTiles = DecodeGroupTileData(groupTileData);

		//Set Data For All Child Tiles
		foreach (TileV2 childTile in childTiles)
		{
			childTile.isPartOfGroupTile = true;
			childTile.tcToUse = groupTileData.tileCollection;
		}

		//Set other data for group tile
		CanBeRotated = groupTileData.canBeRotated;
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

				if (tileString.Length <= 0) continue;

				//Walls
				bool[] tileWalls = new bool[4];
				if (tileString.Length > 1)
				{
					string wallString = tileString.Substring(0, 4);
					if (!HelperMethods.isBinary(wallString))
					{
						Debug.LogError("Tile " + tileString + " at (" + x + ", " + y + ") has incorrect wall encoding.");
						break;
					}

					for (int wallCharIndex = 0; wallCharIndex < wallString.Length; wallCharIndex++)
						tileWalls[wallCharIndex] = wallString[wallCharIndex] == '0' ? false : true;
				}

				//CPs
				List<Vector2Int> tileCPs = new(4);
				if (tileString.Length > 5)
				{
					string cpString = tileString.Substring(5, 4);
					if (!HelperMethods.isBinary(cpString))
					{
						Debug.LogError("Tile " + tileString + " at (" + x + ", " + y + ") has incorrect CP encoding.");
						break;
					}

					for (int cpCharIndex = 0; cpCharIndex < cpString.Length; cpCharIndex++)
						if (cpString[cpCharIndex] == '1')
							tileCPs.Add(new Vector2Int(
								Mathf.RoundToInt((Mathf.Sin(cpCharIndex * Mathf.PI * .5f)) + x) * TileSpawnerV2.TileSize,
								Mathf.RoundToInt((Mathf.Cos(cpCharIndex * Mathf.PI * .5f)) - y) * TileSpawnerV2.TileSize));
				}

				//GameObject
				TileGOV2 tileGO = null;
				if (tileString.Length > 10)
				{
					string goString = tileString.Substring(10);
					string goPath = "lvl_" + groupTileData.tileCollection.levelName + "_" + goString;

					tileGO = Resources.Load<TileGOV2>(goPath);

					if (tileGO == null)
					{
						Debug.LogError("Tile " + tileString + " at (" + xPos + ", " + yPos + ") could not find " + goPath + " in a Resources folder.");
						break;
					}
				}

				//Add to childTiles list
				TileV2 childTile = new(new Vector2Int(xPos, -yPos), tileWalls, tileCPs, tileGO, groupTileData.canBeUnended);
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
		if (childTilesWithCPs.Count <= 0)
		{
			Debug.LogError("Could Not Find Any Child Tiles With CPs");
			return 0;
		}
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

		//Custom GO
		if (customGO != null)
		{
			customGO.transform.position = (Vector3Int)childTiles[0].TilePosition;
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

		//Custom GO
		if (customGO != null)
		{
			customGO.transform.position = (Vector3Int)childTiles[0].TilePosition;
			customGO.transform.Rotate(new Vector3(0, 0, degrees));
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
				if (!childTiles.Contains(childSurroundingPair.Value) && !surroundingTiles.Keys.Contains(childSurroundingPair.Key))
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

	public override TileV2 GetClosestTile(TileV2 otherTile)
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

	public override void Spawn(TileCollectionV2 tc, TilePoolV2 tp)
	{
		Debug.Log("Group Tile Spawn, Spawning " + childTiles.Count + " tiles.");
		foreach (TileV2 childTile in childTiles) childTile.Spawn(groupTileData.tileCollection, tp);
	}

	public override void ChangeLoadState(bool loadState)
	{
		foreach (TileV2 childTile in childTiles) childTile.ChangeLoadState(loadState);
	}

	public override void AddTile(TileDataManagerV2 tdm)
	{
		TileDataManagerV2.TilesPerType[TileType.GroupTile]++;
		foreach (TileV2 childTile in childTiles)
		{
			childTile.AddTile(tdm);
		}
	}

	public override void ValidateTile()
	{
		//Instantiate Custom GO
		if (groupTileData.customGO != null)
		{
			customGO = UnityEngine.Object.Instantiate(groupTileData.customGO, (Vector3Int)GetCustomGOPos(), Quaternion.Euler(childTiles[0].TileRotation.eulerAngles - new Vector3(0,0,90)));
			customGO.transform.localScale = new Vector3(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
		}
	}

	Vector2Int GetCustomGOPos()
	{
		Vector2Int pos = childTiles[0].TilePosition;
		for (int i = 1; i < childTiles.Count; i++)
		{
			if (childTiles[i].TilePosition.x < pos.x) pos.x = childTiles[i].TilePosition.x;
			if (childTiles[i].TilePosition.y > pos.y) pos.y = childTiles[i].TilePosition.y;
		}
		return pos;
	}
}
