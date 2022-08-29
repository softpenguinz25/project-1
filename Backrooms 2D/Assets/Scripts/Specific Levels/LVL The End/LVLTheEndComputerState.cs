using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LVLTheEndComputerState : MonoBehaviour
{
	SpriteRenderer sr;

    public enum ComputerState
	{
		Idle,
		Active
	}

	public event Action<ComputerState> ComputerStateChanged;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ComputerStateChanged?.Invoke(ComputerState.Active);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ComputerStateChanged?.Invoke(ComputerState.Idle);
		}
	}
}
