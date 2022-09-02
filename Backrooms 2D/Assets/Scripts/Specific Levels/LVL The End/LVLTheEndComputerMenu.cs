using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LVLTheEndComputerMenu : MonoBehaviour
{
	[Header("References")]
    [SerializeField] LVLTheEndComputerState compState;

	[Header("Menu")]
	[SerializeField] GameObject menu, backButton, passwordMenu, h97menu;
	[SerializeField] List<TMP_InputField> numberInputs = new List<TMP_InputField>();
	int currentEntry = 9999;

	[Header("Next Level")]
	[SerializeField] AudioSource nextLevelSFX;
	[SerializeField] Animator nextLevelAnimator;
	[SerializeField] AnimationClip nextLevelAnimationClip;

	private void OnEnable()
	{
		compState.ComputerStateChanged += MenuOpened;
	}

	private void OnDisable()
	{
		compState.ComputerStateChanged -= MenuOpened;
	}

	private void MenuOpened(LVLTheEndComputerState.ComputerState currentComputerState)
	{
		if (currentComputerState != LVLTheEndComputerState.ComputerState.Menu) return;

		menu.SetActive(true);
	}

	private void Update()
	{
		#region Close Menu
		if (Input.GetKeyDown(KeyCode.Escape))
			CloseMenu();		
		#endregion
	}

	public void CloseMenu()
	{
		menu.SetActive(false);
		FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Menu_Out");
		compState.ChangeCurrentState(LVLTheEndComputerState.ComputerState.Highlight);
	}

	bool autoSelectedInputField;
	public void InputFieldSelected()
	{
		if (autoSelectedInputField)
		{
			autoSelectedInputField = false;
			return;
		}

		FindObjectOfType<AudioManager>().Play("LVLTheEnd_Code_Select");
	}

	public void ChangeCurrentEntry(int digitToReplace)
	{

		/*//THANKS V4Vendetta! https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
		string newEntryString = currentEntry.ToString().Remove(digitToReplace, 1).Insert(digitToReplace, numberInputs[digitToReplace].text);
		currentEntry = int.Parse(newEntryString);*/
		FindObjectOfType<AudioManager>().PlayOneShot("LVLTheEnd_Code_Type");

		digitToReplace--;
		if (!string.IsNullOrEmpty(numberInputs[digitToReplace].text))
		{
			int nextInputFieldIndex = digitToReplace++ >= numberInputs.Count - 1 ? 0 : digitToReplace++;

			numberInputs[nextInputFieldIndex].ActivateInputField();
			numberInputs[nextInputFieldIndex].Select();

			autoSelectedInputField = true;
		}
	}

	public void SubmitEntry()
	{
		FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Menu_Select");

		string currentEntryString = "";
		foreach(TMP_InputField inputField in numberInputs)
		{
			currentEntryString += inputField.text;
		}
		currentEntry = int.Parse(currentEntryString);

		//Debug.Log("Entry Submitted: " + currentEntry);
		if(currentEntry != LVLTheEndPasswordGenerator.password)
		{
			FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Code_Wrong");
		}
		else
		{
			FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_Code_Correct");
			OpenH97Menu();
		}
	}

	public void OpenH97Menu()
	{
		passwordMenu.SetActive(false);
		h97menu.SetActive(true);
	}

	public void GoToNextLevel()
	{
		h97menu.SetActive(false);
		FindObjectOfType<AudioManager>().Play("LVLTheEnd_Computer_RunH97");
		backButton.SetActive(false);
		StartCoroutine(NextLevelCoroutine(nextLevelAnimationClip.length));
	}

	private IEnumerator NextLevelCoroutine(float transitionDelay)
	{
		nextLevelSFX.Play();
		nextLevelAnimator.SetTrigger("Level Transition");

		yield return new WaitForSeconds(transitionDelay);

		FindObjectOfType<SceneLoader>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
