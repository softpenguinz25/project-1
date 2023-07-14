using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class CinematicBlackBars : MonoBehaviour
{
	Animator anim;

	float defaultSpeed;
	int defaultAnimationIndex;

	[SerializeField] UnityEvent transitionEvents;
	private void Awake()
	{
		anim = GetComponent<Animator>();
	}
	private void Start()
	{
		defaultSpeed = anim.speed;
		defaultAnimationIndex = (int)anim.GetFloat("Index");
	}
	public void SetAnimationSpeed(float animationSpeed) => anim.speed = animationSpeed;
	public void SetAnimationIndex(int animationIndex) => anim.SetFloat("Index", animationIndex);
	public void TriggerIn()
	{
		anim.SetTrigger("In");
	}
	public void TriggerOut()
	{
		anim.SetTrigger("Out");
	}
	public void TransitionFunctionality() => transitionEvents?.Invoke();
	public void DefaultVars()
	{
		anim.speed = defaultSpeed;
		anim.SetFloat("Index", defaultAnimationIndex);
	}
}
