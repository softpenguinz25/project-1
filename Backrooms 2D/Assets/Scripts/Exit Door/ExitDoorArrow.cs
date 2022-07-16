using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorArrow : MonoBehaviour
{
	private Animator animator;
	private ExitDoorSpawner eds;

	[SerializeField] private GameObject player;
	private Seeker playerSeeker;
	private AstarPath pathfinding;
	private Path path;
	
	[SerializeField] private float screenBoundsIndent = 8;
	private Vector2 screenSize;

	[Space]
	[SerializeField] private float timeBeforeGenerateNextPath = .1f;

	private GameObject exitDoor;

	private const float angleOffset = 270;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		playerSeeker = player.GetComponent<Seeker>();
		pathfinding = FindObjectOfType<AstarPath>();
		eds = FindObjectOfType<ExitDoorSpawner>();
	}

	private bool sceneJustLaunched = true;
	private void OnEnable()
	{
		eds.ExitDoorSpawned += (exitDoorObj) =>
		{
			gameObject.SetActive(true);
			animator.SetTrigger("StartFlash");
			//exitDoor = exitDoorObj;
		};

		if (sceneJustLaunched) sceneJustLaunched = false;
			else StartCoroutine(GeneratePathCoroutine());
	}

	private void Start()
	{
		gameObject.SetActive(false);
		screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));		
	}

	private IEnumerator GeneratePathCoroutine()
	{
		exitDoor = FindObjectOfType<ExitDoor>().gameObject;
		while (true)
		{
			//Documentation - https://arongranberg.com/astar/docs/gridgraph.html#center

			// This holds all graph data
			AstarData data = AstarPath.active.data;

			// This finds the first graph
			GridGraph gg = data.graphs[0] as GridGraph;

			float tileSize = /*FindObjectOfType<TileSpawner>().tileCollection.tileSize*/1;
			gg.center = new Vector3(Mathf.Round(transform.position.x / tileSize), Mathf.Round(transform.position.y / tileSize), Mathf.Round(transform.position.z / tileSize)) * tileSize;

			pathfinding.Scan();

			/*Debug.Log(player, player);
			Debug.Log(exitDoor, exitDoor);*/
			//Debug.Log("Player seeker " + playerSeeker);
			playerSeeker.StartPath(player.transform.position, exitDoor.transform.position, OnPathComplete);

			yield return new WaitForSeconds(timeBeforeGenerateNextPath);
		}
	}

	private void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
		}
	}

	private void Update()
	{
		if (exitDoor == null)
		{
			//Debug.LogError("Exit door could not be detected!");
			return;
		}

		Vector2 wayPointPos;
		if (Vector2.Distance(path.vectorPath[path.vectorPath.Count - 1], exitDoor.transform.position) < 2f)
		{
			int currentWayPoint = 0;
			wayPointPos = path.vectorPath[currentWayPoint];
			//Debug.Log(wayPointPos + ": " + WayPointInCameraView(wayPointPos));
			while (WayPointInCameraView(wayPointPos))
			{
				currentWayPoint++;

				if (currentWayPoint >= path.vectorPath.Count - 1)
				{
					Debug.LogError("No Waypoint out of camera view!");
					break;
				}

				wayPointPos = path.vectorPath[currentWayPoint];

				if (!WayPointInCameraView(wayPointPos))
				{
					//Debug.Log("Waypoint out of camera view found at pos " + wayPointPos);
					break;
				}
			}
		}
		else
		{
			wayPointPos = exitDoor.transform.position;
		}

		/*Debug.Log("1 | " + wayPointPos + ": " + WayPointInCameraView(wayPointPos));
		wayPointPos = path.vectorPath[1];
		Debug.Log("2 | " + wayPointPos + ": " + WayPointInCameraView(wayPointPos));
		wayPointPos = path.vectorPath[2];
		Debug.Log("3 | " + wayPointPos + ": " + WayPointInCameraView(wayPointPos));
		wayPointPos = path.vectorPath[3];
		Debug.Log("4 | " + wayPointPos + ": " + WayPointInCameraView(wayPointPos));
		wayPointPos = path.vectorPath[4];*/

		//Position
		if (GetComponent<SpriteRenderer>().color.a <= 0.01f)
		{
			Vector3 newPos = wayPointPos;
			Vector3 cameraPos = Camera.main.transform.position;
			newPos.x = Mathf.Clamp(newPos.x, cameraPos.x + (-screenSize.x * .5f + screenBoundsIndent), cameraPos.x + (screenSize.x * .5f - screenBoundsIndent));
			newPos.y = Mathf.Clamp(newPos.y, cameraPos.y + (-screenSize.y * .5f + screenBoundsIndent), cameraPos.y + (screenSize.y * .5f - screenBoundsIndent));

			transform.position = newPos;
		}

		//Rotation
		//THANKS Kastenessen! https://answers.unity.com/questions/1350050/how-do-i-rotate-a-2d-object-to-face-another-object.html
		//THANKS sean 244! https://answers.unity.com/questions/1592029/how-do-you-make-enemies-rotate-to-your-position-in.html
		if (GetComponent<SpriteRenderer>().color.a <= 0.01f)
		{
			Vector3 targetPos = wayPointPos;
			Vector2 direction = targetPos - transform.position;

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(Vector3.forward * (angle + angleOffset));
		}
	}

	private bool WayPointInCameraView(Vector2 wayPointPos)
	{
		Vector2 horizontalBounds = new Vector2((-screenSize.x * .5f + screenBoundsIndent) + player.transform.position.x, (screenSize.x * .5f - screenBoundsIndent) + player.transform.position.x);
		Vector2 verticalBounds = new Vector2((-screenSize.y * .5f + screenBoundsIndent) + player.transform.position.y, (screenSize.y * .5f - screenBoundsIndent) + player.transform.position.y);

		//Debug.Log("Horizontal Bounds: " + horizontalBounds);
		//Debug.Log("Vertical Bounds: " + verticalBounds);

		return wayPointPos.x > horizontalBounds.x &&
				wayPointPos.x < horizontalBounds.y &&
				wayPointPos.y > verticalBounds.x &&
				wayPointPos.y < verticalBounds.y;
	}
}
