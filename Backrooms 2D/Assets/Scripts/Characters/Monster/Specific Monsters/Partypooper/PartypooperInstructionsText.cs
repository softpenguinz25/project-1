using System.Collections;
using UnityEngine;

public class PartypooperInstructionsText : MonoBehaviour
{
	GameObject player;

	[Header("Check To Display Instructions")]
	[SerializeField] string partypooperName = "partypooper";
	[SerializeField] LayerMask wallMask;
	[SerializeField] float indentValue = .2f;

	[Header("Display Instructions GFX")]
	[SerializeField] Vector2 positionOffsetValue;
	[SerializeField] Animator instructionsTextAnimator;
	[SerializeField] AnimationClip displayTextAnimationClip;
	bool displayingInstructionsText;
	private void Start()
	{
		player = FindObjectOfType<PlayerMovement>().gameObject;
	}

	private void Update()
	{
		foreach (MonsterInfo monster in FindObjectsOfType<MonsterInfo>())
		{
			if(monster.monsterName == partypooperName)
			{
				Vector2 monsterViewPos = Camera.main.WorldToViewportPoint(monster.transform.position);

				var old = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = false;				

				RaycastHit2D obstructingWallCheck = Physics2D.Linecast(monster.transform.position, player.transform.position, wallMask);
				Physics2D.queriesHitTriggers = old;

				if (MonsterInCameraViewport(monsterViewPos, indentValue) && obstructingWallCheck.collider == null && !displayingInstructionsText)
				{
					StartCoroutine(DisplayInstructionsText(monster.gameObject));
				}
			}
		}
	}

	private static bool MonsterInCameraViewport(Vector2 monsterViewPos, float indentValue)
	{
		return monsterViewPos.x >= indentValue && monsterViewPos.x <= 1 - indentValue && monsterViewPos.y >= indentValue && monsterViewPos.y <= 1 - indentValue;
	}

	private IEnumerator DisplayInstructionsText(GameObject monster)
	{
		GetComponent<RectTransform>().SetParent(monster.transform);
		GetComponent<RectTransform>().localPosition = new Vector3(positionOffsetValue.x, positionOffsetValue.y, GetComponent<RectTransform>().localPosition.z);

		displayingInstructionsText = true;
		instructionsTextAnimator.SetTrigger("Display Text");

		FindObjectOfType<AudioManager>().Play("Monster_Partypooper_InstructionText");

		yield return new WaitForSeconds(displayTextAnimationClip.length);

		Destroy(gameObject);
	}
}
