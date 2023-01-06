using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	private TileDataManager tm;
	private TileLoader tl;

	[Header("Spawning")]
	public TileCollection tileCollection;
	[SerializeField] [Range(0.001f, 1)] private float timeBtwnSpawns = .001f;
	[SerializeField] private LayerMask tileMask;

	[Header("Wall Breaking")]
	[SerializeField] private LayerMask wallMask;

	[Header("Testing")]
	public List<Transform> validCPs = new List<Transform>();

	public event Action<TilePrefab> TileSpawned;

	private bool canSpawnTilesEvent = false;
	public event Action<bool> CanSpawnTiles;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
		tl = FindObjectOfType<TileLoader>();
		if (tl != null && !tl.enabled) tl = null;
	}

	private void Start()
	{
		SpawnFirstTile();
	}

	private void SpawnFirstTile()
	{
		TilePrefab firstTile = Instantiate(tileCollection.firstTile.tilePrefab, Vector3.zero, Quaternion.identity);
		if (!firstTile.isGroupTile && !firstTile.isBigTile)
		{
			tm.AddTile(firstTile);
			foreach (Transform connectionPoint in firstTile.connectionPoints) tm.AddConnectionPoint(connectionPoint);
		}
		else
		{
			//Regular CPs
			foreach(TilePrefab tile in firstTile.tileArea)
			{
				tm.AddTile(tile);
				foreach (Transform connectionPoint in tile.connectionPoints) tm.AddConnectionPoint(connectionPoint);				
			}

			//Special CPs
			foreach (Transform specialCP in firstTile.specialCPs) tm.AddConnectionPoint(specialCP);
		}

		StartCoroutine(SpawnTilesCoroutine());
	}

	private bool doWaitTime = true;
	private IEnumerator SpawnTilesCoroutine()
	{
		while (true)
		{
			if (doWaitTime) yield return new WaitForSeconds(timeBtwnSpawns);
			else doWaitTime = true;

			Transform randomReferenceCP = null;
			#region 1. Pick Connection Point
			if (tl != null)
			{
				List<Transform> validConnectionPoints = new List<Transform>();
				while (validConnectionPoints.Count <= 0)
				{
					//Debug.Log("(Spawn Tile) Valid Connection Points Count: " + validConnectionPoints.Count);
					//Debug.Log("(Spawn Tile) TileManager Connection Points Count: " + tm.connectionPoints.Count);
					foreach (Transform connectionPoint in tm.connectionPoints)
					{
						if (connectionPoint == null) continue;
						Vector2 roundedConnectionPointPos = new Vector2(Mathf.Round(connectionPoint.transform.position.x / tl.chunkSize), Mathf.Round(connectionPoint.transform.position.y / tl.chunkSize)) * tl.chunkSize;
						//Debug.Log("(Spawn Tile) Rounded Connection Pos: " + roundedConnectionPointPos);
						//Debug.Log("(Spawn Tile) Current Player Chunk: " + tl.CurrentPlayerChunk());
						if (Vector2.Distance(roundedConnectionPointPos, tl.CurrentPlayerChunk().transform.position) < /*Mathf.Sqrt(tl.chunkSize * tl.chunkSize + tl.chunkSize * tl.chunkSize) + .01f*/tl.chunkSizeHypotenuse)//Pythagorean theorem!
						{
							validConnectionPoints.Add(connectionPoint);
							//Debug.Log("(Spawn Tile) Added Valid Connection Point: " + connectionPoint);
						}
						else
						{
							tl.AddChunk(roundedConnectionPointPos);
							//Debug.Log("(Spawn Tile) Chunk Added at Position: " + roundedConnectionPointPos);
						}
					}

					if (validConnectionPoints.Count <= 0 && canSpawnTilesEvent)
					{
						CanSpawnTiles?.Invoke(false);

						canSpawnTilesEvent = false;
					}
					else if (validConnectionPoints.Count > 0) break;

					yield return null;
				}
				//yield return new WaitUntil(() => validConnectionPoints.Count > 0);
				validCPs = validConnectionPoints;
				randomReferenceCP = validConnectionPoints[UnityEngine.Random.Range(0, validConnectionPoints.Count)];
				//Debug.Log("Random Connection Point Found: " + randomConnectionPoint);
			}
			else
			{
				if (tm.connectionPoints.Count <= 0) FindObjectOfType<TileRestarter>().RestartTileGeneration();
				else randomReferenceCP = tm.connectionPoints[UnityEngine.Random.Range(0, tm.connectionPoints.Count)];
			}
			#endregion

			#region 2. Spawn Tile
			TileTemplate randomTileTemplate;
			CPData cpData = randomReferenceCP.GetComponent<CPData>();

			if (cpData == null || !cpData.enabled || !cpData.gameObject.activeSelf) randomTileTemplate = RandomSpawnChanceTile(tileCollection);
			else randomTileTemplate = RandomSpawnChanceTile(cpData.tileCollection);
			TilePrefab newlySpawnedtilePrefab;
			if (!randomTileTemplate.tilePrefab.isGroupTile)
			{
				//Debug.Log("Overlapping tiles detected");
				newlySpawnedtilePrefab = Instantiate(randomTileTemplate.tilePrefab, randomReferenceCP.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90));
			}
			else
			{
				Vector2 checkPos = randomReferenceCP.position;

				//Correct Position
				float epsilon = .01f;
				Vector2 diffBetweenConnectionAndTile = randomReferenceCP.position - randomReferenceCP.parent.position;

				bool connectionPointIsUp = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y > epsilon;
				bool connectionPointIsRight = diffBetweenConnectionAndTile.x > epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;
				bool connectionPointIsDown = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y < -epsilon;
				bool connectionPointIsLeft = diffBetweenConnectionAndTile.x < -epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;

				if (connectionPointIsUp && randomTileTemplate.tilePrefab.canConnectDown) checkPos += randomTileTemplate.tilePrefab.positionOffsetValueDown;
				else if (connectionPointIsRight && randomTileTemplate.tilePrefab.canConnectLeft) checkPos += randomTileTemplate.tilePrefab.positionOffsetValueLeft;
				else if (connectionPointIsDown && randomTileTemplate.tilePrefab.canConnectUp) checkPos += randomTileTemplate.tilePrefab.positionOffsetValueUp;
				else if (connectionPointIsLeft && randomTileTemplate.tilePrefab.canConnectRight) checkPos += randomTileTemplate.tilePrefab.positionOffsetValueRight;
				else if (!randomTileTemplate.tilePrefab.canBeRotated)
				{
					//Debug.Log("Cannot execute position offset");
					continue;
				}

				/*foreach(Collider2D collider in Physics2D.OverlapBoxAll(checkPos, randomTileTemplate.tilePrefab.tileSize, 0, tileMask))
				{
					Debug.Log(randomTileTemplate.tilePrefab + " at pos: " + checkPos, randomConnectionPoint);
					Debug.Log(collider.gameObject, collider.gameObject);
				}*/

				if (Physics2D.OverlapBoxAll(checkPos, randomTileTemplate.tilePrefab.tileSize, 0, tileMask).Length <= 0)
				{
					//Debug.Log("Smart Instiating " + randomTileTemplate.tilePrefab.gameObject.name);
					newlySpawnedtilePrefab = Instantiate(randomTileTemplate.tilePrefab, randomReferenceCP.position/*checkPos*/, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90));
				}
				else
				{
					continue;
				}
			}
			//Debug.Log("Tile Instantiated: " + tilePrefab.name, tilePrefab);
			newlySpawnedtilePrefab.referenceTile = randomReferenceCP.gameObject.GetComponentInParent<TilePrefab>(true).gameObject;/*randomConnectionPoint.parent.gameObject;
			if(tilePrefab.referenceTile.GetComponent<TilePrefab>() == null) tilePrefab.referenceTile = randomConnectionPoint.root.gameObject;*/
			#endregion

			#region 2.5. Apply Special Tile Settings (If Applicable)
			if(newlySpawnedtilePrefab.isBigTile && !newlySpawnedtilePrefab.isGroupTile)
			{
				if (newlySpawnedtilePrefab.GetComponent<TileGraphics>() != null) newlySpawnedtilePrefab.GetComponent<TileGraphics>().ToggleGraphics(false);

				yield return null;
				Transform newlySpawnedTileCP = newlySpawnedtilePrefab.connectionPoints[UnityEngine.Random.Range(0, newlySpawnedtilePrefab.connectionPoints.Count)];				
				//correct rotation before moving
				//this while condition took a lot of testing. the angle diff needs to be -90 for this to work, idk why lol
				while (Mathf.Abs(HelperMethods.GetPositiveAngle(Mathf.DeltaAngle(randomReferenceCP.eulerAngles.z, newlySpawnedTileCP.eulerAngles.z)) - 180) > 1)
				{
					newlySpawnedtilePrefab.transform.eulerAngles = new Vector3(0, 0, newlySpawnedtilePrefab.transform.eulerAngles.z + 90);
				}

				Vector3 diffToMove = randomReferenceCP.parent.parent.position - newlySpawnedTileCP.position;
				newlySpawnedtilePrefab.transform.position += diffToMove;

				yield return null;

				#region Obstruction
				#region Get All Overlapping GOs
				//Thanks Baste! https://forum.unity.com/threads/cant-get-physics2d-overlapbox-to-hit-triggers.1068140/
				var old = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;
				ContactFilter2D contactFilter = new ContactFilter2D();
				List<Collider2D> tilesDetectedInArea = new List<Collider2D>();
				Physics2D.OverlapCollider(newlySpawnedtilePrefab.GetComponent<CompositeCollider2D>(), contactFilter, tilesDetectedInArea);
				Physics2D.queriesHitTriggers = old;

				List<Collider2D> invalidTiles = new List<Collider2D>();
				foreach (Collider2D collider in tilesDetectedInArea)
				{
					TilePrefab colliderTilePrefab = collider.GetComponent<TilePrefab>();
					WallData colliderWallData = collider.GetComponent<WallData>();

					if (colliderWallData != null)
					{
						if (colliderWallData.transform.IsChildOf(newlySpawnedtilePrefab.transform)) { invalidTiles.Add(collider); continue; }
						else continue;
					}

					if (colliderTilePrefab == null || colliderTilePrefab.isGroupTile || colliderTilePrefab.transform.IsChildOf(newlySpawnedtilePrefab.transform)) 
					{ 
						invalidTiles.Add(collider); 
						continue; 
					}
				}

				foreach (Collider2D invalidTile in invalidTiles)
				{
					if (tilesDetectedInArea.Contains(invalidTile))
					{
						tilesDetectedInArea.Remove(invalidTile);
					}
				}
				#endregion

				#region Obstruction Test
				bool obstructingTileDetected = tilesDetectedInArea.Count > 1 ? true : false;
				if (obstructingTileDetected)
				{
					foreach (Transform cp in newlySpawnedtilePrefab.connectionPoints) tm.DestroyConnectionPoint(cp);
					tm.DestroyConnectionPoint(randomReferenceCP);
					Destroy(newlySpawnedtilePrefab.gameObject);
					continue;
				}
				#endregion
				#endregion

				//Apply Data
				//TODO: Make this if statement work
				/*if (newlySpawnedtilePrefab.GetComponentInChildren<IBigTile>() != null)*/ randomReferenceCP.parent.GetComponent<IBigTile>().BigTilePlaced(randomReferenceCP, newlySpawnedTileCP, false);//REFERENCE TILE
				/*if (randomConnectionPoint.transform.parent.GetComponent<IBigTile>() != null)*/ newlySpawnedTileCP.parent.GetComponent<IBigTile>().BigTilePlaced(randomReferenceCP, newlySpawnedTileCP, true);//NEWLY SPAWNED TILE

				AddTile(newlySpawnedtilePrefab);

				int currentAreaTile = 0;				
				foreach (TilePrefab areaTile in newlySpawnedtilePrefab.tileArea)
				{
					AddTile(areaTile);
					currentAreaTile++;
					if (currentAreaTile % 3 == 0)
						yield return null;//Stop lagspikes
				}

				if(newlySpawnedtilePrefab.GetComponent<TileGraphics>() != null) newlySpawnedtilePrefab.GetComponent<TileGraphics>().ToggleGraphics(true);

				continue;
			}
			else if (!newlySpawnedtilePrefab.isBigTile && newlySpawnedtilePrefab.isGroupTile)
			{
				//Debug.Log("Executing Group Tile Logic...");

				//Temporary Rotation Correction
				if (!newlySpawnedtilePrefab.canBeRotated) newlySpawnedtilePrefab.transform.eulerAngles = Vector3.zero;

				//Correct Position
				float epsilon = .01f;
				Vector2 diffBetweenConnectionAndTile = randomReferenceCP.position - randomReferenceCP.parent.position;

				bool connectionPointIsUp = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y > epsilon;
				bool connectionPointIsRight = diffBetweenConnectionAndTile.x > epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;
				bool connectionPointIsDown = Mathf.Abs(diffBetweenConnectionAndTile.x) < epsilon && diffBetweenConnectionAndTile.y < -epsilon;
				bool connectionPointIsLeft = diffBetweenConnectionAndTile.x < -epsilon && Mathf.Abs(diffBetweenConnectionAndTile.y) < epsilon;

				//Debug.Log("CP Up: " + connectionPointIsUp + "Can Connect Up: " + tilePrefab.canConnectUp);
				//Debug.Log("CP Right: " + connectionPointIsRight + "Can Connect Right: " + tilePrefab.canConnectRight);
				//Debug.Log("CP Down: " + connectionPointIsDown + "Can Connect Down: " + tilePrefab.canConnectDown);
				//Debug.Log("CP Left: " + connectionPointIsLeft + "Can Connect Left: " + tilePrefab.canConnectLeft);

				if (!((connectionPointIsUp && newlySpawnedtilePrefab.canConnectDown) || (connectionPointIsRight && newlySpawnedtilePrefab.canConnectLeft) || (connectionPointIsDown && newlySpawnedtilePrefab.canConnectUp) || (connectionPointIsLeft && newlySpawnedtilePrefab.canConnectRight)) && newlySpawnedtilePrefab.specialCPs.Count <= 0)
				{
					Debug.Log("Destroyed " + newlySpawnedtilePrefab.name);
					Destroy(newlySpawnedtilePrefab.gameObject);
					doWaitTime = false;
					continue;
				}
				else if (newlySpawnedtilePrefab.referenceTile.GetComponent<TilePrefab>().isGroupTile) {/*do noting lol*/ }
				else if (connectionPointIsUp && newlySpawnedtilePrefab.canConnectDown) { Vector2 newPos = (Vector2)newlySpawnedtilePrefab.transform.position + newlySpawnedtilePrefab.positionOffsetValueDown; newlySpawnedtilePrefab.transform.position = newPos; }
				else if (connectionPointIsRight && newlySpawnedtilePrefab.canConnectLeft) { Vector2 newPos = (Vector2)newlySpawnedtilePrefab.transform.position + newlySpawnedtilePrefab.positionOffsetValueLeft; newlySpawnedtilePrefab.transform.position = newPos; }
				else if (connectionPointIsDown && newlySpawnedtilePrefab.canConnectUp) { Vector2 newPos = (Vector2)newlySpawnedtilePrefab.transform.position + newlySpawnedtilePrefab.positionOffsetValueUp; newlySpawnedtilePrefab.transform.position = newPos; }
				else if (connectionPointIsLeft && newlySpawnedtilePrefab.canConnectRight) { Vector2 newPos = (Vector2)newlySpawnedtilePrefab.transform.position + newlySpawnedtilePrefab.positionOffsetValueRight; newlySpawnedtilePrefab.transform.position = newPos; }

				//Check if tiles are obstructing

				//Debug.Log("Doing obstruction test...");

				List<TilePrefab> areaTiles = newlySpawnedtilePrefab.GetComponentsInChildren<TilePrefab>().ToList();
				areaTiles.Remove(newlySpawnedtilePrefab);

				#region Old (Laggy) (CHECKS LIKE 50000 TILES LOL)
				/*bool groupTileInIllegalPosition = false;
				int tileIndex = 0;
				foreach (TilePrefab areaTile in areaTiles)
				{
					foreach (TilePrefab existingTile in tm.tiles)
					{
						tileIndex++;
						Debug.Log("tileIndex: " + tileIndex);
						if (Vector2.Distance(areaTile.transform.position, existingTile.transform.position) < epsilon)
						{
							groupTileInIllegalPosition = true;
							break;
						}

						if (tileIndex % 100 == 0)
							yield return null;//Stop crashing from lag
					}
					if (groupTileInIllegalPosition) break;
					//yield return null;//Stop crashing from lag
				}

				if (groupTileInIllegalPosition)
				{
					Destroy(tilePrefab.gameObject);
					continue;
				}*/
				#endregion

				#region Get All GOs in Physics2D.OverlapAreaAll
				//Thanks Baste! https://forum.unity.com/threads/cant-get-physics2d-overlapbox-to-hit-triggers.1068140/
				var old = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;
				List<Collider2D> tilesDetectedInArea = Physics2D.OverlapAreaAll(newlySpawnedtilePrefab.checkForObstructingTilesPointA.position, newlySpawnedtilePrefab.checkForObstructingTilesPointB.position/*, tileMask*/).ToList();
				Physics2D.queriesHitTriggers = old;

				List<Collider2D> invalidTiles = new List<Collider2D>();
				foreach (Collider2D collider in tilesDetectedInArea)
				{
					TilePrefab colliderTilePrefab = collider.GetComponent<TilePrefab>();
					WallData colliderWallData = collider.GetComponent<WallData>();

					if (colliderWallData != null)
					{
						if (colliderWallData.transform.IsChildOf(newlySpawnedtilePrefab.transform)) { invalidTiles.Add(collider); continue; }
						else continue;
					}
					/*else*/
					if (colliderTilePrefab == null/* && colliderWallData == null*/) { invalidTiles.Add(collider); continue; }
					else if (colliderTilePrefab.isGroupTile) { invalidTiles.Add(collider); continue; }
					else if (colliderTilePrefab.transform.IsChildOf(newlySpawnedtilePrefab.transform)) { invalidTiles.Add(collider); continue; }
				}

				foreach (Collider2D invalidTile in invalidTiles)
				{
					if (tilesDetectedInArea.Contains(invalidTile))
					{
						tilesDetectedInArea.Remove(invalidTile);
						//Debug.Log("Removed: " + invalidTile, invalidTile);
					}
				}
				#endregion

				#region Obstruction Test
				bool obstructingTileDetected = tilesDetectedInArea.Count > 1 ? true : false;
				if (obstructingTileDetected)
				{
					//Debug.Log("Deleting " + tilePrefab.name + " at pos " + tilePrefab.transform.position + " (" + tilesDetectedInArea.Count + " tiles found)", tilePrefab);
					/*foreach(Collider2D tileDetectedInArea in tilesDetectedInArea)
					{
						Debug.Log("Deleting " + tilePrefab.name + " at pos " + tilePrefab.transform.position + "(" + tileDetectedInArea.gameObject.name + ")", tileDetectedInArea.gameObject);
					}*/
					Destroy(newlySpawnedtilePrefab.gameObject);
					//doWaitTime = false;
					continue;
				}
				#endregion

				//Passed the obstruction test

				//Correct rotation
				//Debug.Log(tilePrefab.canBeRotated);
				if (newlySpawnedtilePrefab.canBeRotated)
				{
					if (newlySpawnedtilePrefab.referenceTile.GetComponent<TilePrefab>().isGroupTile)
					{
						Transform spawnedGroupTileReference = randomReferenceCP.GetComponentInParent<TilePrefab>(true).transform;//randomConnectionPoint.root;
																																	 //Debug.Log("Tile Ref: " + spawnedGroupTileReference, spawnedGroupTileReference);
						bool groupTileHasValidRotation = false;
						while (!groupTileHasValidRotation)
						{
							foreach (Transform connectionPoint in newlySpawnedtilePrefab.specialCPs)
							{
								if (Vector3.Distance(connectionPoint.position, spawnedGroupTileReference.position) < .01f)
								{
									groupTileHasValidRotation = true;
									//Debug.Log("Connection Point Pos: " + connectionPoint.position, connectionPoint);
									//Debug.Log("Reference Tile Pos: " + spawnedGroupTileReference.position, spawnedGroupTileReference);
									break;
								}
							}

							if (groupTileHasValidRotation) break;

							newlySpawnedtilePrefab.transform.eulerAngles = new Vector3(0, 0, newlySpawnedtilePrefab.transform.eulerAngles.z + 90);

							yield return null;
						}
					}
					else
					{
						bool groupTileHasValidRotation = false;

						while (!groupTileHasValidRotation)
						{
							List<Transform> regularCPs = new List<Transform>();
							foreach (TilePrefab areaTile in newlySpawnedtilePrefab.tileArea)
							{
								foreach (Transform regularCP in areaTile.connectionPoints)
								{
									regularCPs.Add(regularCP);
								}
							}

							foreach (Transform regularCP in regularCPs)
							{
								if (Vector2.Distance(regularCP.position, newlySpawnedtilePrefab.referenceTile.transform.position) < .01f)
								{
									groupTileHasValidRotation = true;
									break;
								}
							}

							if (groupTileHasValidRotation) break;

							newlySpawnedtilePrefab.transform.eulerAngles = new Vector3(0, 0, newlySpawnedtilePrefab.transform.eulerAngles.z + 90);

							yield return null;
						}
					}
				}

				//Apply data
				foreach (Transform scp in newlySpawnedtilePrefab.specialCPs)
				{
					tm.AddConnectionPoint(scp);
					//Debug.Log("Adding " + scp.name);
				}

				AddTile(newlySpawnedtilePrefab);

				int currentAreaTile = 0;
				foreach (TilePrefab areaTile in areaTiles)
				{
					//Debug.Log("adding tile");
					AddTile(areaTile);
					currentAreaTile++;
					if (currentAreaTile % 3 == 0)
						yield return null;//Stop lagspikes
				}

				continue;
			}
			#endregion

			#region 3. Rotate Tile
			Transform spawnedTileReference = randomReferenceCP.parent;
			bool tileHasValidRotation = false;
			Transform connectedPoint;
			while (!tileHasValidRotation)
			{
				foreach (Transform connectionPoint in newlySpawnedtilePrefab.connectionPoints)
				{
					if (Vector3.Distance(connectionPoint.position, spawnedTileReference.position) < .01f)
					{
						connectedPoint = connectionPoint;
						tileHasValidRotation = true;
					}
				}

				if (tileHasValidRotation) break;

				newlySpawnedtilePrefab.transform.eulerAngles = new Vector3(0, 0, newlySpawnedtilePrefab.transform.eulerAngles.z + 90);
				yield return null;
			}
			#endregion

			#region 4. Destroy walls that lead to dead ends
			List<Transform> otherCPs = newlySpawnedtilePrefab.connectionPoints.Count > 0 ? newlySpawnedtilePrefab.connectionPoints : newlySpawnedtilePrefab.specialCPs;
			Transform referenceCP = null;
			foreach (Transform cp in otherCPs)
			{
				if (Vector2.Distance(cp.position, newlySpawnedtilePrefab.referenceTile.transform.position) < .1f)
				{
					referenceCP = cp;
					break;
				}
			}
			if (referenceCP == null) Debug.LogError("Could not find reference CP!");
			otherCPs.Remove(referenceCP);//remove reference tile cp bc it impossile to destroy ref tile since it's already correct

			foreach (Transform otherCP in otherCPs)
			{
				RaycastHit2D[] linecastHitColliders = Physics2D.LinecastAll(newlySpawnedtilePrefab.transform.position, otherCP.transform.position, wallMask);
				if (linecastHitColliders.Count() > 0)
				{
					foreach (RaycastHit2D linecastHitDeadEndWall in linecastHitColliders)
					{
						WallData deadEndWall = linecastHitDeadEndWall.collider.GetComponent<WallData>();
						if (deadEndWall != null)
							if (deadEndWall.isBreakable)
							{
								//Debug.Log("Wall Destroyed!");
								Destroy(deadEndWall.gameObject);
							}
					}
				}
			}
			#endregion

			#region 5. Update Manager Lists + Check Connection Points
			AddTile(newlySpawnedtilePrefab);
			#endregion
		}
	}

	private void AddTile(TilePrefab tilePrefab)
	{
		tm.AddTile(tilePrefab);
		foreach (Transform connectionPoint in tilePrefab.connectionPoints) tm.AddConnectionPoint(connectionPoint);
		tm.CheckConnectionPoints();

		TileSpawned?.Invoke(tilePrefab);

		if (!canSpawnTilesEvent)
		{
			CanSpawnTiles?.Invoke(true);
			canSpawnTilesEvent = true;
		}
	}

	//THANKS Dev Leonardo! https://www.youtube.com/watch?v=Gj7UU5IU3-E
	private TileTemplate RandomSpawnChanceTile(TileCollection tileCollection)
	{
		float totalDenomination = 0;

		foreach (TileTemplate tile in tileCollection.tiles)
		{
			totalDenomination += tile.spawnChance;
		}

		float randomNumber = UnityEngine.Random.Range(0, totalDenomination);	
		
		foreach(TileTemplate tile in tileCollection.tiles)
		{
			if(tile.spawnChance >= randomNumber)
			{
				return tile;
			}

			randomNumber -= tile.spawnChance;
		}

		Debug.LogError("Random Tile Generation Failed!");
		return null;
	}
}
