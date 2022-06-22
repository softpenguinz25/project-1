using UnityEngine;

public class EndingMenu : MonoBehaviour
{
    public void PlayVideo()
	{
		Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
	}

	public void Quit()
	{
		Application.Quit();
	}
}
