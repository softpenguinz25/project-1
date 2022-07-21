using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WallData : MonoBehaviour
{
	private TilePrefab wallParentTile;
    public bool isBreakable = true;

	[SerializeField] private bool checkForDeadEnd = true;

	private void Awake()
	{
		wallParentTile = GetComponentInParent<TilePrefab>();
	}

	private void Start()
	{
		//Debug.Log("wall data");
		InvokeRepeating(nameof(CheckForDeadEnd), 1, 1);
	}

	private void CheckForDeadEnd()
	{
		if (!checkForDeadEnd) return;

		//Debug.Log(wallParentTile);
		if (wallParentTile == null) return;

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
