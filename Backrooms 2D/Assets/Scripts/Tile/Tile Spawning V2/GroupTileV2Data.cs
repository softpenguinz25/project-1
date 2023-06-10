using Array2DEditor;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Group Tile", menuName = "Tile/Group Tile", order = 2)]
public class GroupTileV2Data : ScriptableObject
{
	[ShowOnly]
	[SerializeField]
	private string documentation =
		"<i><color=grey>Documentation V1.13</color></i>" +
		"\n" +
		"1010 0001 hall = Hall tile w/ top and bottom walls with a CP on the open left side.\n" +
		"\n" +
		"Tile connections are:\n" +
		"\t-Anything connected to a tile\n" +
		"\t\t-Walls\n" +
		"\t\t-CPs\n" +
		"\t-Are written in this order: <color=yellow><b>TOP, RIGHT, BOTTOM, LEFT</b></color>\n" +
		"\n" +
		"First 4 Digits = Walls\n" +
		"\t-Type \"-\" if there are no Walls for this tile\n" +
		"\t-1010 Means:\n" +
		"\t\t-Top: Wall\n" +
		"\t\t-Right: No Wall\n" +
		"\t\t-Bottom: Wall\n" +
		"\t\t-Left: No Wall\n" +
		"\n" +
		"Second 4 Digits = CPs\n" +
		"\t-Leave this blank if there are no CPs for this tile\n" +
		"\t-0001 Means:\n" +
		"\t\t-Top: No CP\n" +
		"\t\t-Right: No CP\n" +
		"\t\t-Bottom: No CP\n" +
		"\t\t-Left: CP\n" +
		"\n" +
		"Last Word = Specific GameObject To Be Spawned\n" +
		"\t-<b>NOT NECCESARY</b>, only use if tile walls <b>WILL NOT</b> change\n" +
		"\t\t-Tile collection specified will be used if no last word is used\n" +
		"\t-GameObject MUST be in the <color=cyan><b>same folder or subfolder</b></color> of the scriptable object\n" +
		"\t-Only <b>part</b> of the GameObject only needs to contain the word\n" +
		"\t-If multiple GameObjects contain the word, theoretically the first GameObject will be used";

	[Space]
	public TileCollectionV2 tileCollection;
	public Array2DString tileLayout;
	[Tooltip("If you want the group tile to have custom scripting, use this. Leave blank if custom scripting is not required")]
	public GameObject customGO;
	public bool canBeRotated = true;
}
