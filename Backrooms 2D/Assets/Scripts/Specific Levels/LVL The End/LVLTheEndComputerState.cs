using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LVLTheEndComputerState : MonoBehaviour
{
	SpriteRenderer sr;
	[SerializeField] Collider2D interactCol;
	[SerializeField] Joystick joystick;

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

	private void OnEnable()
	{
		ComputerStateChanged += (computerState) =>
		{
			if (computerState == ComputerState.Highlight) joystick.Enabled = true;
		};
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
					ActivateComputerMenu();
				else if (Input.GetMouseButtonDown(0))
				{
					//Thanks Kyle Banks! https://kylewbanks.com/blog/unity-2d-detecting-gameobject-clicks-using-raycasts
					Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					mousePos.z = 0;

					if (interactCol.bounds.Contains(mousePos))
					{
						ActivateComputerMenu();
					}
				}
				else if (Input.touchCount > 0)
				{
					Touch touch = Input.GetTouch(0);
					Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
					touchPos.z = 0f;

					if (interactCol.bounds.Contains(touchPos))
					{
						ActivateComputerMenu();
					}
				}

				break;
		}
	}

	private void ActivateComputerMenu()
	{
		joystick.Enabled = false;

		currentState = ComputerState.Menu;
	}
}
