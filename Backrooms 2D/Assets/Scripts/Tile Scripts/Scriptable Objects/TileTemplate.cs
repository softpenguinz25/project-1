using MyBox;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tile Template", menuName = "Tile/Tile Template", order = 1)]
public class TileTemplate : ScriptableObject
{
    public TilePrefab tilePrefab;
    [Tooltip("0 = low chance, 5 = high chance")][Range(0, 5)] public float spawnChance;

    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private string AutoDetectTilePrefab()
    {
        string filePath = AssetDatabase.GetAssetPath(this);
        string[] folders = filePath.Split("/");
        filePath = filePath.Replace(folders[folders.Length - 1], "");
        filePath = filePath.Replace(folders[folders.Length - 2], "");
        filePath += "Tile Prefabs/" + name + ".prefab";

        TilePrefab targetTilePrefab = AssetDatabase.LoadAssetAtPath<TilePrefab>(filePath);

        if (targetTilePrefab == null) return "Count not find " + name + " at " + filePath;
        tilePrefab = targetTilePrefab;
        return tilePrefab.ToString() + " retrieved from " + AssetDatabase.GetAssetPath(tilePrefab);
    }
    #endif
}