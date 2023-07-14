using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] UnityEvent EnableEvents, DisableEvents;
    public void EnablePlayer()
	{
		EnableEvents?.Invoke();
		FindObjectOfType<Joystick>().Enabled = true;
	}

	public void DisablePlayer()
	{
		DisableEvents?.Invoke();
		FindObjectOfType<Joystick>().Enabled = false;
	}
}
