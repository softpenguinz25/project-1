using UnityEngine;

public class PartypooperAttack : MonoBehaviour
{
	[Header("Info")]
	[SerializeField] string partygoerName;

	[Header("GFX")]
	[SerializeField] GameObject deathParticles;
	float screenShakeIntensity = 2.5f, screenShakeTime = .75f;

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
		Instantiate(deathParticles, partygoerToAttack.transform.position, Quaternion.Euler(-90, 0, 0));
		FindObjectOfType<CinemachineShake>().ShakeCamera(screenShakeIntensity, screenShakeTime);

		Destroy(partygoerToAttack);
		Destroy(gameObject);
	}
}
