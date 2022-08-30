using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LVLTheEndComputerMenu : MonoBehaviour
{
	[Header("References")]
    [SerializeField] LVLTheEndComputerState compState;

	[Header("Menu")]
	[SerializeField] GameObject menu, passwordMenu, h97menu;
	[SerializeField] List<TMP_InputField> numberInputs = new List<TMP_InputField>();
	int currentEntry = 9999;

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
		compState.ChangeCurrentState(LVLTheEndComputerState.ComputerState.Highlight);
	}

	public void ChangeCurrentEntry(int digitToReplace)
	{
		digitToReplace--;

		/*//THANKS V4Vendetta! https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
		string newEntryString = currentEntry.ToString().Remove(digitToReplace, 1).Insert(digitToReplace, numberInputs[digitToReplace].text);
		currentEntry = int.Parse(newEntryString);*/

		if (!string.IsNullOrEmpty(numberInputs[digitToReplace].text))
		{
			int nextInputFieldIndex = digitToReplace++ >= numberInputs.Count - 1 ? 0 : digitToReplace++;

			numberInputs[nextInputFieldIndex].ActivateInputField();
			numberInputs[nextInputFieldIndex].Select();
		}
	}

	public void SubmitEntry()
	{
		string currentEntryString = "";
		foreach(TMP_InputField inputField in numberInputs)
		{
			currentEntryString += inputField.text;
		}
		currentEntry = int.Parse(currentEntryString);

		Debug.Log("Entry Submitted: " + currentEntry);
		if(currentEntry != LVLTheEndPasswordGenerator.password)
		{
			Debug.Log("Access Denied.");
		}
		else
		{
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
		StartCoroutine(FindObjectOfType<LevelTransition>().PlayTransitionCoroutine());
	}
}
