using MyBox;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherMenu : MonoBehaviour
{
	[Header("Functionality")]
	[SerializeField] SceneReference firstLevel;
	//[SerializeField] SceneReference finalLevel;

	[Header("Ads")]
	[SerializeField] RewardedAdsMenuManager rewardedAdsMenuManager;
	[SerializeField] int playAdEveryXIterations = 2;
	static int iteration;

	public event Action<int> SwitchScene;

	public void LoadScenePlusIndex(int index)
	{
		FinalTimeUI.SetLevelsSkipped(true);

		#region Loading Scenes		
		//Debug.Log("Num Scenes in Build Settings: " + SceneManager.sceneCountInBuildSettings);
		int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + index;
		//Debug.Log("Before: " + nextSceneIndex);
		/*Debug.Log(nextSceneIndex);
		Debug.Log("First Level Build Index: " + SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex);
		Debug.Log("Final Level Build Index: " + SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex);*/
		if (nextSceneIndex < /*SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex*/1) nextSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
		else if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings - 1) nextSceneIndex = 1/*SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex*/;

		//Debug.Log("After: " + nextSceneIndex);
		/*Debug.Log(nextSceneIndex);*/
		#endregion

		if(index == 0)//don't play ad if we're just restarting the level
		{
			//Debug.Log("restarting level...");
			LoadScene(nextSceneIndex);
			return;
		}

		iteration++;
#if (UNITY_IOS || UNITY_ANDROID)
		//Debug.Log("Play ad? " + (iteration % 2 == 1));
		if(iteration % playAdEveryXIterations == 1) LoadSceneAfterAd(nextSceneIndex);
		else LoadScene(nextSceneIndex);
#else
		LoadScene(nextSceneIndex);
#endif
	}

	private void LoadSceneAfterAd(int nextSceneIndex)
	{
		rewardedAdsMenuManager.ShowAd(() =>
		{
			LoadScene(nextSceneIndex);
		}
		);		
	}

	public void IncrementLoadScene(Action rewardedAction, int index)
	{
		iteration++;

		int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + index;
		if (nextSceneIndex < SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex) nextSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
		else if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings - 1) nextSceneIndex = SceneManager.GetSceneByName(firstLevel.SceneName).buildIndex;

		if (iteration % playAdEveryXIterations == 1)
		{
			rewardedAdsMenuManager.ShowAd(() =>
		{
			rewardedAction();
		});
		}
		else LoadScene(nextSceneIndex);
	}

	private void LoadScene(int nextSceneIndex)
	{
		SwitchScene?.Invoke(nextSceneIndex);
	}
}
