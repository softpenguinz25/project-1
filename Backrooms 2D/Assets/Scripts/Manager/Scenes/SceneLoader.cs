using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[Tooltip("Container for object, won't be disabled")] [SerializeField] private GameObject managerObject;
	[Tooltip("Starting scenes (load these scenes when the game starts)")] [SerializeField] private List<SceneReference> runtimeScenes = new List<SceneReference>();
	private void Awake()
	{
		foreach (SceneReference scene in runtimeScenes)
		{
			SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Additive);
		}
	}

	public void LoadScene(SceneReference sceneToLoad)
	{
		SceneManager.LoadScene(sceneToLoad.SceneName);
	}

	public void LoadScene(string sceneToLoad)
	{
		SceneManager.LoadScene(sceneToLoad);
	}

	public void LoadScene(int sceneToLoad)
	{
		//if (sceneToLoad < 0 || sceneToLoad > SceneManager.sceneCountInBuildSettings - 1) { sceneToLoad = Mathf.Clamp(sceneToLoad, 0, SceneManager.sceneCountInBuildSettings - 1); Debug.LogError("Could not load scene at build index" + sceneToLoad + "!"); }
		SceneManager.LoadScene(sceneToLoad);
	}

	public void LoadScenes(List<SceneReference> loadScenes, List<SceneReference> unloadScenes, bool enableBaseScene = true)
	{
		foreach (SceneReference scene in unloadScenes)
		{
			SceneManager.UnloadSceneAsync(scene.SceneName);
		}

		foreach (SceneReference scene in loadScenes)
		{
			SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
		}

		foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			if(managerObject != null) if (go == managerObject) continue;
			go.SetActive(enableBaseScene);
		}
	}
}