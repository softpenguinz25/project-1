using UnityEngine;

public class ExitDoor : MonoBehaviour
{
	[SerializeField] private float raycastCheckRotationDistance = .5f;
	[SerializeField] private LayerMask tileMask;
	private void Start()
	{
		InvokeRepeating(nameof(CorrectRotation), 0, 5);
	}

	[ContextMenu("Correct Rotation")]
	public void CorrectRotation()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
		Debug.Log(hit.collider);
		while (hit.collider != null)
		{
			transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
			hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
		}
	}

	private void Update()
	{
		Debug.DrawRay(transform.position, transform.up * raycastCheckRotationDistance, Color.red);
	}
}
