using UnityEngine;

public class LVLAbandonedOfficeDoor : MonoBehaviour, IInteract
{
	[SerializeField] float playerAngle;
	bool isOpen;
	//[SerializeField] Collider2D doorTrigger;
	Vector3 MidPos
	{
		get
		{
			Vector3 result = Vector3.zero;

			int i = 0;
			foreach(Transform child in transform)
			{
				result += child.transform.position;
				i++;
			}

			return result / i;
		}
	}
    [SerializeField] float activationRadius;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(MidPos, activationRadius);
	}

	/*private void Start()
	{
		doorTrigger.transform.position = MidPos;
	}*/

	public void Interact(GameObject player)
	{
		playerAngle = HelperMethods.AngleDir(transform, player.transform);
		//Open
		if (!isOpen)
		{
			transform.localEulerAngles = new Vector3(0, 0, 90 * Mathf.Sign(HelperMethods.AngleDir(transform, player.transform)));
			isOpen = true;
		}
		//Close
		else
		{
			transform.localEulerAngles = Vector3.zero;

			isOpen = false;
		}
	}
}
