using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisField : MonoBehaviour
{
    public Mesh debrisMesh;
    public Material debrisMaterial;
    public int debrisCount = 1000;

    [Header("Debris Field Shape")]
    public float bottomRadius = 5f;
    public float topRadius = 10f;
    public float height = 5f; 
    public float thickness = 2f; 

    [Header("Rotation Control")]
    public float globalRotationSpeed = 20f;
    public float minSpinSpeed = -30f; 
    public float maxSpinSpeed = 30f;  

    private ComputeBuffer argsBuffer;
    private ComputeBuffer transformBuffer;
    private Matrix4x4[] matrices;
    private float[] spinSpeeds;

    struct DebrisData
    {
        public Matrix4x4 matrix;
    }

    void Start()
    {
        InitializeDebris();
    }

    void InitializeDebris()
    {
        matrices = new Matrix4x4[debrisCount];
        spinSpeeds = new float[debrisCount];
        DebrisData[] debrisData = new DebrisData[debrisCount];

        for (int i = 0; i < debrisCount; i++)
        {
            float yPosition = Random.Range(-height / 2, height / 2);
            float t = Mathf.InverseLerp(-height / 2, height / 2, yPosition);
            float outerRadius = Mathf.Lerp(bottomRadius, topRadius, t);
            float innerRadius = outerRadius - thickness;

            float radius = Random.Range(innerRadius, outerRadius);
            float angle = Random.Range(0f, 360f);
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            position.y = yPosition;

            Quaternion rotation = Random.rotation;
            Vector3 scale = Vector3.one * Random.Range(0.2f, 1f);

            matrices[i] = Matrix4x4.TRS(position, rotation, scale);
            debrisData[i].matrix = matrices[i];

            //Assign random spin speed per instance
            spinSpeeds[i] = Random.Range(minSpinSpeed, maxSpinSpeed);
        }

        transformBuffer = new ComputeBuffer(debrisCount, sizeof(float) * 16);
        transformBuffer.SetData(debrisData);

        debrisMaterial.SetBuffer("_TransformBuffer", transformBuffer);

        uint[] args = new uint[5] { debrisMesh.GetIndexCount(0), (uint)debrisCount, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
    }

    void Update()
    {
        float deltaAngle = globalRotationSpeed * Time.deltaTime;

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 position = matrices[i].GetColumn(3);

            //Orbital movement
            Quaternion globalRotation = Quaternion.Euler(0, deltaAngle, 0);
            position = globalRotation * position;
            matrices[i].SetColumn(3, position);

            //Self-rotation (spinning around own origin)
            Quaternion selfRotation = Quaternion.Euler(0, spinSpeeds[i] * Time.deltaTime, 0);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(selfRotation);
            matrices[i] = matrices[i] * rotationMatrix;
        }

        transformBuffer.SetData(matrices);
        Graphics.DrawMeshInstancedIndirect(debrisMesh, 0, debrisMaterial, new Bounds(Vector3.zero, Vector3.one * (topRadius * 2)), argsBuffer);
    }

    void OnDestroy()
    {
        if (transformBuffer != null) transformBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();
    }
}