using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInstancer : MonoBehaviour
{
    public Mesh grassMesh; // The grass mesh
    public Material grassMaterial; // The material for the grass
    public int instanceCount = 1000; // Number of grass instances
    public Transform surface; // The surface to plant grass on
    public float placementRadius = 10f; // Area within which grass is placed

    private Matrix4x4[] instanceMatrices;
    private List<Vector3> normals;

    void Start()
    {
        // Generate instances
        GenerateGrassInstances();
    }

    void GenerateGrassInstances()
    {
        instanceMatrices = new Matrix4x4[instanceCount];
        normals = new List<Vector3>();

        Mesh surfaceMesh = surface.GetComponent<MeshFilter>().sharedMesh;
        Transform surfaceTransform = surface.transform;

        // Get surface mesh data
        surfaceMesh.GetNormals(normals);

        // Distribute grass instances
        for (int i = 0; i < instanceCount; i++)
        {
            // Randomly select a point on the surface
            int randomIndex = Random.Range(0, surfaceMesh.vertexCount);
            Vector3 vertex = surfaceMesh.vertices[randomIndex];
            Vector3 normal = normals[randomIndex];

            // Transform to world space
            Vector3 worldPosition = surfaceTransform.TransformPoint(vertex);
            Vector3 worldNormal = surfaceTransform.TransformDirection(normal);

            // Align grass to the surface normal
            Quaternion rotation = Quaternion.LookRotation(worldNormal);
            Matrix4x4 matrix = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);

            instanceMatrices[i] = matrix;
        }
    }

    void Update()
    {
        // Draw grass instances
        Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, instanceMatrices);
    }
}
