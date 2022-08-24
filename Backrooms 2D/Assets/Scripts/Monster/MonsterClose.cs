using MyBox;
using System;
using UnityEditor;
using UnityEngine;

public class MonsterClose : MonoBehaviour
{
	private MonsterMovement mm;

	[Header("Debugging")]
	[SerializeField] private bool visualizeRay = false;

	[Header("Is Close Parameters")]
	[SerializeField] private LayerMask obstacleMask;
	[SerializeField] private float closeThreshold = 8f;
	[SerializeField] private float farThreshold = 25f;
	private bool hasActivatedCloseEvent;
	//public Collider2D obstrucingObjectsTest;
	public event Action MonsterIsClose;
	public event Action PlayerHasEscaped;

	public bool IsClose
	{
		get
		{
			var old = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = false;

			bool obstructingObjectDetected = Physics2D.Linecast(transform.position, mm.CurrentTarget.transform.position, obstacleMask);

			Physics2D.queriesHitTriggers = old;

			bool playerIsClose = Vector2.Distance(transform.position, mm.CurrentTarget.transform.position) < closeThreshold;
			//Debug.Log("obj detected: " + obstructingObjectDetected + " player is close: " + playerIsClose, this);
			//obstrucingObjectsTest = obstructingObjectDetected.collider;
			return !obstructingObjectDetected && playerIsClose;
		}
	}

	public bool IsFarUnobstructing
	{
		get
		{
			var old = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = false;

			bool obstructingObjectDetected = Physics2D.Linecast(transform.position, mm.CurrentTarget.transform.position, obstacleMask);

			Physics2D.queriesHitTriggers = old;

			bool playerIsClose = Vector2.Distance(transform.position, mm.CurrentTarget.transform.position) >= farThreshold;
			//Debug.Log("obj detected: " + obstructingObjectDetected + " player is close: " + playerIsClose, this);
			//obstrucingObjectsTest = obstructingObjectDetected.collider;
			return !obstructingObjectDetected && playerIsClose;
		}
	}

	public bool IsFarObstructing
	{
		get
		{
			var old = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = false;

			bool obstructingObjectDetected = Physics2D.Linecast(transform.position, mm.CurrentTarget.transform.position, obstacleMask);

			Physics2D.queriesHitTriggers = old;

			bool playerIsClose = Vector2.Distance(transform.position, mm.CurrentTarget.transform.position) >= farThreshold;
			//Debug.Log("obj detected: " + obstructingObjectDetected + " player is close: " + playerIsClose, this);
			//obstrucingObjectsTest = obstructingObjectDetected.collider;
			return obstructingObjectDetected && playerIsClose;
		}
	}
	
	private void Awake()
	{
		mm = GetComponent<MonsterMovement>();
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		if (visualizeRay)
		{
			Vector3 rayDir = mm.CurrentTarget.transform.position - transform.position;
			if(IsClose) Debug.DrawRay(transform.position, rayDir, Color.red);
			else if (GetComponent<MonsterMovement>().isSlow)Debug.DrawRay(transform.position, rayDir, Color.yellow);
			else if(IsFarUnobstructing) Debug.DrawRay(transform.position, rayDir, Color.white);
			else if(IsFarObstructing) Debug.DrawRay(transform.position, rayDir, Color.gray);
			else Debug.DrawRay(transform.position, rayDir, Color.cyan);
		}
	}

	private void Update()
	{
		if (IsClose)//If nothing is between monster and player
		{
			//Debug.Log("Is close!");
			if (hasActivatedCloseEvent)
			{
				MonsterIsClose?.Invoke();
				hasActivatedCloseEvent = false;
			}
		}
		else
		{
			//Debug.Log("Far away :(");
			if (!hasActivatedCloseEvent)
			{
				PlayerHasEscaped?.Invoke();
				hasActivatedCloseEvent = true;
			}
		}
	}
}
