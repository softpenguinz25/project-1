using UnityEngine;
using Pathfinding;
using System.Collections;

//THANKS BRACKEYS! https://www.youtube.com/watch?v=jvtFUfJ6CP8
public class MonsterMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	private Seeker seeker;

	private GameObject player;

	private MonsterClose mc;

	private AstarPath pathfinding;
    private Path path;
	private int currentWaypoint = 0;
	private bool reachedEndOfPath = false;

	[Header("Monster Attributes")]
	public float speed = 200f;
	//private float currentSpeed;
	[SerializeField] private float nextWaypointDistance = 3f;

	[HideInInspector] public Vector2 Force;

	/*[Header("Monster Is Close Attributes")]
	[SerializeField] private float isCloseSpeedMultiplier = 1.5f;
	private float closeSpeed
	{
		get
		{
			return roamingSpeed * isCloseSpeedMultiplier;
		}
	}*/

	private void Awake()
	{
		pathfinding = FindObjectOfType<AstarPath>();
		rb = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker>();

		player = FindObjectOfType<PlayerMovement>().gameObject;

		//mc = GetComponent<MonsterClose>();
	}

	/*private void OnEnable()
	{
		mc.MonsterIsClose += () => 
		{
			Debug.Log("IS CLOSE");
			currentSpeed = closeSpeed;
			GetComponent<SpriteRenderer>().color = Color.cyan;
		};

		mc.PlayerHasEscaped += () =>
		{
			Debug.Log("player has escaped....");

			currentSpeed = roamingSpeed;
			GetComponent<SpriteRenderer>().color = Color.blue;
		};
	}*/

	private void Start()
	{
		//currentSpeed = roamingSpeed;

		StartCoroutine(GeneratePathCoroutine());
	}

	private IEnumerator GeneratePathCoroutine()
	{
		while (true)
		{
			//Documentation - https://arongranberg.com/astar/docs/gridgraph.html#center

			// This holds all graph data
			AstarData data = AstarPath.active.data;

			// This creates a Grid Graph
			GridGraph gg = data.graphs[0] as GridGraph;

			float tileSize = /*FindObjectOfType<TileSpawner>().tileCollection.tileSize*/1;
			gg.center = new Vector3(Mathf.Round(transform.position.x / tileSize), Mathf.Round(transform.position.y / tileSize), Mathf.Round(transform.position.z / tileSize)) * tileSize;

			pathfinding.Scan();

			seeker.StartPath(rb.position, player.transform.position, OnPathComplete);

			yield return new WaitForSeconds(.5f);
		}
	}

	private void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}

	private void Update()
	{
		if (path == null) return;

		if (currentWaypoint >= path.vectorPath.Count)
		{
			reachedEndOfPath = true;
			return;
		}
		else
		{
			reachedEndOfPath = false;
		}

		Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
		Vector2 force = direction * speed * Time.deltaTime;
		Force = force;

		rb.AddForce(force);

		float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

		if (distance < nextWaypointDistance)
		{
			currentWaypoint++;
		}
	}
}
