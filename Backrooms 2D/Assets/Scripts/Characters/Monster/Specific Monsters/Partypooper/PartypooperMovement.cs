using UnityEngine;

public class PartypooperMovement : MonsterMovement
{
	[SerializeField] float radiusAroundPlayer;
	[SerializeField] LayerMask wallMask;

	#region Gizmo Debugging
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(CurrentTarget.transform.position, radiusAroundPlayer);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(TargetPoint(), .5f);

		Gizmos.color = Color.white;
		//Gizmos.DrawLine(transform.position, targetPoint, wallMask)
	}
	#endregion

	MonsterChaseGFX mcgfx;

	#region Attacking Partypooper Event
	public override void Awake()
	{
		base.Awake();
		mcgfx = FindObjectOfType<MonsterChaseGFX>();
	}
	private void OnEnable()
	{
		mcgfx.MonsterIsClose += AttackPartygoer;
	}
	private void OnDisable()
	{
		mcgfx.MonsterIsClose -= AttackPartygoer;
	}
	#endregion


	[SerializeField] GameObject partygoerPrefab;
	bool partygoerDetected;
	private void AttackPartygoer(GameObject partygoer)
	{
		if (partygoer.GetComponent<MonsterInfo>().monsterName == partygoerPrefab.GetComponent<MonsterInfo>().monsterName)
		{
			ChangeTarget(partygoer);
			partygoerDetected = true;
		}
	}

	public override Vector2 TargetPoint()
	{
		if (partygoerDetected) return base.TargetPoint();

		Vector2 targetPoint = CurrentTarget.transform.position;
		Vector2 direction = (transform.position - CurrentTarget.transform.position).normalized;
		targetPoint += direction * radiusAroundPlayer;
		RaycastHit2D wallObstructing = Physics2D.Linecast(transform.position, CurrentTarget.transform.position, wallMask);
		
		if (wallObstructing.collider == null) return targetPoint;
		else return base.TargetPoint();
	}
}
