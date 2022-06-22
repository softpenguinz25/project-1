using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LVLPoolroomPoolSpeed : MonoBehaviour
{
	public static event Action<GameObject> SlowDown;
	public static event Action<GameObject> SpeedUp;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		SlowDown?.Invoke(collision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		SpeedUp?.Invoke(collision.gameObject);	
	}
}
