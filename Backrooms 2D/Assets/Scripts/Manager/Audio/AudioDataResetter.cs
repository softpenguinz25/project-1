using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDataResetter : MonoBehaviour
{
	[SerializeField] AudioMixer mainMixer;

	public static List<string> lowpassParams = new List<string>() { "Ambience Lowpass Cutoff Frequency", "Entities Lowpass Cutoff Frequency", "SFX Lowpass Cutoff Frequency" };
	public static float[] startingLowpassFrequencies;
	static bool lowpassFrequenciesAssigned;

	void Start()
    {
		Array.Resize(ref startingLowpassFrequencies, lowpassParams.Count);

		for (int i = 0; i < lowpassParams.Count; i++)
		{
			if (!lowpassFrequenciesAssigned)
			{
				mainMixer.GetFloat(lowpassParams[i], out startingLowpassFrequencies[i]);
			}
			else
				mainMixer.SetFloat(lowpassParams[i], startingLowpassFrequencies[i]);
		}
		lowpassFrequenciesAssigned = true;
	}
}
