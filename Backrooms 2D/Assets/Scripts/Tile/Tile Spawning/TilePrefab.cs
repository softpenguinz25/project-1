using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class TilePrefab : MonoBehaviour
{
    public GameObject referenceTile;
    public bool isGroupTile;

    [Header("IF IS GROUP TILE = FALSE")]
    [Space]
    public List<Transform> connectionPoints;

    [Space]
    [Space]
    [Space]
    [Header("IF IS GROUP TILE = TRUE")]
    public List<TilePrefab> tileArea;
    [ConditionalField(nameof(isGroupTile))] public Transform checkForObstructingTilesPointA, checkForObstructingTilesPointB;
    [ConditionalField(nameof(isGroupTile))] public Vector2 tileSize;
    [ConditionalField(nameof(isGroupTile))] public bool canConnectUp, canConnectRight, canConnectDown, canConnectLeft;
    [ConditionalField(nameof(canConnectUp))] public Vector2 positionOffsetValueUp;
    [ConditionalField(nameof(canConnectRight))] public Vector2 positionOffsetValueRight;
    [ConditionalField(nameof(canConnectDown))] public Vector2 positionOffsetValueDown;
    [ConditionalField(nameof(canConnectLeft))] public Vector2 positionOffsetValueLeft;

    [Space]
    public bool canBeRotated = false;
    public List<Transform> specialCPs;

#if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private string AutoDetectPoints()
    {
        foreach(Transform childObj in GetComponentsInChildren<Transform>())
		{
            if (childObj.CompareTag("Connection Point")) connectionPoints.Add(childObj);
		}
        return connectionPoints.Count + " transforms found in object, cached";
    }

    [ContextMenu("Detect Overlapping Tiles")]
    public void DetectOverlappingTiles()
	{
        //Thanks Baste! https://forum.unity.com/threads/cant-get-physics2d-overlapbox-to-hit-triggers.1068140/
        var old = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = true;
        List<Collider2D> tilesDetectedInArea = Physics2D.OverlapAreaAll(checkForObstructingTilesPointA.position, checkForObstructingTilesPointB.position).ToList();
        Physics2D.queriesHitTriggers = old;

        /*List<Collider2D> invalidTiles = new List<Collider2D>();
        foreach (Collider2D collider in tilesDetectedInArea)
        {
            TilePrefab colliderTilePrefab = collider.GetComponent<TilePrefab>();
            if (colliderTilePrefab == null) { invalidTiles.Add(collider); continue; }
            else if (colliderTilePrefab.isGroupTile) { invalidTiles.Add(collider); continue; }
            else if (colliderTilePrefab.transform.IsChildOf(transform)) { invalidTiles.Add(collider); continue; }
        }

        foreach (Collider2D invalidTile in invalidTiles)
        {
            if (tilesDetectedInArea.Contains(invalidTile))
            {
                tilesDetectedInArea.Remove(invalidTile);
                //Debug.Log("Removed: " + invalidTile, invalidTile);
            }
        }*/

        foreach (Collider2D collider in tilesDetectedInArea)
		{
            Debug.Log(collider.gameObject.name, collider.gameObject);
		}
    }
    #endif
}
