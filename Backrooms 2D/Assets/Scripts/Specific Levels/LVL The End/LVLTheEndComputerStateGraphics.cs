using UnityEngine;

[RequireComponent(typeof(LVLTheEndComputerState))]
public class LVLTheEndComputerStateGraphics : MonoBehaviour
{
    LVLTheEndComputerState compState;

	[SerializeField] SpriteRenderer sr;
	[SerializeField] Sprite regularSprite, outlineSprite;
	[SerializeField] GameObject instructionsText;

	private void Awake()
	{
		compState = GetComponent<LVLTheEndComputerState>();
	}

	private void OnEnable()
	{
		compState.ComputerStateChanged += ChangeCompGraphics;
	}

	private void OnDisable()
	{
		compState.ComputerStateChanged -= ChangeCompGraphics;
	}

	private void ChangeCompGraphics(LVLTheEndComputerState.ComputerState compState)
	{
		switch (compState)
		{
			case LVLTheEndComputerState.ComputerState.Idle:
				sr.sprite = regularSprite;
				instructionsText.SetActive(false);
				break;
			case LVLTheEndComputerState.ComputerState.Active:
				sr.sprite = outlineSprite;
				instructionsText.SetActive(true);
				break;
		}
	}
}
