using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	protected void LoadScene(SceneReference sceneToLoad) => SceneManager.LoadScene(sceneToLoad.SceneName);
	protected void LoadScene(string sceneToLoad) => SceneManager.LoadScene(sceneToLoad);
	protected void LoadScene(int sceneToLoad) => SceneManager.LoadScene(sceneToLoad);

	protected void LoadScenes(List<SceneReference> loadScenes, List<SceneReference> unloadScenes)
	{
		foreach (SceneReference scene in unloadScenes) SceneManager.UnloadSceneAsync(scene.SceneName);
		foreach (SceneReference scene in loadScenes) SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
	}
}