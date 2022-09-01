using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LVLTheEndNote : MonoBehaviour
{
    enum NoteState
	{
        Idle,
        On
	}
    NoteState currentNoteState;

    [Header("References")]
    [SerializeField] SpriteRenderer sr;

    /*[HideInInspector]*/ public int noteNumber;
	/*[HideInInspector]*/ public string correspondingLetter;
	/*[HideInInspector]*/ public int correspondingPasswordCode;

    [Header("Note GFX")]
    [SerializeField] Sprite normalNote;
    [SerializeField] Sprite outlinedNote;
    [SerializeField] TMP_Text noteText;

	private void Start()
	{
        currentNoteState = NoteState.Idle;

        correspondingLetter = ExcelColumnFromNumber(noteNumber);
        correspondingPasswordCode = GetDigitFromInt(LVLTheEndPasswordGenerator.password, noteNumber);
        SetText();
	}

	private void SetText()
	{
        noteText.text = correspondingLetter + "-" + correspondingPasswordCode;
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

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && currentNoteState == NoteState.Idle)
		{
            ToggleText(NoteState.On);
		}
	}

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentNoteState == NoteState.On)
        {
            ToggleText(NoteState.Idle);
        }
    }

    void ToggleText(NoteState newNoteState)
	{
		switch (newNoteState)
		{
            case NoteState.On:                
                sr.sprite = outlinedNote;
                noteText.gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play("LVLTheEnd_Note_Seen");

                currentNoteState = NoteState.On;
                break;
            case NoteState.Idle:
                sr.sprite = normalNote;
                noteText.gameObject.SetActive(false);

                currentNoteState = NoteState.Idle;
                break;

        }
	}
}
