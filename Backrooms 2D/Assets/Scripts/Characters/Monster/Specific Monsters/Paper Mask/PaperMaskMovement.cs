using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaperMaskMovement : MonsterMovement
{
	MonsterChaseGFX mcgfx;

	enum MonsterState
	{
		Roaming,
		Chasing
	}

	MonsterState lastMonsterState;
	MonsterState currentMonsterState;

	[Header("Paper Mask Roaming")]
    [SerializeField] List<Transform> roamingPoints = new List<Transform>();
	int targetPointIndex;
	Transform targetPoint;

	[Header("Paper Mask Chasing")]
	[Tooltip("If the monster's line of sight hasn't seen the player in x time, go back to roaming mode")][SerializeField] float startDelayUntilRoam;
	float currentDelayUntilRoam;
	Vector2 lastMonsterPos;
    public List<Transform> RoamingPoints
	{
		get
		{
			return roamingPoints;
		}
	}

	[Header("Paper Mask Player In Computer Behaviour")]
	[SerializeField] GameObject computerUI;
	bool playerInComputerUI
	{
		get
		{
			return computerUI.activeSelf;
		}
	}
	[SerializeField] MonsterStats playerInComputerUIStats;
	public override MonsterStats CurrentStats
	{
		get
		{
			if (playerInComputerUI && CurrentTarget == Player && MC.IsClose) return playerInComputerUIStats;

			return base.CurrentStats;
		}
	}

	public override void Awake()
	{
		base.Awake();

		mcgfx = FindObjectOfType<MonsterChaseGFX>();
	}

	private void OnEnable()
	{
		mcgfx.MonsterIsClose += FocusOnPlayer;
	}	

	private void OnDisable()
	{
		mcgfx.MonsterIsClose -= FocusOnPlayer;
	}

	private void FocusOnPlayer(GameObject monster)
	{
		//Debug.Log("Focusing on player");
		ChangeTarget(Player);
		currentMonsterState = MonsterState.Chasing;
	}

	public override void Start()
	{
		base.Start();

		currentMonsterState = MonsterState.Roaming;
		lastMonsterState = currentMonsterState;

		currentDelayUntilRoam = startDelayUntilRoam;

		targetPoint = roamingPoints[targetPointIndex];
		ChangeTarget(targetPoint.gameObject);
	}

	public override void Update()
	{
		#region Variable Setting
		if (playerInComputerUI) ChangeTarget(Player);
		#endregion

		#region State Switching
		if (lastMonsterState != currentMonsterState)
		{
			switch (currentMonsterState)
			{
				case MonsterState.Roaming:
					FocusOnClosestRoamingPoint();
					break;
			}

			lastMonsterState = currentMonsterState;
		}
		#endregion

		#region Roaming
		if (HasReachedCurrentTarget() && currentMonsterState == MonsterState.Roaming)
		{
			//Debug.Log("Old target point index: " + targetPointIndex);
			targetPointIndex = IncrementTargetPoint();
			//Debug.Log("New target point index: " + targetPointIndex);
			ChangeTarget(roamingPoints[targetPointIndex].gameObject);
			//Debug.Log("Target changed to: " + pathPoints[targetPointIndex]);
		}
		#endregion

		#region Chasing
		if (currentMonsterState == MonsterState.Chasing)
		{
			if (!MC.IsClose)
			{
				if(currentDelayUntilRoam > 0)
				{
					currentDelayUntilRoam -= Time.deltaTime;
				}
				else
				{
					currentMonsterState = MonsterState.Roaming;
				}
			}
			else
			{
				currentDelayUntilRoam = startDelayUntilRoam;
			}
		}
		#endregion

		base.Update();
	}

	private void FocusOnClosestRoamingPoint()
	{
		Transform closestRoamingPoint;
		List<float> roamingPointDistances = new List<float>();
		Dictionary<float, Transform> roamingPointDistancePoints = new Dictionary<float, Transform>();
		foreach (Transform roamingPoint in roamingPoints)
		{
			roamingPointDistances.Add(Vector2.Distance(transform.position, roamingPoint.position));
			roamingPointDistancePoints.Add(Vector2.Distance(transform.position, roamingPoint.position), roamingPoint);
		}
		closestRoamingPoint = roamingPointDistancePoints[roamingPointDistances.OrderBy(item => item).First()];

		ChangeTarget(closestRoamingPoint.gameObject);
		targetPointIndex = roamingPoints.IndexOf(closestRoamingPoint);

		//Debug.Log(closestRoamingPoint);
	}

	public override Vector2 TargetPoint()
	{
		if(currentMonsterState == MonsterState.Roaming)
			return base.TargetPoint();
		else
		{
			if (MC.IsClose)
			{
				lastMonsterPos = transform.position;
				return Player.transform.position;
			}
			else
				return lastMonsterPos;
		}
	}

	private int IncrementTargetPoint()
	{
		//Debug.Log("Target Point Index: " + targetPointIndex + " vs Path Points Count - 1: " + (pathPoints.Count - 1));
		//Debug.Log(targetPointIndex++);
		targetPointIndex++;
		return targetPointIndex >= roamingPoints.Count - 1 ? 0 : targetPointIndex++;
	}
}
