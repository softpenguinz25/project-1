using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLLobbyTransitionToCutscene : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] string fadeInAnimatorTriggerName = "Fade In", fadeOutAnimatorTriggerName = "Fade Out";

	[SerializeField] GameObject transitionGO;
	public List<LVLLobbyCutscene> lobbyCutscenes;
	[SerializeField] LVLLobbyInstructionsText lobbyInstructionsText;
	[SerializeField] GameObject videoTransitionGO;

	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] Joystick playerJoystick;
	float originalPlayerSpeed;

	Transform playerFailPos, playerWinPos;

	public event Action TransitionedOutOfCutscene;

	private void Start()
	{
		originalPlayerSpeed = playerMovement.CurrentSpeed;
	}

	public void StartTransitionIntoCutscene(Transform playerFailPos, Transform playerWinPos)
	{
		animator.SetTrigger(fadeInAnimatorTriggerName);

		this.playerFailPos = playerFailPos;
		this.playerWinPos = playerWinPos;
	}

	public void DisablePlayerMovement()
	{
		playerMovement.ChangeSpeed(0);
		playerJoystick.Enabled = false;
	}

    public void TransitionIntoCutscene()
	{
		playerMovement.transform.position = playerFailPos.position;
		transitionGO.SetActive(true);		
	}

	public void StartTransitionOutOfCutscene(bool playerWon)
	{
		if (playerWon) playerMovement.transform.position = playerWinPos.position;

		animator.SetTrigger(fadeOutAnimatorTriggerName);
	}

	public void TransitionOutOfCutscene()
	{
		transitionGO.SetActive(false);
		foreach (LVLLobbyCutscene lobbyCutscene in lobbyCutscenes) lobbyCutscene.enabled = false;
		lobbyInstructionsText.enabled = true;

		videoTransitionGO.transform.eulerAngles = Vector3.zero;

		playerMovement.ChangeSpeed(originalPlayerSpeed);
		playerJoystick.Enabled = true;

		TransitionedOutOfCutscene?.Invoke();
	}
}
