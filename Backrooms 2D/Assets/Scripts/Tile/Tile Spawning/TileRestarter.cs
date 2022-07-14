using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileRestarter : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> scriptsToDisable;

	public void RestartTileGeneration()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    /*public void DisableScripts()
	{
		foreach (MonoBehaviour script in scriptsToDisable)
		{
			script.StopAllCoroutines();
			script.enabled = false;
		}
		Debug.LogWarning("No More Connection Points!");
	}*/
}
