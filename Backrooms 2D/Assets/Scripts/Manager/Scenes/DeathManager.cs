using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}
