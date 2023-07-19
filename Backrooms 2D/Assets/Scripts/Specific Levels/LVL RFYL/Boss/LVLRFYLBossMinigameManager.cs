using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameManager : MonoBehaviour
{
	[SerializeField] Animator cutsceneAnim;
	[SerializeField] Animator bossAnim;
	[SerializeField] Animator miniGameAnim;
	int bossCutscene, currentMinigame;
    public void SetAnimatorState(AnimationEvent enabled)
	{
		bossAnim.enabled = !enabled.stringParameter.Equals("");
	}
	public void StartMinigame()
	{
		currentMinigame++;
		miniGameAnim.SetInteger("Minigame Number", currentMinigame);
		miniGameAnim.SetTrigger("Start Minigame");
	}
	public void EndMinigame()
	{
		miniGameAnim.SetTrigger("End Minigame");
		bossCutscene++;
		//print("boss cutscene: " + bossCutscene);
		cutsceneAnim.SetInteger("Boss Cutscene", bossCutscene);
		cutsceneAnim.SetTrigger("Play Cutscene");
	}
}
