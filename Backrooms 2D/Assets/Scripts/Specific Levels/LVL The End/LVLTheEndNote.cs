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

    bool isNoteDiscovered;
    public event Action<int, int> NoteDiscovered;

	private void Start()
	{
        currentNoteState = NoteState.Idle;

        correspondingLetter = HelperMethods.ExcelColumnFromNumber(noteNumber);
        correspondingPasswordCode = HelperMethods.GetDigitFromInt(LVLTheEndPasswordGenerator.password, noteNumber);
        SetText();

        float zRotation = transform.eulerAngles.z;
        while (zRotation > 90) zRotation -= 180;
        while (zRotation < -90) zRotation += 180;
        noteText.transform.rotation = Quaternion.Euler(0, 0, zRotation);
	}

	private void SetText()
	{
        noteText.text = correspondingLetter + "-" + correspondingPasswordCode;
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

                if (!isNoteDiscovered)
                {
                    NoteDiscovered?.Invoke(noteNumber, correspondingPasswordCode);
                    isNoteDiscovered = true;
				}                
                break;
            case NoteState.Idle:
                sr.sprite = normalNote;
                noteText.gameObject.SetActive(false);

                currentNoteState = NoteState.Idle;
                break;

        }
	}
}
