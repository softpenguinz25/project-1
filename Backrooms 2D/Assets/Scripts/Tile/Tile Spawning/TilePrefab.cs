using MyBox;
using System.Collections.Generic;
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
    #endif
}