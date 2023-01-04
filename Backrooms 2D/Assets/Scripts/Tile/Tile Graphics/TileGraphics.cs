using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGraphics : MonoBehaviour
{
	[SerializeField] List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

/*	private IEnumerator Start()
	{
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
		yield return null;
	}*/

	public void ToggleGraphics(bool toggle)
	{
		if(!toggle) spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			spriteRenderer.enabled = toggle;
		}
	}
}
