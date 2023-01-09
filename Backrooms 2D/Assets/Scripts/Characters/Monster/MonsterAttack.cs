using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
	[Header("Jumpscare")]
	[SerializeField] private JumpscareMonster jumpscareMonsterData;
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			StartCoroutine(FindObjectOfType<DeathManager>().DeathSequenceCoroutine(jumpscareMonsterData));
		}
	}
}
