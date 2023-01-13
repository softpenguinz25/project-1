using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : Interact
{	
	private void Update()
	{
#if UNITY_IOS || UNITY_ANDROID
		if (Input.touchCount > 0)
		{
			InteractWithEnvironment(InteractType.Player, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
		}
		else if (Input.GetMouseButtonDown(0))
		{
			//Thanks Kyle Banks! https://kylewbanks.com/blog/unity-2d-detecting-gameobject-clicks-using-raycasts
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;
			//Debug.Log("Mouse Pos 2D: " + mousePos);

			InteractWithEnvironment(InteractType.Player, mousePos);
		}
#else
if (Input.GetKeyDown(KeyCode.E))
		{
			InteractWithEnvironment(InteractType.Player);
		}
#endif
	}
}
