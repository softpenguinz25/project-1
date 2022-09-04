using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLFunSpawner : MonsterSpawner
{
	[Header("LVL Fun Specific Stats")]
	[SerializeField] List<GameObject> initialEnemyOrder;
	[SerializeField] float partyPooperSpawnDelay;
	int currentIteration;

	//bool doSecondSpawn;
	
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
			StartCoroutine(base.SpawnMonsterAfterDelayCoroutine(initialEnemyOrder[0], true));//dont need to check if monster is chasing player bc this is the first monster spawn (and a monster cant chase the palyer if not mosnter exeiststs ahahhahahha)
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
				enemyIndex = (currentIteration - 1) % 2;
			}
			//Debug.Log(currentIteration + " --> " + enemyIndex + " | " + initialEnemyOrder[enemyIndex]);

		/*if (GetComponent<MonsterSpawner>().IsMonsterChasingPlayer())
		{
			Debug.Log("Cannot spawn monster, as a monster is chasing the player!");
			StartCoroutine(RestartSystem());
			yield break;
		}*/

		StartCoroutine(base.SpawnMonsterAfterDelayCoroutine(initialEnemyOrder[enemyIndex], spawnInstantly));
		currentIteration++;

		/*if (doSecondSpawn) currentIteration++;
		else doSecondSpawn = true;*/
			
		/*}*/
	}
}
