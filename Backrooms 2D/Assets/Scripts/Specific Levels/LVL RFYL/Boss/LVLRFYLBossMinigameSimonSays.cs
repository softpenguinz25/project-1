using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LVLRFYLBossMinigameSimonSays : LVLRFYLBossMinigame
{
	public enum CodeValue { Green, Red, Yellow, Blue }
	[Header("Functionality")]
	[SerializeField] CodeValueCodeGODictionary codeGOs;
	[SerializeField] LVLRFYLBossMinigameButton triggerGO;

	[SerializeField] int codeLength = 5;
	List<CodeValue> code;

	int codeLengthOfCurrentRound;
	int numCorrectInCurrentRound;

	[SerializeField] bool disableConsecutive = true;

	[Header("GFX")]
	[SerializeField] float timeBtwnCodeDisplaysInSeconds = 1;

	public int CodeLengthOfCurrentRound { get => codeLengthOfCurrentRound; set => codeLengthOfCurrentRound = value; }

	void GenerateCode()
	{
		code = new();
		CodeLengthOfCurrentRound = 1;
		numCorrectInCurrentRound = 0;

		for (int i = 0; i < codeLength; i++)
		{
			code.Add(disableConsecutive ? AvailableCodes()[Random.Range(0, AvailableCodes().Count)] : Enum.GetValues(typeof(CodeValue)).Cast<CodeValue>().ToList()[Random.Range(0, Enum.GetValues(typeof(CodeValue)).Cast<CodeValue>().ToList().Count)]);
		}

		triggerGO.Unpressed();
		foreach (LVLRFYLBossMinigameSimonSaysCodeGO codeGO in codeGOs.Values) codeGO.Disable(false);
		//DisplayCode(CodeLengthOfCurrentRound);
	}

	List<CodeValue> AvailableCodes()//NO REPEATS!
	{
		List<CodeValue> availableCodes = Enum.GetValues(typeof(CodeValue)).Cast<CodeValue>().ToList();
		if (code.Count > 0) availableCodes.Remove(code[code.Count - 1]);
		return availableCodes;
	}

	private void OnEnable()
	{
		GenerateCode();
	}

	public void InputCodeValue(CodeValue codeValue)
	{
		foreach(CodeValue value in Enum.GetValues(typeof(CodeValue)).Cast<CodeValue>().ToList())
		{
			if (value == codeValue) codeGOs[value].Disable(true);
			else codeGOs[value].Enable();
		}

		//if (disableConsecutive) 

		if (codeValue == code[numCorrectInCurrentRound])
		{
			numCorrectInCurrentRound++;
			//print(numCorrectInCurrentRound);
			if (numCorrectInCurrentRound >= CodeLengthOfCurrentRound)
			{
				numCorrectInCurrentRound = 0;
				CodeLengthOfCurrentRound++;

				if (CodeLengthOfCurrentRound < codeLength + 1)
				{
					triggerGO.Unpressed();
					foreach (LVLRFYLBossMinigameSimonSaysCodeGO codeGO in codeGOs.Values) codeGO.Disable(true);

					FindObjectOfType<AudioManager>().Play("LVLRFYL_Minigame_Progress");
					//DisplayCode(CodeLengthOfCurrentRound);
				}
				else WinGame();
			}
		}
		else
		{
			Debug.Log("WRONG. u inputted " + codeValue + ", was supposed to be " + code[numCorrectInCurrentRound] + " (" + numCorrectInCurrentRound + " in code)");
			RestartGame();
		}
	}

	public void DisplayCode()
	{
		//Debug.Log("DISPLAYING CODE! :D");
		foreach (LVLRFYLBossMinigameSimonSaysCodeGO codeGO in codeGOs.Values) codeGO.Disable(false);

		StartCoroutine(DisplayCodeCoroutine(CodeLengthOfCurrentRound));
	}

	private IEnumerator DisplayCodeCoroutine(int numToDisplay)
	{
		yield return new WaitForSeconds(timeBtwnCodeDisplaysInSeconds);
		for (int i = 0; i < numToDisplay; i++)
		{
			//Debug.Log("Displaying " + numToDisplay + " codes | Current Display: " + code[i] + " | Waiting " + timeBtwnCodeDisplaysInSeconds + " seconds");
			DisplayCodeValue(code[i]);
			yield return new WaitForSeconds(timeBtwnCodeDisplaysInSeconds);
		}

		foreach (LVLRFYLBossMinigameSimonSaysCodeGO codeGO in codeGOs.Values) codeGO.Enable();
	}

	private void DisplayCodeValue(CodeValue codeValue)
	{
		codeGOs[codeValue].Display();
	}

	public override void RestartGame()
	{
		base.RestartGame();
		triggerGO.IndicateFail();
		GenerateCode();
	}

	public override void WinGame()
	{
		Debug.Log("u are weiner");
		base.WinGame();
	}
}
