using UnityEngine;
using Pathfinding;
using System.Collections;

//THANKS BRACKEYS! https://www.youtube.com/watch?v=jvtFUfJ6CP8
public class MonsterMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	private Seeker seeker;

	private GameObject player;

	private TileSpawner ts;

	private AstarPath pathfinding;
    private Path path;
	private int currentWaypoint = 0;
	private bool reachedEndOfPath = false;

	[Header("Monster Attributes")]
	[SerializeField] private float speed = 200f;
	[SerializeField] private float nextWaypointDistance = 3f;

	private void Awake()
	{
		pathfinding = FindObjectOfType<AstarPath>();
		rb = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker>();

		player = FindObjectOfType<PlayerMovement>().gameObject;

		ts = FindObjectOfType<TileSpawner>();
	}

	private void Start()
	{
		StartCoroutine(GeneratePathCoroutine());
		/*ts.CanSpawnTiles += (canSpawnTiles) => 
		{
			Debug.Log(canSpawnTiles);
			if (canSpawnTiles) StartCoroutine(GeneratePathCoroutine());
			else StopCoroutine(GeneratePathCoroutine());
		};*/
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

			gg.center = transform.position;

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

		rb.AddForce(force);

		float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

		if (distance < nextWaypointDistance)
		{
			currentWaypoint++;
		}
	}
}
