//Thanks Code Monkey! https://www.youtube.com/watch?v=ACf1I27I6Tk
using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
	CinemachineVirtualCamera cinemachineVirtualCamera;
	float shakeTimer;
	float shakeTimerTotal;
	float startingIntensity;

	private void Awake()
	{
		cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
	}

	public void ShakeCamera(float intensity, float time)
	{
		CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

		startingIntensity = intensity;
		shakeTimerTotal = time;
		shakeTimer = time;
	}

	private void Update()
	{
		if (shakeTimer > 0)
		{
			shakeTimer -= Time.deltaTime;

			//Timer over!
			CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
				cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
				Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
		}
	}
}
