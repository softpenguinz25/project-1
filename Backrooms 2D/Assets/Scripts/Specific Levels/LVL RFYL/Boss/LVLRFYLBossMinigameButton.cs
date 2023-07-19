using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLRFYLBossMinigameButton : MonoBehaviour
{
	bool isPressed;

	public SpriteRenderer sr;
	[SerializeField] Sprite unpressedSprite;
	[SerializeField] Sprite pressedSprite;

	[SerializeField] float repressThresholdTimeInSeconds = .1f;

	[SerializeField] Color normalColor = new Color(68f / 255, 68f / 255, 68f / 255), failColor = new Color(1, 1f / 2, 0);
	[SerializeField] bool stayPressed = false;
	[SerializeField] UnityEvent PressedEvent;

	public virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !isPressed)
			Pressed();
	}

	public virtual void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && isPressed && !stayPressed)		
			Unpressed();
	}

	public virtual void Pressed()
	{
		isPressed = true;
		sr.sprite = pressedSprite;
		FindObjectOfType<AudioManager>().Play("LVLRFYL_Minigame_Button");

		ResetIndication();

		PressedEvent?.Invoke();
	}

	public virtual void Unpressed()
	{
		Invoke(nameof(SetIsPressedFalse), repressThresholdTimeInSeconds);
		sr.sprite = unpressedSprite;
	}
	void SetIsPressedFalse() => isPressed = false;

	public void ResetIndication()
	{
		sr.color = normalColor;
	}

	public void IndicateFail()
	{
		sr.color = failColor;
	}
}
