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
        float red = 0;
        float green = 0;
        float blue = 0;
        float alpha = 1;
        
        float normalization = 10; // maximum value that may be held (/will be accounted for) in the player.shifts variable

        float normalizedShift = player.shifts/normalization;

        // calculate red based on normalization
        if (normalizedShift == 0f)
        {
            red = 0f;
        }
        else
        if (normalizedShift < 0.2)
        {
            red = 1; 
        } else if (normalizedShift < 0.4){
            red = (0.4f - normalizedShift) * 5 / 2;
        } else if (normalizedShift < 0.8){
            red = 0;
        } else if (normalizedShift < 1.0){
            red = (normalizedShift - 0.8f) * 5 * 0.75f;
        } else {
            red = 0.75f;
        }

        // calculate green
        if (normalizedShift < 0.2){
            green = ((float) normalizedShift) * 5 / 2;
        } else if (normalizedShift < 0.6){
            green = 1;
        } else if (normalizedShift < 0.8){
            green = (0.8f - normalizedShift) * 5;
        } else {
            green = 0;
        }

        // calculate blue
        if (normalizedShift < 0.4){
            blue = 0;
        } else if (normalizedShift < 0.6){
            blue = (normalizedShift - 0.4f) * 5 / 2;
        } else {
            blue = 1;
        }

        // set currentLaserColor
        currentLaserColor = new Color(red, green, blue, alpha);
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
