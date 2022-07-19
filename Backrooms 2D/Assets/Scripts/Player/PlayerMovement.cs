using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	public Vector2 Movement
	{
		get
		{
			Vector3 moveVector = new Vector3();
			#if UNITY_STANDALONE		
			moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;		
			#elif UNITY_ANDROID || UNITY_IOS
			moveVector = new Vector2(joystick.Horizontal, joystick.Vertical);
			#endif
			return moveVector;
		}
	}
	[Header("Player Vars")]
	public float speed = 5;

	[Header("Mobile Movement")]
	[SerializeField] private Joystick joystick;

	private void Start(){
		joystick = FindObjectOfType<Joystick>();
		joystick.gameObject.SetActive(true);
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + Movement * speed * Time.fixedDeltaTime);
	}
}
