using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemAlmondWater : ItemInLevel
{
	[Header("Use Case")]
	[SerializeField] AnimationCurve speedMultiplierCurve;
	bool itemUsed;

	[Header("GFX")]
	[SerializeField] GameObject particles;
	public override void EquipItem(GameObject player)
	{
		if (itemUsed) return;
		StartCoroutine(player.GetComponent<PlayerMovement>().ChangeSpeedWithCurveCoroutine(speedMultiplierCurve));
		PlayGFX();
		DisableItem();
	}

	void PlayGFX()
	{
		Instantiate(particles, transform.position, Quaternion.Euler(-90, 0, 0));
		FindObjectOfType<AudioManager>().Play("Item_AlmondWater_Drink");
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
