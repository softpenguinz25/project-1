using UnityEngine;

public abstract class ItemInLevel : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			EquipItem(collision.gameObject);
		}
	}

	public abstract void EquipItem(GameObject player);
}
