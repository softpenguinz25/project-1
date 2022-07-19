using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterChaseGFX : MonoBehaviour
{
	private GameObject player;

	[SerializeField] private bool doChaseGFX = true;
	[ConditionalField(nameof(doChaseGFX))] [SerializeField] private float closeThreshold = 10f;
	[ConditionalField(nameof(doChaseGFX))] [SerializeField] private bool checkObstructingObjects = true;
	[ConditionalField(nameof(doChaseGFX))] [SerializeField] private LayerMask obstructingObstaclesMask;

	private void Awake()
	{
		player = FindObjectOfType<PlayerMovement>().gameObject;
	}

	private void Update()
	{
		if (!doChaseGFX) return;

		foreach(MonsterMovement mm in FindObjectsOfType<MonsterMovement>())
		{
			GameObject monster = mm.gameObject;
			if (!checkObstructingObjects) ActivateMonsterChaseGFX();
			else
			{
				var old = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = false;

				if (!Physics2D.Linecast(player.transform.position, monster.transform.position, obstructingObstaclesMask)) ActivateMonsterChaseGFX();

				Physics2D.queriesHitTriggers = old;
			}
		}
	}

	private Volume monsterChasePostProcessing;
	private bool chaseSequenceActivated = false;

	[Space]
	[ConditionalField(nameof(doChaseGFX))] [SerializeField] private AnimationCurve postProcessingWeightCurve;
	[ConditionalField(nameof(doChaseGFX))] [SerializeField] private float timeUntilRestartSequence = 20;

	private void Start()
	{
		monsterChasePostProcessing = GameObject.Find("Monster Chase Post Processing").GetComponent<Volume>();
	}

	private void ActivateMonsterChaseGFX()
	{
		if (chaseSequenceActivated) return;
		chaseSequenceActivated = true;

		FindObjectOfType<AudioManager>().PlayOneShot("Monster_Chase_Activated");
		FindObjectOfType<AudioManager>().PlayOneShot("Monster_Chase_Heartbeat");
		StartCoroutine(PostProcessingCurve());

		Invoke(nameof(RestartChaseSequence), timeUntilRestartSequence);
	}

	private IEnumerator PostProcessingCurve()
	{
		float t = 0;
		while(t < postProcessingWeightCurve.keys[postProcessingWeightCurve.keys.Length - 1].time)
		{
			t += Time.deltaTime;
			monsterChasePostProcessing.weight = postProcessingWeightCurve.Evaluate(t);

			yield return null;
		}
	}

	private void RestartChaseSequence()
	{
		chaseSequenceActivated = false;
	}
}
