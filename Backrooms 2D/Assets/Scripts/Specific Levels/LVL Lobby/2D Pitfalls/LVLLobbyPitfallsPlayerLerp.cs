using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LVLLobbyPitfallsPlayerLerp : MonoBehaviour
{
	[SerializeField] protected AnimationCurve lerpCurve;
	protected IEnumerator InAnimCoroutine(AnimationCurve lerpCurve, Vector3 startPos, Vector3 targetPos, Vector3 startRot, Vector3 targetRot, UnityEvent lerpFinished)
	{
		//Debug.Log(startPos + "" + targetPos + "" + startRot + "" + targetRot);

		float t = 0;
		Keyframe lastKey = lerpCurve.keys[lerpCurve.keys.Length - 1];
		while (t < lastKey.time)
		{
			float interpolateVal = lerpCurve.Evaluate(t / lastKey.time);
			//Debug.Log(t / lastKey.time);
			//Debug.Log(interpolateVal);
			transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, interpolateVal), Quaternion.Euler(Vector3.Lerp(startRot, targetRot, interpolateVal)));
			//Debug.Log(transform.position, this);
			yield return null;
			t += Time.deltaTime;
		}
		//print("finished");
		transform.SetPositionAndRotation(targetPos, Quaternion.Euler(targetRot));

		lerpFinished?.Invoke();
	}
}
