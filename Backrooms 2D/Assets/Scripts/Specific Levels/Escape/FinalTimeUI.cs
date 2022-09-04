using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalTimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalTimeText;
	static double finalTime;
	static double timeSubtraction;
	static bool levelsSkipped;
	private void OnEnable()
	{
		finalTime = Time.realtimeSinceStartupAsDouble - timeSubtraction;

		if (levelsSkipped)
		{
			finalTimeText.text = "Final Time: LEVELS SKIPPED";
			return;
		}

		int intTime = (int)finalTime;
		int minutes = intTime / 60;
		int seconds = intTime % 60;
		double fraction = finalTime * 1000;
		fraction = (fraction % 1000);
		string timeText = string.Format("{0:00}m:{1:00}s:{2:0}ms", minutes, seconds, fraction);
		finalTimeText.text = "Final Time: " + timeText;
	}

	public static void ResetTimer()
	{
		timeSubtraction = Time.realtimeSinceStartupAsDouble;
		SetLevelsSkipped(false);
	}

	public static void SetLevelsSkipped(bool _levelsSkipped)
	{
		levelsSkipped = _levelsSkipped;
	}
}
