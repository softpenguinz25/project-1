using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	private Vector2 movement
	{
		get
		{
			Vector3 moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			return moveVector.normalized;
		}
	}
	[SerializeField] private float speed = 5;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}


	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
	}
}
