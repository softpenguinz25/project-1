using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [SerializeField] int targetFrameRate = 30;

	private void Start()
	{
		//THANKS Huacanacha! https://answers.unity.com/questions/300467/how-to-limit-frame-rate-in-unity-editor.html
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = targetFrameRate;
	}
}
