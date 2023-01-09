using UnityEngine;

public class MonsterInteract : Interact
{
    [SerializeField] float interactFrequency = 1;
	private void Start()
	{
		InvokeRepeating(nameof(EntityInteract), interactFrequency, interactFrequency);
	}

	void EntityInteract()
	{
		InteractWithEnvironment(InteractType.Entity);
	}
}
