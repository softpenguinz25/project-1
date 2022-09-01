using UnityEngine;

public class LVLTheEndRoamingPathVisualizer : MonoBehaviour
{
	[Header("References")]
    [SerializeField] PaperMaskMovement pmm;

	[Header("Visualize")]
	[SerializeField] Color pathColor = Color.green;
	private void OnDrawGizmos()
	{
		Gizmos.color = pathColor;
		for (int i = 0; i < pmm.RoamingPoints.Count; i++)
		{
			if (i >= pmm.RoamingPoints.Count - 1)
			{
				Gizmos.DrawLine(pmm.RoamingPoints[i].position, pmm.RoamingPoints[0].position);
				break;
			}

			Gizmos.DrawLine(pmm.RoamingPoints[i].position, pmm.RoamingPoints[i + 1].position);
		}
	}
}
