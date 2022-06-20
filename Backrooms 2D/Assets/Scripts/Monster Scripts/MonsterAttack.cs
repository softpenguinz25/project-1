using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			FindObjectOfType<DeathManager>().ExecuteDeathSequence();
		}
	}
}
