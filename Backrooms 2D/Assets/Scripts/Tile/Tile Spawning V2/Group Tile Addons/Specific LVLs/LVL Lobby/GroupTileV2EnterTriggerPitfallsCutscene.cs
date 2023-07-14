using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2EnterTriggerPitfallsCutscene : GroupTileV2EnterTrigger
{
	//[SerializeField] Transform playerFailPos, playerWinPos;

	[SerializeField] LVLLobbyPitfallsPlayerInAnim inAnim;
	//LVLLobbyTransitionToCutscene lvlLobbyTransitionIntoCutscene;

	private void Awake()
	{
		//lvlLobbyTransitionIntoCutscene = FindObjectOfType<LVLLobbyTransitionToCutscene>();
	}

	public override void PlayerEntered()
	{
		base.PlayerEntered();
		inAnim.gameObject.SetActive(true);
		//lvlLobbyTransitionIntoCutscene.StartTransitionIntoCutscene(playerFailPos, playerWinPos);
	}
}
