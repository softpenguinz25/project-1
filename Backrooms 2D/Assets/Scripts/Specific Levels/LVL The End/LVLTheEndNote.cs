using System.Linq;
using UnityEngine;

public class LVLTheEndNote : MonoBehaviour
{
    [HideInInspector] public int noteNumber;
	[HideInInspector] public string correspondingLetter;
	[HideInInspector] public int correspondingPasswordCode;

	private void Start()
	{
        correspondingLetter = ExcelColumnFromNumber(noteNumber);
        correspondingPasswordCode = GetDigitFromInt(LVLTheEndPasswordGenerator.password, noteNumber);
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

        return digitChar;
	}
}
