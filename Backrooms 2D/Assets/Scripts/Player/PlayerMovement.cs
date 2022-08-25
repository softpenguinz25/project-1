using System.Collections;
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
#elif UNITY_WEBGL
			if(Mathf.Abs(joystick.Horizontal) < .01f && Mathf.Abs(joystick.Vertical) < .01f) moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
			else moveVector = new Vector2(joystick.Horizontal, joystick.Vertical);			
#endif
			return moveVector;
		}
	}
	[Header("Player Vars")]
	[SerializeField] float startSpeed = 5;
	public float StartSpeed
	{
		get
		{
			return startSpeed;
		}
	}
	float currentSpeed;
	public float CurrentSpeed
	{
		get
		{
			return currentSpeed;
		}
	}


	//[Header("Mobile Movement")]
	private Joystick joystick;

	private void Start(){
		joystick = FindObjectOfType<Joystick>();
		joystick.gameObject.SetActive(true);
#if UNITY_STANDALONE
		joystick.gameObject.SetActive(false);
#endif

		currentSpeed = startSpeed;
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + Movement * currentSpeed * Time.fixedDeltaTime);
	}

	public void ChangeSpeed(float speed)
	{
		currentSpeed = speed;
	}
	
	public IEnumerator ChangeSpeedWithCurveCoroutine(AnimationCurve multiplierSpeedCurve)
	{
		float currentSpeedBeforeCurve = currentSpeed;
		float currentTime = 0;
		//Debug.Log("Last Keyframe Time: " + multiplierSpeedCurve[multiplierSpeedCurve.keys.Length - 1].time);
		while (currentTime < multiplierSpeedCurve[multiplierSpeedCurve.keys.Length - 1].time)
		{
			//Debug.Log(currentSpeedBeforeCurve);
			currentTime += Time.deltaTime;
			currentSpeed = currentSpeedBeforeCurve * multiplierSpeedCurve.Evaluate(currentTime);

			yield return null;
		}

		currentSpeed = startSpeed;
	}
}
