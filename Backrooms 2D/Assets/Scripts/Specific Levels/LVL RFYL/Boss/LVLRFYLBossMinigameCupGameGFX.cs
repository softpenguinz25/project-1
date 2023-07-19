using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameCupGameGFX : LVLRFYLBossMinigame
{
	[Header("References")]
	[SerializeField] LVLRFYLBossMinigameCupGameShuffler cupGameShuffler;
	//bool isShuffling;

	[Header("GFX")]
	[SerializeField] Animator shuffleAnim;
	[SerializeField] GameObject ballProxy;

	/*private void Start()
	{
		isShuffling = cupGameShuffler.IsShuffling;
	}

	private void Update()
	{
		if (isShuffling == cupGameShuffler.IsShuffling) return;
		isShuffling = cupGameShuffler.IsShuffling;

		UpdateGFX(isShuffling);
	}*/

	private void OnEnable()
	{
		cupGameShuffler.IsShuffling += UpdateGFX;
	}

	private void OnDisable()
	{
		cupGameShuffler.IsShuffling -= UpdateGFX;
	}

	public void UpdateGFX(bool isShuffling)
	{
		ballProxy.SetActive(!isShuffling);
		switch (isShuffling)
		{
			case true:
				foreach (LVLRFYLBossMinigameCupGameCup cup in cupGameShuffler.CupsInOrder) cup.Disable();
				break;
			case false:
				foreach (LVLRFYLBossMinigameCupGameCup cup in cupGameShuffler.CupsInOrder)
				{
					print("ball pos: " + cup.BallPos.position);
					cup.Enable();
				}
				print("setting ball pos to cup" + cupGameShuffler.GetCupIndexFromCupNum(cupGameShuffler.CorrectCup) + " | pos: " + cupGameShuffler.CupsInOrder[cupGameShuffler.GetCupIndexFromCupNum(cupGameShuffler.CorrectCup) - 1].BallPos.position);
				ballProxy.transform.position = cupGameShuffler.CupsInOrder[cupGameShuffler.GetCupIndexFromCupNum(cupGameShuffler.CorrectCup) - 1].BallPos.position;
				//shuffleAnim.SetTrigger("Shuffle Complete");
				//shuffleAnim.SetFloat("Ball Pos", cupGameShuffler.GetCupIndexFromCupNum(cupGameShuffler.CorrectCupNum));
				break;
		}
	}

	public void PlaySFXGFX(AnimationEvent animationEvent)
	{
		PlaySFX(animationEvent);
	}
}
