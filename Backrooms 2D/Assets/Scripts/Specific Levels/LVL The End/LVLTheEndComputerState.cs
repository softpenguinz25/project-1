using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LVLTheEndComputerState : MonoBehaviour
{
	SpriteRenderer sr;

    public enum ComputerState
	{
		Idle,
		Highlight,
		Menu
	}

	#region ComputerState Functions
	[HideInInspector] ComputerState previousState;
	ComputerState currentState;
	public ComputerState CurrentState
	{
		get
		{
			return currentState;
		}
	}
	public void ChangeCurrentState(ComputerState newState)
	{
		currentState = newState;
	}
	public event Action<ComputerState> ComputerStateChanged;
	#endregion

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		previousState = currentState;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			currentState = ComputerState.Highlight;			
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			currentState = ComputerState.Idle;
		}
	}

	private void Update()
	{
		if(currentState != previousState)
		{
			ComputerStateChanged?.Invoke(currentState);
			previousState = currentState;
		}

		switch (currentState)
		{
			case (ComputerState.Highlight):
				if (Input.GetKeyDown(KeyCode.E))
					currentState = ComputerState.Menu;
				break;
		}
	}	
}
