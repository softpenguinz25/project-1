using UnityEngine;
using TMPro;

public class DebugUIText : MonoBehaviour
{
    TextMeshProUGUI debugText;
	string startingText;
    [SerializeField] GameObject debugObj;

	private void Awake()
	{
		debugText = GetComponent<TextMeshProUGUI>();
		startingText = debugText.text;
	}

	private void Update()
	{
		debugText.text = startingText + debugObj.transform.position;
	}
}
