using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameCupGameCup : LVLRFYLBossMinigame
{
	[Header("References")]
	[SerializeField] LVLRFYLBossMinigameCupGamePicker cupGamePicker;
	[SerializeField] Collider2D col;
	[SerializeField] Animator animator;

	[Header("Functionality")]
	[SerializeField] Transform ballPos;
	int cupNum;

	public Transform BallPos { get => ballPos; set => ballPos = value; }
	public int CupNum { get => cupNum; set => cupNum = value; }

	private void Start()
	{
		Disable();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			animator.SetTrigger("Open");
			cupGamePicker.PickCup(CupNum);
		}
	}

	public void Enable()
	{
		col.isTrigger = true;
	}
	public void Disable()
	{
		col.isTrigger = false;
	}
}
