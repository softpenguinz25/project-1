using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//[RequireComponent(typeof(GhostTileGizmo))]
//[ExtensionOfNativeClass]
[Serializable]
public class TileV2
{
	[Header("Position")]
	public Vector3 tilePosition;

	[Header("Walls")]
	public bool[] walls = new bool[4];
	int numWalls => GetNumWalls();

	[Header("Walls")]
	public List<Vector3> cps;

	//Tile Type
	public enum TileType { Open, Split, Hall, Corner, End, Closed }
	[SerializeField] TileType tileType => GetTileType();

	public TileV2(Vector3 pos, TileType tileType)
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

	public List<Vector3> InitialCPs()
	{
		List<Vector3> result = new List<Vector3>();

		for (int i = 0; i < walls.Length; i++)
		{
			if (!walls[i])
			{
				switch (i)
				{
					case 0: result.Add(new Vector3(0, 1)); break;
					case 1: result.Add(new Vector3(1, 0)); break;
					case 2: result.Add(new Vector3(0, -1)); break;
					case 3: result.Add(new Vector3(-1, 0)); break;
				}
			}
		}

		return result;
	}

	public void Rotate(int degrees)
	{
		if (degrees % 90 != 0)
		{
			Debug.LogError("Cannot rotate tile by a non-multiple of 90.");
			return;
		}

		int rotations = degrees / 90;

		bool[] rotatedWalls = new bool[walls.Length];

		for (int i = 0; i < walls.Length; i++)
		{
			int newIndex = (i + rotations) % walls.Length;
			rotatedWalls[newIndex] = walls[i];
		}

		walls = rotatedWalls;
	}

	[Header("Debugging")]
	float testingSphereWidth = .1f;
	Color tileColor = Color.blue, cpColor = Color.cyan;

	//[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
	public void DrawTile()
	{
		Gizmos.color = tileColor;

		Gizmos.DrawSphere(tilePosition, testingSphereWidth);

		for (int i = 0; i < walls.Length; i++)
		{
			switch (i)
			{
				case 0: if (walls[i]) { Gizmos.color = tileColor; Gizmos.DrawLine(tilePosition + new Vector3(-.5f, .5f), tilePosition + new Vector3(.5f, .5f)); } else { Gizmos.color = cpColor; Gizmos.DrawSphere(tilePosition + new Vector3(0, 1), testingSphereWidth); } break;
				case 1: if (walls[i]) { Gizmos.color = tileColor; Gizmos.DrawLine(tilePosition + new Vector3(.5f, .5f), tilePosition + new Vector3(.5f, -.5f)); } else { Gizmos.color = cpColor; Gizmos.DrawSphere(tilePosition + new Vector3(1, 0), testingSphereWidth); } break;
				case 2: if (walls[i]) { Gizmos.color = tileColor; Gizmos.DrawLine(tilePosition + new Vector3(.5f, -.5f), tilePosition + new Vector3(-.5f, -.5f)); } else { Gizmos.color = cpColor; Gizmos.DrawSphere(tilePosition + new Vector3(0, -1), testingSphereWidth); } break;
				case 3: if (walls[i]) { Gizmos.color = tileColor; Gizmos.DrawLine(tilePosition + new Vector3(-.5f, -.5f), tilePosition + new Vector3(-.5f, .5f)); } else { Gizmos.color = cpColor; Gizmos.DrawSphere(tilePosition + new Vector3(-1, 0), testingSphereWidth); } break;
			}
		}
	}

	public void RemoveCP(Vector3 cpPos)
	{
		if (cps.Contains(cpPos)) cps.Remove(cpPos);
	}
}
