//THANKS gamesplusjames! https://www.youtube.com/watch?v=8PXPyyVu_6I

using UnityEngine;

public class ScreenShakeController : MonoBehaviour
{
    private float shakeTimeRemaining, shakePower, shakeFadeTime, shakeRotation;

    [SerializeField] float rotationMultiplier;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			StartShake(.5f, 1f);
		}
	}

	private void LateUpdate()
	{
		if(shakeTimeRemaining > 0)
		{
			shakeTimeRemaining -= Time.deltaTime;

			float xAmount = Random.Range(-1f, 1f) * shakePower;
			float yAmount = Random.Range(-1f, 1f) * shakePower;

			transform.position += new Vector3(xAmount, yAmount, 0f);

			shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);

			shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
		}

		transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
	}

	public void StartShake(float length, float power)
	{
		shakeTimeRemaining = length;
		shakePower = power;

		shakeFadeTime = power / length;

		shakeRotation = power * rotationMultiplier;
	}
}
