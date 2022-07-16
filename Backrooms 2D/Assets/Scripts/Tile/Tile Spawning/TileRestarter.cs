using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileRestarter : MonoBehaviour
{
	[SerializeField] private int maxTileCount = 1000;
    [SerializeField] private List<MonoBehaviour> scriptsToDisable;

	public void RestartTileGeneration()
	{
		if (FindObjectOfType<TileDataManager>().tiles.Count > maxTileCount) return;

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
