using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLFunSpawner : MonsterSpawner
{
	[Header("LVL Fun Specific Stats")]
	[SerializeField] List<GameObject> initialEnemyOrder;
	[SerializeField] float partyPooperSpawnDelay;
	int currentIteration;

	bool doSecondSpawn;
	
	public override IEnumerator SpawnMonsterAfterDelayCoroutine(GameObject monsterToSpawn = null, bool spawnInstantly = false)
	{
		/*if (currentIteration <= 0)//first monster spawn...
		{
			yield return new WaitForSeconds(partyPooperDelaySpawn);
			SpawnMonster(initialEnemyOrder[currentIteration]);
			currentIteration++;
		}

		else*/
		//Debug.Log(currentIteration);
		if(currentIteration <= 0)
		{
			yield return new WaitForSeconds(partyPooperSpawnDelay);
			SpawnMonster(initialEnemyOrder[currentIteration]);
			currentIteration++;
			yield break;
		}

		/*{*/
			int enemyIndex;
			if (currentIteration <= 1)
			{
				enemyIndex = currentIteration;
			}
			else
			{
				enemyIndex = (currentIteration /*- 1*/) % 2;
			}

			StartCoroutine(base.SpawnMonsterAfterDelayCoroutine(initialEnemyOrder[enemyIndex]));

		if (doSecondSpawn) currentIteration++;
		else doSecondSpawn = true;
			
		/*}*/
	}
}
