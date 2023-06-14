using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2EnterTriggerPitfallsCutscene : GroupTileV2EnterTrigger
{
	[SerializeField] Transform playerFailPos, playerWinPos;
	LVLLobbyTransitionToCutscene lvlLobbyTransitionIntoCutscene;

	private void Awake()
	{
		lvlLobbyTransitionIntoCutscene = FindObjectOfType<LVLLobbyTransitionToCutscene>();
	}

	public override void PlayerEntered()
	{
		lvlLobbyTransitionIntoCutscene.StartTransitionIntoCutscene(playerFailPos, playerWinPos);
	}
}
