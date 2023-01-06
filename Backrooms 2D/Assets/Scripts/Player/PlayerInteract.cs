using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
	[SerializeField] LayerMask interactLayer;
	[SerializeField] float reachRadius;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, reachRadius);
	}

	private void Update()
	{
		//IInteract interactObject = collision.GetComponent<IInteract>();
		//if (Input.GetKeyDown(KeyCode.E)) Debug.Log(collision.gameObject.name, collision);
		if (Input.GetKeyDown(KeyCode.E))
		{
			var old = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = true;

			foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, reachRadius, interactLayer))
			{
				//Debug.Log(col.gameObject.name, col);
				IInteract interactObject = col.GetComponent<IInteract>();
				if (interactObject != null) interactObject.Interact(gameObject);
			}

			Physics2D.queriesHitTriggers = old;
		}
	}
}
