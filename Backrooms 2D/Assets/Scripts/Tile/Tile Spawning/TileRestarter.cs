using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileRestarter : MonoBehaviour
{
	[Tooltip("If total tiles are > this value --> don't restart game")][SerializeField] private int maxTileRestartCount = 1000;
    [SerializeField] private List<MonoBehaviour> scriptsToDisable;

	public void RestartTileGeneration()
	{
		if (FindObjectOfType<TileDataManager>().tiles.Count > maxTileRestartCount) return;

		Debug.Log("Restarting Tile Generation...");
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
