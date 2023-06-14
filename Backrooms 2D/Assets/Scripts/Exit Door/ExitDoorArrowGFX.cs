using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorArrowGFX : MonoBehaviour
{
	[SerializeField] AudioSource arrowSFX;
    public void PlayArrowSFX()
	{
		arrowSFX.Play();
	}
}
