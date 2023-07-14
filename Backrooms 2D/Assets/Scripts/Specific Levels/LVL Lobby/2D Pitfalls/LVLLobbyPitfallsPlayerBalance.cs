using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LVLLobbyPitfallsPlayerBalance : MonoBehaviour
{
	[SerializeField] LVLLobbyPitfallsPlayerBalanceGFX playerBalanceGFX;
	
	[Header("Balance")]
	[SerializeField] float firstRotationSpeed = 30;
	[MinMaxRange(10, 300)][SerializeField] RangedFloat rotationSpeedBounds = new RangedFloat(60, 90);
	float rotationSpeed;
	float rotation;

	[Header("Fall")]
	[SerializeField] LVLLobbyPitfallsPlayerFallAnim fallAnim;
	[Range(10, 90)] [SerializeField] float fallThreshold = 60;
	Vector2 rotationBounds;
	//Vector2 playerNormalizedCardinalDir;

	public UnityEvent BalanceStart;

	private void OnEnable()
	{
		rotationSpeed = firstRotationSpeed;
		rotation = Mathf.Round(transform.eulerAngles.z);
		//playerNormalizedCardinalDir = HelperMethods.SnapToCardinalDirection(playerBalanceGFX.PlayerGFXSpeed);
		rotationBounds = new Vector2(transform.eulerAngles.z - fallThreshold, transform.eulerAngles.z + fallThreshold);
		BalanceStart?.Invoke();
	}

	private void Update()
	{
		Rotating();
		//print(rotation + " | " + rotationBounds);
		if (!enabled) return;
		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) Balance();
		if (rotation < rotationBounds.x || rotation > rotationBounds.y) Fall();
	}

	private void Rotating()
	{
		rotation += rotationSpeed * Time.deltaTime;
		rotation %= 360;
		//transform.eulerAngles = new Vector3(rotation, 0, 0);
		/*transform.eulerAngles = new Vector3(
			Mathf.Abs(playerBalanceGFX.PlayerGFXSpeed.x) >= Mathf.Abs(playerBalanceGFX.PlayerGFXSpeed.y) ? rotation : 0,
			0,
			Mathf.Abs(playerBalanceGFX.PlayerGFXSpeed.x) < Mathf.Abs(playerBalanceGFX.PlayerGFXSpeed.y) ? rotation : 0);*/
		/*Vector2 playerNormalizedCardinalDir = HelperMethods.SnapToCardinalDirection(playerBalanceGFX.PlayerGFXSpeed);
		if(!Equals(playerNormalizedCardinalDir, this.playerNormalizedCardinalDir))
		{
			//print(Mathf.Round(Vector2.SignedAngle(this.playerNormalizedCardinalDir, playerNormalizedCardinalDir)));
			rotation += Mathf.Round(Vector2.SignedAngle(this.playerNormalizedCardinalDir, playerNormalizedCardinalDir));
			rotation = *//*Mathf.Abs(*//*rotation % 180*//*)*//*= ((rotation + 45) % 180) - 45*//*;
			this.playerNormalizedCardinalDir = playerNormalizedCardinalDir;
		}*/
		transform.eulerAngles = new Vector3(0, 0, rotation);
	}

	public void AngleChanged(AnimationEvent animationEvent)
	{
		rotation += animationEvent.floatParameter;
	}

	public void Balance()
	{
		rotationSpeed = Random.Range(rotationSpeedBounds.Min, rotationSpeedBounds.Max) * -Math.Sign(rotationSpeed);
		FindObjectOfType<AudioManager>().Play("LVLLobby_Pitfall_Balance");
	}

	void Fall()
	{
		//print("falllll");
		fallAnim.Fall();
	}

	public void StopBalancing()
	{
		rotationSpeed = 0;
	}

	public void ResetBalance()
	{
		StopBalancing();
		rotation = 0;
	}
}
