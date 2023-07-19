using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] Animator anim;
    
	public void TriggerFlash()
	{
		anim.SetTrigger("Flash");
	}
	
	public void SetFlashLength(AnimationEvent animationEvent)
	{
		if(animationEvent.floatParameter > 0) 
			anim.SetFloat("Flash Speed", 1 / animationEvent.floatParameter);
	}
}
