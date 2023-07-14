using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLLobbyPitfallsPlayerInAnim : LVLLobbyPitfallsPlayerLerp
{
	[SerializeField] Transform playerGFX;

    Transform player;

	Vector3 targetPos, targetRot;

	[SerializeField] Animator travelAcrossPitfallsAnim;

	public UnityEvent StartAnimStarted;
	public UnityEvent StartAnimFinished;

	private void Init()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;

		//transform.localScale = new Vector3(1 / TileSpawnerV2.TileSize, 1 / TileSpawnerV2.TileSize, 1 / TileSpawnerV2.TileSize);

		targetPos = transform.position /*- playerGFX.localPosition*/;
		targetRot = transform.eulerAngles;

		player.GetComponent<PlayerManager>().DisablePlayer();

		/*player.GetComponent<SpriteRenderer>().enabled = false;
		FindObjectOfType<Joystick>().Enabled = false;*/

		travelAcrossPitfallsAnim.enabled = false;
		//StartAnimFinished += () => travelAcrossPitfallsAnim.enabled = true;

		StartAnimStarted?.Invoke();
	}

	private void OnEnable()
	{
		Init();

		Vector3 startPos = player.position - playerGFX.localPosition;
		Vector3 startRot = player.eulerAngles;

		StartCoroutine(InAnimCoroutine(lerpCurve, startPos, targetPos, startRot, targetRot, StartAnimFinished));
	}

	public void ResetInAnim()
	{
		transform.position = targetPos;
		transform.eulerAngles = targetRot;
	}
}
