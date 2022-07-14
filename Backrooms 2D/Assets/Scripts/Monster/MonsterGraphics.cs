using System;
using UnityEngine;

public class MonsterGraphics : MonoBehaviour
{
	private MonsterMovement mm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	[Header("Sprite")]
	[SerializeField] private Sprite[] playerPoses = new Sprite[4];
	[SerializeField] private float spriteThreshold = .4f;

	[Header("Poolrooms Slow Down")]
	[SerializeField] private float slowDownSpeedMultiplier = .25f;
	private float slowDownSpeed
	{
		get
		{
			return originalSpeed * slowDownSpeedMultiplier;
		}
	}
	private float originalSpeed;

	private void Awake()
	{
		mm = GetComponent<MonsterMovement>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		originalSpeed = mm.speed;
		sr.sprite = playerPoses[2];	
	}

	private void OnEnable()
	{
		LVLPoolroomPoolSpeed.SlowDown += SlowDown;
		LVLPoolroomPoolSpeed.SpeedUp += SpeedUp;
	}

	private void OnDisable()
	{
		LVLPoolroomPoolSpeed.SlowDown -= SlowDown;
		LVLPoolroomPoolSpeed.SpeedUp -= SpeedUp;
	}

	private void SlowDown(GameObject objToSlow)
	{
		if (objToSlow == gameObject) { mm.speed = slowDownSpeed; FindObjectOfType<AudioManager>().PlayOneShot("LVLPoolrooms_Splash"); }
	}

	private void SpeedUp(GameObject objToSpeedUp)
	{
		if (objToSpeedUp == gameObject) mm.speed = originalSpeed;
	}

	private void Update()
	{
		if (rb.velocity.y > spriteThreshold) sr.sprite = playerPoses[0];
		else if (rb.velocity.y < -spriteThreshold) sr.sprite = playerPoses[2];
		else if (rb.velocity.x > spriteThreshold) sr.sprite = playerPoses[1];
		else if (rb.velocity.x < -spriteThreshold) sr.sprite = playerPoses[3];

		/*Vector2 roundedForce = new Vector2(Math.Sign(mm.Force.x), Math.Sign(mm.Force.y));
		Debug.Log(roundedForce);
		if (roundedForce.y > 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[0];
		else if (roundedForce.y < 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[2];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x > 0) sr.sprite = playerPoses[1];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x < 0) sr.sprite = playerPoses[3];*/
	}
}
