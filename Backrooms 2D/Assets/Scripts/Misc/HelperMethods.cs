using UnityEngine;

public static class HelperMethods
{
    public static float GetPositiveAngle(float angle)
	{
		float result = angle;
		while (result < 0) result += 360;
		return result;
	}

	//Thank you yoyo! https://forum.unity.com/threads/left-right-test-function.31420/
	//returns negative when to the left, positive to the right
	public static float AngleDir(Transform A, Transform B)
	{
		Vector3 localPos = B.InverseTransformPoint(A.transform.position);
		return localPos.x;
	}
}
