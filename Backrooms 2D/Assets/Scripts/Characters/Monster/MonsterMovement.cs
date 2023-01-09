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
	public GameObject Player
	{
		get
		{
			return player;
		}
	}
	private GameObject currentTarget;
	public GameObject CurrentTarget
	{
		get
		{
			return currentTarget;
		}
	}

	private MonsterClose mc;
	public MonsterClose MC
	{
		get
		{
			return mc;
		}
	}

	private AstarPath pathfinding;
	private Path path;
	private int currentWaypoint = 0;

	[Header("Regular Monster Attributes")]
	[SerializeField] private MonsterStats regularStats;

	[Header("Slow Monster Attributes")]
	[SerializeField] private float slowThreshold = 5;
	[HideInInspector] public bool isSlow;
	[Space]
	[SerializeField] private MonsterStats slowStats;

	[Header("Close Monster Attributes")]
	[SerializeField] private MonsterStats closeToPlayerStats;
	[Header("Far Monster Attributes")]
	[SerializeField] private MonsterStats farUnobstructing;
	[SerializeField] private MonsterStats farObstructing;
	[Header("LVL Poolrooms Monster Attributes")]
	[SerializeField] private MonsterStats inPoolStats;
	[HideInInspector] public bool monsterIsInPool;

	public virtual MonsterStats CurrentStats
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

	public virtual void Awake()
	{
		pathfinding = FindObjectOfType<AstarPath>();
		rb = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker>();

		player = FindObjectOfType<PlayerMovement>().gameObject;

		mc = GetComponent<MonsterClose>();
	}

	public virtual void Start()
	{
		currentTarget = player;

		oldStat = CurrentStats;		

		StartCoroutine(GeneratePathCoroutine());
	}
	
	private IEnumerator GeneratePathCoroutine()
	{
		while (true)
		{
			if (mc.IsClose /*|| mc.IsFarUnobstructing*/) { yield return null; continue; }

			currentTimeBeforeGenerateNextPath = CurrentStats.timeBeforeGenerateNextPath;

			//Documentation - https://arongranberg.com/astar/docs/gridgraph.html#center

			// This holds all graph data
			AstarData data = AstarPath.active.data;
			//Debug.Log(data.graphs.Length);
			// This creates a Grid Graph
			GridGraph gg = data.graphs[0] as GridGraph;

			float tileSize = /*FindObjectOfType<TileSpawner>().tileCollection.tileSize*/1;
			gg.center = new Vector3(Mathf.Round(transform.position.x / tileSize), Mathf.Round(transform.position.y / tileSize), Mathf.Round(transform.position.z / tileSize)) * tileSize;

			pathfinding.Scan();

			seeker.StartPath(rb.position, currentTarget.transform.position, OnPathComplete);

			while (currentTimeBeforeGenerateNextPath > 0)
			{
				currentTimeBeforeGenerateNextPath -= Time.deltaTime;
				yield return null;
				if (CurrentStats != oldStat)
				{
					oldStat = CurrentStats;
					break;
				}
			}
		}
	}

	private void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
			ResetCurrentWaypoint();
		}
	}

	void ResetCurrentWaypoint()
	{
		currentWaypoint = 0;
	}

	public virtual void Update()
	{
		if (DeathManager.jumpscareIsPlaying)
		{
			rb.velocity = Vector2.zero;
			return;
		}

		SetupVariables();
		
		//if (Vector2.Distance(new Vector2(float.MaxValue, float.MaxValue), TargetPoint()) < 1) return;
		
		MoveToPoint(TargetPoint());

		if (mc.IsClose || path == null) return;

		IncrementWaypointIfCloseEnough();
	}	

	private void SetupVariables()
	{
		isSlow = rb.velocity.sqrMagnitude < slowThreshold ? true : false;

		rb.drag = CurrentStats.linearDrag;

		if (currentTarget == null) currentTarget = player;
	}

	public virtual Vector2 TargetPoint()
	{
		Vector2 targetPoint = new Vector2();

		if (path != null)
		{
			//Debug.Log(currentWaypoint + ", " + path.vectorPath.Count);
			if (currentWaypoint >= path.vectorPath.Count) ResetCurrentWaypoint(); //return new Vector2(float.MaxValue, float.MaxValue);

			targetPoint = path.vectorPath[currentWaypoint];
			//Debug.Log("Current point " + currentWaypoint + " is at " + targetPoint);
		}
		if (mc.IsClose || path == null) targetPoint = currentTarget.transform.position;
		
		return targetPoint;
	}

	private void MoveToPoint(Vector2 targetPoint)
	{		
		Vector2 direction = (targetPoint - rb.position).normalized;
		Vector2 force = direction * CurrentStats.speed * Time.deltaTime * rb.mass;

		rb.AddForce(force);
	}

	private void IncrementWaypointIfCloseEnough()
	{
		float distance = Vector2.Distance(rb.position, TargetPoint());

		if (distance < CurrentStats.nextWaypointDistance)
		{
			currentWaypoint++;
		}
	}

	public bool HasReachedCurrentTarget()
	{
		//Debug.Log(CurrentStats.nextWaypointDistance + " vs " + Vector2.Distance(rb.position, currentTarget.transform.position) + " = " + (Vector2.Distance(rb.position, currentTarget.transform.position) < CurrentStats.nextWaypointDistance ? "True" : "False"));
		return Vector2.Distance(rb.position, currentTarget.transform.position) < CurrentStats.nextWaypointDistance;
	}

	public void ChangeTarget(GameObject newTarget)
	{
		currentTarget = newTarget;
	}
}
