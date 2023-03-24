using System;
using System.Collections;
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

	private void Awake()
	{
		tdm = GetComponent<TileDataManagerV2>();
		tl = GetComponent<TileLoaderV2>();
	}

	void Start()
    {
        CreateInitialGhostTile();
    }

	private void OnEnable()
	{
		tl.PlayerChunkChanged += () => { playerChunkChanged = true; };
	}

	void CreateInitialGhostTile()
	{
        TileV2 initialGhostTile = new(Vector3.one, tc.initialTile);
		tdm.AddTile(initialGhostTile);

		StartCoroutine(SpawnTileLoop());
	}

	private IEnumerator SpawnTileLoop()
	{
		while (true)
		{
			yield return null;

			//Check # of Load CPs
			if (tl.loadCPs.Count < 0)
			{
				//Check # of CPs
				if (tdm.cpDict.Count < 0)
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

			TileV2 newTile = new(Vector3.one, tc.GetRandomTile());

			//Pick a Random Connection Point in Chunk 
			Vector3 referenceCP = tl.chunkCPs[Random.Range(0, tl.chunkCPs.Count)];

			SpawnTile(newTile, referenceCP);
		}
	}

	private void SpawnTile(TileV2 newTile, Vector3 referenceCP)
	{
		FindValidTileRotation(newTile, referenceCP);

		//Delete reference CP of old tile + reference CP of new tile

	}

	//ONLY GROUP TILES
	private bool FindValidTileRotation(TileV2 newTile, Vector3 referenceCP)
	{
		//No tiles exist in this pos - Valid!
		if (tdm.tileDict[newTile.tilePosition] == null) return true;

		Vector3 connectingCP = newTile.cps[Random.Range(0, newTile.cps.Count)];

		//Position ghost tile where it's connecting CP matches with tile of reference CP
		newTile.tilePosition += referenceCP - connectingCP;

		//Check if any existing tiles are obstructing this tile
		int obstructionCheckIndex = 0;
		while (tdm.tileDict[newTile.tilePosition] != null)
		{
			//Obstruction detected: Rotate 90 Degrees, Repeat
			newTile.Rotate(90);
			newTile.tilePosition += referenceCP - connectingCP;

			obstructionCheckIndex++;
			if (obstructionCheckIndex >= 3) return false;
		}

		//No tiles exist in this pos - Valid!
		return true;
	}
}
