using System.Collections;
using UnityEngine;

public class MonsterGraphics : MonoBehaviour
{
	private MonsterMovement mm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	[Header("Sprite")]
	[SerializeField] private Sprite[] playerPoses = new Sprite[4];
	[SerializeField] private float spriteThreshold = .4f;

	[Header("Sound")]
	[SerializeField] AnimationCurve ambienceFadeInCurve;
	float startingAudioLevel;

	[Header("Footsteps")]
	[SerializeField] ParticleSystem footstepParticles;

	[Header("Other GFX")]
	[SerializeField] GameObject splashParticles;
	[SerializeField] GameObject pushableObstacleParticle;

	/*[Header("Poolrooms Slow Down")]
	[SerializeField] private float slowDownSpeedMultiplier = .25f;
	private float slowDownSpeed
	{
		get
		{
			return originalSpeed * slowDownSpeedMultiplier;
		}
	}
	private float originalSpeed;	*/

	private void Awake()
	{
		mm = GetComponent<MonsterMovement>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();

		startingAudioLevel = GetComponent<AudioSource>().volume;
	}

	private IEnumerator Start()
	{
		//originalSpeed = mm.CurrentStats.speed;
		sr.sprite = playerPoses[2];

		float t = 0;
		while (t < ambienceFadeInCurve.keys[ambienceFadeInCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;

			GetComponent<AudioSource>().volume = ambienceFadeInCurve.Evaluate(t) * startingAudioLevel;
			yield return null;
		}

		GetComponent<AudioSource>().volume = startingAudioLevel;
	}

	private void OnEnable()
	{
		LVLPoolroomPoolSpeed.SlowDown += SlowDown;
		LVLPoolroomPoolSpeed.SpeedUp += SpeedUp;

		/*#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += SpeedUpEditor;
		#endif*/
	}

	private void OnDisable()
	{
		//mm.CurrentStats.speed = originalSpeed;
		LVLPoolroomPoolSpeed.SlowDown -= SlowDown;
		LVLPoolroomPoolSpeed.SpeedUp -= SpeedUp;
	}

	private void SlowDown(GameObject objToSlow)
	{
		if (objToSlow == gameObject) { mm.monsterIsInPool = true; FindObjectOfType<AudioManager>().PlayOneShot("LVLPoolrooms_Splash"); Instantiate(splashParticles, transform.position, Quaternion.Euler(0, 0, -90)); }
	}

	private void SpeedUp(GameObject objToSpeedUp)
	{
		if (objToSpeedUp == gameObject) mm.monsterIsInPool = false;
	}

	/*#if UNITY_EDITOR
	private void SpeedUpEditor(PlayModeStateChange playModeState)
	{
		if(playModeState == PlayModeStateChange.ExitingPlayMode) mm.CurrentStats.speed = originalSpeed;
	}
	#endif*/

	private void Update()
	{
		if (rb.velocity.y > spriteThreshold) sr.sprite = playerPoses[0];
		else if (rb.velocity.y < -spriteThreshold) sr.sprite = playerPoses[2];
		else if (rb.velocity.x > spriteThreshold) sr.sprite = playerPoses[1];
		else if (rb.velocity.x < -spriteThreshold) sr.sprite = playerPoses[3];

		#region Footstep Particles
		/*bool isMovingUp = pm.Movement.x*/
		if (rb.velocity.sqrMagnitude > .8f)
		{
			if (!footstepParticles.isPlaying)
				footstepParticles.Play();
		}
		else
		{
			if (footstepParticles.isPlaying)
				footstepParticles.Stop();
		}
		#endregion
		/*Vector2 roundedForce = new Vector2(Math.Sign(mm.Force.x), Math.Sign(mm.Force.y));
		Debug.Log(roundedForce);
		if (roundedForce.y > 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[0];
		else if (roundedForce.y < 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[2];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x > 0) sr.sprite = playerPoses[1];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x < 0) sr.sprite = playerPoses[3];*/
	}

	[SerializeField] AudioSource collidedWithPushableObstacleSFX;
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Pushable Obstacle"))
		{
			collidedWithPushableObstacleSFX.Play();
			Instantiate(pushableObstacleParticle, collision.transform.position, Quaternion.Euler(-90, 0, 0));
		}
	}
}
