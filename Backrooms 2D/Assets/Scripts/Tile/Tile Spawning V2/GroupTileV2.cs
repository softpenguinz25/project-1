using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2 : MonoBehaviour
{
    public List<TileV2> ghostTiles = new List<TileV2>();
    [SerializeField] Color overallColor;
	private void Start()
	{
		foreach (TileV2 ghostTile in ghostTiles)
		{
			//ghostTile.tileColor = overallColor;
		}
	}
}
