using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textObj;
	public void SetText(string text)
	{
		textObj.text = text;
	}
	public void SetText(AnimationEvent textEvent)
	{
		textObj.text = textEvent.stringParameter;
	}
}
