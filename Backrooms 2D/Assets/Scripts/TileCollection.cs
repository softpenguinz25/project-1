using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tile Collection", menuName = "Tile/Tile Collection", order = 1)]
public class TileCollection : ScriptableObject
{
    public List<Tile> tiles;
    public Tile firstTile;
}
