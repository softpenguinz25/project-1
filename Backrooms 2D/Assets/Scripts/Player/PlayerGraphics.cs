using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGraphics : MonoBehaviour
{
	private PlayerMovement pm;

	//private SpriteRenderer sr;

	[Header("Animation")]
	//Back, right, front, left
	private Animator animator;
	private float lastHorizontalMovement, lastVerticalMovement;
	//[SerializeField] private Sprite[] playerPoses = new Sprite[4];
	//[SerializeField] private float changeSpriteThreshold = .1f;

	[Header("Footsteps")]
	[SerializeField] private float velocityThreshold = .4f;
	[SerializeField] [MinMaxRange(0, 1.5f)] private RangedFloat startTimeBtwnFootsteps = new RangedFloat(0.4f, 0.7f);
	[SerializeField] [MinMaxRange(0, 1.5f)] private RangedFloat maxTimeBtwnFootsteps = new RangedFloat(0.1f, 0.25f);
	private Vector2 lastPos;
	private float timeBtwnFootsteps;
	private bool playFirstFootstep;
	private bool canPlayFootstep = true;

	[Header("Poolrooms Slow Down")]
	[SerializeField] private float slowDownSpeedMultiplier = .5f;
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
		animator = GetComponent<Animator>();
		//sr = GetComponent<SpriteRenderer>();

		pm = GetComponent<PlayerMovement>();
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
		if (objToSlow == gameObject) { pm.ChangeSpeed(slowDownSpeed); FindObjectOfType<AudioManager>().PlayOneShot("LVLPoolrooms_Splash"); }
	}
	
	private void SpeedUp(GameObject objToSpeedUp)
	{		
		if (objToSpeedUp == gameObject) pm.ChangeSpeed(originalSpeed);		
	}


	private void Start()
	{
		//sr.sprite = playerPoses[2];

		lastPos = transform.position;
		timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);

		originalSpeed = pm.StartSpeed;
	}

	private void Update()
	{
		/*if (pm.Movement.x > 0) sr.sprite = playerPoses[1];
		else if (pm.Movement.x < 0) sr.sprite = playerPoses[3];
		else if (pm.Movement.y > 0) sr.sprite = playerPoses[0];
		else if (pm.Movement.y < 0) sr.sprite = playerPoses[2];*/

		animator.SetFloat("Speed", pm.Movement.sqrMagnitude);
		if(pm.Movement.sqrMagnitude > .01f)
		{
			animator.SetFloat("Horizontal_Moving", pm.Movement.x);
			animator.SetFloat("Vertical_Moving", pm.Movement.y);

			lastHorizontalMovement = pm.Movement.x;
			lastVerticalMovement = pm.Movement.y;
		}
		else
		{
			animator.SetFloat("Horizontal_Idle", lastHorizontalMovement);
			animator.SetFloat("Vertical_Idle", lastVerticalMovement);
		}
	}

	private void FixedUpdate()
	{
		if(pm.Movement.sqrMagnitude > velocityThreshold && Vector2.Distance(transform.position, lastPos) > velocityThreshold)
		{
			if (playFirstFootstep)
			{
				playFirstFootstep = false;

				if (canPlayFootstep)
				{
					FindObjectOfType<AudioManager>().Play(SceneManager.GetActiveScene().name.Replace(" ", string.Empty) + "_Player_Footsteps");
					StartCoroutine(CanPlayFootstep());
				}				
			}

			if(timeBtwnFootsteps > 0)
			{
				timeBtwnFootsteps -= Time.deltaTime;
			}
			else
			{
				timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
				
				FindObjectOfType<AudioManager>().Play(SceneManager.GetActiveScene().name.Replace(" ", string.Empty) + "_Player_Footsteps");
				StartCoroutine(CanPlayFootstep());
			}
		}
		else
		{
			if (canPlayFootstep)
			{
				timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
			}
			playFirstFootstep = true;
		}

		/*bool movingVertically = Mathf.Abs(pm.Movement.y) > Mathf.Abs(pm.Movement.x);
		if (movingVertically)
		{
			if(pm.Movement.y > changeSpriteThreshold) sr.sprite = playerPoses[0];
			else if (pm.Movement.y < -changeSpriteThreshold) sr.sprite = playerPoses[2];
		}
		else
		{
			if (pm.Movement.x > changeSpriteThreshold) sr.sprite = playerPoses[1];
			else if (pm.Movement.x < -changeSpriteThreshold) sr.sprite = playerPoses[3];
		}*/

		lastPos = transform.position;
	}

	private IEnumerator CanPlayFootstep()
	{
		canPlayFootstep = false;

		yield return new WaitForSeconds(Random.Range(maxTimeBtwnFootsteps.Min, maxTimeBtwnFootsteps.Max));

		canPlayFootstep = true;
	}
}
