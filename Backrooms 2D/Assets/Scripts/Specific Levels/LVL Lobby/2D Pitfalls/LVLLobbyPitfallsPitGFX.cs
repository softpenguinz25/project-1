using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LVLLobbyPitfallsPitGFX : MonoBehaviour
{
	SpriteRenderer sr;

	[SerializeField] Sprite midSprite;
    [SerializeField][Tooltip("Open Part on Top")]Sprite[] edgeSprites;
    [SerializeField][Tooltip("Open Part on Top-Left")]Sprite[] cornerSprites;
    [SerializeField] float perspectiveDistanceThreshold;
	[SerializeField] float circleSize = .2f;

	[Tooltip("Determines the direction + severity of the pit (ex: (3, 0) = left pit with steepness of 3)")] Vector2Int pitGFXVector;

	public Transform PlayerCamera { get => CinemachineCore.Instance.GetActiveBrain(0).transform;}

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		perspectiveDistanceThreshold *= TileSpawnerV2.TileSize;

		pitGFXVector = (Vector2Int)Vector3Int.RoundToInt(PlayerCamera.transform.position - transform.position);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector2(-perspectiveDistanceThreshold * (edgeSprites.Length ) + transform.position.x, transform.position.y), new Vector2(perspectiveDistanceThreshold * (edgeSprites.Length ) + transform.position.x, transform.position.y));
		for (int horizontalEdgeSpriteIndex = -edgeSprites.Length ; horizontalEdgeSpriteIndex <= edgeSprites.Length ; horizontalEdgeSpriteIndex++) Gizmos.DrawSphere(new Vector2(horizontalEdgeSpriteIndex * perspectiveDistanceThreshold, 0) + (Vector2)transform.position, perspectiveDistanceThreshold * circleSize);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(new Vector2(transform.position.x, -perspectiveDistanceThreshold * (edgeSprites.Length ) + transform.position.y), new Vector2(transform.position.x, perspectiveDistanceThreshold * (edgeSprites.Length ) + transform.position.y));
		for (int verticalEdgeSpriteIndex = -edgeSprites.Length ; verticalEdgeSpriteIndex <= edgeSprites.Length ; verticalEdgeSpriteIndex++) Gizmos.DrawSphere(new Vector2(0, verticalEdgeSpriteIndex * perspectiveDistanceThreshold) + (Vector2)transform.position, perspectiveDistanceThreshold * circleSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(new Vector2(-perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.x, perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.y), new Vector2(perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.x, -perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.y));
		Gizmos.DrawLine(new Vector2(perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.x, perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.y), new Vector2(-perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.x, -perspectiveDistanceThreshold * (cornerSprites.Length ) + transform.position.y));
		for (int verticalEdgeSpriteIndex = -edgeSprites.Length ; verticalEdgeSpriteIndex <= edgeSprites.Length ; verticalEdgeSpriteIndex++) { Gizmos.DrawSphere(new Vector2(-verticalEdgeSpriteIndex * perspectiveDistanceThreshold, verticalEdgeSpriteIndex * perspectiveDistanceThreshold) + (Vector2)transform.position, perspectiveDistanceThreshold * circleSize); Gizmos.DrawSphere(new Vector2(verticalEdgeSpriteIndex * perspectiveDistanceThreshold, verticalEdgeSpriteIndex * perspectiveDistanceThreshold) + (Vector2)transform.position, perspectiveDistanceThreshold * circleSize); }
	}

	private void Update()
	{
		//Debug.Log(FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject, FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject);
		Vector2Int pitGFXVector = (Vector2Int)Vector3Int.RoundToInt((PlayerCamera.transform.position - transform.position) / perspectiveDistanceThreshold);

		if (Equals(pitGFXVector, this.pitGFXVector)) return;
		this.pitGFXVector = pitGFXVector;

		//Debug.Log(pitGFXVector + " | " + " Mag: " + Mathf.CeilToInt(pitGFXVector.magnitude) + " | " + Vector2.Angle(Vector2.up, pitGFXVector));

		if (Equals(pitGFXVector, new Vector2Int(0, 0))) { sr.sprite = midSprite; return; }
		bool isEdgeSprite = Vector2.Angle(Vector2.up, pitGFXVector) % 90 <= 22.5 || Vector2.Angle(Vector2.up, pitGFXVector) % 90 >= 67.5;
		sr.sprite = isEdgeSprite ? edgeSprites[Mathf.Clamp(Mathf.CeilToInt(pitGFXVector.magnitude) - 1, 0, edgeSprites.Length - 1)] : cornerSprites[Mathf.Clamp(Mathf.CeilToInt(pitGFXVector.magnitude) - 1, 0, cornerSprites.Length - 1)];
		transform.eulerAngles = new Vector3(0, 0, Mathf.Round(Vector2.SignedAngle(isEdgeSprite ? Vector2.up : new Vector2(-1, 1), pitGFXVector) / 90) * 90);
	}
}
