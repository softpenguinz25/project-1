using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
	[SerializeField] private bool startingFade = true;
	[SerializeField] private float transitionDelay = 5f;

	public event Action<int> LoadNextLevel;

	private void Start()
	{
		if (startingFade) animator.SetTrigger("Starting Fade");
	}

	public IEnumerator PlayTransitionCoroutine()
	{
		animator.SetTrigger("Level Transition");

		yield return new WaitForSeconds(transitionDelay);

		LoadNextLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void TransitionToNextLevel()
	{
		LoadNextLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
