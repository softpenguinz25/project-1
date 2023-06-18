using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLRFYLBossMinigameButton : MonoBehaviour
{
	bool isPressed;

	public SpriteRenderer sr;
	[SerializeField] Sprite unpressedSprite;
	[SerializeField] Sprite pressedSprite;

	[SerializeField] float repressThresholdTimeInSeconds = .1f;

	public virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !isPressed)
			Pressed();
	}

	public virtual void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && isPressed)		
			Unpressed();
	}

	public virtual void Pressed()
	{
		isPressed = true;
		sr.sprite = pressedSprite;
	}

	public virtual void Unpressed()
	{
		Invoke(nameof(SetIsPressedFalse), repressThresholdTimeInSeconds);
		sr.sprite = unpressedSprite;
	}
	void SetIsPressedFalse() => isPressed = false;
}
