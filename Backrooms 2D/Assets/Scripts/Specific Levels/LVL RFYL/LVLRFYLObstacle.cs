using System;
using System.Collections;
using UnityEngine;

public class LVLRFYLObstacle : MonoBehaviour
{
	Material spriteRendererMat;

	[Header("Functionality")]
	[SerializeField] AnimationCurve tintCurve;
	bool sequenceTriggered;

	[Header("GFX")]
	[SerializeField] Color tintColor = new Color(1, 1, 1, 0);
	[SerializeField] GameObject destroyParticles;
	[SerializeField] SeparatedSFX destroySFX;

	private void Awake()
	{
		spriteRendererMat = GetComponent<SpriteRenderer>().material;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (sequenceTriggered) return;

		if (collision.gameObject.GetComponent<MonsterMovement>() != null)
		{
			StartCoroutine(DestroyObstacle());
		}
		else if(collision.gameObject.GetComponentInParent<ExitDoor>() != null)
		{
			Destroy(gameObject);
		}
	}

	private IEnumerator DestroyObstacle()
	{
		sequenceTriggered = true;

		destroySFX.Play();

		#region Shader (White Tint)
		float t = 0;

		while (t < tintCurve.keys[tintCurve.length - 1].time)
		{
			t += Time.deltaTime;

			tintColor.a = tintCurve.Evaluate(t) / tintCurve.keys[tintCurve.length - 1].time;
			spriteRendererMat.SetColor("_SpriteTint", tintColor);

			yield return null;
		}
		#endregion

		Instantiate(destroyParticles, transform.position, Quaternion.Euler(-90, 0, 0));
		Destroy(gameObject);
	}
}
