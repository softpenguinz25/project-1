using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LVLFunWallColor : MonoBehaviour
{
	private static List<Color> wallColors = new List<Color>
	{
		Color.red,
		Color.blue,
		Color.green,
		Color.cyan,
		Color.magenta,
		Color.yellow
	};

	private void Start()
	{
		GetComponent<SpriteRenderer>().color = wallColors[Random.Range(0, wallColors.Count)];
	}
}
