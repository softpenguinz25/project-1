using System;
using UnityEngine;

public class LVLTheEndOverhangWall : MonoBehaviour
{
	SpriteRenderer sr;

	[Range(0,1)] [SerializeField] float transparentOpacity = .1f;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ChangeTransparencyState(false);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ChangeTransparencyState(true);
		}
	}

	bool hasPlayedSFX;
	private void ChangeTransparencyState(bool isOpaque)
	{
		switch (isOpaque)
		{
			case true:
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);

				if (hasPlayedSFX)
				{
					FindObjectOfType<AudioManager>().Play("LVLTheEnd_Overhang_Opaque");
					hasPlayedSFX = false;
				}				
				break;
			case false:
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparentOpacity);

				if (!hasPlayedSFX)
				{
					FindObjectOfType<AudioManager>().Play("LVLTheEnd_Overhang_Transparent");
					hasPlayedSFX = true;
				}
				break;
		}
	}
}
