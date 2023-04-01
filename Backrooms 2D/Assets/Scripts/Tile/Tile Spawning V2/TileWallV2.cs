using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallV2 : MonoBehaviour
{
    [ShowOnly]
    [SerializeField] string documentation =
        "Wall Index communicates the <i>position</i> of the wall to other scripts. Use the wall's position relative to the tile it belongs to determine this number.\n" +
        "\n" +
        "<b>0 = Top</b>\n" +
        "<b>1 = Right</b>\n" +
        "<b>2 = Bottom</b>\n" +
        "<b>3 = Left</b>";
    [Space]
    [Range(0, 3)] public int wallIndex;
}
