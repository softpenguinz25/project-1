using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	public Vector2 Movement
	{
		get
		{
			Vector3 moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			return moveVector.normalized;
		}
	}
	public float speed = 5;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}


	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + Movement * speed * Time.fixedDeltaTime);
	}
}