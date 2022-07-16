using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CPData : MonoBehaviour
{
	private TileDataManager tm;
	private TilePrefab thisTilePrefab;

    public TileCollection tileCollection;

	[Space]
	[SerializeField] private bool checkSpecialCPIsLegal;
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private LayerMask tileMask;
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private Vector2 boxSize = new Vector2(8, 8);

	[Space]
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private GameObject wallPrefab;
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private BC2DCollection collidersCheck;
	//[ConditionalField(nameof(checkSpecialCPIsLegal))] public float testNumOverlappingTiles;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
		thisTilePrefab = gameObject.GetComponentInParent<TilePrefab>(true);
	}

	/*private IEnumerator Start()
	{
		yield return new WaitForSeconds(1);

		StartCoroutine(CheckIfCPIsIllegal());
	}*/

	private void OnEnable()
	{
		StartCoroutine(CheckIfCPIsIllegal());
	}

	private IEnumerator CheckIfCPIsIllegal()
	{
		yield return new WaitForSeconds(1);

		while (true)
		{
			List<Collider2D> colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, tileMask).ToList();
			List<Collider2D> illegalColliders = new List<Collider2D>();
			foreach(Collider2D col in colliders)
			{
				if (col.gameObject.GetComponent<TilePrefab>() == null) illegalColliders.Add(col);			
			}
			foreach(Collider2D illegalCol in illegalColliders)
			{
				colliders.Remove(illegalCol);
			}
			collidersCheck.Value = colliders.ToArray();
			//testNumOverlappingTiles = colliders.Count;
			//collidersCheck.Value = colliders.ToArray();

			bool buildWall = false;
			if (colliders.Count < 64) buildWall = true;
			else
			{
				foreach (Collider2D col in colliders)
				{
					if (col.transform.parent.name != "-TILE AREA-") buildWall = true;
				}
			}

			//Debug.Log(gameObject.name + ": " + colliders.Count + " tiles detected.", this);
			if (colliders.Count > 0/* && colliders.Count < 64*/)
			{
				DestroyCP(buildWall);
			}
			yield return new WaitForSeconds(.2f);
		}
	}

	private void DestroyCP(bool buildWall)
	{
		tm.connectionPoints.Remove(transform);
		thisTilePrefab.specialCPs.Remove(transform);
		if(buildWall)
			BuildWall();
		//Debug.Log("Destroying " + gameObject.name, transform.parent);
		Destroy(gameObject);
	}

	private void BuildWall()
	{
		Vector3 eulerAngles = Mathf.Abs(transform.localPosition.x) > .01f ? new Vector3(0, 0, 90) : Vector3.zero;
		GameObject wall = Instantiate(wallPrefab, transform.position, Quaternion.identity, transform.parent);
		wall.transform.localPosition *= .494140375f;
		wall.transform.localEulerAngles = eulerAngles;
	}
}

[Serializable]
public class BC2DCollection : CollectionWrapper<Collider2D> { }
