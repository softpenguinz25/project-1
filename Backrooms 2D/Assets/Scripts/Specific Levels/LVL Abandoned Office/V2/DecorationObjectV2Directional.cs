using com.spacepuppy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecorationObjectV2Directional : DecorationObjectV2
{
	[ShowOnly] [SerializeField] string instructions = "This Should Be Placed On The <b>\"Up\"</b> Facing Sprite.";
	[SerializeField] GameObject rightGO, downGO, leftGO;

	public override Quaternion GetRotation(TileGOV2 tileGO)
	{
		return Quaternion.identity;
	}

	public override GameObject GetGOToSpawn(TileGOV2 tileGO)
	{
		//Debug.Log(tileGO.transform.eulerAngles.z);
		int directionRotation = Mathf.RoundToInt(tileGO.transform.eulerAngles.z) / 90;
		switch (directionRotation)
		{
			case 0: return gameObject;
			case 1: return rightGO;
			case 2: return downGO;
			case 3: return leftGO;
		}

		Debug.LogError("Could not choose directional decoration orientation.");
		return null;
	}
}
