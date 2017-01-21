using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Laser : MonoBehaviour
{
    [SerializeField]
    private Transform origin;

    [SerializeField]
    private const int bufferSize = 500;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    [Range(0f, 2f)]
    private float velocityInheritanceFactor = 0.2f;

    [SerializeField]
    private float width = 10f;

    [SerializeField]
    [Range(1, 10)]
    private float colorFactorMultiplier;

    private float widthHalfed;

    private MeshFilter filter;
    private Mesh mesh;

    private CircularBuffer<Vector3> velocities;
    private CircularBuffer<Vector3> vertices;
    private CircularBuffer<int> triangles;
    private CircularBuffer<Color> colors;

    private int lastLeftIndex, lastRightIndex;

    private PlayerController player;
    
    private Color currentLaserColor = Color.black;

	void Start ()
    {
        mesh = new Mesh();
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;

        player = FindObjectOfType<PlayerController>();
        player.OnShiftHappens += SetLaserColor;
        player.OnNoShiftHappens += SetLaserColor;
        if (origin == null)
        {
            Debug.LogWarning("Origin was left blank, using origin of this GameObject instead!");
            origin = transform;
        }

        widthHalfed = width * 0.5f;
        velocities = new CircularBuffer<Vector3>(bufferSize);
        vertices = new CircularBuffer<Vector3>(bufferSize);
        colors = new CircularBuffer<Color>(bufferSize);

        triangles = new CircularBuffer<int>((bufferSize-2) * 3);
	}

    public void Update()
    {
        ApplyVelocities();
        AddNewVertices();
    }

    public void AddNewVertices()
    { 
        AddVertex(origin.position - Vector3.right * widthHalfed);
        lastLeftIndex = vertices.IndexOfLastItemAdded;

        AddVertex(origin.position + Vector3.right * widthHalfed);
        lastRightIndex = vertices.IndexOfLastItemAdded;

        mesh.vertices = vertices.Content;
        mesh.triangles = triangles.Content;
        mesh.colors = colors.Content;
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

        velocities.Push(player.Velocity * velocityInheritanceFactor + origin.forward * speed);
        colors.Push(currentLaserColor);
    }

    private void SetLaserColor()
    {
        if (player.shifts < 1)
        {
            currentLaserColor = Color.black;
        }
        else if (player.shifts < 4)
        {
            currentLaserColor = Color.red;
        }
        else if (player.shifts < 8)
        {
            currentLaserColor = Color.green;
        }
        else
        {
            currentLaserColor = Color.blue;
        }
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
