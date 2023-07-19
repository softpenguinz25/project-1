using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
	[Header("Functionality")]
	[SerializeField] TextMeshProUGUI textObj;

	[Header("GFX")]
	[SerializeField] [MinMaxRange(0.01f, 1f)] RangedFloat timeBtwnDialogueSFXRange;
	public void SetText(string text)
	{
		textObj.text = text;
	}
	public void SetText(AnimationEvent textEvent)
	{
		textObj.text = textEvent.stringParameter;
		StartCoroutine(PlayDialogueSFX(textEvent));
	}

	private IEnumerator PlayDialogueSFX(AnimationEvent textEvent)
	{
		for (int i = 0; i < textEvent.intParameter; i++)
		{
			FindObjectOfType<AudioManager>().Play(textEvent.floatParameter <= 0 ? "Polish_Dialogue_Speech" : "Polish_Dialogue_Speech_Evil");
			yield return new WaitForSeconds(Random.Range(timeBtwnDialogueSFXRange.Min, timeBtwnDialogueSFXRange.Max));
		}
	}
}
