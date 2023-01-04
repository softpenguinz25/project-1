using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LVLAbandonedOfficeDoorAddCP : MonoBehaviour
{
	TilePrefab parentTile;
	[SerializeField] Transform cp;
	[SerializeField] LayerMask tileMask;

	private void Awake()
	{
		parentTile = GetComponentInParent<TilePrefab>();

		//set this object as child of tile its currently in
		var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;
		List<Collider2D> overlappingColliders = Physics2D.OverlapCircleAll(transform.position, .1f, tileMask).ToList();
		GameObject tileContainedIn = null;
		foreach(Collider2D col2D in overlappingColliders)
		{
			//Debug.Log(col2D, col2D);
			if (!col2D.transform.IsChildOf(transform.parent) && col2D.transform.IsChildOf(parentTile.transform)) tileContainedIn = col2D.gameObject;
		}
		Physics2D.queriesHitTriggers = old;
		
		transform.parent = tileContainedIn.transform;

		AddCP(cp);
	}

	private void AddCP(Transform cp)
	{
		parentTile.AddCP(cp);
	}
}
