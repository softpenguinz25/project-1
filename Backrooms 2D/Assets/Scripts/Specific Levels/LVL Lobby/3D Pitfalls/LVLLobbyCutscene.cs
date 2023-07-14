using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LVLLobbyCutscene : MonoBehaviour
{
	public LVLLobbyTransitionToCutscene lvlLobbyTransitionToCutscene;
	public GameObject cutsceneVideoGO;
	public VideoPlayer cutsceneVideo;

	public virtual void DisableCutscene(bool didWin)
	{
		enabled = false;
	}
}
