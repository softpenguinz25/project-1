using MyBox;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
	[SerializeField] private SceneReference currentScene;

	/*private void Awake()
	{
		currentScene.Scene = SceneManager.GetActiveScene();
	}*/

	public void ExecuteDeathSequence()
	{
		FindObjectOfType<SceneLoader>().LoadScene(currentScene);
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			FindObjectOfType<SceneLoader>().LoadScene(currentScene);
		}
	}
#endif
}
