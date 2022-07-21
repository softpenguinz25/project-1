using UnityEngine;

public class LVLRFYLObstacle : MonoBehaviour
{
	[SerializeField] private float timeUntilDestroy = 1f;
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<MonsterMovement>() != null)
		{
			Destroy(gameObject, timeUntilDestroy);
		}
		else if(collision.gameObject.GetComponentInParent<ExitDoor>() != null)
		{
			Destroy(gameObject);
		}
	}
}
