using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExitDoorSpawnerTrigger : MonoBehaviour
{
	ExitDoorSpawner eds;
	LevelTransition lt;
	TileDataManagerV2 tdm;

	[Tooltip("Over, then down, DON'T ACCOUNT FOR TILE SIZE")][SerializeField] Vector2Int tilePos;

	static bool hasSpawned = false;

	public void Awake()
	{
		eds = FindObjectOfType<ExitDoorSpawner>();
		lt = FindObjectOfType<LevelTransition>();
		tdm = FindObjectOfType<TileDataManagerV2>();
	}

	public void OnEnable()
	{
		EditorApplication.playModeStateChanged += (playmodeStateChange) =>
		{
			if (playmodeStateChange == PlayModeStateChange.ExitingPlayMode)
			{
				hasSpawned = false;
			}
		};
		lt.LoadNextLevel += (int sceneIndex) => { hasSpawned = false; };
	}

	private void Start()
	{
		if (hasSpawned) return;
		Vector2Int groupTilePivot = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
		Vector2Int tilePosDifference = new Vector2Int(tilePos.x * TileSpawnerV2.TileSize, -tilePos.y * TileSpawnerV2.TileSize);
		//Debug.Log(groupTilePivot + " | " + tilePosDifference);
		eds.SpawnDoor(tdm.TileDict[groupTilePivot + tilePosDifference]);
		hasSpawned = true;
	}

	//[Tooltip("Over, then down, DON'T ACCOUNT FOR TILE SIZE")][SerializeField] Vector2Int tilePos;
	/*public override void SpawnDoorMechanism()
	{
		if (hasSpawned) return;

		Vector2Int groupTilePivot = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
		//Debug.Log(groupTilePivot);
		Vector2Int tilePosDifference = new Vector2Int(tilePos.x * TileSpawnerV2.TileSize, -tilePos.y * TileSpawnerV2.TileSize);
		SpawnDoor(tdm.TileDict[groupTilePivot + tilePosDifference]);
		hasSpawned = true;
	}*/
}
