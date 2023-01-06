using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGraphics : MonoBehaviour
{
	List<Transform> childrenObjects = new List<Transform>();
	[SerializeField] string invisibleToFOV, tileLayer;

/*	private IEnumerator Start()
	{
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
		yield return null;
	}*/

	public void ToggleGraphics(bool toggle)
	{
		if (!toggle) childrenObjects = transform.GetComponentsInChildren<Transform>().ToList();

		foreach (Transform child in childrenObjects)
		{
			if (child == null) continue;
			SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
			if (childSpriteRenderer != null) childSpriteRenderer.enabled = toggle;
			child.gameObject.layer = toggle ? LayerMask.NameToLayer(tileLayer) : LayerMask.NameToLayer(invisibleToFOV);
		}
	}
}
