using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Laser : MonoBehaviour
{
    [SerializeField]
    private Transform origin;

    [SerializeField]
    private const int bufferSize = 300;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    [Range(0f, 2f)]
    private float velocityInheritanceFactor = 0.2f;

    [SerializeField]
    private float width = 10f;

    private float widthHalfed;

    private MeshFilter filter;
    private Mesh mesh;
    private CircularBuffer<Vector3> vertices;
    private CircularBuffer<int> triangles;
    private CircularBuffer<Vector3> velocities;

    private Vector3 lastPosition;

    private int lastLeftIndex, lastRightIndex;

	void Start ()
    {
        mesh = new Mesh();
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;

        if (origin == null)
        {
            Debug.LogWarning("Origin was left blank, using origin of this GameObject instead!");
            origin = transform;
        }

        lastPosition = origin.position;

        widthHalfed = width * 0.5f;
        velocities = new CircularBuffer<Vector3>(bufferSize);
        vertices = new CircularBuffer<Vector3>(bufferSize);
        triangles = new CircularBuffer<int>((bufferSize-2) * 3);
	}

    public void Update()
    {
        ApplyVelocities();
        AddNewVertices();

        lastPosition = origin.position;
    }

    public void AddNewVertices()
    {
        AddVertex(origin.position - Vector3.right * widthHalfed);
        lastLeftIndex = vertices.IndexOfLastItemAdded;

        AddVertex(origin.position + Vector3.right * widthHalfed);
        lastRightIndex = vertices.IndexOfLastItemAdded;

        mesh.vertices = vertices.Content;
        mesh.triangles = triangles.Content;
    }

    private void AddVertex(Vector3 vertex)
    {
        vertices.Push(vertex);

        if (lastLeftIndex != lastRightIndex)
        {
            triangles.Push(vertices.IndexOfLastItemAdded);
            triangles.Push(lastLeftIndex);
            triangles.Push(lastRightIndex);
        }

        velocities.Push((origin.position - lastPosition) * velocityInheritanceFactor + origin.forward * speed);
    }

    private void ApplyVelocities()
    {
        Vector3[] v = vertices.Content;

        for (int i = 0; i < v.Length; i++)
        {
            v[i] = v[i] + velocities.Content[i] * Time.deltaTime;
        }
    }
}
