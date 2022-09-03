using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private void Start()
	{
		Resume();
	}

	public void Pause()
	{
		Time.timeScale = 0;
	}

	public void Resume()
	{
		Time.timeScale = 1;
	}
}
