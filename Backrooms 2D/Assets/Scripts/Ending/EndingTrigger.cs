using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
	[SerializeField] private Animator endingAnimator;
	[SerializeField] private string triggerName = "Ending Triggered";
    private static bool hasActivatedTrigger;
	public static bool HasActivatedTrigger { get { return hasActivatedTrigger; } }

	private void Start()
	{
		hasActivatedTrigger = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !hasActivatedTrigger)
		{
			endingAnimator.SetTrigger(triggerName);
			hasActivatedTrigger = true;
		}
	}
}
