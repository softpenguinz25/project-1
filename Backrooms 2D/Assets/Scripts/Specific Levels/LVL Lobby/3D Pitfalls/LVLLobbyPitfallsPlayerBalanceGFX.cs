using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLLobbyPitfallsPlayerBalanceGFX : MonoBehaviour
{
	//[SerializeField] LVLLobbyPitfallsPlayerBalance playerBalance;
	[SerializeField] Animator playerAnim;
	[SerializeField] AnimationClip travellingAcrossPitfallsClip;
	[SerializeField] Animator[] playerGFXAnim;
	[SerializeField] AnimationCurve travellingSpeedCurve;
	bool canTravel = true;
	Vector2 playerGFXSpeed;
	public Vector2 PlayerGFXSpeed { get => playerGFXSpeed; set => playerGFXSpeed = value; }
	[SerializeField] Transform playerGFX;

	private void Start()
	{
		//playerBalance.BalanceStart += () => enabled = true;
		enabled = false;
	}

	private void Update()
	{
		foreach(Animator playerGFXAnim in playerGFXAnim) playerGFXAnim.SetFloat("Balancing Animation Speed", 1);

		canTravel = true;
		StartCoroutine(PlayAnimationWithSpeedAnimationCurve(playerAnim, "Travel Across Pitfalls Animation Speed", travellingSpeedCurve));
		
		enabled = false;
	}

	private IEnumerator PlayAnimationWithSpeedAnimationCurve(Animator anim, string paramName, AnimationCurve animCurve)
	{
		//anim.SetFloat(paramName, animCurve.keys[0].time);
		float t = 0;
		while (t < travellingAcrossPitfallsClip.length && canTravel)
		{
			Vector2 beforePos = transform.position;
			//Debug.Log(playerGFXAnim.GetCurrentAnimatorStateInfo(0).normalizedTime / playerGFXAnim.GetCurrentAnimatorStateInfo(0).length * animCurve.keys[animCurve.keys.Length - 1].time);
			//Debug.Log(animCurve.Evaluate(playerGFXAnim.GetCurrentAnimatorStateInfo(0).normalizedTime / playerGFXAnim.GetCurrentAnimatorStateInfo(0).length * animCurve.keys[animCurve.keys.Length - 1].time));
			//Debug.Log(playerGFXAnim.GetCurrentAnimatorStateInfo(0).length);
			//Debug.Log(t + " | " + travellingAcrossPitfallsClip.length);
			//Debug.Log(travellingAcrossPitfallsClip.length);
			//Debug.Log(animCurve.keys[animCurve.keys.Length - 1].time);
			float animSpeed = animCurve.Evaluate(t / travellingAcrossPitfallsClip.length * animCurve.keys[animCurve.keys.Length - 1].time)/** animCurve.keys[animCurve.keys.Length - 1].time)*/;
			anim.SetFloat(paramName, animSpeed);
			//Debug.Log(anim.GetFloat(paramName));
			yield return null;

			PlayerGFXSpeed = new Vector2((transform.position.x - beforePos.x) / Time.deltaTime, (transform.position.y - beforePos.y) / Time.deltaTime);
			foreach (Animator playerGFXAnim in playerGFXAnim)  playerGFXAnim.SetFloat("Player X Speed", PlayerGFXSpeed.x);
			foreach(Animator playerGFXAnim in playerGFXAnim) playerGFXAnim.SetFloat("Player Y Speed", PlayerGFXSpeed.y);

			t += Time.deltaTime * animSpeed;
		}
		//print("done");
		anim.SetFloat(paramName, canTravel ? animCurve.keys[animCurve.keys.Length - 1].time : 0);
	}

	public void StopTravelling()
	{
		canTravel = false;
	}

	public void ResetPlayer()
	{
		StopTravelling();
		playerGFX.localScale = Vector3.one;
	}
}
