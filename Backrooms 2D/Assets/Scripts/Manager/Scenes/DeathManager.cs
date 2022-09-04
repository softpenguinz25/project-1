using MyBox;
using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
	public static float jumpscareDuration = 1;
	public static bool jumpscareIsPlaying;

	[SerializeField] private SceneReference currentScene;

	/*private void Awake()
	{
		currentScene.Scene = SceneManager.GetActiveScene();
	}*/

	public IEnumerator DeathSequenceCoroutine(JumpscareMonster jumpscareMonsterType)
	{
		//jumspscareAnimator.Play(jumpscareClip.name);
		//jumpscareSFX.Play();
		if (jumpscareIsPlaying) { Debug.LogWarning("Jumpscare already playing!"); yield break; }

		jumpscareIsPlaying = true;

		FindObjectOfType<JumpscareManager>().PlayJumpscare(jumpscareMonsterType);

		yield return new WaitForSeconds(jumpscareDuration)/*new WaitForSeconds(jumpscareClip.length)*/;

		FindObjectOfType<SceneLoader>().LoadScene(currentScene);

		jumpscareIsPlaying = false;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			FindObjectOfType<SceneLoader>().LoadScene(currentScene);
		}
	}
#endif
}
