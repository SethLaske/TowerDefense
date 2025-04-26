using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineMeshGenerator : MonoBehaviour
{
    public SplineContainer spline;
    public float width = 1.0f; // Width of the path
    public int resolution = 50; // Number of segments

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateMesh();
    }

    void GenerateMesh()
    {
        
        if (spline == null || spline.KnotLinkCollection.Count < 2)
        {
            Debug.LogError("Spline is null or has insufficient points.");
            Debug.Log($"Points: {spline.KnotLinkCollection.Count}");
            return;
        }

        Vector3[] vertices = new Vector3[resolution * 2];
        int[] triangles = new int[(resolution - 1) * 6];

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);
            Vector3 position = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);
            tangent = tangent.normalized;
            Vector3 normal = Vector3.Cross(tangent, Vector3.forward).normalized; // Ensure the width is applied correctly

            vertices[i * 2] = position - normal * (width / 2);
            vertices[i * 2 + 1] = position + normal * (width / 2);

            if (i < resolution - 1)
            {
                int start = i * 2;
                int next = start + 2;

                // First triangle
                triangles[i * 6] = start;
                triangles[i * 6 + 1] = next + 1;
                triangles[i * 6 + 2] = start + 1;

                // Second triangle
                triangles[i * 6 + 3] = start;
                triangles[i * 6 + 4] = next;
                triangles[i * 6 + 5] = next + 1;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        if (triangles.Length == 0)
        {
            Debug.LogError("No triangles were generated for the mesh.");
            return;
        }
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds(); // Forces Unity to correctly compute AABB

    }
}
