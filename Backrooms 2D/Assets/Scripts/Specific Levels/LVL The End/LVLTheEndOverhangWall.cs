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

	private void ChangeTransparencyState(bool isOpaque)
	{
		switch (isOpaque)
		{
			case true:
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
				break;
			case false:
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparentOpacity);
				break;
		}
	}
}
