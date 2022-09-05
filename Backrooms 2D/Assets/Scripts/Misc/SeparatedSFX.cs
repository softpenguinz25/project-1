using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SeparatedSFX : MonoBehaviour
{
	AudioSource audioSource;
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}
	public void Play()
	{
		transform.parent = null;
		audioSource.Play();
		Destroy(gameObject, audioSource.clip.length);
	}
}
