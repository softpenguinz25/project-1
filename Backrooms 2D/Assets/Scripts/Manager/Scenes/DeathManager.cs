using MyBox;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
	[SerializeField] private SceneReference currentScene;
    public void ExecuteDeathSequence()
	{
		FindObjectOfType<SceneLoader>().LoadScene(currentScene);
	}
}
