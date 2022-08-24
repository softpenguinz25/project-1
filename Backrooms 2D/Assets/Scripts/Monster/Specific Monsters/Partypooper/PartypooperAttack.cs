using UnityEngine;

public class PartypooperAttack : MonoBehaviour
{
	[SerializeField] string partygoerName;

	private void OnTriggerEnter2D(Collider2D col)
	{
		MonsterInfo monsterInRange = col.GetComponent<MonsterInfo>();
		if (monsterInRange == null) return;

		if(monsterInRange.monsterName == partygoerName)
		{
			AttackPartygoer(monsterInRange.gameObject);
		}	
	}

	private void AttackPartygoer(GameObject partygoerToAttack)
	{
		FindObjectOfType<AudioManager>().Play("Monster_Partypooper_Attack");

		Destroy(partygoerToAttack);
		Destroy(gameObject);
	}
}
