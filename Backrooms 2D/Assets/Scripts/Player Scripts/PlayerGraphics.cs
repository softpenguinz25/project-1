using MyBox;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
	private PlayerMovement pm;

	[Header("Footsteps")]
	[SerializeField] private float velocityThreshold = .4f;
	[SerializeField] [MinMaxRange(0, 1.5f)] private RangedFloat startTimeBtwnFootsteps = new RangedFloat(0.4f, 0.7f);
	private Vector2 lastPos;
	private float timeBtwnFootsteps;
	private bool playFirstFootstep;

	private void Awake()
	{
		pm = GetComponent<PlayerMovement>();
	}

	private void Start()
	{
		lastPos = transform.position;
		timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
	}

	private void Update()
	{		
		if(pm.Movement.sqrMagnitude > velocityThreshold)
		{
			if (playFirstFootstep)
			{
				FindObjectOfType<AudioManager>().Play("LVLLobby_Player_Footsteps");
				playFirstFootstep = false;
			}

			if(timeBtwnFootsteps > 0)
			{
				timeBtwnFootsteps -= Time.deltaTime;
			}
			else
			{
				timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
				
				FindObjectOfType<AudioManager>().Play("LVLLobby_Player_Footsteps");
			}
		}
		else
		{
			timeBtwnFootsteps = Random.Range(startTimeBtwnFootsteps.Min, startTimeBtwnFootsteps.Max);
			playFirstFootstep = true;
			Debug.Log("Not Moving");
		}		
	}
}
