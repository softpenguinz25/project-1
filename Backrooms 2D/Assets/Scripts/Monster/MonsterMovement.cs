using UnityEngine;
using Pathfinding;
using System.Collections;

//THANKS BRACKEYS! https://www.youtube.com/watch?v=jvtFUfJ6CP8
[RequireComponent(typeof(Rigidbody2D))]
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

	[HideInInspector] public Vector2 Force;

	[Header("Regular Monster Attributes")]
	[SerializeField] private MonsterStats regularStats;
	/*[SerializeField] private float timeBeforeGenerateNextPath = 1.5f;
	public float speed = 200f;
	private float originalSpeed;
	//private float currentSpeed;
	[SerializeField] private float nextWaypointDistance = 3f;
	[SerializeField] private float linearDrag = 3;*/

	[Header("Slow Monster Attributes")]
	[SerializeField] private float slowThreshold = 5;
	[HideInInspector] public bool isSlow;
	[Space]
	[SerializeField] private MonsterStats slowStats;
	/*[SerializeField] private float slowSpeedMultiplier = 1f;
	[SerializeField] private float slowTimeBeforeGenerateNextPath = 1.5f;
	[SerializeField] private float slowNextWaypointDistance = 3f;
	[SerializeField] private float slowLinearDrag = 3;*/

	[Header("Close Monster Attributes")]
	[SerializeField] private MonsterStats closeToPlayerStats;
	[Header("Far Monster Attributes")]
	[SerializeField] private MonsterStats farUnobstructing;
	[SerializeField] private MonsterStats farObstructing;
	[Header("LVL Poolrooms Monster Attributes")]
	[SerializeField] private MonsterStats inPoolStats;
	[HideInInspector] public bool monsterIsInPool;
	/*[SerializeField] private float closeSpeedMultiplier = 1.5f;
	*//*[SerializeField] *//*private float closeTimeBeforeGenerateNextPath = 1.5f;
	*//*[SerializeField] *//*private float closeNextWaypointDistance = 3f;
	[SerializeField] private float closeLinearDrag = 3;*/

	public MonsterStats CurrentStats
	{
		get
		{
			if (monsterIsInPool) return inPoolStats;
			else if (mc.IsClose) return closeToPlayerStats;
			else if (isSlow) return slowStats;
			else if (mc.IsFarUnobstructing) return farUnobstructing;
			else if (mc.IsFarObstructing) return farObstructing;
			else return regularStats;
		}
	}
	private MonsterStats oldStat;
	float currentTimeBeforeGenerateNextPath;

	//True variables
	/*private float TrueTimeBeforeGenerateNextPath
	{
		get
		{
			if (mc.IsClose) return closeTimeBeforeGenerateNextPath;
			return isSlow ? slowTimeBeforeGenerateNextPath : timeBeforeGenerateNextPath;
		}
	}
	private float TrueSpeed
	{
		get
		{
			if (mc.IsClose) return speed = originalSpeed * closeSpeedMultiplier;
			return speed = originalSpeed * (isSlow ? slowSpeedMultiplier : 1);
		}
	}
	private float TrueNextWaypointDistance
	{
		get
		{
			if (mc.IsClose) return closeNextWaypointDistance;
			return isSlow ? slowNextWaypointDistance : nextWaypointDistance;
		}
	}*/

	private void Awake()
	{
		pathfinding = FindObjectOfType<AstarPath>();
		rb = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker>();

		player = FindObjectOfType<PlayerMovement>().gameObject;

		mc = GetComponent<MonsterClose>();
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
		oldStat = CurrentStats;		

		StartCoroutine(GeneratePathCoroutine());
	}
	
	private IEnumerator GeneratePathCoroutine()
	{
		while (true)
		{
			if (mc.IsClose) { yield return null; continue; }

			currentTimeBeforeGenerateNextPath = CurrentStats.timeBeforeGenerateNextPath;

			//Documentation - https://arongranberg.com/astar/docs/gridgraph.html#center

			// This holds all graph data
			AstarData data = AstarPath.active.data;

			// This creates a Grid Graph
			GridGraph gg = data.graphs[0] as GridGraph;

			float tileSize = /*FindObjectOfType<TileSpawner>().tileCollection.tileSize*/1;
			gg.center = new Vector3(Mathf.Round(transform.position.x / tileSize), Mathf.Round(transform.position.y / tileSize), Mathf.Round(transform.position.z / tileSize)) * tileSize;

			pathfinding.Scan();

			seeker.StartPath(rb.position, player.transform.position, OnPathComplete);

			while (currentTimeBeforeGenerateNextPath > 0)
			{
				currentTimeBeforeGenerateNextPath -= Time.deltaTime;
				yield return null;
				if (CurrentStats != oldStat)
				{
					//Debug.Log("switching monster mode to " + CurrentStats.name);
					oldStat = CurrentStats;
					break;
				}
			}

			//yield return new WaitForSeconds(CurrentStat.timeBeforeGenerateNextPath);
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
		isSlow = rb.velocity.sqrMagnitude < slowThreshold ? true : false;

		rb.drag = CurrentStats.linearDrag;

		/*if (path == null) 
		{ 
			Debug.Log(gameObject.name + " could not detect a path to follow!", gameObject); 
			return; 
		}*/
		Vector2 targetPoint = new Vector2();
		if (path != null)
		{
			if (currentWaypoint >= path.vectorPath.Count)
			{
				reachedEndOfPath = true;
				//Debug.Log("Reached End of Path!");
				return;
			}
			else
			{
				reachedEndOfPath = false;
			}

			targetPoint = path.vectorPath[currentWaypoint];

			/*MonsterSpawnTiles monsterSpawnTiles = GetComponent<MonsterSpawnTiles>();
			if(monsterSpawnTiles != null)
			{
				Vector2 roundedTargetPoint = ;
				*//*if(Physics2D.OverlapPoint(targetPoint))*//*
			}*/
		}
		if(mc.IsClose || path == null) targetPoint = player.transform.position;
		//Debug.Log(currentWaypoint);
		Vector2 direction = (targetPoint - rb.position).normalized;
		Vector2 force = direction * CurrentStats.speed * Time.deltaTime * rb.mass;
		Force = force;

		rb.AddForce(force);

		if (mc.IsClose || path == null) return;

		float distance = Vector2.Distance(rb.position, targetPoint);

		if (distance < CurrentStats.nextWaypointDistance)
		{
			currentWaypoint++;
		}
	}
}
