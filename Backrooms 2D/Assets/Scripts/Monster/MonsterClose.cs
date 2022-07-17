using System;
using UnityEngine;

public class MonsterClose : MonoBehaviour
{
	private GameObject player;

	private MonsterMovement mm;

	[Header("Debugging")]
	[SerializeField] private bool visualizeRay = false;

	[Header("Is Close Parameters")]
	[SerializeField] private LayerMask tileMask;
	[SerializeField] private float distanceBtwnCloseActivate = 8f;
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

			bool obstructingObjectDetected = Physics2D.Linecast(transform.position, player.transform.position, tileMask);

			Physics2D.queriesHitTriggers = old;

			bool playerIsClose = Vector2.Distance(transform.position, player.transform.position) < distanceBtwnCloseActivate;
			//Debug.Log("obj detected: " + obstructingObjectDetected + " player is close: " + playerIsClose, this);
			//obstrucingObjectsTest = obstructingObjectDetected.collider;
			return !obstructingObjectDetected && playerIsClose;
		}
	}

	private void Awake()
	{
		player = FindObjectOfType<PlayerMovement>().gameObject;

		mm = GetComponent<MonsterMovement>();
	}

	private void OnDrawGizmos()
	{
		if (visualizeRay)
		{
			Vector3 rayDir = player.transform.position - transform.position;
			if(IsClose) Debug.DrawRay(transform.position, rayDir, Color.red);
			else if (GetComponent<MonsterMovement>().isSlow)Debug.DrawRay(transform.position, rayDir, Color.yellow);
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
