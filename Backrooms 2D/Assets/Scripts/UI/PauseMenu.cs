using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] SceneReference startingScene;
	[SerializeField] AudioMixer mixer;

	private void Start()
	{
		Resume();
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
		SceneManager.LoadScene(startingScene.SceneName);
	}

	public void SetVolume(float sliderValue)
	{
		mixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);
	}
}
