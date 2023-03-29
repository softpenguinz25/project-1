using com.spacepuppy.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TileV2
{
	[Header("Position")]
	public Vector2Int tilePosition;

	[Header("Walls")]
	public bool[] walls = new bool[4];
	int numWalls => GetNumWalls();

	[Header("Walls")]
	public List<Vector2Int> cps;

	//Tile Type
	public enum TileType { Open, Split, Hall, Corner, End, Closed }
	[SerializeField] TileType tileType => GetTileType();

	public TileV2(Vector2Int pos, TileType tileType)
	{
		if (walls.Length != 4)
		{
			Debug.LogError("GhostTile Created With walls[] Length Not 4!");
			return;
		}

		tilePosition = pos;
		walls = GetWallsFromTileType(tileType);
		cps = InitialCPs();
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
	public TileType GetTileType()
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

	public bool[] GetWallsFromTileType(TileType tileType)
	{
		switch (tileType)
		{
			case TileType.Open: return new bool[] { false, false, false, false };
			case TileType.Split:
				bool[] split = new bool[] { false, false, false, false };
				split[Random.Range(0, split.Length)] = true;
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
				end[Random.Range(0, end.Length)] = false;
				return end;
			case TileType.Closed: return new bool[] { true, true, true, true };
		}

		Debug.LogError("TileType to bool[] Conversion Didn't Work!");
		return new bool[] { true, true, true, true };
	}

	public List<Vector2Int> InitialCPs()
	{
		List<Vector2Int> result = new List<Vector2Int>();

		for (int i = 0; i < walls.Length; i++)
		{
			if (!walls[i])
			{
				switch (i)
				{
					case 0: result.Add(new Vector2Int(0, 1) + tilePosition); break;
					case 1: result.Add(new Vector2Int(1, 0) + tilePosition); break;
					case 2: result.Add(new Vector2Int(0, -1) + tilePosition); break;
					case 3: result.Add(new Vector2Int(-1, 0) + tilePosition); break;
				}
			}
		}

		return result;
	}

	public void MoveTile(Vector2Int dir)
	{
		tilePosition += dir;
		for (int i = 0; i < cps.Count; i++) cps[i] += dir;
	}

	public void Rotate(int degrees)
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
		//Debug.Log("Num Rotations: " + rotations);
		for (int i = 0; i < cps.Count; i++)
		{
			Vector2Int localCPPos = ConvertCPToLocalSpace(cps[i]);

			cps[i] = Vector2Int.RoundToInt(VectorUtil.RotateBy(localCPPos, -degrees)) + tilePosition;
		}
	}

	[Header("Debugging")]
	float testingSphereWidth = .1f;
	Color tileColor = Color.blue, cpColor = Color.cyan;
	float wallDistFromTile = .45f;

	public void DrawTile(Color tileColor = new Color(), Color cpColor = new Color(), float sphereWidth = 0)
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

	public void RemoveCP(Vector2Int cpPos)
	{
		if (cps.Contains(cpPos)) cps.Remove(cpPos);
	}

	Vector2Int ConvertCPToLocalSpace(Vector2Int cp)
	{
		return cp - tilePosition;
	}

	public int DirBetweenTiles(TileV2 otherTile)
	{
		Vector2Int aPos = tilePosition, bPos = otherTile.tilePosition;

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
		int dir = DirBetweenTiles(otherTile);
		
		if(dir == -1)
		{
			return -1;
		}

		//Increment numWalls
		switch (dir)
		{
			case 0: numWalls += (walls[0] ? 1 : 0) + (otherTile.walls[2] ? 1 : 0); break;
			case 1: numWalls += (walls[1] ? 1 : 0) + (otherTile.walls[3] ? 1 : 0); break;
			case 2: numWalls += (walls[2] ? 1 : 0) + (otherTile.walls[0] ? 1 : 0); break;
			case 3: numWalls += (walls[3] ? 1 : 0) + (otherTile.walls[1] ? 1 : 0); break;
		}

		return numWalls;
	}

	public void RemoveWalls(TileV2 otherTile)
	{
		//Determine dir btwn tiles
		int dir = DirBetweenTiles(otherTile);

		if (dir == -1)
		{
			return;
		}

		//Increment numWalls
		switch (dir)
		{
			case 0: walls[0] = false; otherTile.walls[2] = false; break;
			case 1: walls[1] = false; otherTile.walls[3] = false; break;
			case 2: walls[2] = false; otherTile.walls[0] = false; break;
			case 3: walls[3] = false; otherTile.walls[1] = false; break;
		}
	}

	public string toString()
	{
		return tilePosition.ToString();
	}
}
