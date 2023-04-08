using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderEventManager : SceneLoader
{
	//TODO: Refactor this workflow
	[SerializeField] TileSpawnerV2 ts;
	[SerializeField] LevelTransition lt;
	[SerializeField] DeathManager dm;
	[SerializeField] SceneSwitcherMenu ssm;
	[SerializeField] GoalItems[] giS;

	private void OnEnable()
	{
		if(ts != null) ts.NoCPsRemaining += () => LoadScene(SceneManager.GetActiveScene().name);
		if(lt != null) lt.LoadNextLevel += (nextLevelBuildIndex) => LoadScene(nextLevelBuildIndex);
		if(dm != null) dm.ResetScene += (currentScene) => LoadScene(currentScene);
		if(ssm != null) ssm.SwitchScene += (nextSceneIndex) => LoadScene(nextSceneIndex);
		if(giS.Length > 0) foreach (GoalItems gi in giS) gi.NextLevel += (nextLevelIndex) => LoadScene(nextLevelIndex);
	}
}
