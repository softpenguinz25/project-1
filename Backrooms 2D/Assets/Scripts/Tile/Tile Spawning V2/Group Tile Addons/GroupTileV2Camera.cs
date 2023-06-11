using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2Camera : GroupTileV2Addon
{
	[Header("Confiner")]
	[SerializeField] PolygonCollider2D cameraConfiner;

	[Header("Camera")]
    GameObject virtualCamGO;
	[SerializeField] CinemachineVirtualCamera virtualCamera;

	public override void Start()
	{
		base.Start();

		virtualCamera.m_Follow = GameObject.FindGameObjectWithTag("Player").transform;

		virtualCamGO = virtualCamera.gameObject;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player") && !collision.isTrigger)
		{
			virtualCamGO.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !collision.isTrigger)
		{
			virtualCamGO.SetActive(false);
		}
	}

	public override void ScaleVarsWithTileSize()
	{
		virtualCamera.m_Lens.OrthographicSize *= TileSpawnerV2.TileSize;
	}
}
