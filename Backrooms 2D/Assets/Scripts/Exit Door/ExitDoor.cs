using UnityEngine;

public class ExitDoor : MonoBehaviour
{
	[Header("Door Spawn")]
	[SerializeField] private float raycastCheckRotationDistance = .5f;
	[SerializeField] private LayerMask tileMask;
	private bool isInDoor;	

	[Header("Player In Door")]
	[SerializeField] private GameObject fourthWall;

	private const float angleOffset = 90;
	private void Start()
	{
		fourthWall.SetActive(false);
		InvokeRepeating(nameof(CorrectRotation), 0, 1f);
	}

	[ContextMenu("Correct Rotation")]
	public void CorrectRotation()
	{
		if (isInDoor) return;

		//Rotate towards player
		Vector3 targetPos = FindObjectOfType<PlayerMovement>().transform.position;
		Vector2 direction = targetPos - transform.position;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(Vector3.forward * (angle + angleOffset));
		transform.eulerAngles = new Vector3(Mathf.Round(transform.eulerAngles.x / 90) * 90, Mathf.Round(transform.eulerAngles.y / 90) * 90, Mathf.Round(transform.eulerAngles.z / 90) * 90);

		//Rotate if is hitting a wall
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, raycastCheckRotationDistance, tileMask);
		while (hit.collider != null && !hit.collider.isTrigger)
		{
			transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
			hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			isInDoor = true;
			ExecuteEndingSequence();
		}
	}

	private void ExecuteEndingSequence()
	{
		fourthWall.SetActive(true);
		FindObjectOfType<AudioManager>().Play("Elevator_Transition");
		StartCoroutine(FindObjectOfType<LevelTransition>().PlayTransitionCoroutine());
	}

	private void Update()
	{
		Debug.DrawRay(transform.position, -transform.up * raycastCheckRotationDistance, Color.red);
	}
}
