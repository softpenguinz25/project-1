using UnityEngine;

public class LVLTheEndPasswordGenerator : MonoBehaviour
{
	[SerializeField] int numNotesToSpawn = 4;
	public static int NumNotesToSpawn;
    public static int password;

	private void Awake()
	{
		NumNotesToSpawn = numNotesToSpawn;
	}

	private void Start()
	{
		GeneratePassword();
	}

	private void GeneratePassword()
	{
		password = Random.Range((int)Mathf.Pow(10, numNotesToSpawn - 1), (int)Mathf.Pow(10, numNotesToSpawn) - 1);
		Debug.Log("Password Generated: " + password);
	}
}
