using MyBox;
using System;
using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
	public static float jumpscareDuration = 1;
	public static bool jumpscareIsPlaying;

	[SerializeField] private SceneReference currentScene;

	public event Action<SceneReference> ResetScene;

	public IEnumerator DeathSequenceCoroutine(JumpscareMonster jumpscareMonsterType)
	{
		if (jumpscareIsPlaying) { Debug.LogWarning("Jumpscare already playing!"); yield break; }

		jumpscareIsPlaying = true;

		FindObjectOfType<JumpscareManager>().PlayJumpscare(jumpscareMonsterType);

		yield return new WaitForSeconds(jumpscareDuration);

		ResetScene?.Invoke(currentScene);

		jumpscareIsPlaying = false;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ResetScene?.Invoke(currentScene);
		}
	}
#endif
}
