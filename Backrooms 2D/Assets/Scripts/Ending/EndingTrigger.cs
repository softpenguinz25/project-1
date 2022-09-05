using System;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
	[SerializeField] private Animator endingAnimator;
	[SerializeField] private string triggerName = "Ending Triggered";
    private static bool hasActivatedTrigger;
	public static bool HasActivatedTrigger { get { return hasActivatedTrigger; } }

	//[SerializeField] Animator animatorToDisable;
	[SerializeField] List<GameObject> gosToDisable = new List<GameObject>();

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

			DisableOtherUI();
		}
	}

	private void DisableOtherUI()
	{
		//animatorToDisable.enabled = false;
		foreach(GameObject go in gosToDisable)
		{
			go.SetActive(false);
		}
	}
}
