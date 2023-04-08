using MyBox;
using UnityEngine;

public class LVLElectricalStationGate : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Animator animator;

	[Header("Gate Variables")]
    [SerializeField] RangedFloat gateOpenState;
    [SerializeField] RangedFloat gateCloseState;
	 bool isGateOpen;
	float currentTimeBtwnGateActivation;

	[Header("GFX")]
	[SerializeField] AudioSource openSFX;
	[SerializeField] AudioSource closeSFX;

	private void Start()
	{
		isGateOpen = Random.value < .5f;

		animator.SetBool("Is Gate Open", isGateOpen);
		currentTimeBtwnGateActivation = Random.Range(gateOpenState.Min, gateOpenState.Max);
	}

	private void Update()
	{
		if(currentTimeBtwnGateActivation > 0)
		{
			currentTimeBtwnGateActivation -= Time.deltaTime;
		}
		else
		{
			ActivateGate();
		}
	}

	private void ActivateGate()
	{		
		isGateOpen = !isGateOpen;
		if (animator.GetBool("Is Gate Open") != isGateOpen)
		{
			animator.SetBool("Is Gate Open", isGateOpen);

			if (isGateOpen)
			{
				openSFX.Play();
			}
			else
			{
				closeSFX.Play();
			}
		}

		RangedFloat timeBtwnGateToggle = isGateOpen ? gateOpenState : gateCloseState;
		currentTimeBtwnGateActivation = Random.Range(timeBtwnGateToggle.Min, timeBtwnGateToggle.Max);
	}
}
