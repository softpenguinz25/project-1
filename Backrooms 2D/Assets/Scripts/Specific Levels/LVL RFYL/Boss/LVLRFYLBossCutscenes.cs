using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossCutscenes : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(false);
	}

	[SerializeField] Dialogue dialogue;
	public void SetDialogueText(AnimationEvent animationEvent)
	{
		dialogue.SetText(animationEvent);
	}

	[SerializeField] CinematicBlackBars cinematicBlackBars;
	public void SetCinematicBlackBars(AnimationEvent triggerIn)
	{
		if (!triggerIn.stringParameter.Equals("")) cinematicBlackBars.TriggerIn();
		else cinematicBlackBars.TriggerOut();
	}

	[SerializeField] GroupTileV2Camera groupTileCamera;
	public void SetGroupTileCamera(AnimationEvent enable)
	{
		if (!enable.stringParameter.Equals(""))
		{
			if (enable.floatParameter > 0) 
				groupTileCamera.EnableCamera(enable.floatParameter); 
			else 
				groupTileCamera.EnableCamera();
		}
		else 
		{ 
			if (enable.floatParameter > 0) 
				groupTileCamera.DisableCamera(enable.floatParameter); 
			else 
				groupTileCamera.DisableCamera(); 
		}
	}

	[SerializeField] LVLRFYLBossMinigameManager minigameManager;
	public void SetBossAnimatorState(AnimationEvent animState)
	{
		minigameManager.SetAnimatorState(animState);
	}

	public void SetPlayerControls(AnimationEvent playerControlsState)
	{
		if (!playerControlsState.stringParameter.Equals("")) FindObjectOfType<PlayerManager>().EnablePlayerControls();
		else FindObjectOfType<PlayerManager>().DisablePlayerControls();
	}

	public void StartMinigame()
	{
		minigameManager.StartMinigame();
	}

	[SerializeField] Animator cupAnimator;
	public void SetCupAnimator(AnimationEvent state)
	{
		cupAnimator.enabled = !state.stringParameter.Equals("");
	}

	[SerializeField] Flash flash;
	public void TriggerFlash(AnimationEvent flashTime)
	{
		flash.SetFlashLength(flashTime);
		flash.TriggerFlash();
	}

	public void TransitionToNextLevel()
	{
		FindObjectOfType<LevelTransition>().TransitionToNextLevel();
	}
}
