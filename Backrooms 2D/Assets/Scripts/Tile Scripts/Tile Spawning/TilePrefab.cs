using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    public List<Transform> connectionPoints;

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
