using UnityEngine;

[RequireComponent(typeof(TileV2))]
public class GhostTileGizmoV2 : MonoBehaviour
{
    TileV2 gt;

	private void Awake()
	{
		gt = GetComponent<TileV2>();
	}

	private void OnDrawGizmos()
	{
		//Gizmos.color = gt.tileColor;

		for (int i = 0; i < gt.walls.Length; i++)
		{
			if (gt.walls[i])
			{
				switch (i)
				{
					case 0: Gizmos.DrawLine(gt.tilePosition + new Vector2(-.5f, .5f), gt.tilePosition + new Vector2(.5f, .5f)); break;
					case 1: Gizmos.DrawLine(gt.tilePosition + new Vector2(.5f, .5f), gt.tilePosition + new Vector2(.5f, -.5f)); break;
					case 2: Gizmos.DrawLine(gt.tilePosition + new Vector2(.5f, -.5f), gt.tilePosition + new Vector2(-.5f, -.5f)); break;
					case 3: Gizmos.DrawLine(gt.tilePosition + new Vector2(-.5f, -.5f), gt.tilePosition + new Vector2(-.5f, .5f)); break;
				}
			}
		}
	}
}
