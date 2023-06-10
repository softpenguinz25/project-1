using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TileDataManagerV2))]
[RequireComponent(typeof(TileLoaderV2))]
[RequireComponent(typeof(TilePoolV2))]
public class TileSpawnerV2 : MonoBehaviour
{
	[Header("References")]
	TileDataManagerV2 tdm;
	TileLoaderV2 tl;
	[SerializeField] TileCollectionV2 tc;  public TileCollectionV2 Tc { get => tc; set => tc = value; }
	TilePoolV2 tp;

	[Header("Spawning Options")]
	[SerializeField] int tileSize;

	[Header("Spawning Instance Vars")]
	bool playerChunkChanged;
	List<TileV2> tilesToSpawn = new();
	bool mustUseRegularTile;
	TileV2 mustUseReferenceCpTile = null;

	[Header("Debugging")]
	[SerializeField] int applyGhostTileFrameDelays = 0;
	[SerializeField] int applyRealTileFrameDelays = 0;

	public event Action NoCPsRemaining;

	public static int TileSize;

	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();
		tl = GetComponent<TileLoaderV2>();
		tp = GetComponent<TilePoolV2>();

		mustUseRegularTile = false;
		mustUseReferenceCpTile = null;

		TileSize = tileSize;
	}

	private void OnEnable()
	{
		tl.PlayerChunkChanged += () => { playerChunkChanged = true; };
	}

	void Start()
	{
		if(tp.enabled) tp.AllocateTilePool();
		CreateInitialGhostTile();
	}

	void CreateInitialGhostTile()
	{
		TileV2 initialGhostTile = !tc.UseCustomInitialRotation ? new(tc.InitialTile) : new(tc.InitialTile, tc.InitialRotation);
		initialGhostTile.AddTile(tdm);

		StartCoroutine(SpawnTileLoop());
	}

	private IEnumerator SpawnTileLoop()
	{
		int loopIndex = 0;
		while (true)
		{
			//Delay a frame (FOR TESTING)
			loopIndex++;
			if (applyGhostTileFrameDelays != 0 && loopIndex % applyGhostTileFrameDelays == 0) yield return null;

			//Check # of Load CPs
			if (tl.loadCPs.Count <= 0)
			{
				/*-----SPAWN REAL TILES-----*/
					//Use an instance list in case more loadTiles are added during the spawning process
					foreach(TileV2 loadTile in tl.loadTiles) tilesToSpawn.Add(loadTile);

					//Spawn all tiles in instance list (will spawn new tiles if instance list changes)
					for (int tileToSpawnIndex = 0; tileToSpawnIndex < tilesToSpawn.Count; tileToSpawnIndex++)
					{
						TileV2 loadTile = tilesToSpawn[tileToSpawnIndex];
						if (!loadTile.HasSpawned)
						{
							TileCollectionV2 tc = loadTile.tcToUse == null ? this.tc : loadTile.tcToUse;
							loadTile.Spawn(tc, tp);
							if (applyRealTileFrameDelays != 0 && tileToSpawnIndex % applyRealTileFrameDelays == 0) yield return null;
						}
					}

					//Clear instance list
					tilesToSpawn.Clear();

				//Check # of CPs
				if (tdm.CpDict.Count <= 0)
				{
					//Restart generation if no CPs found
					Debug.LogError("No More CPs! Terminating Spawn Tile Loop.");
					NoCPsRemaining?.Invoke();
					yield break;
				}

				//Check if player has entered a new chunk
				yield return new WaitUntil(() => playerChunkChanged);
				playerChunkChanged = false;

				continue;
			}

			/*-----SPAWN GHOST TILE-----*/
			//Create newTile Object
			TileV2.TileType randomTileType = tc.GetRandomTileType(mustUseRegularTile);
			bool isGroupTile = tc.IsGroupTile(randomTileType);

			TileV2 newTile = !isGroupTile ? new TileV2(randomTileType) : new GroupTileV2(randomTileType, tc.GetRandomTileOnTileType(randomTileType) as GroupTileV2Data);
			newTileGizmo = newTile;

			//Pick a Random Tile Based on CP in Chunk 
			TileV2 referenceCPTile = mustUseReferenceCpTile ?? tl.loadCPs.Keys.ElementAt(Random.Range(0, tl.loadCPs.Count));
			referenceTileGizmo = referenceCPTile;

			//Pick a Random Connection Point in New Tile
			int connectingCPIndex = newTile.GetConnectingCPIndex();

			//Position ghost tile where it's connecting CP matches with tile of reference CP
			if (newTile.Cps.Count <= 0)
			{
				Debug.LogError(newTile.ToString() + " has no CPs!");
				yield break;
			}

			newTile.MoveTileByDir(referenceCPTile.TilePosition - newTile.Cps[connectingCPIndex]);

			//Check if any existing tiles are obstructing this tile OR if newTile is placed in a position a referenceTile CP is not at
			int obstructionCheckIndex = 0;
			while (newTile.IsOverlappingWithPosList(tdm.TileDict.Keys.ToHashSet()) || 
				!newTile.IsOverlappingWithPosList(referenceCPTile.Cps.ToHashSet()))
			{
				//If tile cannot be rotated, skip this step
				if (!newTile.CanBeRotated)
				{
					Debug.LogError("NO VALID TILE ROTATION FOUND!");
					obstructionCheckIndex += 4;
					break;
				}

				//Obstruction detected: Rotate 90 Degrees, Repeat
				newTile.Rotate(90);

				newTile.MoveTileByDir(referenceCPTile.TilePosition - newTile.Cps[connectingCPIndex]);

				if (obstructionCheckIndex >= 4)
				{
					Debug.LogError("NO VALID TILE ROTATION FOUND!");
					break;
				}

				obstructionCheckIndex++;
			}

			if (obstructionCheckIndex >= 4)
			{
				//Check if TC has reg tiles
				if (tc.HasRegTiles())
				{
					//Yes - use reg tile on that CP next spawn pass
					mustUseRegularTile = true;
					mustUseReferenceCpTile = referenceCPTile;
				}
				else
				{
					//No - Close off all reference CP tile CPs
					for (int referenceCPTileIndex = referenceCPTile.Cps.Count - 1; referenceCPTileIndex >= 0; referenceCPTileIndex--) {
						Vector2Int referenceCP = referenceCPTile.Cps[referenceCPTileIndex];
						referenceCPTile.RemoveCP(tc, tdm, referenceCP, true);
					}
				}

				newTileGizmo = null;
				referenceTileGizmo = null;

				continue;
			}

			mustUseRegularTile = false;
			mustUseReferenceCpTile = null;

			//No tiles exist in this pos - Valid!
			newTile.ValidateTile();
			newTile.AddTile(tdm);
			newTileGizmo = null;
			referenceTileGizmo = null;

			//Delete reference CP of old tile + connecting CP of new tile
			referenceCPTile.RemoveCP(tc, tdm, newTile.TilePosition);
			newTile.RemoveCP(tc, tdm, newTile.Cps[connectingCPIndex]);

			//Remove CPs in newTile overlapping with other tiles
			Dictionary<Vector2Int, TileV2> surroundingTiles = newTile.GetSurroundingTiles(tdm);
			for (int i = newTile.Cps.Count - 1; i >= 0; i--)
			{
				if (surroundingTiles.ContainsKey(newTile.Cps[i]))
				{
					//Remove Dead Ends
					if (Random.value > tc.deadEndProbability)
						if (newTile.NumWallsBetweenTiles(newTile, surroundingTiles[newTile.Cps[i]]) > 0)
							newTile.RemoveWallsBetweenTiles(newTile, surroundingTiles[newTile.Cps[i]]);

					newTile.RemoveCP(tc, tdm, newTile.Cps[i]);
				}
			}

			//Remove CPs in surrounding tiles overlapping with newTile pos
			for (int surroundingTileIndex = surroundingTiles.Count - 1; surroundingTileIndex >= 0; surroundingTileIndex--)
			{
				TileV2 surroundingTile = surroundingTiles[surroundingTiles.Keys.ElementAt(surroundingTileIndex)];
				for (int surroundingTileCPIndex = surroundingTile.Cps.Count - 1; surroundingTileCPIndex >= 0; surroundingTileCPIndex--)
				{
					if (newTile.PosOverlaps(surroundingTile.Cps[surroundingTileCPIndex]))
					{
						//Remove Dead Ends
						if (Random.value > tc.deadEndProbability)
							if (surroundingTile.NumWallsBetweenTiles(surroundingTile, newTile.GetClosestTile(surroundingTile)) > 0)
								surroundingTile.RemoveWallsBetweenTiles(surroundingTile, newTile.GetClosestTile(surroundingTile));

						surroundingTile.RemoveCP(tc, tdm, surroundingTile.Cps[surroundingTileCPIndex]);
					}
				}
			}
		}
	}

	TileV2 newTileGizmo;
	Color newTileGizmoColor = new(1, 0, 1), newCPGizmoColor = new Color(1, .5f, 1) * TileSize;
	float newTileGizmoTestingSphereRadius = .25f;

	TileV2 referenceTileGizmo;
	Color referenceTileGizmoColor = Color.yellow;
	Vector3 referenceTileGizmoTestingCubeSize = new Vector3(.25f, .25f, .25f) * TileSize;

	private void OnDrawGizmos()
	{
		if (newTileGizmo != null) newTileGizmo.DrawTile(newTileGizmoColor, newCPGizmoColor, newTileGizmoTestingSphereRadius);

		if (referenceTileGizmo != null)
		{
			Gizmos.color = referenceTileGizmoColor;
			Gizmos.DrawCube((Vector3Int)referenceTileGizmo.TilePosition, referenceTileGizmoTestingCubeSize);
		}
	}
}
