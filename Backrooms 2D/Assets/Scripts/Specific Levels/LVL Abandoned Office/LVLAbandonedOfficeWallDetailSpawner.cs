using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLAbandonedOfficeWallDetailSpawner : MonoBehaviour
{
	enum DetailType{
		Forward,
		Right,
		Backward,
		Left
	}

	[Header("Setup")]
    [SerializeField] Transform detailPlacerPoint;
    [SerializeField] [Range(0, 1)] float placeDetailProbability = .5f;

	[Header("Checks")]
	[SerializeField] Collider2D overlapCheckCollider;

	[Header("Spawning")]
	[SerializeField] DecorationObject cubicle;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(Random.Range(.4f, .6f));

		if (Random.value > placeDetailProbability) yield break;

		List<Collider2D> overlappingColliders = new List<Collider2D>();
		List<Collider2D> validOverlappingColliders = new List<Collider2D>();
		ContactFilter2D contactFilter = new ContactFilter2D();
		Physics2D.OverlapCollider(overlapCheckCollider, contactFilter, overlappingColliders);
		foreach(Collider2D col in overlappingColliders)
		{
			if (!col.transform.IsChildOf(transform) && col.gameObject.layer != LayerMask.NameToLayer("Invisible Tile") /*&& !(col.GetComponent<LVLAbandonedOfficeWallDetailSpawner>() || col.transform.parent.GetComponent<LVLAbandonedOfficeWallDetailSpawner>())*/) validOverlappingColliders.Add(col);
		}

		if(validOverlappingColliders.Count > 0) yield break;

		PlaceDetail(cubicle);
	}

	private void PlaceDetail(DecorationObject decoObj)
	{
		switch (GetDetailType())
		{
			case DetailType.Forward: Instantiate(decoObj.forwardObj, detailPlacerPoint.position, Quaternion.identity, detailPlacerPoint); break;
			case DetailType.Right: Instantiate(decoObj.rightObj, detailPlacerPoint.position, Quaternion.identity, detailPlacerPoint); break;
			case DetailType.Backward: Instantiate(decoObj.backwardObj, detailPlacerPoint.position, Quaternion.identity, detailPlacerPoint); break;
			case DetailType.Left: Instantiate(decoObj.leftObj, detailPlacerPoint.position, Quaternion.identity, detailPlacerPoint); break;
		}
	}

	DetailType GetDetailType()
	{
		if (Mathf.Abs(HelperMethods.GetPositiveAngle(transform.eulerAngles.z)) < 1) return DetailType.Forward;
		else if (Mathf.Abs(HelperMethods.GetPositiveAngle(transform.eulerAngles.z) - 90) < 1) return DetailType.Right;
		else if (Mathf.Abs(HelperMethods.GetPositiveAngle(transform.eulerAngles.z) - 180) < 1) return DetailType.Backward;
		else if (Mathf.Abs(HelperMethods.GetPositiveAngle(transform.eulerAngles.z) - 270) < 1) return DetailType.Left;
		else
		{
			Debug.Log("Could not detect detail type!");
			return DetailType.Forward;
		}
	}
}
