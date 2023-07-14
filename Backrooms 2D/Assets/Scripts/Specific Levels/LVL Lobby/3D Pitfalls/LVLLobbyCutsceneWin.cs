using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLLobbyCutsceneWin : LVLLobbyCutscene
{
	bool isWinning;

    [SerializeField] List<LVLLobbyCutscene> cutsceneScriptsToChangeState;

	private void Update()
	{
		if (cutsceneVideo.clockTime < cutsceneVideo.length)
		{
			isWinning = false;
			foreach(LVLLobbyCutscene lobbyCutscene in cutsceneScriptsToChangeState) lobbyCutscene.enabled = true;
			return;
		}
		if (isWinning) return;

		isWinning = true;
		WinCutscene();

	}

	private void WinCutscene()
	{
		foreach (LVLLobbyCutscene lobbyCutscene in cutsceneScriptsToChangeState) lobbyCutscene.DisableCutscene(true);
		lvlLobbyTransitionToCutscene.StartTransitionOutOfCutscene(true);
	}
}
