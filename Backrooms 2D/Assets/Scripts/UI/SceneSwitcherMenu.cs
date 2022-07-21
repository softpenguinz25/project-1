using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherMenu : MonoBehaviour
{
    public void LoadScenePlusIndex(int index)
	{
		SceneLoader sl = FindObjectOfType<SceneLoader>();
		if (sl != null) sl.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
		else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
	}
}
