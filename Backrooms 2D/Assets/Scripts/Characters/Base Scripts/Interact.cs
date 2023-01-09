using UnityEngine;

public class Interact : MonoBehaviour
{
	public enum InteractType
	{
		Player,
		Entity
	}

	[SerializeField] LayerMask interactLayer;
	[SerializeField] float reachRadius;
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, reachRadius);
	}

	public void InteractWithEnvironment(InteractType interactType)
	{
		var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = true;

		foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, reachRadius, interactLayer))
		{
			//Debug.Log(col.gameObject.name, col);
			IInteract interactObject = col.GetComponent<IInteract>();
			if (interactObject != null)
			{
				switch (interactType) {
					case InteractType.Player:
						interactObject.PlayerInteract(gameObject);
						break;
					case InteractType.Entity:
						interactObject.EntityInteract(gameObject);
						break;
				}
			}
		}

		Physics2D.queriesHitTriggers = old;
	}
}
