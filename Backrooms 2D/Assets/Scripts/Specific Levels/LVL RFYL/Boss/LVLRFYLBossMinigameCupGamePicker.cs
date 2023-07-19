using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLRFYLBossMinigameCupGamePicker : LVLRFYLBossMinigame
{
	[SerializeField] LVLRFYLBossMinigameCupGameShuffler cupGameShuffler;
	[SerializeField] Animator animator;
	public UnityEvent Restart;
	int processWinTime = 1;
	public void PickCup(int cupNum)
	{
		//print("Pick Cup: " + cupNum + ", Cup Index From Cup Num: " + cupGameShuffler.GetCupIndexFromCupNum(cupNum));
		animator.SetFloat("Cup Picked", cupGameShuffler.GetCupIndexFromCupNum(cupNum));
		animator.SetBool("Choose Correct", cupNum == cupGameShuffler.CorrectCupNum);
		animator.SetTrigger("Choose Cup");
		if (cupNum == cupGameShuffler.CorrectCupNum) WinGame();
		else
		{
			Debug.Log("Wrong Cup: you chose " + cupNum + " while correct was " + cupGameShuffler.CorrectCupNum);
			RestartGame();
		}
		//CupPicked?.Invoke(cupNum == cupGameShuffler.CorrectCupNum);
	}

	public override void RestartGame()
	{
		base.RestartGame();
		Restart?.Invoke();
	}
	public override void WinGame()
	{
		print("WEINER IS YOU");
		foreach (LVLRFYLBossMinigameCupGameCup cup in cupGameShuffler.CupsInOrder) cup.Disable();
		Invoke(nameof(CallBaseWinGame), processWinTime);
	}

	public void CallBaseWinGame()
	{
		base.WinGame();
	}
}
