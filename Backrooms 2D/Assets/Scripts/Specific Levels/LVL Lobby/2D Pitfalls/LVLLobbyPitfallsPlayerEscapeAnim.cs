using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLLobbyPitfallsPlayerEscapeAnim : LVLLobbyPitfallsPlayerLerp
{
	[SerializeField] Transform winPos;
	[SerializeField] Transform playerGFX;

	[SerializeField] UnityEvent EndAnimStarted;

	UnityAction EndAnimFinishedAction;
	public UnityEvent EndAnimFinished;
    public void Escape()
	{
		EndAnimStarted?.Invoke();
		//print("ESCAPE");
		PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
		EndAnimFinishedAction = null;
		EndAnimFinishedAction += () => {
			playerMovement.GetComponent<PlayerManager>().EnablePlayer();
			playerMovement.MoveToPos(winPos.position);
			playerMovement.GetComponent<PlayerGraphics>().SetIdleAnimParams(0, -1);
		};

		EndAnimFinished.AddListener(EndAnimFinishedAction);
			
		StartCoroutine(InAnimCoroutine(lerpCurve, transform.position, winPos.position - playerGFX.localPosition, transform.eulerAngles, winPos.eulerAngles, EndAnimFinished));
	}
}
