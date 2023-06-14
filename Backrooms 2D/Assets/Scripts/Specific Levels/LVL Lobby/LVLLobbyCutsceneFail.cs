using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLLobbyCutsceneFail : LVLLobbyCutscene
{
	bool isFailing;

	[SerializeField] float angleThreshold = 80;

	[SerializeField] List<LVLLobbyCutscene> cutsceneScriptsToChangeState;
	private void Update()
	{
		//print(cutsceneVideo.transform.eulerAngles.z % 360);
		if (Mathf.Abs(cutsceneVideoGO.transform.eulerAngles.z % 360) <= angleThreshold || Mathf.Abs((360 - cutsceneVideoGO.transform.eulerAngles.z) % 360) <= angleThreshold)
		{
			isFailing = false;
			foreach (LVLLobbyCutscene lobbyCutscene in cutsceneScriptsToChangeState) lobbyCutscene.enabled = true;
			return;
		}
		if (isFailing) return;

		isFailing = true;
		FailCutscene();
	}

	private void FailCutscene()
	{
		//print("starting fail cutsceneon");
		foreach (LVLLobbyCutscene lobbyCutscene in cutsceneScriptsToChangeState) lobbyCutscene.DisableCutscene(false);
		FindObjectOfType<AudioManager>().Play("LVLLobby_Pitfall_Fall");
		lvlLobbyTransitionToCutscene.StartTransitionOutOfCutscene(false);
	}
}
