public static class HelperMethods
{
    public static float GetPositiveAngle(float angle)
	{
		float result = angle;
		while (result < 0) result += 360;
		return result;
	}
}
