using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] Button pauseButton, resumeButton; 

	[SerializeField] SceneReference startingScene;
	[SerializeField] AudioMixer mixer;

	[SerializeField] SceneSwitcherMenu sceneSwitcherMenu;

	private void Start()
	{
		Resume();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//Debug.Log("Current Time Scale: " + Time.timeScale);
			if (Time.timeScale == 0) resumeButton.onClick.Invoke();
			else if (Time.timeScale == 1) pauseButton.onClick.Invoke();
		}
	}

	public void Pause()
	{
		Time.timeScale = 0;
	}

	public void Resume()
	{
		Time.timeScale = 1;
	}

	public void Quit()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void RestartGame()
	{
		FinalTimeUI.ResetTimer();
#if UNITY_IOS || UNITY_ANDROID
		sceneSwitcherMenu.IncrementLoadScene(() => { SceneManager.LoadScene(startingScene.SceneName); }, SceneManager.GetSceneByName(startingScene.SceneName).buildIndex);		
#else
		sceneSwitcherMenu.LoadScenePlusIndex(-SceneManager.GetActiveScene().buildIndex + 1);
#endif
	}

	public void SetVolume(float sliderValue)
	{
		mixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);
	}
}
