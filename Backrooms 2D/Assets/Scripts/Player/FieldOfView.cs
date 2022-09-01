//THANKS CODEMONKEY! https://www.youtube.com/watch?v=CSeUMTaNFYk

using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    [SerializeField] private float fov = 90;
    [SerializeField] private float viewDistance = 10f;
    private Vector3 origin;
    [SerializeField] private float fovAngle;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private int rayCount = 800;

    [Header("Hello darkness my old friend")]
    [SerializeField] private float wallAccuracy = .05f;

    [Header("Exceptions")]
    List<Collider2D> excludedColliders = new List<Collider2D>();
    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        float angle = fovAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] verticies = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[rayCount * 3];

        verticies[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            var old = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;

            Vector3 vertex;
            Vector3 raycastDir = GetVectorFromAngle(angle);
            RaycastHit2D initialRaycastHit2D = Physics2D.Raycast(origin, raycastDir, viewDistance, tileMask);
            Vector2 pointThroughWall = initialRaycastHit2D.point + .01f * (Vector2)raycastDir;
			while (Physics2D.OverlapPoint(pointThroughWall, tileMask)/* == initalRaycastHit2D.collider*/)
			{
				/*GameObject initialWall = initalRaycastHit2D.collider.gameObject;
				GameObject wallThroughInitialWall = Physics2D.OverlapPoint(pointThroughWall).gameObject;
				if (wallThroughInitialWall.gameObject != initialWall)
				{
					float wallRotation = wallThroughInitialWall.transform.eulerAngles.z;
					Vector2 wallPosition = wallThroughInitialWall.transform.position;
					bool isSandwichWall = (Mathf.Abs(Mathf.Abs(wallRotation) - Mathf.Abs(initialWall.transform.eulerAngles.z)) < .1f || (Mathf.Abs(Mathf.Abs(wallRotation) - Mathf.Abs(initialWall.transform.eulerAngles.z)) < 180.1f && Mathf.Abs(Mathf.Abs(wallRotation) - Mathf.Abs(initialWall.transform.eulerAngles.z)) > 179.9f)) && Vector2.Distance(wallPosition, initialWall.transform.position) < .5f;
					if(!isSandwichWall)
						break;
				}
	*/
				pointThroughWall += wallAccuracy * (Vector2)raycastDir;
		    }

		    Physics2D.queriesHitTriggers = old;

            //Vector2 extendedPoint = initalRaycastHit2D.point + raycastExtend * (Vector2)raycastDir;
            //RaycastHit2D[] raycastHit2DThroughWallCols = Physics2D.RaycastAll(extendedPoint, -raycastDir, viewDistance, layerMask);

            if (initialRaycastHit2D.collider == null)
            {
                //No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;

            }
            else
			{
				//Hit object
				#region Calculate Collider Offset
				//initalRaycastHit2D = CalculateColliderOffset(initalRaycastHit2D);

				//Vector2 colliderHitPos = (Vector2)initalRaycastHit2D.collider.transform.position + colliderOffset;
				//Thanks Threeli, renman3000, outsky! https://forum.unity.com/threads/place-an-object-on-the-opposite-side-of-another-from-a-third.143509/
				//Vector2 pointThroughWall = initalRaycastHit2D.point + initalRaycastHit2D.point - colliderHitPos;
				#endregion

				/*RaycastHit2D otherSideOfWall = new RaycastHit2D();
				foreach (RaycastHit2D raycastHit2D in raycastHit2DThroughWallCols)
				{
					if (raycastHit2D.collider == initalRaycastHit2D.collider)
					{
						otherSideOfWall = raycastHit2D;
						break;
					}
					else if (Vector2.Distance(raycastHit2D.point + CalculateColliderOffset(raycastHit2D), initalRaycastHit2D.point + CalculateColliderOffset(initalRaycastHit2D)) < 1.01f)
					{
                        otherSideOfWall = raycastHit2D;
                        break;
                    }
				}*/

				vertex = pointThroughWall;
			}

			verticies[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 10000f);
    }

	private static Vector2 CalculateColliderOffset(RaycastHit2D raycastHit2D)
	{
		Vector2 colliderOffset = raycastHit2D.collider.offset;
		GameObject colliderTile = raycastHit2D.collider.gameObject;

		if (Mathf.Abs(colliderTile.transform.eulerAngles.z - 90) < .01f) colliderOffset = new Vector2(colliderOffset.y, -colliderOffset.x);
		else if (Mathf.Abs(colliderTile.transform.eulerAngles.z + 180) < .01f) colliderOffset *= -1;
		else if (Mathf.Abs(colliderTile.transform.eulerAngles.z + 90) < .01f) colliderOffset = new Vector2(-colliderOffset.y, colliderOffset.x);
		//else Debug.LogError("Rotation Checks didn't work :sadge:");


		//      Forward
		//
		//      (x,y)
		//
		//(-y,x)        (y,-x)
		//
		//      (-x,-y)
		return colliderOffset;
	}

	public void SetOrigin(Vector3 origin)
	{
        this.origin = origin;
	}

    public void SetAimDirection(Vector3 aimDir)
	{
        fovAngle = GetAngleFromVectorFloat(aimDir) - fov / 2f;
	}

	#region Helper Functions
	private Vector3 GetVectorFromAngle(float angle)
	{
        //angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

    private float GetAngleFromVectorFloat(Vector3 dir)
	{
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n > 0) n += 360;

        return n;
	}
	#endregion
}
