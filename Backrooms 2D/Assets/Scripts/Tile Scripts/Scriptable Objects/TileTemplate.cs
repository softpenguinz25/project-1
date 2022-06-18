using UnityEngine;
[CreateAssetMenu(fileName = "New Tile Template", menuName = "Tile/Tile Template", order = 1)]
public class TileTemplate : ScriptableObject
{
    public TilePrefab tilePrefab;
    [Tooltip("1 = low chance, 5 = high chance")][Range(1, 5)] public float spawnChance;
}