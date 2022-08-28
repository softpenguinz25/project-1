using MyBox;
using UnityEngine;

public class LVLElectricalStationGate : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Animator animator;

	[Header("Gate Variables")]
    [SerializeField] RangedFloat timeBtwnGateActivationBounds;
	[SerializeField] bool isGateOpen = false;
	float currentTimeBtwnGateActivation;

	[Header("GFX")]
	[SerializeField] AudioSource openSFX;
	[SerializeField] AudioSource closeSFX;

	private void Start()
	{
		animator.SetBool("Is Gate Open", isGateOpen);
		currentTimeBtwnGateActivation = Random.Range(timeBtwnGateActivationBounds.Min, timeBtwnGateActivationBounds.Max);
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
		animator.SetBool("Is Gate Open", isGateOpen);

		if (isGateOpen)
		{
			openSFX.Play();
		}
		else
		{
			closeSFX.Play();
		}

		currentTimeBtwnGateActivation = Random.Range(timeBtwnGateActivationBounds.Min, timeBtwnGateActivationBounds.Max);
	}
}
