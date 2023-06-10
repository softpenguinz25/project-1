using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LVLAbandonedOfficeDoorData : MonoBehaviour, IBigTile
{
	TilePrefab parentTile;
	#region Instructions
	[SerializeField] TextMeshPro instructionsText;
	[SerializeField] string PCInstructions = "'E' to \nOpen", mobileInstructions = "Tap to \noOpen";
	static bool instructionsShown = false;

	IEnumerator Start()
	{
#if UNITY_IOS || UNITY_ANDROID
		instructionsText.text = mobileInstructions;
#else
		instructionsText.text = PCInstructions;
#endif

		yield return new WaitForSeconds(1f);

		if (instructionsShown || !isFunctioning) yield break;

		instructionsText.gameObject.SetActive(true);
		instructionsText.transform.eulerAngles = Vector3.zero;
		
		instructionsShown = true;
	}
#endregion

#region Door CP
	[Header("Door CP")]
	[SerializeField] Transform cp;
	[SerializeField] LayerMask tileMask;
	[SerializeField] SpriteRenderer midPiece;
	[MustBeAssigned] [SerializeField] Sprite defaultSprite;
	[MustBeAssigned] [SerializeField] Sprite doorSprite;
	bool isSkinned;
	public bool IsSkinned => isSkinned;

	private void Awake()
	{
		midPiece.sprite = defaultSprite;

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

	public void BigTilePlaced(Transform referenceCP, Transform spawnedCP, bool isNewTile)
	{
		midPiece.sprite = doorSprite;
		isSkinned = true;
		
		if(!isNewTile) ConnectDoors(spawnedCP);
	}
#endregion

#region Door Functionality Detection
	[Header("Door Functionality Detection")]
	[SerializeField] GameObject doorParentPrefab;
	[SerializeField] Transform doorParentPivot;
	[SerializeField] Collider2D otherDoorColliderCheck;
	bool isFunctioning;
	//LVLAbandonedOfficeDoorData otherDoor;
	public bool IsFunctioning => isFunctioning;
	private void ConnectDoors(Transform spawnedCP)
	{
		/*var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;
		List<Collider2D> overlappingColliders = new List<Collider2D>();
		Physics2D.queriesHitTriggers = old;

		ContactFilter2D contactFilter = new ContactFilter2D();
		Physics2D.OverlapCollider(otherDoorColliderCheck, contactFilter, overlappingColliders);

		LVLAbandonedOfficeDoorData otherDoor = null;
		foreach (Collider2D col in overlappingColliders)
		{
			if (col.GetComponent<LVLAbandonedOfficeDoorData>() != null && !col.transform.IsChildOf(transform)) otherDoor = col.GetComponent<LVLAbandonedOfficeDoorData>();
		}
		this.otherDoor = otherDoor;

		isFunctioning = true;
		otherDoor.isFunctioning = true;*/

		LVLAbandonedOfficeDoorData otherDoor = spawnedCP.parent.GetComponent<LVLAbandonedOfficeDoorData>();

		GameObject doorParentObject = Instantiate(doorParentPrefab, doorParentPivot.position, Quaternion.Euler(transform.eulerAngles), transform.parent);
		midPiece.transform.parent = doorParentObject.transform;
		otherDoor.midPiece.transform.parent = doorParentObject.transform;

		isFunctioning = true;
	}

	/*private IEnumerator Start()
	{
		yield return new WaitForSeconds(.5f);	

		//Debug.Log("Detecting if any doors nearby...", gameObject);
		/*var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;
		List<Collider2D> overlappingColliders = Physics2D.OverlapCollider(otherDoorColliderCheck, .1f, tileMask).ToList();
		Physics2D.queriesHitTriggers = old;*//*

		List<Collider2D> overlappingColliders = new List<Collider2D>();
		ContactFilter2D contactFilter = new ContactFilter2D();
		Physics2D.OverlapCollider(otherDoorColliderCheck, contactFilter, overlappingColliders);

		//TODO: NOTHING IS GETTING PAST THIS CHECK!? WHY!? HAS I EVER!?
		LVLAbandonedOfficeDoorData otherDoor = null;
		foreach (Collider2D col in overlappingColliders)
		{
			//if(isSkinned && !isFunctioning) Debug.Log(col.gameObject.name, gameObject);
			//Debug.Log(col.GetComponent<LVLAbandonedOfficeDoor>());
			//Debug.Log(col.transform.IsChildOf(transform));
			if (col.GetComponent<LVLAbandonedOfficeDoorData>() != null && !col.transform.IsChildOf(transform)) otherDoor = col.GetComponent<LVLAbandonedOfficeDoorData>();
		}
		//Debug.Log("Other Door: " + otherDoor, otherDoor);
		//If door is already functioning, don't run parent code
		//Debug.Log(otherDoor + "" + isFunctioning + isSkinned + otherDoor.isFunctioning + otherDoor.isSkinned, gameObject);
		this.otherDoor = otherDoor;
		if (otherDoor == null || isFunctioning || !isSkinned || otherDoor.IsFunctioning *//*|| !otherDoor.IsSkinned*//*)
		{
			StartCoroutine(Start());
			yield break;
		}
		//Debug.Log("This Door: " + gameObject.name, gameObject);
		//Debug.Log("Other Door: " + otherDoor.name, otherDoor);

		isFunctioning = true;
		otherDoor.isFunctioning = true;

		GameObject doorParentObject = Instantiate(doorParentPrefab, doorParentPivot.position, Quaternion.identity, transform.parent);
		midPiece.transform.parent = doorParentObject.transform;
		otherDoor.midPiece.transform.parent = doorParentObject.transform;
	}*/
#endregion

	/*[ContextMenu("Print Other Door Data")]
	public void PrintOtherDoorData()
	{
		Debug.Log(otherDoor + "" + isFunctioning + isSkinned + otherDoor.isFunctioning + otherDoor.isSkinned, gameObject);
	}*/
}
