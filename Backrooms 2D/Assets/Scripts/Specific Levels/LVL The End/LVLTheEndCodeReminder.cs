using TMPro;
using UnityEngine;

public class LVLTheEndCodeReminder : MonoBehaviour
{
    LVLTheEndNoteSpawner lvlTheEndNoteSpawner;

	[Header("String")]
	[SerializeField] string format = "X = _";
	[SerializeField] bool isAdditive = true;
	[SerializeField] char numPlaceholder = '_';
	[SerializeField] char charPlaceholder = 'X';

	[Header("GFX")]
    [SerializeField] TextMeshProUGUI codeReminderText;
	[SerializeField] Animator codeReminderAnimator;
	[SerializeField] string codeReminderAnimatorTriggerName = "Remind";
	[SerializeField] string codeReminderAnimatorFadeOutBoolName = "Fade Out";
	int numTimesActivated;

	private void Awake()
	{
		lvlTheEndNoteSpawner = FindObjectOfType<LVLTheEndNoteSpawner>();
	}

	private void OnEnable()
	{
		lvlTheEndNoteSpawner.NoteSpawned += (lvlTheEndNote) => {
			lvlTheEndNote.NoteDiscovered += TriggerCodeReminder;
		};
	}

	private void Start()
	{
		if (!isAdditive) codeReminderText.text = format;
		else codeReminderText.text = "";
	}

	private void TriggerCodeReminder(int codePosition, int codeValue)
	{
		codeReminderText.text = CodeReminderText(codePosition, codeValue);
		codeReminderAnimator.SetTrigger(codeReminderAnimatorTriggerName);

		numTimesActivated++;
		if (numTimesActivated >= LVLTheEndPasswordGenerator.NumNotesToSpawn)
			codeReminderAnimator.SetBool(codeReminderAnimatorFadeOutBoolName, false);
	}

	string CodeReminderText(int codePosition, int codeValue)
	{
		if (!isAdditive)
		{
			int stringIndex = HelperMethods.GetNthIndex(format, numPlaceholder, codePosition);
			if (stringIndex == -1) return "NULL";

			return HelperMethods.ReplaceAt(codeReminderText.text, stringIndex, 1, codeValue.ToString());
		}
		else
		{
			return codeReminderText.text + format.Replace(numPlaceholder, codeValue.ToString()[0]).Replace(charPlaceholder, HelperMethods.ExcelColumnFromNumber(codePosition)[0]);
		}
	}
}
