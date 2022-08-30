using UnityEngine;

public class LVLTheEndPasswordGenerator : MonoBehaviour
{
    public static int password;

	private void Start()
	{
		GeneratePassword();
	}

	private void GeneratePassword()
	{
		password = Random.Range(1000, 9999);
		Debug.Log("Password Generated: " + password);
	}
}
