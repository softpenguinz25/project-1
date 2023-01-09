using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGraphics : MonoBehaviour
{
	List<Transform> childrenObjects = new List<Transform>();
	Dictionary<GameObject, int> startingChildLayers = new Dictionary<GameObject, int>();
	[SerializeField] string invisibleToFOV;

/*	private IEnumerator Start()
	{
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
		yield return null;
	}*/

	public void ToggleGraphics(bool toggle)
	{
		if (!toggle)
		{
			childrenObjects = transform.GetComponentsInChildren<Transform>().ToList();
			foreach(Transform child in childrenObjects)
			{
				startingChildLayers.Add(child.gameObject, child.gameObject.layer);
			}
		}

		int i = 0;
		foreach (Transform child in childrenObjects)
		{
			if (child == null) continue;
			SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
			if (childSpriteRenderer != null) childSpriteRenderer.enabled = toggle;
			child.gameObject.layer = toggle ? startingChildLayers[child.gameObject] : LayerMask.NameToLayer(invisibleToFOV);

			i++;
		}
	}
}
