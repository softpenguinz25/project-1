using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLLobbyPitfallsPlayerFallAnim : MonoBehaviour
{
	[SerializeField] LVLLobbyPitfallsPlayerBalance playerBalance;
	[SerializeField] LVLLobbyPitfallsPlayerBalanceGFX playerBalanceGFX;

	private bool isFalling;
	public bool IsFalling { get => isFalling; set => isFalling = value; }

	[SerializeField] Animator anim;
	[SerializeField] UnityEvent fallEvents;

	private void Start()
	{
		IsFalling = false;
	}

	public void Fall()
	{
		//print("FALLING");
		if (IsFalling) return;
		IsFalling = true;

		playerBalance.StopBalancing();
		playerBalanceGFX.StopTravelling();

		Vector2 posBeforeFall = transform.position;
		anim.SetTrigger("Fall");
		transform.position = posBeforeFall;
		//print(posBeforeFall + " | " + transform.position);
		FindObjectOfType<AudioManager>().Play("LVLLobby_Pitfall_Fall");
		fallEvents?.Invoke();
	}

	public void Unfall()
	{
		IsFalling = false;
	}
}
