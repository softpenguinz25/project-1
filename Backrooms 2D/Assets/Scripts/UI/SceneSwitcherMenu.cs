using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherMenu : MonoBehaviour
{
	[SerializeField] SceneReference firstLevel;
	//[SerializeField] SceneReference finalLevel;
    public void LoadScenePlusIndex(int index)
	{
		FinalTimeUI.SetLevelsSkipped(true);

		SceneLoader sl = FindObjectOfType<SceneLoader>();
		int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + index;
		/*Debug.Log(nextSceneIndex);
		Debug.Log("First Level Build Index: " + SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex);
		Debug.Log("Final Level Build Index: " + SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex);*/
		if (nextSceneIndex < SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex) nextSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
		else if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings - 1) nextSceneIndex = SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex;
		/*Debug.Log(nextSceneIndex);*/
		if (sl != null) sl.LoadScene(nextSceneIndex);
		else SceneManager.LoadScene(nextSceneIndex);
	}
}
