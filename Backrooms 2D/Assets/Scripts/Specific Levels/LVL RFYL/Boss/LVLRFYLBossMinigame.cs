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

	}
	public virtual void WinGame()
	{
		personalWinEvents?.Invoke();
		minigameManager.EndMinigame();
	}
}
