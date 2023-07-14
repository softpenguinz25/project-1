using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLLobbyPitfallsPlayerBalanceManager : MonoBehaviour
{
	//Vector3 startPos, startRot, startScale;
	[SerializeField] Animator anim;
	[SerializeField] Transform failPos;
	[SerializeField] UnityEvent resetEvents;

	/*private void Start()
	{
		startPos = transform.position; startRot = transform.eulerAngles; startScale = transform.localScale;
	}*/
	public void ResetPlayer(bool didWin)
	{
		if (didWin)
			//anim.SetBool("Can Travel Across Pitfalls", false);
			return;
		anim.SetTrigger("Reset");

		/*transform.position = startPos;
		transform.eulerAngles = startRot;
		transform.localScale = startScale;*/
		FindObjectOfType<PlayerManager>().EnablePlayer();
		FindObjectOfType<PlayerMovement>().MoveToPos(failPos.position);

		resetEvents?.Invoke();
	}
}
