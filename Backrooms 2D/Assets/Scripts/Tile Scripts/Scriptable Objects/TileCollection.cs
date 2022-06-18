using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tile Collection", menuName = "Tile/Tile Collection", order = 1)]
public class TileCollection : ScriptableObject
{
    public List<TileTemplate> tiles;
    public TileTemplate firstTile;
}
