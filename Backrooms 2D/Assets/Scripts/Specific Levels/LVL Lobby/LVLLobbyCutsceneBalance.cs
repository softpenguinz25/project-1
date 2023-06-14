using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLLobbyCutsceneBalance : LVLLobbyCutscene
{
	LVLLobbyTransitionToCutscene transitionToCutscene;

	float firstBalanceSpeed = 50f;
	[SerializeField] [MinMaxRange(0.01f, 300f)] RangedFloat rotationSpeedRange;
	float rotationSpeed;

	bool doBalance = true;

	private void Start()
	{
		transitionToCutscene = FindObjectOfType<LVLLobbyTransitionToCutscene>();
	}

	private void OnEnable()
	{
		rotationSpeed = firstBalanceSpeed;
		doBalance = true;
	}

	void SetRotationSpeed()
	{
		rotationSpeed = Random.Range(rotationSpeedRange.Min, rotationSpeedRange.Max) * -Mathf.Sign(rotationSpeed);
		FindObjectOfType<AudioManager>().Play("LVLLobby_Pitfall_Balance");
	}

	private void Update()
	{
		if (doBalance && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)) SetRotationSpeed();

		cutsceneVideoGO.transform.eulerAngles += new Vector3(0, 0, rotationSpeed) * Time.deltaTime;
	}

	public override void DisableCutscene(bool didWin)
	{
		if (didWin)
		{
			doBalance = false;
			rotationSpeed = 0;
			base.DisableCutscene(didWin);
		}
		else
		{
			doBalance = false;
		}
	}
}
