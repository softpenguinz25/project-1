using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameReactionTimeGFX : MonoBehaviour
{
	[SerializeField] IntGameObjectDictionary roundGOsGFX;

    [SerializeField] LVLRFYLBossMinigameReactionTime reactionTime;

	[SerializeField] float timeUntilNextRoundInSeconds = 1;

	private void OnEnable()
	{
		reactionTime.Reset += () => ChangeGraphics(1);
		reactionTime.RoundPromoted += ChangeGraphics;
	}

	private void OnDisable()
	{
		reactionTime.RoundPromoted -= ChangeGraphics;
	}

	private void ChangeGraphics(int currentRound)
	{
		StartCoroutine(StartNextRoundCoroutine(currentRound));
	}

	private IEnumerator StartNextRoundCoroutine(int currentRound)
	{
		yield return new WaitForSeconds(timeUntilNextRoundInSeconds);

		for (int i = 1; i <= roundGOsGFX.Count; i++)
			roundGOsGFX[i].SetActive(i == currentRound);

		reactionTime.StartRound();
	}
}
