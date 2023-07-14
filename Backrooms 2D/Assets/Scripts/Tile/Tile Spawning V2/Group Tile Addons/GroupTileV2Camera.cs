using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTileV2Camera : GroupTileV2Addon
{
	[Header("Confiner")]
	[SerializeField] PolygonCollider2D cameraConfiner;
	[SerializeField] bool useConfinerAsTrigger = true;

	[Header("Follow")]
	[SerializeField] bool followPlayer = true;

	[Header("Camera")]
	[SerializeField] CinemachineVirtualCamera virtualCamera;
    protected GameObject virtualCamGO => virtualCamera.gameObject;

	CinemachineBrain activeCinemachineBrain { get => FindObjectOfType<CinemachineBrain>(); }

	public override void Start()
	{
		base.Start();

		virtualCamera.m_Follow = followPlayer ? virtualCamera.m_Follow == null ? GameObject.FindGameObjectWithTag("Player").transform : virtualCamera.m_Follow : null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player") && !collision.isTrigger && useConfinerAsTrigger)
		{
			EnableCamera();
		}
	}

	public void EnableCamera()
	{
		virtualCamGO.SetActive(true);
	}

	public void EnableCamera(float blendTime)
	{
		virtualCamGO.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(ResetBlendTime(activeCinemachineBrain.m_DefaultBlend.BlendTime));
		activeCinemachineBrain.m_DefaultBlend.m_Time = blendTime;
	}

	private IEnumerator ResetBlendTime(float blendTime)
	{
		yield return new WaitForSeconds(activeCinemachineBrain.m_DefaultBlend.BlendTime);
		activeCinemachineBrain.m_DefaultBlend.m_Time = blendTime;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !collision.isTrigger && useConfinerAsTrigger)
		{
			DisableCamera();
		}
	}

	public void DisableCamera()
	{
		virtualCamGO.SetActive(false);
	}

	public void DisableCamera(float blendTime)
	{
		virtualCamGO.SetActive(false);
		StopAllCoroutines();
		StartCoroutine(ResetBlendTime(activeCinemachineBrain.m_DefaultBlend.BlendTime));
		activeCinemachineBrain.m_DefaultBlend.m_Time = blendTime;
	}

	public override void ScaleVarsWithTileSize()
	{
		virtualCamera.m_Lens.OrthographicSize *= TileSpawnerV2.TileSize;
	}
}
