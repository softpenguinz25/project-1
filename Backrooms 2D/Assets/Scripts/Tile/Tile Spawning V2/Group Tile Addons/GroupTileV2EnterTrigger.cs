using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GroupTileV2EnterTrigger : MonoBehaviour
{
	[SerializeField] bool disableAfterEnter;
	bool hasEntered;
	[SerializeField] UnityEvent enterEvents;
	[SerializeField] UnityEvent exitEvents;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !hasEntered) PlayerEntered();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !hasEntered) PlayerExit();
	}

	public virtual void PlayerEntered()
	{
		if(disableAfterEnter) hasEntered = true;
		enterEvents?.Invoke();
	}

	public virtual void PlayerExit()
	{
		exitEvents?.Invoke();
	}
}
