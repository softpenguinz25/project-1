using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameSimonSaysCodeGO : MonoBehaviour
{
	[SerializeField] SpriteRenderer sr;
	[SerializeField] Collider2D col;

 	[Header("Functionality")]
	[SerializeField] LVLRFYLBossMinigameSimonSays.CodeValue codeValue;
	[SerializeField] LVLRFYLBossMinigameSimonSays minigameSimonSays;

	bool isColliding = false;
	[SerializeField] float resetCollidingThreshold = .25f;

	[Header("GFX")]
	[SerializeField] float timeDisplayedInSeconds = 1;
	Color originalColor = Color.white;
	[SerializeField] Color displaySpriteRendererColorOverlay = Color.gray;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !isColliding)
		{
			isColliding = true;
			Invoke(nameof(ResetColliding), resetCollidingThreshold);
			//Debug.Log("PLAYER DETECTED");
			minigameSimonSays.InputCodeValue(codeValue);
			Display();
		}
	}

	void ResetColliding()
	{
		isColliding = false;
	}

	public void Enable()
	{
		col.enabled = true;
		col.isTrigger = true;
	}

	public void Disable(bool allowPlayerInside)
	{
		if (allowPlayerInside) col.enabled = false;
		else
		{
			col.enabled = true;
			col.isTrigger = false;
		}
	}

	public void Display()
	{
		//Debug.Log("Displaying GO GFX!", this);
		StartCoroutine(DisplayCoroutine());
	}

	private IEnumerator DisplayCoroutine()
	{
		sr.color = displaySpriteRendererColorOverlay;

		yield return new WaitForSeconds(timeDisplayedInSeconds);

		sr.color = originalColor;
	}
}
