using UnityEngine;

public class EndingMenu : MonoBehaviour
{
    public void PlayVideo()
	{
		Application.OpenURL("https://youtu.be/OXRUEqbV2oM");
	}

	public void Quit()
	{
		Application.Quit();
	}
}
