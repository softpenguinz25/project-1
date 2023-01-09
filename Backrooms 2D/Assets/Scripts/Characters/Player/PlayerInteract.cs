using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : Interact
{	
	private void Update()
	{
		//IInteract interactObject = collision.GetComponent<IInteract>();
		//if (Input.GetKeyDown(KeyCode.E)) Debug.Log(collision.gameObject.name, collision);
		if (Input.GetKeyDown(KeyCode.E))
		{
			InteractWithEnvironment(InteractType.Player);
		}
	}
}
