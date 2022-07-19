using MyBox;
using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
	public static float jumpscareDuration = 1;

	[SerializeField] private Animator jumspscareAnimator;
	[SerializeField] private SceneReference currentScene;

	/*private void Awake()
	{
		currentScene.Scene = SceneManager.GetActiveScene();
	}*/

	public IEnumerator DeathSequenceCoroutine(JumpscareMonster jumpscareMonsterType)
	{
		//jumspscareAnimator.Play(jumpscareClip.name);
		//jumpscareSFX.Play();

		FindObjectOfType<JumpscareManager>().PlayJumpscare(jumpscareMonsterType);

		yield return new WaitForSeconds(jumpscareDuration)/*new WaitForSeconds(jumpscareClip.length)*/;

		FindObjectOfType<SceneLoader>().LoadScene(currentScene);
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
