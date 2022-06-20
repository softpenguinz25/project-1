using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorArrow : MonoBehaviour
{
	private ExitDoorSpawner eds;

	private GameObject exitDoor;

	private const float angleOffset = 270;
	[SerializeField] private float screenBoundsIndent = 8;

	private Vector2 screenSize;
	private void Awake()
	{
		eds = FindObjectOfType<ExitDoorSpawner>();
	}

	private void OnEnable()
	{
		eds.ExitDoorSpawned += (exitDoorObj) =>
		{
			gameObject.SetActive(true);
			exitDoor = exitDoorObj;
		};		
	}

	private void Start()
	{
		gameObject.SetActive(false);
		screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
	}

	private void Update()
	{
		if(exitDoor == null)
		{
			Debug.LogError("Exit door could not be detected!");
			return;
		}

		//Position
		Vector3 newPos = exitDoor.transform.position;		
		Vector3 cameraPos = Camera.main.transform.position;
		newPos.x = Mathf.Clamp(newPos.x, cameraPos.x + (-screenSize.x * .5f + screenBoundsIndent), cameraPos.x + (screenSize.x * .5f - screenBoundsIndent));
		newPos.y = Mathf.Clamp(newPos.y, cameraPos.y + (-screenSize.y * .5f + screenBoundsIndent), cameraPos.y + (screenSize.y * .5f - screenBoundsIndent));

		transform.position = newPos;

		//Rotation
		//THANKS Kastenessen! https://answers.unity.com/questions/1350050/how-do-i-rotate-a-2d-object-to-face-another-object.html
		//THANKS sean 244! https://answers.unity.com/questions/1592029/how-do-you-make-enemies-rotate-to-your-position-in.html
		Vector3 targetPos = exitDoor.transform.position;
		Vector2 direction = targetPos - transform.position;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(Vector3.forward * (angle + angleOffset));
	}
}
