using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLAbandonedOfficeDivider : MonoBehaviour
{
	Collider2D dividerCollider;

	private void Awake()
	{
		dividerCollider = GetComponent<Collider2D>();
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(2.5f);

		CheckOverlappingObjects();
	}

	[ContextMenu("Check Overlapping Objects")]
	void CheckOverlappingObjects()
	{
		List<Collider2D> overlappingColliders = new List<Collider2D>();
		List<Collider2D> validOverlappingColliders = new List<Collider2D>();
		ContactFilter2D contactFilter = new ContactFilter2D();
		Physics2D.OverlapCollider(dividerCollider, contactFilter, overlappingColliders);
		foreach (Collider2D col in overlappingColliders)
		{
			if(col.GetComponent<LVLAbandonedOfficeDivider>() != null) validOverlappingColliders.Add(col);
		}

		/*foreach (Collider2D col in validOverlappingColliders)
		{
			Debug.Log(col.gameObject.name, col);
		}*/

		if (validOverlappingColliders.Count <= 0) /*yield break*/return;

		Destroy(gameObject);
	}
}
