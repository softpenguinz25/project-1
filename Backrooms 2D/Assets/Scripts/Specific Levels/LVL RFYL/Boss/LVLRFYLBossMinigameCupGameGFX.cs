using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameCupGameGFX : LVLRFYLBossMinigame
{
	[Header("References")]
    [SerializeField] LVLRFYLBossMinigameCupGameShuffler cupGameShuffler;
	bool isShuffling;

	[Header("GFX")]
	[SerializeField] GameObject ball;

	private void Start()
	{
		isShuffling = cupGameShuffler.IsShuffling;
	}

	private void Update()
	{
		if (isShuffling == cupGameShuffler.IsShuffling) return;
		isShuffling = cupGameShuffler.IsShuffling;

		UpdateGFX(isShuffling);
	}

	private void UpdateGFX(bool isShuffling)
	{
		ball.SetActive(!isShuffling);
		switch (isShuffling)
		{
			case true:
				foreach (LVLRFYLBossMinigameCupGameCup cup in cupGameShuffler.CupsInOrder) cup.Disable();
				break;
			case false:
				foreach (LVLRFYLBossMinigameCupGameCup cup in cupGameShuffler.CupsInOrder) cup.Enable();
				ball.transform.position = cupGameShuffler.GetCupFromCupNum(cupGameShuffler.CorrectCup).BallPos.position;
				break;
		}
	}
}
