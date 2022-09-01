using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLFunSpawner : MonsterSpawner
{
	[Header("LVL Fun Specific Stats")]
	[SerializeField] List<GameObject> initialEnemyOrder;
	[SerializeField] float partyPooperDelaySpawn;
	int currentIteration;
	
	public override IEnumerator SpawnMonsterAfterDelayCoroutine(GameObject monsterToSpawn = null)
	{
		if (currentIteration <= 0)
		{
			yield return new WaitForSeconds(partyPooperDelaySpawn);
			SpawnMonster(initialEnemyOrder[currentIteration]);
			currentIteration++;
		}
		else
		{
			int enemyIndex;
			if (currentIteration <= 1)
			{
				enemyIndex = 1;
			}
			else
			{
				enemyIndex = (currentIteration - 1) % 2;
			}

			StartCoroutine(base.SpawnMonsterAfterDelayCoroutine(initialEnemyOrder[enemyIndex]));
			currentIteration++;
		}
	}
}
