using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameReactionTimeInput : LVLRFYLBossMinigameButton
{
    [SerializeField] LVLRFYLBossMinigameReactionTime reactionTime;

	public override void Pressed()
	{
		reactionTime.StopCursor();
		base.Pressed();
	}

	public override void Unpressed()
	{
		base.Unpressed();
	}
}
