using System.Collections.Generic;
using UnityEngine;

public class TileDisabler : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> scriptsToDisable;

    public void DisableScripts()
	{
		foreach (MonoBehaviour script in scriptsToDisable)
		{
			script.StopAllCoroutines();
			script.enabled = false;
		}
		Debug.LogWarning("No More Connection Points!");
	}
}
