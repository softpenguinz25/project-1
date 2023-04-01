using com.spacepuppy.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TileV2
{
	//Position
	[HideInInspector] public Vector2Int tilePosition;

	//Walls
	[SerializeField] protected bool[] walls = new bool[4];
	int numWalls => GetNumWalls();

	//CPs
	public List<Vector2Int> cps;

	//Tile Type
	protected virtual TileV2 Tile(TileV2 closestTile)
	{
		return this;
	}

	public enum TileType { Open, Split, Hall, Corner, End, Closed,
		GroupTile1,
		GroupTile2,
		GroupTile3,
		GroupTile4,
		GroupTile5,
		GroupTile6,
		GroupTile7,
		GroupTile8,
		GroupTile9,
		GroupTile10,
		GroupTile11,
		GroupTile12,
		GroupTile13,
		GroupTile14,
		GroupTile15,
		GroupTile16,
	}
	[SerializeField] TileType tileType => GetTileType();

	//Spawn
	[HideInInspector] public bool hasSpawned;
	GameObject goToSpawn;
	GameObject tileGO;

	//Debugging
	float testingSphereWidth = .1f;
	Color tileColor = Color.blue, cpColor = Color.cyan;
	float wallDistFromTile = .45f;

	public TileV2(TileType tileType)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		walls = GetWallsFromTileType(tileType);
		cps = InitialCPs();
	}

	public TileV2(bool[] walls)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		this.walls = walls;
		cps = InitialCPs();
	}

	public TileV2(Vector2Int tilePosition, bool[] walls, List<Vector2Int> cps, GameObject goToSpawn)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		this.tilePosition = tilePosition;
		this.walls = walls;
		this.cps = cps;
		this.goToSpawn = goToSpawn;
	}

	public virtual int GetConnectingCPIndex()
	{
		return Random.Range(0, cps.Count);
	}

	int GetNumWalls()
	{
		int numWalls = 0;
		foreach (bool wall in walls)
		{
			if (wall) numWalls++;
		}
		return numWalls;
	}
	protected virtual TileType GetTileType()
	{
		if (numWalls == 0) return TileType.Open;
		if (numWalls == 1) return TileType.Split;
		if (numWalls == 3) return TileType.End;
		if (numWalls == 4) return TileType.Closed;

		int[] wallPos = new int[numWalls];
		int currentIndex = 0;
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i])
			{
				wallPos[currentIndex] = i + 1;
				currentIndex++;
			}
		}

		if (Mathf.Abs(wallPos[0] - wallPos[1]) == 2) return TileType.Hall;
		return TileType.Corner;
	}

	bool[] GetWallsFromTileType(TileType tileType)
	{
		switch (tileType)
		{
			case TileType.Open: return new bool[] { false, false, false, false };
			case TileType.Split:
				bool[] split = new bool[] { false, false, false, false };
				int splitIndex = Random.Range(0, split.Length);
				split[splitIndex] = true;
				return split;
			case TileType.Hall:
				if (Random.value < .5f) return new bool[] { true, false, true, false };
				else return new bool[] { false, true, false, true };
			case TileType.Corner:
				int cornerChance = Random.Range(1, 5);
				switch (cornerChance)
				{
					case 1: return new bool[] { true, true, false, false };
					case 2: return new bool[] { false, true, true, false };
					case 3: return new bool[] { false, false, true, true };
					case 4: return new bool[] { true, false, false, false };
				}
				return new bool[] { true, true, false, false };
			case TileType.End:
				bool[] end = new bool[] { true, true, true, true };
				int endIndex = Random.Range(0, end.Length);
				end[endIndex] = false;
				return end;
			case TileType.Closed: return new bool[] { true, true, true, true };
			default: return null;
		}
	}

	protected virtual List<Vector2Int> InitialCPs()
	{
		List<Vector2Int> result = new List<Vector2Int>();

		for (int i = 0; i < walls.Length; i++)
		{
			if (!walls[i])
			{
				result.Add(new Vector2Int(
					Mathf.RoundToInt(Mathf.Sin(i * Mathf.PI * .5f)), 
					Mathf.RoundToInt(Mathf.Cos(i * Mathf.PI * .5f))) 
					+ tilePosition);
			}
		}

		return result;
	}

	public virtual bool IsOverlappingWithPosList(List<Vector2Int> posList)
	{
		return posList.Contains(tilePosition);
	}

	public virtual void MoveTileByDir(Vector2Int dir)
	{
		tilePosition += dir;
		for (int i = 0; i < cps.Count; i++) cps[i] += dir;
	}

	public virtual void Rotate(int degrees)
	{
		if (degrees % 90 != 0)
		{
			Debug.LogError("Cannot rotate tile by a non-multiple of 90.");
			return;
		}

		int rotations = degrees / 90;

		//Rotate Walls
		bool[] rotatedWalls = new bool[walls.Length];

		for (int i = 0; i < walls.Length; i++)
		{
			int newIndex = (i + rotations) % walls.Length;
			rotatedWalls[newIndex] = walls[i];
		}

		walls = rotatedWalls;

		//Rotate CPs
		for (int i = 0; i < cps.Count; i++)
		{
			Vector2Int localCPPos = ConvertCPToLocalSpace(this, cps[i]);

			cps[i] = Vector2Int.RoundToInt(VectorUtil.RotateBy(localCPPos, -degrees)) + tilePosition;
		}
	}

	int GetTileRotation()
	{
		switch (GetTileType())
		{
			case TileType.Open: return Random.Range(0, 4) * -90;
			case TileType.Split:
				int splitIndex = 0;
				for (int splitWallIndex = 1; splitWallIndex < walls.Length; splitWallIndex++) 
					if (walls[splitWallIndex]) { splitIndex = splitWallIndex; break; }
				return splitIndex * -90;
			case TileType.Hall: return walls[0] ? Random.Range(0, 2) * -180 : Random.Range(0, 2) * -180 - 90;
			case TileType.Corner:
				int totalCornerIndex = 0;
				for (int i = 0; i < walls.Length; i++) if (walls[i]) totalCornerIndex += i;
				switch (totalCornerIndex)
				{
					case 1: return 0;
					case 3: return walls[1] ? -90 : -270;
					case 5: return 180;
					default: Debug.LogError("Corner rotation calculation didn't work"); return 45;
				}
			case TileType.End:
				int endIndex = 0;
				for (int endWallIndex = 1; endWallIndex < walls.Length; endWallIndex++)
					if (!walls[endWallIndex]) { endIndex = endWallIndex; break; }
				return endIndex * -90;
			case TileType.Closed: return Random.Range(0, 4) * -90;
		}

		Debug.LogError("Tile rotation calculation didn't work");
		return 45;
	}
	
	public virtual void DrawTile(Color tileColor = new Color(), Color cpColor = new Color(), float sphereWidth = 0)
	{
		float testingSphereWidth = sphereWidth == 0 ? this.testingSphereWidth : sphereWidth;

		Gizmos.color = tileColor == Color.clear ? this.tileColor : tileColor;
		Gizmos.DrawSphere((Vector3Int)tilePosition, testingSphereWidth);

		for (int i = 0; i < walls.Length; i++)
		{
			switch (i)
			{
				case 0: if (walls[i]) Gizmos.DrawLine(tilePosition + new Vector2(-wallDistFromTile, wallDistFromTile), tilePosition + new Vector2(wallDistFromTile, wallDistFromTile)); break;
				case 1: if (walls[i]) Gizmos.DrawLine(tilePosition + new Vector2(wallDistFromTile, wallDistFromTile), tilePosition + new Vector2(wallDistFromTile, -wallDistFromTile)); break;
				case 2: if (walls[i]) Gizmos.DrawLine(tilePosition + new Vector2(wallDistFromTile, -wallDistFromTile), tilePosition + new Vector2(-wallDistFromTile, -wallDistFromTile)); break;
				case 3: if (walls[i]) Gizmos.DrawLine(tilePosition + new Vector2(-wallDistFromTile, -wallDistFromTile), tilePosition + new Vector2(-wallDistFromTile, wallDistFromTile)); break;
			}
		}

		Gizmos.color = cpColor == Color.clear ? this.cpColor : cpColor;
		foreach(Vector2Int cp in cps)
		{
			Gizmos.DrawSphere((Vector3Int)cp, testingSphereWidth);
		}
	}

	public virtual void RemoveCP(TileDataManagerV2 tdm, Vector2Int cp)
	{
		if (!cps.Contains(cp))
		{
			Debug.LogError("Cannot Remove CP bc parent tile does not contain cp! (TileV2)");
			return;
		}
			
		tdm.RemoveCP(this, cp);
		cps.Remove(cp);
	}

	protected virtual Vector2Int ConvertCPToLocalSpace(TileV2 cpOwner, Vector2Int cp)
	{
		return cp - cpOwner.tilePosition;
	}

	protected virtual int DirBetweenTiles(TileV2 tile, TileV2 otherTile)
	{
		Vector2Int aPos = tile.tilePosition, bPos = otherTile.tilePosition;

		if (Vector2Int.Distance(aPos, bPos) != 1)
		{
			Debug.Log("Tiles are not adjacent!");
			return -1;
		}

		if (bPos.y > aPos.y) return 0;
		if (bPos.x > aPos.x) return 1;
		if (bPos.y < aPos.y) return 2;
		if (bPos.x < aPos.x) return 3;

		Debug.LogError("Direction Calculation Between " + aPos + " and " + bPos + " messed up.");
		return -1;
	}

	public int NumWallsBetweenTiles(TileV2 otherTile)
	{
		int numWalls = 0;

		//Determine dir btwn tiles
		int dir = DirBetweenTiles(Tile(otherTile), otherTile);
		
		if(dir == -1) return -1;

		//Increment numWalls
		int tileWalls = Tile(otherTile).walls[dir] ? 1 : 0;
		int otherTileWalls = otherTile.walls[(dir + 2) % 4] ? 1 : 0;

		numWalls += tileWalls + otherTileWalls;

		return numWalls;
	}

	public void RemoveWalls(TileV2 otherTile)
	{
		//Determine dir btwn tiles
		int dir = DirBetweenTiles(Tile(otherTile), otherTile);
		int otherDir = (dir + 2) % 4;

		if (dir == -1)
		{
			return;
		}

		//Alter wall gizmos
		Tile(otherTile).walls[dir] = false;
		otherTile.walls[otherDir] = false;

		//Destroy GO walls if GO has spawned
		if (Tile(otherTile).hasSpawned) 
			foreach (TileWallV2 tileWall in Tile(otherTile).tileGO.GetComponentsInChildren<TileWallV2>()) 
				if (tileWall.wallIndex == dir) 
					UnityEngine.Object.Destroy(tileWall.gameObject);

		if (otherTile.hasSpawned) 
			foreach (TileWallV2 otherTileWall in otherTile.tileGO.GetComponentsInChildren<TileWallV2>()) 
				if (otherTileWall.wallIndex == otherDir) 
					UnityEngine.Object.Destroy(otherTileWall.gameObject);
	}

	public virtual void Spawn(TileCollectionV2 tc)
	{
		hasSpawned = true;

		GameObject goToSpawn = this.goToSpawn == null ? (GameObject)tc.tileSpawnChances[tileType].tileGO : this.goToSpawn;
		tileGO = UnityEngine.Object.Instantiate(goToSpawn, (Vector3Int)tilePosition, Quaternion.Euler(0, 0, GetTileRotation()));

		Vector2Int chunk = TileLoaderV2.GetChunkFromPos(tilePosition, TileLoaderV2.ChunkSize);
		if (TileLoaderV2.chunkGOs.ContainsKey(chunk))
			tileGO.transform.parent = TileLoaderV2.chunkGOs[chunk].transform;
	}

	public virtual void ChangeLoadState(bool loadState)
	{
		tileGO.SetActive(loadState);
	} 

	public virtual void AddTile(TileDataManagerV2 tdm)
	{
		tdm.AddTile(this);
	}

	public override string ToString()
	{
		return tilePosition.ToString() + " (" + tileType.ToString() +")";
	}
}
