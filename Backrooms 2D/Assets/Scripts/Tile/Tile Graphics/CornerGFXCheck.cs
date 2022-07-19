using UnityEngine;

public class CornerGFXCheck : MonoBehaviour
{
    [SerializeField] private LayerMask tileMask/* = LayerMask.GetMask("Tile")*/;

	private void OnEnable()
	{
		if (FindObjectOfType<TileDataManager>() != null) FindObjectOfType<TileSpawner>().CanSpawnTiles += CheckCorner;
		//GetComponentInParent<TilePrefab>().gameObject;
	}

	/*private void OnDestroy()
	{
		FindObjectOfType<TileDataManager>().TileAdded -= CheckCorner;
	}*/

	private void OnDisable()
	{
		if (FindObjectOfType<TileDataManager>() != null) FindObjectOfType<TileSpawner>().CanSpawnTiles -= CheckCorner;
	}

	[ContextMenu("Corner Check")]
	public void CheckCorner(bool tilesAreSpawning)
	{
		if (tilesAreSpawning) return;

		//send out 4 raycasts
		//if any raycast hit nearby tile --> keep corner
		//otherwise destroy corner

		bool wallIsUp = false, wallIsRight = false, wallIsDown = false, wallIsLeft = false;

		var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = false;

		RaycastHit2D upCast = Physics2D.Raycast(transform.position, transform.up, .2f, tileMask); if (upCast) if (upCast.collider.gameObject != gameObject && upCast.collider.GetComponent<CornerGFXCheck>() == null) wallIsUp = true;
		RaycastHit2D rightCast = Physics2D.Raycast(transform.position, transform.right, .2f, tileMask); if (rightCast) if (rightCast.collider.gameObject != gameObject && rightCast.collider.GetComponent<CornerGFXCheck>() == null) wallIsRight = true;
		RaycastHit2D downCast = Physics2D.Raycast(transform.position, -transform.up, .2f, tileMask); if (downCast) if (downCast.collider.gameObject != gameObject && downCast.collider.GetComponent<CornerGFXCheck>() == null) wallIsDown = true;
		RaycastHit2D leftCast = Physics2D.Raycast(transform.position, -transform.right, .2f, tileMask); if (leftCast) if (leftCast.collider.gameObject != gameObject && leftCast.collider.GetComponent<CornerGFXCheck>() == null) wallIsLeft = true;

		Physics2D.queriesHitTriggers = old;

		bool wallIsNearby = wallIsUp || wallIsRight || wallIsDown || wallIsLeft;

		if (!wallIsNearby) Destroy(gameObject);

		/*Debug.Log("Up: " + upCast.collider);
		Debug.Log("Right: " + rightCast.collider);
		Debug.Log("Down: " + downCast.collider);
		Debug.Log("Left: " + leftCast.collider);*/
	}
}
