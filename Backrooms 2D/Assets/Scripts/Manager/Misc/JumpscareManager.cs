using System;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{	
    private Animator animator;

	private void Awake()
	{
        animator = GetComponent<Animator>();
	}
	
	public void PlayJumpscare(JumpscareMonster jumpscareMonsterData)
	{
		Image monsterImage = GameObject.Find(jumpscareMonsterData.monsterName).GetComponent<Image>();
		monsterImage.color = new Color(monsterImage.color.r, monsterImage.color.g, monsterImage.color.b, 1);

		animator.SetTrigger("Jumpscare Template");
		jumpscareMonsterData.jumpscareSFX.PlayOneShot(jumpscareMonsterData.jumpscareSFX.clip);
	}
}

[Serializable] 
public struct JumpscareMonster
{
	public string monsterName;
	public AudioSource jumpscareSFX;
}
