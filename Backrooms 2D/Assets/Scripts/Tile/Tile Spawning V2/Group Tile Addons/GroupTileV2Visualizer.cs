using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2Visualizer : GroupTileV2Addon
{
    [Header("References")]
	[SerializeField] GroupTileV2Data groupTileData;

	[Header("Visualizer")]
	[SerializeField] float globalTileSize = 1;
	[SerializeField] Color visualizerColor = Color.red;	
	[SerializeField] Vector2 tileSize = new Vector2(.95f, .95f);

	private void OnDrawGizmos()
	{
		if (!enabled) return;

		Gizmos.color = visualizerColor;

		for (int x = 0; x < groupTileData.tileLayout.GridSize.x; x++)
		{
			for (int y = 0; y < groupTileData.tileLayout.GridSize.y; y++)
			{
				//Debug.Log(groupTileData.tileLayout.GetCell(x, y));
				if (groupTileData.tileLayout.GetCell(x, y).Length <= 0) continue;

				Vector2 center = new Vector2(transform.position.x + x * globalTileSize, transform.position.y + -1 * y * globalTileSize);
				Gizmos.DrawWireCube(center, tileSize);
			}
		}
	}

	public override void ScaleVarsWithTileSize()
	{
		globalTileSize *= TileSpawnerV2.TileSize;
		tileSize *= TileSpawnerV2.TileSize;
	}
}
