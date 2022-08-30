using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLTheEndNoteSpawner : MonoBehaviour
{
    [SerializeField] int numNotesToSpawn = 4;
    [SerializeField] List<GameObject> possibleSpawnpoints;
    [SerializeField] GameObject note;

    [ButtonMethod]
    public void AutodetectSpawnpoints()
	{
        possibleSpawnpoints.Clear();
        foreach(Transform child in transform)
		{
            possibleSpawnpoints.Add(child.gameObject);
		}
	}

	private void Start()
	{
        List<GameObject> chosenSpawnpoints = GetRandomItemsFromList<GameObject>(possibleSpawnpoints, numNotesToSpawn);
        int index = 1;
        foreach(GameObject chosenSP in chosenSpawnpoints)
		{
            GameObject spawnedNote = Instantiate(note, chosenSP.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            spawnedNote.GetComponent<LVLTheEndNote>().noteNumber = index;

            index++;
		}
	}

	//THANKS DefianmtMoMo! https://www.google.com/search?q=unity+get+random+values+from+list&rlz=1C1RXQR_enUS997US997&oq=unity+get+random+values+from+list&aqs=chrome..69i57j0i546l5.8266j0j7&sourceid=chrome&ie=UTF-8 (with Grepper chrome extension)
	public static List<T> GetRandomItemsFromList<T>(List<T> list, int number)
    {
        // this is the list we're going to remove picked items from
        List<T> tmpList = new List<T>(list);
        // this is the list we're going to move items to
        List<T> newList = new List<T>();

        // make sure tmpList isn't already empty
        while (newList.Count < number && tmpList.Count > 0)
        {
            int index = Random.Range(0, tmpList.Count);
            newList.Add(tmpList[index]);
            tmpList.RemoveAt(index);
        }

        return newList;
    }
}
