using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemAlmondWater : ItemInLevel
{
	[Header("Use Case")]
	[SerializeField] AnimationCurve speedMultiplierCurve;
	bool itemUsed;

	[Header("GFX")]
	[SerializeField] GameObject particles;
	[SerializeField] AnimationCurve audioFadeInCurve;
	float startingAudioLevel;
	[SerializeField] float screenShakeIntensity = 1.5f, screenShakeTime = .5f;
	public override void EquipItem(GameObject player)
	{
		if (itemUsed) return;
		StartCoroutine(player.GetComponent<PlayerMovement>().ChangeSpeedWithCurveCoroutine(speedMultiplierCurve));
		PlayGFX();
		DisableItem();
	}

	private void Awake()
	{
		startingAudioLevel = GetComponent<AudioSource>().volume;
	}

	private IEnumerator Start()
	{
		float t = 0;
		while(t < audioFadeInCurve.keys[audioFadeInCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;

			GetComponent<AudioSource>().volume = audioFadeInCurve.Evaluate(t) * startingAudioLevel;
			yield return null;
		}

		GetComponent<AudioSource>().volume = startingAudioLevel;
	}

	void PlayGFX()
	{
		Instantiate(particles, transform.position, Quaternion.Euler(-90, 0, 0));
		FindObjectOfType<AudioManager>().Play("Item_AlmondWater_Drink");
		FindObjectOfType<CinemachineShake>().ShakeCamera(screenShakeIntensity, screenShakeTime);
	}

	void DisableItem()
	{
		itemUsed = true;
		foreach(Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
		GetComponent<Light2D>().enabled = false;
		GetComponent<AudioSource>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		Destroy(gameObject, speedMultiplierCurve[speedMultiplierCurve.length - 1].time + .5f);//arbitrary .5 to ensure coroutine runs fully
	}
}
