using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilepsyWarning : MonoBehaviour
{
	[SerializeField] AnimationClip epilepyClip;
	[SerializeField] SceneReference firstLevel;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(epilepyClip.length);

		TransitionToGame();
	}

	public void TransitionToGame()
	{
		SceneManager.LoadScene(firstLevel.SceneName);
	}
}
