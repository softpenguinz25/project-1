using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroupTileV2EnterTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player")) PlayerEntered();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player")) PlayerExit();
	}

	public virtual void PlayerEntered()
	{
		
	}

	public virtual void PlayerExit()
	{

	}
}
