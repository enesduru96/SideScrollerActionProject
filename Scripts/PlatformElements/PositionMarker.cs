using UnityEngine;

public class PositionMarker : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float arrowSize = 1f;
    [SerializeField] private Mesh arrowMesh;


    private void OnEnable()
    {
        if (WorldObjectPooler.Instance != null)
            WorldObjectPooler.Instance.RegisterPositionMarker(this);
    }

    private void OnDisable()
    {
        if (WorldObjectPooler.Instance != null)
            WorldObjectPooler.Instance.UnregisterPositionMarker(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if (arrowMesh == null)
        {
            arrowMesh = CreateArrowMesh();
        }

        Matrix4x4 gizmoMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * arrowSize);
        Gizmos.matrix = gizmoMatrix;

        Gizmos.DrawMesh(arrowMesh);
    }

    private Mesh CreateArrowMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GizmoArrow2D";


        Vector3[] vertices = {
            new Vector3( 0,  0,  0.5f),
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3( 0.5f, 0, -0.5f),
            new Vector3( 0,  0, -0.2f),
        };

        int[] triangles = {
            0, 1, 3,
            0, 3, 2,

            3, 1, 0,
            2, 3, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}