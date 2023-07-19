using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLRFYLBossMinigameReactionTime : LVLRFYLBossMinigame
{
	bool inWindow;
	int currentRound = 1;
	[SerializeField] int finalRound = 3;
	[SerializeField] Animator animator;

	public UnityEvent Reset;
	public event Action<int> RoundPromoted;
	public event Action Win;
	public void SetCurrentRound(AnimationEvent v)
	{
		//print("Set current round: " + v.intParameter);
		if ((int)(v.intParameter / Math.Pow(10, Math.Floor(Math.Log10(v.intParameter)))) != currentRound) return;

		inWindow = v.intParameter % 10 == 1;
		print(inWindow);
	}

	public void StopCursor()
	{
		animator.SetFloat("Speed", 0);

		if (!inWindow) RestartGame();
		else
		{
			currentRound++;//print(currentRound);
			if (currentRound > finalRound) WinGame();
			else RoundPromoted?.Invoke(currentRound);
		}
	}

	public void StartRound()
	{
		animator.SetFloat("Offset", 0);
		animator.SetFloat("Speed", 1);
	}

	public void StopRound()
	{
		animator.SetFloat("Speed", 1);
		animator.SetFloat("Offset", 1);
	}

	public override void RestartGame()
	{
		//print("ur bad");
		base.RestartGame();

		currentRound = 1;
		Reset?.Invoke();
	}

	public override void WinGame()
	{
		print("u r weiner");
		base.WinGame();

		Win?.Invoke();
	}
}
