//THANKS CODEMONKEY! https://www.youtube.com/watch?v=CSeUMTaNFYk

using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    [SerializeField] private float fov = 90;
    [SerializeField] private float viewDistance = 10f;
    private Vector3 origin;
    [SerializeField] private float fovAngle;
    [SerializeField] private LayerMask layerMask;
    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        int rayCount = 50;
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
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                //No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;

            }
            else
            {
                //Hit object
                vertex = raycastHit2D.point;
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
