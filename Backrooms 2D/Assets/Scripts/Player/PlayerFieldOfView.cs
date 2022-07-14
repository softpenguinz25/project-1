using UnityEngine;

public class PlayerFieldOfView : MonoBehaviour
{
    private FieldOfView fov;

	private void Awake()
	{
		fov = FindObjectOfType<FieldOfView>();

		if (fov == null) Debug.LogError("Count not find FieldOfView!");
	}

	private void Update()
	{
		if (fov == null) return;

		fov.SetOrigin(transform.position);
		//fov.SetAimDirection();
	}
}
