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
	[SerializeField] bool applyFrameDelays;

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
        TileV2 initialGhostTile = new(Vector2Int.zero, tc.initialTile);
		tdm.AddTile(initialGhostTile);

		StartCoroutine(SpawnTileLoop());
	}

	private IEnumerator SpawnTileLoop()
	{
		while (true)
		{
			//Delay a frame (FOR TESTING)
			if(applyFrameDelays) yield return null;

			//Check # of Load CPs
			if (tl.loadCPs.Count <= 0)
			{
				//Check # of CPs
				if (tdm.cpDict.Count <= 0)
				{
					//Restart generation if no CPs found
					Debug.LogError("No More CPs! Terminating Spawn Tile Loop.");
					yield break;
				}
				else
				{
					//Check if player has entered a new chunk
					yield return new WaitUntil(() => playerChunkChanged);
					playerChunkChanged = false;
				}

				continue;
			}

			/*-----SPAWN TILE-----*/
			//No tiles exist in this pos - Valid!
			TileV2 newTile = new(Vector2Int.zero, tc.GetRandomTile());
			newTileGizmo = newTile;

			//Pick a Random Tile Based on CP in Chunk 
			TileV2 referenceCPTile = tl.loadCPs.Keys.ElementAt(Random.Range(0, tl.loadCPs.Count));
			referenceTileGizmo = referenceCPTile;

			//Pick a Random Connection Point in New Tile
			int connectingCPIndex = Random.Range(0, newTile.cps.Count);

			//Position ghost tile where it's connecting CP matches with tile of reference CP
			newTile.MoveTile(referenceCPTile.tilePosition - newTile.cps[connectingCPIndex]);

			if (applyFrameDelays) yield return null;

			//Check if any existing tiles are obstructing this tile
			int obstructionCheckIndex = 0;
			while (tdm.tileDict.ContainsKey(newTile.tilePosition) || !referenceCPTile.cps.Contains(newTile.tilePosition))
			{
				if (obstructionCheckIndex >= 4)
				{
					Debug.LogError("NO VALID TILE ROTATION FOUND!");
					break;
				}

				//Obstruction detected: Rotate 90 Degrees, Repeat
				newTile.Rotate(90);
				newTile.MoveTile(referenceCPTile.tilePosition - newTile.cps[connectingCPIndex]);

				obstructionCheckIndex++;

				if (applyFrameDelays) yield return null;
			}
			if (obstructionCheckIndex >= 4) continue;

			//No tiles exist in this pos - Valid!
			tdm.AddTile(newTile);
			newTileGizmo = null;
			referenceTileGizmo = null;

			//Delete reference CP of old tile + connecting CP of new tile
			tdm.RemoveCP(referenceCPTile, newTile.tilePosition);
			tdm.RemoveCP(newTile, newTile.cps[connectingCPIndex]);

			//Remove CPs in newTile overlapping with other tiles
			Dictionary<Vector2Int, TileV2> surroundingTiles = tdm.GetSurroundingTiles(newTile.tilePosition);
			for (int i = newTile.cps.Count - 1; i >= 0; i--)
			{
				if (surroundingTiles.ContainsKey(newTile.cps[i]))
				{
					//Remove Dead Ends
					if (Random.value > tc.deadEndProbability)
						if (newTile.NumWallsBetweenTiles(surroundingTiles[newTile.cps[i]]) > 0)
							newTile.RemoveWalls(surroundingTiles[newTile.cps[i]]);

					tdm.RemoveCP(newTile, newTile.cps[i]);
				}
			}

			//Remove CPs in surrounding tiles overlapping with newTile pos
			for (int i = surroundingTiles.Count - 1; i >= 0; i--)
			{
				TileV2 surroundingTile = surroundingTiles[surroundingTiles.Keys.ElementAt(i)];
				for (int j = surroundingTile.cps.Count - 1; j >= 0; j--)
				{
					if (surroundingTile.cps[j] == newTile.tilePosition)
					{
						//Remove Dead Ends
						if (Random.value > tc.deadEndProbability)
							if (surroundingTile.NumWallsBetweenTiles(newTile) > 0)
								surroundingTile.RemoveWalls(newTile);

						tdm.RemoveCP(surroundingTile, surroundingTile.cps[j]);
					}
				}
			}
		}
	}

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
}
