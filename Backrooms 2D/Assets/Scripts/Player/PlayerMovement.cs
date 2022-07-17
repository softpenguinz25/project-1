using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	public Vector2 Movement
	{
		get
		{
			Vector3 moveVector = new Vector3();
			#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
			moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			#endif
			#if UNITY_ANDROID || UNITY_EDITIOR
			moveVector = new Vector2(joystick.Horizontal, joystick.Vertical);
			#endif
			return moveVector.normalized;
		}
	}
	[Header("Player Vars")]
	public float speed = 5;

	#if UNITY_ANDROID || UNITY_EDITIOR
	[Header("Mobile Movement")]
	[SerializeField] private Joystick joystick;

	private void Start(){
		joystick = FindObjectOfType<Joystick>();
		joystick.gameObject.SetActive(true);
	}
	#endif

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + Movement * speed * Time.fixedDeltaTime);
	}
}
