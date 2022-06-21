using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGraphics : MonoBehaviour
{
	private PlayerMovement pm;

	private SpriteRenderer sr;

	[Header("Sprite")]
	[SerializeField] private Sprite[] playerPoses = new Sprite[4];

	[Header("Footsteps")]
	[SerializeField] private float velocityThreshold = .4f;
	[SerializeField] [MinMaxRange(0, 1.5f)] private RangedFloat startTimeBtwnFootsteps = new RangedFloat(0.4f, 0.7f);
	[SerializeField] [MinMaxRange(0, 1.5f)] private RangedFloat maxTimeBtwnFootsteps = new RangedFloat(0.1f, 0.25f);
	private Vector2 lastPos;
	private float timeBtwnFootsteps;
	private bool playFirstFootstep;
	private bool canPlayFootstep = true;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();

		pm = GetComponent<PlayerMovement>();
	}

	private void Start()
	{
		sr.sprite = playerPoses[2];

		lastPos = transform.position;
		timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
	}

	private void Update()
	{
		if (pm.Movement.x > 0) sr.sprite = playerPoses[1];
		else if (pm.Movement.x < 0) sr.sprite = playerPoses[3];
		else if (pm.Movement.y > 0) sr.sprite = playerPoses[0];
		else if (pm.Movement.y < 0) sr.sprite = playerPoses[2];
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
		lastPos = transform.position;
	}

	private IEnumerator CanPlayFootstep()
	{
		canPlayFootstep = false;

		yield return new WaitForSeconds(Random.Range(maxTimeBtwnFootsteps.Min, maxTimeBtwnFootsteps.Max));

		canPlayFootstep = true;
	}
}
