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

	public event Action MonsterIsClose;
	public event Action PlayerHasEscaped;

	private bool IsClose
	{
		get
		{
			Vector3 rayDir = player.transform.position - transform.position;
			return !Physics2D.Raycast(transform.position, rayDir, rayDir.sqrMagnitude, tileMask) && Vector2.Distance(transform.position, player.transform.position) < distanceBtwnCloseActivate;
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
			Debug.DrawRay(transform.position, rayDir, Color.cyan, rayDir.sqrMagnitude);
		}
	}

	private void Update()
	{
		Vector3 rayDir = player.transform.position - transform.position;;
		if (IsClose)//If nothing is between monster and player
		{
			if (hasActivatedCloseEvent)
			{
				MonsterIsClose?.Invoke();
				hasActivatedCloseEvent = false;
			}
		}
		else
		{
			if (!hasActivatedCloseEvent)
			{
				PlayerHasEscaped?.Invoke();
				hasActivatedCloseEvent = true;
			}
		}
	}
}
