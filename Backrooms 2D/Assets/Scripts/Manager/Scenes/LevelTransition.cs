using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
	[SerializeField] private float transitionDelay = 5f;

    public IEnumerator PlayTransitionCoroutine()
	{
		animator.SetTrigger("Level Transition");

		yield return new WaitForSeconds(transitionDelay);

		FindObjectOfType<SceneLoader>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
