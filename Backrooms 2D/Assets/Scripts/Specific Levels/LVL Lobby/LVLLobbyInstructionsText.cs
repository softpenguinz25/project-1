using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LVLLobbyInstructionsText : MonoBehaviour
{
	[SerializeField] VideoPlayer cutsceneVideo;
	[SerializeField] TextMeshProUGUI instructionsText;
	[SerializeField] string pcInstructions = "Press <b> SPACE </b> to Balance", mobileInstructions = "<b> TAP </b> to Balance";
    [SerializeField] LVLLobbyTransitionToCutscene lobbyTransitionToCutscene;

	private void Start()
	{
#if UNITY_IOS || UNITY_ANDROID
		instructionsText.text = mobileInstructions;
#else
		instructionsText.text = pcInstructions;
#endif
	}

	private void OnEnable()
	{
		instructionsText.color = new Color(instructionsText.color.r, instructionsText.color.g, instructionsText.color.b, 1);
		cutsceneVideo.time = 1 / cutsceneVideo.frameRate;
		cutsceneVideo.playbackSpeed = 0;
	}

	private void Update()
	{
#if UNITY_IOS || UNITY_ANDROID
		if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)) PlayGame();
#else
		if(Input.GetKeyDown(KeyCode.Space)) PlayGame();
#endif
	}

	void PlayGame()
	{
		foreach (LVLLobbyCutscene lobbyCutscene in lobbyTransitionToCutscene.lobbyCutscenes) lobbyCutscene.enabled = true;
		cutsceneVideo.playbackSpeed = 1;
		instructionsText.color = new Color(instructionsText.color.r, instructionsText.color.g, instructionsText.color.b, 0);

		FindObjectOfType<AudioManager>().Play("LVLLobby_Pitfall_Balance");

		enabled = false;
	}
}
