using UnityEngine;

public class OverlapBoxTest : MonoBehaviour
{
    [SerializeField] private Vector2 boxSize;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.2f);
		Gizmos.DrawCube(transform.position, boxSize);
	}

	private void Start()
    {
        Collider2D[] allOverlappingColliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
        foreach(Collider2D overlappingCollider in allOverlappingColliders)
		{
            Debug.Log(overlappingCollider.gameObject.name, overlappingCollider);
		}
    }
}
