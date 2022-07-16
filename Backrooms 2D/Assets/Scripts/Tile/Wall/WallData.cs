using UnityEngine;

public class WallData : MonoBehaviour
{
    public bool isBreakable = true;

	private void Start()
	{
		InvokeRepeating(nameof(CheckForDeadEnd), 1, 1);
	}

	private void CheckForDeadEnd()
	{
		if (GetComponentInParent<TilePrefab>() == null) return;

		TilePrefab wallParentTile = GetComponentInParent<TilePrefab>();
		if (wallParentTile.referenceTile == null) return;

		TilePrefab wallParentReferenceTile = wallParentTile.referenceTile.GetComponent<TilePrefab>();
	
		RaycastHit2D[] linecastHitColliders = Physics2D.LinecastAll(wallParentTile.transform.position, wallParentReferenceTile.transform.position, gameObject.layer);
		if (linecastHitColliders.Length > 0)
		{
			foreach (RaycastHit2D linecastHitDeadEndWall in linecastHitColliders)
			{
				WallData deadEndWall = linecastHitDeadEndWall.collider.GetComponent<WallData>();
				if (deadEndWall != null)
					if (deadEndWall.isBreakable)
						Destroy(deadEndWall.gameObject);
			}
		}
	}
}
