using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GroupTileV2EnterTriggerFinalRoom : GroupTileV2EnterTrigger
{
	[Header("Functionality")]
	[SerializeField] GameObject wall;
	ExitDoorArrow eda;

	[Header("GFX")]
	[SerializeField] AudioMixer mainMixer;
	[Tooltip("name of the mixers (ex: \"ambience\")")][SerializeField] List<string> mixersTooMuffle;
	[SerializeField] AnimationCurve lowpassCurve;
	[SerializeField] float targetLowpassCutoffFrequency;

	private void Awake()
	{
		eda = Resources.FindObjectsOfTypeAll<ExitDoorArrow>()[0];
	}

	public override void PlayerEntered()
	{
		eda.DeactivateArrow();

		MonsterSpawner ms = FindObjectOfType<MonsterSpawner>();
		ms.enabled = false;
		ms.Disable();
		foreach (MonsterInfo monster in FindObjectsOfType<MonsterInfo>()) Destroy(monster.gameObject);
		wall.SetActive(true);

		StartCoroutine(MuffleSounds());
	}

	public override void PlayerExit()
	{
		eda.ActivateArrow();
	}

	private IEnumerator MuffleSounds()
	{
		float t = 0;
		while (t < lowpassCurve.keys[lowpassCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;

			for (int i = 0; i < AudioDataResetter.lowpassParams.Count; i++)
				for (int j = 0; j < mixersTooMuffle.Count; j++)
					if (AudioDataResetter.lowpassParams[i].Contains(mixersTooMuffle[j], System.StringComparison.CurrentCultureIgnoreCase)) 
						mainMixer.SetFloat(AudioDataResetter.lowpassParams[i], Mathf.Lerp(targetLowpassCutoffFrequency, AudioDataResetter.startingLowpassFrequencies[i], lowpassCurve.Evaluate(t)));

			yield return null;
		}

		for (int i = 0; i < AudioDataResetter.lowpassParams.Count; i++)
			for (int j = 0; j < mixersTooMuffle.Count; j++)
				if (AudioDataResetter.lowpassParams[i].Contains(mixersTooMuffle[j], System.StringComparison.CurrentCultureIgnoreCase)) 
					mainMixer.SetFloat(AudioDataResetter.lowpassParams[i], targetLowpassCutoffFrequency);
	}
}
