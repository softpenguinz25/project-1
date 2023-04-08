using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LVLTheEndNoteSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleSpawnpoints;
    [SerializeField] GameObject note;

    public event Action<LVLTheEndNote> NoteSpawned;

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
        List<GameObject> chosenSpawnpoints = HelperMethods.GetRandomItemsFromList<GameObject>(possibleSpawnpoints, LVLTheEndPasswordGenerator.NumNotesToSpawn);
        int index = 1;
        foreach(GameObject chosenSP in chosenSpawnpoints)
		{
            GameObject spawnedNote = Instantiate(note, chosenSP.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
			LVLTheEndNote lvlTheEndNote = spawnedNote.GetComponent<LVLTheEndNote>();
			lvlTheEndNote.noteNumber = index;
            NoteSpawned?.Invoke(lvlTheEndNote);

            index++;
		}
	}
}
