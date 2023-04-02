using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TileDataManagerV2))]
[RequireComponent(typeof(TileLoaderV2))]
public class TileSpawnerV2 : MonoBehaviour
{
	[Header("References")]
	TileDataManagerV2 tdm;
	TileLoaderV2 tl;
	[SerializeField] TileCollectionV2 tc;

	bool playerChunkChanged;

	[Header("Debugging")]
	[SerializeField] bool applyGhostTileFrameDelays;
	[SerializeField] bool applyRealTileFrameDelays = true;

	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();
		tl = GetComponent<TileLoaderV2>();
	}

	private void OnEnable()
	{
		tl.PlayerChunkChanged += () => { playerChunkChanged = true; };
	}

	void Start()
	{
		CreateInitialGhostTile();
	}

	void CreateInitialGhostTile()
	{
		TileV2 initialGhostTile = new(tc.initialTile);
		initialGhostTile.AddTile(tdm);

		StartCoroutine(SpawnTileLoop());
	}

	private IEnumerator SpawnTileLoop()
	{
		while (true)
		{
			//Delay a frame (FOR TESTING)
			if (applyGhostTileFrameDelays) yield return null;

			//Check # of Load CPs
			if (tl.loadCPs.Count <= 0)
			{
				/*-----SPAWN REAL TILES-----*/
				//TODO: tl.loadTiles changes when player moves too fast
				List<TileV2> loadTiles = tl.loadTiles;
				foreach (TileV2 loadTile in loadTiles)
				{
					if (!loadTile.hasSpawned)
					{
						loadTile.Spawn(tc);
						if (applyRealTileFrameDelays) yield return null;
					}
				}

				//Check if player has entered a new chunk
				yield return new WaitUntil(() => playerChunkChanged);
				playerChunkChanged = false;

				//Check # of CPs
				if (tdm.cpDict.Count <= 0)
				{
					//Restart generation if no CPs found
					Debug.LogError("No More CPs! Terminating Spawn Tile Loop.");
					yield break;
				}

				continue;
			}

			/*-----SPAWN GHOST TILE-----*/
			//No tiles exist in this pos - Valid!
			TileV2.TileType randomTileType = tc.GetRandomTileType();
			bool isGroupTile = tc.IsGroupTile(randomTileType);
			//TileV2 newTile = new(randomTileType);
			TileV2 newTile = !isGroupTile ? new TileV2(randomTileType) : new GroupTileV2(randomTileType, tc.tileSpawnChances[randomTileType].tileGO as GroupTileV2Data);
			newTileGizmo = newTile;

			//Pick a Random Tile Based on CP in Chunk 
			TileV2 referenceCPTile = tl.loadCPs.Keys.ElementAt(Random.Range(0, tl.loadCPs.Count));
			referenceTileGizmo = referenceCPTile;

			//Pick a Random Connection Point in New Tile
			int connectingCPIndex = newTile.GetConnectingCPIndex();

			//Position ghost tile where it's connecting CP matches with tile of reference CP
			if (newTile.cps.Count <= 0)
			{
				Debug.LogError(newTile.ToString() + " has no CPs!");
				yield break;
			}

			if (applyGhostTileFrameDelays) yield return null;

			newTile.MoveTileByDir(referenceCPTile.tilePosition - newTile.cps[connectingCPIndex]);

			if (applyGhostTileFrameDelays) yield return null;

			//Check if any existing tiles are obstructing this tile OR if newTile is placed in a position a referenceTile CP is not at
			int obstructionCheckIndex = 0;
			while (newTile.IsOverlappingWithPosList(tdm.tileDict.Keys.ToHashSet()) || !newTile.IsOverlappingWithPosList(referenceCPTile.cps.ToHashSet()))
			{
				if (obstructionCheckIndex >= 4)
				{
					Debug.LogError("NO VALID TILE ROTATION FOUND!");
					break;
				}

				//Obstruction detected: Rotate 90 Degrees, Repeat
				newTile.Rotate(90);

				if (applyGhostTileFrameDelays) yield return null;

				newTile.MoveTileByDir(referenceCPTile.tilePosition - newTile.cps[connectingCPIndex]);

				obstructionCheckIndex++;

				if (applyGhostTileFrameDelays) yield return null;
			}
			if (obstructionCheckIndex >= 4) continue;

			//No tiles exist in this pos - Valid!
			newTile.AddTile(tdm);
			newTileGizmo = null;
			referenceTileGizmo = null;

			//Delete reference CP of old tile + connecting CP of new tile
			referenceCPTile.RemoveCP(tdm, newTile.tilePosition);
			newTile.RemoveCP(tdm, newTile.cps[connectingCPIndex]);

			//Remove CPs in newTile overlapping with other tiles
			Dictionary<Vector2Int, TileV2> surroundingTiles = newTile.GetSurroundingTiles(tdm);
			for (int i = newTile.cps.Count - 1; i >= 0; i--)
			{
				if (surroundingTiles.ContainsKey(newTile.cps[i]))
				{
					//Remove Dead Ends
					if (Random.value > tc.deadEndProbability)
						if (newTile.NumWallsBetweenTiles(newTile, surroundingTiles[newTile.cps[i]]) > 0)
							newTile.RemoveWalls(newTile, surroundingTiles[newTile.cps[i]]);

					newTile.RemoveCP(tdm, newTile.cps[i]);
				}
			}

			//Remove CPs in surrounding tiles overlapping with newTile pos
			for (int surroundingTileIndex = surroundingTiles.Count - 1; surroundingTileIndex >= 0; surroundingTileIndex--)
			{
				TileV2 surroundingTile = surroundingTiles[surroundingTiles.Keys.ElementAt(surroundingTileIndex)];
				for (int surroundingTileCPIndex = surroundingTile.cps.Count - 1; surroundingTileCPIndex >= 0; surroundingTileCPIndex--)
				{
					if (newTile.PosOverlaps(surroundingTile.cps[surroundingTileCPIndex]))
					{
						//Remove Dead Ends
						if (Random.value > tc.deadEndProbability)
							if (surroundingTile.NumWallsBetweenTiles(surroundingTile, newTile) > 0)
								surroundingTile.RemoveWalls(surroundingTile, newTile);

						surroundingTile.RemoveCP(tdm, surroundingTile.cps[surroundingTileCPIndex]);
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	TileV2 newTileGizmo;
	Color newTileGizmoColor = new(1, 0, 1), newCPGizmoColor = new Color(1, .5f, 1);
	float newTileGizmoTestingSphereRadius = .25f;

	TileV2 referenceTileGizmo;
	Color referenceTileGizmoColor = Color.yellow;
	Vector3 referenceTileGizmoTestingCubeSize = new Vector3(.25f, .25f, .25f);
	private void OnDrawGizmos()
	{
		if (newTileGizmo != null) newTileGizmo.DrawTile(newTileGizmoColor, newCPGizmoColor, newTileGizmoTestingSphereRadius);

		if (referenceTileGizmo != null)
		{
			Gizmos.color = referenceTileGizmoColor;
			Gizmos.DrawCube((Vector3Int)referenceTileGizmo.tilePosition, referenceTileGizmoTestingCubeSize);
		}
	}
#endif
}
