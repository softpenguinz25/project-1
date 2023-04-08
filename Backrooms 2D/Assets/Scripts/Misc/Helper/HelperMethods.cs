using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

	public static bool isBinary(string s)
	{
		foreach (var c in s)
			if (c != '0' && c != '1')
				return false;
		return true;
	}

	//Thanks Mike Two and AGuyCalledGerald! https://stackoverflow.com/questions/2571716/find-nth-occurrence-of-a-character-in-a-string
	public static int GetNthIndex(string s, char t, int n)
	{
		if (s.Length <= 0 || n < 0) return -1;

		int count = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == t)
			{
				count++;
				if (count == n)
				{
					return i;
				}
			}
		}
		return -1;
	}

	//Thanks Nicholas Miller! https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
	//// str - the source string
	//// index- the start location to replace at (0-based)
	//// length - the number of characters to be removed before inserting
	//// replace - the string that is replacing characters
	public static string ReplaceAt(this string str, int index, int length, string replace)
	{
		return str.Remove(index, Math.Min(length, str.Length - index))
				.Insert(index, replace);
	}

	//THANKS Adriaan Stander! https://stackoverflow.com/questions/1951517/convert-a-to-1-b-to-2-z-to-26-and-then-aa-to-27-ab-to-28-column-indexes-to
	public static string ExcelColumnFromNumber(int column)
	{
		string columnString = "";
		decimal columnNumber = column;
		while (columnNumber > 0)
		{
			decimal currentLetterNumber = (columnNumber - 1) % 26;
			char currentLetter = (char)(currentLetterNumber + 65);
			columnString = currentLetter + columnString;
			columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
		}
		return columnString;
	}

	public static int GetDigitFromInt(int number, int digit)
	{
		string numberString = number.ToString();
		//THANKS Tim Robinson! https://stackoverflow.com/questions/2416894/how-can-i-get-a-character-in-a-string-by-index
		char digitChar = numberString[digit - 1];

		//Thanks JulesG10! https://www.google.com/search?q=c%23+char+to+int&rlz=1C1RXQR_enUS997US997&oq=c%23+char+to+int&aqs=chrome..69i57j0i512l2j0i22i30l4j69i58.9369j0j7&sourceid=chrome&ie=UTF-8
		return (int)char.GetNumericValue(digitChar);
	}

	//THANKS DefianmtMoMo! https://www.google.com/search?q=unity+get+random+values+from+list&rlz=1C1RXQR_enUS997US997&oq=unity+get+random+values+from+list&aqs=chrome..69i57j0i546l5.8266j0j7&sourceid=chrome&ie=UTF-8 (with Grepper chrome extension)
	public static List<T> GetRandomItemsFromList<T>(List<T> list, int number)
	{
		// this is the list we're going to remove picked items from
		List<T> tmpList = new List<T>(list);
		// this is the list we're going to move items to
		List<T> newList = new List<T>();

		// make sure tmpList isn't already empty
		while (newList.Count < number && tmpList.Count > 0)
		{
			int index = Random.Range(0, tmpList.Count);
			newList.Add(tmpList[index]);
			tmpList.RemoveAt(index);
		}

		return newList;
	}
}
