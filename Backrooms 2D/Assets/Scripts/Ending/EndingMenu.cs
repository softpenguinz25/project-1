using UnityEngine;

public class EndingMenu : MonoBehaviour
{
    public void PlayVideo()
	{
		Debug.Log("playing video...");
		//string baseUrl = "https://www.youtube.com";
		Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
	}

	public void Quit()
	{
		Application.Quit();
	}
}
