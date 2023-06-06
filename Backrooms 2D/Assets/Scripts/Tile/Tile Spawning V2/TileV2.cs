using com.spacepuppy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TileV2
{
	//Position
	private Vector2Int tilePosition;

	//Walls
	[SerializeField] protected bool[] walls = new bool[4];
	int numWalls => GetNumWalls();
	[HideInInspector] bool canBeUnended => goToSpawn == null;

	//CPs
	//TODO: Refactor this into a bool[] and add a get cps method
	private List<Vector2Int> cps;

	//Tile Type
	public enum TileType
	{
		Open, Split, Hall, Corner, End, Closed,
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
	[SerializeField] TileType tileType => GetTileType(walls);
	public TileType TileType_ => tileType;

	//Spawn
	[HideInInspector] private bool hasSpawned;
	TileGOV2 goToSpawn;
	//TODO: REPLACE ALL GOs WITH TILEGOV2
	/*TileType tileTypeToSpawn
	{
		get
		{
			return 
		}
	}*/

	TileGOV2 tileGO;

	//Debugging
	public bool isPartOfGroupTile;
	float testingSphereWidth = .1f * TileSpawnerV2.TileSize, groupTestingSphereWidth = .15f * TileSpawnerV2.TileSize;
	Color tileColor = Color.blue, cpColor = Color.cyan,
		groupTileColor = new Color(1, .5f, 0), groupCpColor = Color.red;
	float wallDistFromTile = .45f * TileSpawnerV2.TileSize;

	public Vector2Int TilePosition { get => tilePosition; set => tilePosition = value; }
	public Quaternion TileRotation { get => Quaternion.Euler(new Vector3(0, 0, GetTileRotation())); set => TileRotation = value; }
	virtual public List<Vector2Int> Cps { get => cps; set => cps = value; }
	public bool HasSpawned { get => hasSpawned; set => hasSpawned = value; }

	public TileV2(TileType tileType)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		walls = GetWallsFromTileType(tileType);
		Cps = InitialCPs();
	}

	public TileV2(bool[] walls)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		this.walls = walls;
		Cps = InitialCPs();
	}

	public TileV2(Vector2Int tilePosition, bool[] walls, List<Vector2Int> cps, TileGOV2 goToSpawn)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		this.TilePosition = tilePosition;
		this.walls = walls;
		this.Cps = cps;
		this.goToSpawn = goToSpawn;
	}

	public virtual int GetConnectingCPIndex()
	{
		return Random.Range(0, Cps.Count);
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
	protected virtual TileType GetTileType(bool[] walls)
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
					Mathf.RoundToInt(Mathf.Sin(i * Mathf.PI * .5f) * TileSpawnerV2.TileSize),
					Mathf.RoundToInt(Mathf.Cos(i * Mathf.PI * .5f) * TileSpawnerV2.TileSize))
					+ TilePosition);
			}
		}

		return result;
	}

	public virtual bool IsOverlappingWithPosList(HashSet<Vector2Int> posList)
	{
		return posList.Contains(TilePosition);
	}

	public virtual void MoveTileByDir(Vector2Int dir)
	{
		TilePosition += dir;
		for (int i = 0; i < Cps.Count; i++) Cps[i] += dir;
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
		for (int i = 0; i < Cps.Count; i++)
		{
			Vector2Int localCPPos = ConvertCPToLocalSpace(this, Cps[i]);

			Cps[i] = Vector2Int.RoundToInt(VectorUtil.RotateBy(localCPPos, -degrees)) + TilePosition;
		}
	}

	int GetTileRotation()
	{
		switch (GetTileType(walls))
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
		float testingSphereWidth = !isPartOfGroupTile ? sphereWidth == 0 ? this.testingSphereWidth : sphereWidth : groupTestingSphereWidth;

		Gizmos.color = !isPartOfGroupTile ? tileColor == Color.clear ? this.tileColor : tileColor : groupTileColor;
		Gizmos.DrawSphere((Vector3Int)TilePosition, testingSphereWidth);

		for (int i = 0; i < walls.Length; i++)
		{
			switch (i)
			{
				case 0: if (walls[i]) Gizmos.DrawLine(TilePosition + new Vector2(-wallDistFromTile, wallDistFromTile), TilePosition + new Vector2(wallDistFromTile, wallDistFromTile)); break;
				case 1: if (walls[i]) Gizmos.DrawLine(TilePosition + new Vector2(wallDistFromTile, wallDistFromTile), TilePosition + new Vector2(wallDistFromTile, -wallDistFromTile)); break;
				case 2: if (walls[i]) Gizmos.DrawLine(TilePosition + new Vector2(wallDistFromTile, -wallDistFromTile), TilePosition + new Vector2(-wallDistFromTile, -wallDistFromTile)); break;
				case 3: if (walls[i]) Gizmos.DrawLine(TilePosition + new Vector2(-wallDistFromTile, -wallDistFromTile), TilePosition + new Vector2(-wallDistFromTile, wallDistFromTile)); break;
			}
		}

		Gizmos.color = !isPartOfGroupTile ? cpColor == Color.clear ? this.cpColor : cpColor : groupCpColor;
		foreach (Vector2Int cp in Cps)
		{
			Gizmos.DrawSphere((Vector3Int)cp, testingSphereWidth);
		}
	}

	public virtual void RemoveCP(TileCollectionV2 tc, TileDataManagerV2 tdm, Vector2Int cp, bool replaceWithWall = false)
	{
		if (!Cps.Contains(cp))
		{
			Debug.LogError("Cannot Remove CP bc parent tile does not contain cp! (TileV2)");
			return;
		}

		if (tdm != null) tdm.RemoveCP(this, cp);
		Cps.Remove(cp);

		if (replaceWithWall) AddWall(tc, ConvertCPToLocalSpace(this, cp));
	}

	protected virtual Vector2Int ConvertCPToLocalSpace(TileV2 cpOwner, Vector2Int cp)
	{
		return cp - cpOwner.TilePosition;
	}

	public virtual Dictionary<Vector2Int, TileV2> GetSurroundingTiles(TileDataManagerV2 tdm)
	{
		return tdm.GetSurroundingTiles(TilePosition);
	}

	protected virtual int DirBetweenTiles(TileV2 tile, TileV2 otherTile)
	{
		Vector2Int aPos = tile.TilePosition, bPos = otherTile.TilePosition;

		if (Vector2Int.Distance(aPos, bPos) != TileSpawnerV2.TileSize)
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

	public virtual int NumWallsBetweenTiles(TileV2 thisTile, TileV2 otherTile)
	{
		int numWalls = 0;

		//Determine dir btwn tiles
		int dir = DirBetweenTiles(thisTile, otherTile);

		if (dir == -1) return -1;

		//Increment numWalls
		int tileWalls = thisTile.walls[dir] ? 1 : 0;
		int otherTileWalls = otherTile.walls[(dir + 2) % 4] ? 1 : 0;

		numWalls += tileWalls + otherTileWalls;

		return numWalls;
	}

	public virtual void RemoveWallsBetweenTiles(TileV2 thisTile, TileV2 otherTile)
	{
		//Determine dir btwn tiles
		int dir = DirBetweenTiles(thisTile, otherTile);
		int otherDir = (dir + 2) % 4;

		if (dir == -1)
			return;

		RemoveWall(thisTile, dir);
		otherTile.RemoveWall(otherTile, otherDir);
	}

	private void AddWall(TileCollectionV2 tc, Vector2Int dir)
	{
		if (dir.magnitude != TileSpawnerV2.TileSize)
		{
			Debug.LogError("Cannot Add Wall bc Dir Magnitude is " + dir.magnitude + " which != 1");
			return;
		}

		int wallIndex = 0;

		//Figure out wall index from dir
		switch (dir)
		{
			//REFACTOR: idk the math for this lol
			case Vector2Int v when v == Vector2Int.up: wallIndex = 0; break;
			case Vector2Int v when v == Vector2Int.right: wallIndex = 1; break;
			case Vector2Int v when v == Vector2Int.down: wallIndex = 2; break;
			case Vector2Int v when v == Vector2Int.left: wallIndex = 3; break;
		}

		//Alter wall gizmos
		walls[wallIndex] = true;

		//Instantiate GO walls if GO has spawned
		if (hasSpawned)
		{
			GameObject wallGO = UnityEngine.Object.Instantiate(tc.WallGO, new Vector2(dir.x * .5f, dir.y * .5f) + TilePosition, Quaternion.Euler(0, 0, wallIndex * 90), tileGO.transform);
			wallGO.transform.localScale = Vector3.one;
		}
	}

	private void RemoveWall(TileV2 thisTile, int dir)
	{
		if (!canBeUnended) return;

		//Alter wall gizmos
		thisTile.walls[dir] = false;

		//Destroy GO walls if GO has spawned
		if (thisTile.hasSpawned)
			foreach (TileWallV2 tileWall in thisTile.tileGO.GetComponentsInChildren<TileWallV2>())
				if (tileWall.wallIndex == dir)
					UnityEngine.Object.Destroy(tileWall.gameObject);
	}

	public virtual bool PosOverlaps(Vector2Int pos)
	{
		return TilePosition == pos;
	}

	public virtual void Spawn(TileCollectionV2 tc, TilePoolV2 tp)
	{
		hasSpawned = true;

		TileGOV2 goToSpawn = this.goToSpawn == null ? ((GameObject)tc.TileSpawnChances[tileType].tileGO).GetComponent<TileGOV2>() : this.goToSpawn;

		if (!tp.enabled)
		{
			tileGO = UnityEngine.Object.Instantiate(goToSpawn, (Vector3Int)TilePosition, Quaternion.Euler(0, 0, GetTileRotation()));
			tileGO.transform.localScale = new Vector3(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
		}
		else
		{
			tileGO = tp.SpawnTileFromPool(goToSpawn.tileType, TilePosition, Quaternion.Euler(0, 0, GetTileRotation()));
			tileGO.transform.localScale = new Vector2(TileSpawnerV2.TileSize, TileSpawnerV2.TileSize);
		}

		Vector2Int chunk = TileLoaderV2.GetChunkFromPos(TilePosition, TileLoaderV2.ChunkSize);
		if (TileLoaderV2.chunkGOs.ContainsKey(chunk))
			tileGO.transform.parent = TileLoaderV2.chunkGOs[chunk].transform;
	}

	public virtual void ChangeLoadState(bool loadState)
	{
		tileGO.gameObject.SetActive(loadState);
	}

	public virtual void AddTile(TileDataManagerV2 tdm)
	{
		tdm.AddTile(this);
	}

	public override string ToString()
	{
		return TilePosition.ToString() + " (" + tileType.ToString() + ")";
	}
}
