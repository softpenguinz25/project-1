using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLRFYLBossMinigame : MonoBehaviour
{
	public UnityEvent personalWinEvents;
	[SerializeField] LVLRFYLBossMinigameManager minigameManager;
    public virtual void RestartGame()
	{
		FindObjectOfType<AudioManager>().Play("LVLRFYL_Minigame_Fail");
	}
	public virtual void WinGame()
	{
		personalWinEvents?.Invoke();
		FindObjectOfType<AudioManager>().Play("LVLRFYL_Minigame_Win");
		minigameManager.EndMinigame();
	}

	public void PlaySFX(AnimationEvent animationEvent)
	{
		//Debug.Log("sfx played: " + animationEvent.stringParameter, this);
		FindObjectOfType<AudioManager>().Play(animationEvent.stringParameter);
	}
}
