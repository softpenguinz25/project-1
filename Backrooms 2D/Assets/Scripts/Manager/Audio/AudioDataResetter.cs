using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDataResetter : MonoBehaviour
{
	[SerializeField] AudioMixer mainMixer;

	//Correct lowpass values
	public static List<string> lowpassParams = new List<string>() { "Ambience Lowpass Cutoff Frequency", "Entities Lowpass Cutoff Frequency", "SFX Lowpass Cutoff Frequency" };
	public static float[] startingLowpassFrequencies;
	static bool lowpassFrequenciesAssigned;

	[Header("Fade In")]
	[SerializeField] AnimationCurve audioFadeInCurve;

	private void Awake()
	{
		mainMixer.SetFloat("Fade In Volume", -99999);
	}

	void Start()
    {
		#region Lowpass Correction
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
		#endregion

		StartCoroutine(AudioFadeIn());
	}

	private IEnumerator AudioFadeIn()
	{
		float t = 0;
		while (t < audioFadeInCurve.keys[audioFadeInCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;
			mainMixer.SetFloat("Fade In Volume", Mathf.Log10(Mathf.Lerp(0.0001f, 1, audioFadeInCurve.Evaluate(t))) * 20);

			yield return null;
		}

		mainMixer.SetFloat("Fade In Volume", 0);
	}
}
