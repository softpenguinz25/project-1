using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameCupGamePicker : LVLRFYLBossMinigame
{
	[SerializeField] LVLRFYLBossMinigameCupGameShuffler cupGameShuffler;
    public void PickCup(int cupNum)
	{
		if (cupNum == cupGameShuffler.CorrectCupNum) WinGame();
		else
		{
			Debug.Log("Wrong Cup: you chose " + cupNum + " while correct was " + cupGameShuffler.CorrectCupNum);
			RestartGame();
		}
	}

	public override void RestartGame()
	{
		base.RestartGame();
	}
	public override void WinGame()
	{
		print("WEINER IS YOU");
		base.WinGame();
	}
}
