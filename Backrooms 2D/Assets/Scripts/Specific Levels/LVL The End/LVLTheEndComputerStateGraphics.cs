using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LVLTheEndComputerState))]
public class LVLTheEndComputerStateGraphics : MonoBehaviour
{
    LVLTheEndComputerState compState;

	[SerializeField] SpriteRenderer sr;
	[SerializeField] Sprite regularSprite, outlineSprite;
	[SerializeField] TMP_Text instructionsText;
	[SerializeField] string pcInstructions = "E To Interact", mobileInstructions = "Tap To Interact";
	//TouchScreenKeyboard keyboard;
	[SerializeField] FloatingJoystick joystick;

	private void Awake()
	{
		compState = GetComponent<LVLTheEndComputerState>();
	}

	private void Start()
	{
		pcInstructions = pcInstructions.ToString();//to stop giving me the "this variable is not in use" warning in unity lmao
		mobileInstructions = mobileInstructions.ToString();////to stop giving me the "this variable is not in use" warning in unity lmao
#if UNITY_STANDALONE
	instructionsText.text = pcInstructions;
#elif UNITY_ANDROID || UNITY_IOS
		instructionsText.text = mobileInstructions;
#endif
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
				instructionsText.gameObject.SetActive(false);

				//keyboard = null;

				joystick.gameObject.SetActive(true);
				break;
			case LVLTheEndComputerState.ComputerState.Highlight:
				FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Menu_Highlight");
				sr.sprite = outlineSprite;
				instructionsText.gameObject.SetActive(true);

				//keyboard = null;

				joystick.gameObject.SetActive(true);
				break;
			case LVLTheEndComputerState.ComputerState.Menu:
				FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Menu_In");

				//keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

				joystick.HideJoystick();
				joystick.gameObject.SetActive(false);
				break;
		}
	}
}
