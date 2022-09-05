using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingMenu : MonoBehaviour
{
	[SerializeField] SceneReference firstLevel;
	[SerializeField] SceneSwitcherMenu sceneSwitcherMenu;
	public void PlayVideo()
	{
		Application.OpenURL("https://www.youtube.com/watch?v=RnkVZDEXl64&list=PLXbf8z2WP7RYHaYOgXyr2urZInTplnxuT&index=2");
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
		sceneSwitcherMenu.IncrementLoadScene(() => { SceneManager.LoadScene(/*firstLevel.SceneName*//*"LVL Lobby"*/1); }, /*SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex*/-8);
	}
}
