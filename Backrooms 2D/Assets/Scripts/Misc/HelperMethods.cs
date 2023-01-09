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
		Vector3 localPos = A.InverseTransformPoint(B.transform.position);
		//Rotate localPos based on rotation of Transform A
		//localPos = Quaternion.AngleAxis(Quaternion.Inverse(A.transform.rotation).eulerAngles.z, Vector2.up) * localPos;
		//Debug.Log(Quaternion.Inverse(A.transform.rotation).eulerAngles.z, A);
		//Debug.Log(localPos, A);
		return -localPos.y;
	}

	public static Vector3 Rotation0360(Vector3 vector3)
	{
		float x = vector3.x; while (x < 0) x += 360;while (x > 360) x -= 360;
		float y = vector3.y; while (y < 0) y += 360; while (y > 360) y -= 360;
		float z = vector3.z; while (z < 0) z += 360; while (z > 360) z -= 360;

		return new Vector3(x, y, z);
	}
}
