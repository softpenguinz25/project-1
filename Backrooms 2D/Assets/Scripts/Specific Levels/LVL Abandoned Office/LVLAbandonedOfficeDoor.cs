using System.Collections;
using UnityEngine;

public class LVLAbandonedOfficeDoor : MonoBehaviour, IInteract
{
	Vector3 startLocalEulerAngles;
	Vector3 currentEulerAngles;

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
	[SerializeField] string doorLayer = "Tile Entity Transparent";
    [SerializeField] float activationRadius;

	Vector3 targetEulerAngleRotation;
	[SerializeField] AnimationCurve doorAnimation;

	bool inAnimation;

	[SerializeField] float health = 20, entityDamageAmount = 10;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(MidPos, activationRadius);

		Gizmos.color = new Color(255, 0, 255);
		//Debug.Log(transform.up);
		Gizmos.DrawLine(transform.position, transform.position + transform.up);
	}

	private IEnumerator Start()
	{
		startLocalEulerAngles = transform.eulerAngles;
		currentEulerAngles = transform.eulerAngles;

		yield return new WaitForSeconds(.5f);

		foreach(Transform child in GetComponentsInChildren<Transform>())
		{
			//Debug.Log(child.gameObject.name, child);
			child.gameObject.layer = LayerMask.NameToLayer(doorLayer);
		}
		//Debug.Log(startLocalEulerAngles, gameObject);ee
	}

	public void PlayerInteract(GameObject player)
	{
		if (inAnimation) return;
		//Open
		if (!isOpen)
		{
			Vector3 oldRot = transform.eulerAngles;
			targetEulerAngleRotation = transform.eulerAngles;
			transform.eulerAngles = targetEulerAngleRotation;
			targetEulerAngleRotation += new Vector3(0, 0, 90 * Mathf.Sign(HelperMethods.AngleDir(transform, player.transform)));
			transform.eulerAngles = oldRot;
			//Debug.Log(currentEulerAngles + " | " + targetEulerAngleRotation, gameObject);

			StartCoroutine(DoorAnimation(targetEulerAngleRotation, doorAnimation));

			isOpen = true;
		}
		//Close
		else
		{
			targetEulerAngleRotation = startLocalEulerAngles;

			//Debug.Log(currentEulerAngles + " | " + targetEulerAngleRotation, gameObject);

			StartCoroutine(DoorAnimation(targetEulerAngleRotation, doorAnimation));

			isOpen = false;
		}
	}

	public void EntityInteract(GameObject entity)
	{
		health -= entityDamageAmount;
		//Debug.Log("Hit Taken! Current Health: " + health);

		if (health <= 0) BreakDoor();
	}

	IEnumerator DoorAnimation(Vector3 targetEulerAngleRotation, AnimationCurve animationCurve)
	{
		inAnimation = true;

		//transform.eulerAngles = HelperMethods.Rotation0360(transform.eulerAngles);
		//targetEulerAngleRotation = HelperMethods.Rotation0360(targetEulerAngleRotation);

		float t = 0;
		float animationLength = animationCurve.keys[animationCurve.keys.Length - 1].time;
		Vector3 startEulerAngles = currentEulerAngles;
		//Debug.Log(transform.eulerAngles + " | " + targetEulerAngleRotation + " - " + Mathf.Abs(transform.eulerAngles.z - targetEulerAngleRotation.z) + " vs. " + Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetEulerAngleRotation.z)), gameObject);

		//Euler angles shenanigans AHHHHHHHHHH
		/*while (Mathf.Abs(Mathf.Abs(transform.eulerAngles.z - targetEulerAngleRotation.z) - 90) > 1)
		{
			if (transform.eulerAngles.z < targetEulerAngleRotation.z) transform.eulerAngles += new Vector3(0, 0, 360);
			else targetEulerAngleRotation += new Vector3(0, 0, 360);
		}*/

		while (t < animationLength)
		{
			//Debug.Log(currentEulerAngles);
			currentEulerAngles = Vector3.Lerp(startEulerAngles, targetEulerAngleRotation, animationCurve.Evaluate(t));
			transform.eulerAngles = currentEulerAngles;

			t += Time.deltaTime;
			yield return null;
		}

		transform.eulerAngles = targetEulerAngleRotation;

		inAnimation = false;
	}

	void BreakDoor()
	{
		Destroy(gameObject);
	}
}
