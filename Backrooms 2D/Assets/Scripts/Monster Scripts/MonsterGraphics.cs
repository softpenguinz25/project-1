using System;
using UnityEngine;

public class MonsterGraphics : MonoBehaviour
{
	private MonsterMovement mm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	[Header("Sprite")]
	[SerializeField] private Sprite[] playerPoses = new Sprite[4];
	[SerializeField] private float spriteThreshold = .4f;

	private void Awake()
	{
		mm = GetComponent<MonsterMovement>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		sr.sprite = playerPoses[2];
	}

	private void Update()
	{
		if (rb.velocity.y > spriteThreshold) sr.sprite = playerPoses[0];
		else if (rb.velocity.y < -spriteThreshold) sr.sprite = playerPoses[2];
		else if (rb.velocity.x > spriteThreshold) sr.sprite = playerPoses[1];
		else if (rb.velocity.x < -spriteThreshold) sr.sprite = playerPoses[3];

		/*Vector2 roundedForce = new Vector2(Math.Sign(mm.Force.x), Math.Sign(mm.Force.y));
		Debug.Log(roundedForce);
		if (roundedForce.y > 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[0];
		else if (roundedForce.y < 0 && Mathf.Abs(roundedForce.x) < 0) sr.sprite = playerPoses[2];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x > 0) sr.sprite = playerPoses[1];
		else if (Mathf.Abs(roundedForce.y) < 0 && roundedForce.x < 0) sr.sprite = playerPoses[3];*/
	}
}
