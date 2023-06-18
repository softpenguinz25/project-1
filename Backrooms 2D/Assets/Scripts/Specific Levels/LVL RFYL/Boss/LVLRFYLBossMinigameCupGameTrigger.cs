using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameCupGameTrigger : LVLRFYLBossMinigameButton
{
	[SerializeField] Animator animator;
	public override void Pressed()
	{
		animator.SetTrigger("Start");
		base.Pressed();
	}

	public override void OnTriggerExit2D(Collider2D collision)
	{
		
	}
}
