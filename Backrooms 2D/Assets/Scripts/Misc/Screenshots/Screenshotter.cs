//THANKS MrM Programming! https://youtu.be/t0erLB3VPiU
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
	[SerializeField] string screenshotFilePath;
    static int count = 0;
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ScreenCapture.CaptureScreenshot(screenshotFilePath + $"screenshot--{count++}.png");
			FindObjectOfType<AudioManager>().Play("Debug_Screenshot");
		}
	}
}
