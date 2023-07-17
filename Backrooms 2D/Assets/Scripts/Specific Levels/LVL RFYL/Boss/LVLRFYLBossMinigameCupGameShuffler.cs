using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LVLRFYLBossMinigameCupGameShuffler : LVLRFYLBossMinigame
{
	[Header("References")]
    [SerializeField] Animator animator;

	int numCups = 3;
	[Header("Order")]
	[SerializeField] int numShuffleIndexes = 10;
    [SerializeField] float regenerateShuffleIndexTime = 1f;
	[SerializeField] int numShuffles = 15;
	int currentNumShuffles;

	[Header("Current Positions")]
	[SerializeField] int correctCup = 2;
	int[] currentPositions = new int[] {1, 2, 3 };
	[SerializeField] LVLRFYLBossMinigameCupGameCup[] cupsInOrder = new LVLRFYLBossMinigameCupGameCup[3];
	
	[Header("Event")]
	bool isShuffling = false;
	bool lastIsShuffling = false;
	public event Action<bool> IsShuffling;
	public int CorrectCupNum
	{
		get
		{
			for (int i = 0; i < numCups; i++)
				if (currentPositions[i] == CorrectCup) return currentPositions[i];

			Debug.LogError("Could not find correct cup!");
			return 0;
		} 
	}

	public int CorrectCup { get => correctCup; set => correctCup = value; }
	public LVLRFYLBossMinigameCupGameCup[] CupsInOrder { get => cupsInOrder; set => cupsInOrder = value; }

	public LVLRFYLBossMinigameCupGameCup GetCupFromCupNum(int cupNum)
	{
		for (int i = 0; i < numCups; i++)
			if (CupsInOrder[i].CupNum == cupNum) return CupsInOrder[i];

		Debug.LogError("Couldn't get cup from " + cupNum);
		return null;
	}

	public int GetCupIndexFromCupNum(int cupNum)
	{
		for (int i = 0; i < numCups; i++)
			if (CupsInOrder[i].CupNum == cupNum) return i + 1;

		Debug.LogError("Couldn't get cup from " + cupNum);
		return 0;
	}

	private void Start()
	{
		lastIsShuffling = isShuffling;

		ResetNumShuffles();
		InvokeRepeating(nameof(RegenerateShuffleIndex), regenerateShuffleIndexTime, regenerateShuffleIndexTime);
	}

	public void RegenerateShuffleIndex()
	{
		animator.SetInteger("Shuffle Index", Random.Range(1, numShuffleIndexes + 1));
	}
	public void ShuffleExecuted(AnimationEvent animationEvent)
	{
		int[] newOrder = animationEvent.intParameter.ToString().ToCharArray().Select(Convert.ToInt32).ToArray();
		for (int i = 0; i < newOrder.Length; i++) newOrder[i] -= 48;

		if (newOrder.Length != numCups/* || Regex.Matches(newOrder.ToString(), "1").Count != 1 || Regex.Matches(newOrder.ToString(), "2").Count != 1 || Regex.Matches(newOrder.ToString(), "3").Count != 1*/) 
		{
			Debug.LogError(newOrder + " has improper formatting!");
			return;
		}

		int[] valuesToArrange = new int[numCups]; Array.Copy(currentPositions, valuesToArrange, numCups); 
		//Array.Copy(currentPositions, valuesToArrange, numCups);
		for (int i = 0; i < numCups; i++)
		{
			//print("Setting " + newOrder.ToString().IndexOf(Convert.ToChar(i)) + " to " + valuesToArrange[i]);
			//print(string.Join("", newOrder) + "|" + (i + 1).ToString() + "|" +string.Join("", newOrder).IndexOf((i + 1).ToString()));
			int currentPositionsIndex = string.Join("", newOrder).IndexOf((i + 1).ToString());
			currentPositions[currentPositionsIndex] = valuesToArrange[i];
			//Debug.Log(valuesToArrange[i]);
			CupsInOrder[currentPositionsIndex].CupNum = valuesToArrange[i];
		}
		
	}
	public void ShuffleUsed()
	{
		//print("shuffle used");
		currentNumShuffles--;
		isShuffling = currentNumShuffles > 0;
		animator.SetInteger("Shuffles Remaining", currentNumShuffles);

		if(isShuffling != lastIsShuffling)
		{
			lastIsShuffling = isShuffling;
			//print("isShuffling invoked: " + isShuffling);
			IsShuffling?.Invoke(isShuffling);
		}
	}
	public void ResetNumShuffles()
	{
		currentNumShuffles = numShuffles + 1;
		//animator.SetInteger("Shuffles Remaining", currentNumShuffles);
	}

	public void ResetCupPositions()
	{
		currentPositions = new int[] { 1, 2, 3 };
	}
}
