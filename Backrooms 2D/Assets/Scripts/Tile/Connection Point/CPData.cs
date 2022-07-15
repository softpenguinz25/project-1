using MyBox;
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
	//[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private BC2DCollection collidersCheck;
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private LayerMask tileMask;
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private Vector2 boxSize = new Vector2(8, 8);

	[Space]
	[ConditionalField(nameof(checkSpecialCPIsLegal))] [SerializeField] private GameObject wallPrefab;

	private void Awake()
	{
		tm = FindObjectOfType<TileDataManager>();
		thisTilePrefab = transform.root.GetComponent<TilePrefab>();
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(1);

		StartCoroutine(CheckIfCPIsIllegal());
	}

	private IEnumerator CheckIfCPIsIllegal()
	{
		while (true)
		{
			List<Collider2D> colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, tileMask).ToList();
			//collidersCheck.Value = colliders.ToArray();
			
			if (colliders.Count > 0 && colliders.Count < 64)
			{
				DestroyCP();
			}
			yield return new WaitForSeconds(.2f);
		}
	}

	private void DestroyCP()
	{
		tm.connectionPoints.Remove(transform);
		thisTilePrefab.specialCPs.Remove(transform);
		BuildWall();
		Debug.Log("Destroying " + gameObject.name, transform.parent);
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

/*[Serializable]
public class BC2DCollection : CollectionWrapper<Collider2D>{}*/
