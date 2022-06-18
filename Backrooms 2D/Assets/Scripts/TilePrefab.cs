using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    public List<Transform> connectionPoints;

    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private string AutoDetectPoints()
    {
        connectionPoints = GetComponentsInChildren<Transform>().ToList();
        connectionPoints.Remove(transform);
        return connectionPoints.Count + " transforms found in object, cached";
    }
    #endif
}
