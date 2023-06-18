using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameSimonSaysTriggerGO : LVLRFYLBossMinigameButton
{
	[SerializeField] LVLRFYLBossMinigameSimonSays minigameSimonSays;
	[SerializeField] Color normalColor = new Color(68f/255, 68f/255, 68f/255), failColor = new Color(1, 1f / 2, 0);

	public override void Pressed()
	{
		ResetIndication();
		minigameSimonSays.DisplayCode(minigameSimonSays.CodeLengthOfCurrentRound);

		base.Pressed();
	}

	public override void OnTriggerExit2D(Collider2D collision) { }

	public override void Unpressed()
	{
		base.Unpressed();
	}

	private void ResetIndication()
	{
		sr.color = normalColor;
	}

	public void IndicateFail()
	{
		sr.color = failColor;
	}
}
