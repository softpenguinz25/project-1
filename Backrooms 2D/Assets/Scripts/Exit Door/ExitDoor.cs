using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExitDoor : MonoBehaviour
{
	[Header("Door Spawn")]
	[SerializeField] private float raycastCheckRotationDistance = .5f;
	[SerializeField] private LayerMask tileMask;
	private bool isInDoor;	

	[Header("Player In Door")]
	[SerializeField] private GameObject fourthWall;

	[Header("GFX")]
	[SerializeField] AudioSource elevatorSFX;
	[SerializeField] AudioMixer mainMixer;
	[SerializeField] float targetLowpassCutoffFrequency = 1125f;
	[SerializeField] AnimationCurve lowpassCurve;

	private const float angleOffset = 90;
	private void Start()
	{		
		fourthWall.SetActive(false);
		InvokeRepeating(nameof(CorrectRotation), 0, 1f);
	}

	[ContextMenu("Correct Rotation")]
	public void CorrectRotation()
	{
		if (isInDoor) return;

		//Rotate towards player
		Vector3 targetPos = FindObjectOfType<PlayerMovement>().transform.position;
		Vector2 direction = targetPos - transform.position;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		Vector3 eulerRot = Vector3.forward * (angle + angleOffset);
		transform.rotation = Quaternion.Euler(eulerRot);

		//Round to nearest 90 deg
		transform.eulerAngles = new Vector3(Mathf.Round(transform.eulerAngles.x / 90) * 90, Mathf.Round(transform.eulerAngles.y / 90) * 90, Mathf.Round(transform.eulerAngles.z / 90) * 90);

		//Rotate if is hitting a wall
		var old = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = false;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
		
		//Debug.Log("0 | " + transform.eulerAngles.z);
		int i = 1;

		if (hit.collider != null)
		{
			while (hit.collider != null)
			{
				switch (i)
				{
					case 1:
						transform.eulerAngles += new Vector3(0, 0, 90 * eulerRot.z >= transform.rotation.z ? -1 : 1);
						hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
						break;
					case 2:
						transform.eulerAngles -= new Vector3(0, 0, 180 * eulerRot.z >= transform.rotation.z ? -1 : 1);
						hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
						break;
					case 3:
						transform.eulerAngles += new Vector3(0, 0, 270 * eulerRot.z >= transform.rotation.z ? -1 : 1);
						hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
						break;
					case 4:
						Debug.LogError("No Valid Exit Door Rotation Found!");
						break;
				}
				//Debug.Log(i + " | " + transform.eulerAngles.z);

				if (hit.collider != null)
				{
					//Debug.Log("Valid Rotation Found!");
					break;
				}
				if (i >= 4)
				{
					break;
				}
				else
				{
					i++;
				}
			}
		}

		Physics2D.queriesHitTriggers = old;

		/*while (hit.collider != null && !hit.collider.isTrigger)
		{
			transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
			hit = Physics2D.Raycast(transform.position, transform.up, raycastCheckRotationDistance, tileMask);
		}*/
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			isInDoor = true;
			ExecuteEndingSequence();
		}
	}

	private void ExecuteEndingSequence()
	{
		fourthWall.SetActive(true);

		elevatorSFX.Play();
		StartCoroutine(MuffleSounds());
		StartCoroutine(FindObjectOfType<LevelTransition>().PlayTransitionCoroutine());
	}

	private IEnumerator MuffleSounds()
	{			
		float t = 0;
		while(t < lowpassCurve.keys[lowpassCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;

			for (int i = 0; i < AudioDataResetter.lowpassParams.Count; i++)
			{
				//Debug.Log("Evalutaintg Curve");
				mainMixer.SetFloat(AudioDataResetter.lowpassParams[i], Mathf.Lerp(targetLowpassCutoffFrequency, AudioDataResetter.startingLowpassFrequencies[i], lowpassCurve.Evaluate(t)));
			}				

			yield return null;
		}

		for (int i = 0; i < AudioDataResetter.lowpassParams.Count; i++)
		{
			mainMixer.SetFloat(AudioDataResetter.lowpassParams[i], targetLowpassCutoffFrequency);
		}
	}

	private void Update()
	{
		Debug.DrawRay(transform.position, -transform.up * raycastCheckRotationDistance, Color.red);
	}
}
