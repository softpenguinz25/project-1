using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLRFYLBossMinigameReactionTimeGFX : MonoBehaviour
{
	[SerializeField] IntGameObjectDictionary roundGOsGFX;

    [SerializeField] LVLRFYLBossMinigameReactionTime reactionTime;

	[SerializeField] float timeUntilNextRoundInSeconds = 1;

	public UnityEvent NextRoundStarted;

	private void OnEnable()
	{
		//reactionTime.Reset += () => ChangeGraphics(1);
		reactionTime.RoundPromoted += ChangeGraphics;
	}

	private void OnDisable()
	{
		reactionTime.RoundPromoted -= ChangeGraphics;
	}

	public void ChangeGraphics(int currentRound)
	{
		FindObjectOfType<AudioManager>().Play("LVLRFYL_Minigame_Progress");
		StartCoroutine(StartNextRoundCoroutine(currentRound));
	}

	private IEnumerator StartNextRoundCoroutine(int currentRound)
	{
		yield return new WaitForSeconds(timeUntilNextRoundInSeconds);

		for (int i = 1; i <= roundGOsGFX.Count; i++)
			roundGOsGFX[i].SetActive(i == currentRound);

		reactionTime.StartRound();

		NextRoundStarted?.Invoke();
	}
}
