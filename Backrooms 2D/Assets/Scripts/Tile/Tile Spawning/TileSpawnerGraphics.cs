using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TileSpawnerGraphics : MonoBehaviour
{
	private void OnEnable()
	{
		GetComponent<TileSpawner>().TileSpawned += (tile) => { GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip); };
	}
}
