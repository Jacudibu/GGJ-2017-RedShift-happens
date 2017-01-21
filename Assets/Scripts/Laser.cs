using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class Laser : MonoBehaviour
{
    [SerializeField]
    private Transform origin;

    [SerializeField]
    private const int bufferSize = 200;

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

    [Header("Wobble FX")]
    public float wobbleStrength = 0.5f;
    public float wobbleSpeed = 10f;

    [Header("Collision Stuff")]
    public bool spawnColliders = false;
    public GameObject colliderPrefab;
    private int redLayer;
    private int greenLayer;
    private int blueLayer;
    public float colliderSpawnInterval;

    private float widthHalfed;

    private MeshFilter filter;
    private Mesh mesh;

    private CircularBuffer<Vector3> velocities;
    private CircularBuffer<Vector3> vertices;
    private CircularBuffer<Vector3> normals;
    private CircularBuffer<Vector3> uvs;
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

        redLayer = LayerMask.NameToLayer("RED");
        greenLayer = LayerMask.NameToLayer("GREEN");
        blueLayer = LayerMask.NameToLayer("BLUE");

        if (origin == null)
        {
            Debug.LogWarning("Origin was left blank, using origin of this GameObject instead!");
            origin = transform;
        }

        // SetupParticleSystem();
        InitializeBuffers();

        widthHalfed = width * 0.5f;
        
        triangles = new CircularBuffer<int>((bufferSize-2) * 3);

        StartCoroutine(Coroutine_Wobble());

        if (spawnColliders)
            StartCoroutine(Coroutine_CreateColliders());
	}

    private void InitializeBuffers()
    {
        velocities = new CircularBuffer<Vector3>(bufferSize);
        vertices = new CircularBuffer<Vector3>(bufferSize);
        normals = new CircularBuffer<Vector3>(bufferSize);
        colors = new CircularBuffer<Color>(bufferSize);
    }

    private void SetupParticleSystem()
    {
        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        if (particles != null)
        {
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Mesh;
            shape.mesh = mesh;
        }
    }

    private void Update()
    {
        ApplyVelocities();
        AddNewVertices();
        //SetupUVs();
    }

    public void AddNewVertices()
    {
        float fatness = widthHalfed;

        AddVertex(origin.position - Vector3.right * fatness);
        lastLeftIndex = vertices.IndexOfLastItemAdded;

        AddVertex(origin.position + Vector3.right * fatness);
        lastRightIndex = vertices.IndexOfLastItemAdded;

        mesh.vertices = vertices.Content;
        mesh.triangles = triangles.Content;
        mesh.normals = normals.Content;
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
        normals.Push(-Vector3.forward);
        colors.Push(currentLaserColor);
    }
    static float normalizationValue = 10f; // maximum value that may be held (/will be accounted for) in the player.shifts variable

    static float NORMALIZED_THIRD = normalizationValue/3f;

    private void SetLaserColor()
    {
        float red = 0;
        float green = 0;
        float blue = 0;
        float alpha = 1;
        
        float normalizedShift = player.floatshifts/normalizationValue;
        // calculate red based on normalization
        if (normalizedShift < 0.01) {
            red = 0;
        } else if (normalizedShift < 0.2)
        {
            red = 1; 
        } else if (normalizedShift < 0.5){
            red = Mathf.Cos((normalizedShift - 0.2f) * NORMALIZED_THIRD * Mathf.PI / 2);
        } else if (normalizedShift < 0.7){
            red = 0;
        } else if (normalizedShift < 1.0){
            red = (1-Mathf.Cos((normalizedShift - 0.7f) * NORMALIZED_THIRD * 0.75f * Mathf.PI / 2));
        } else {
            red = 0.75f;
        }

        // calculate green
        if (normalizedShift < 0.2){
            green = 1 - Mathf.Cos(normalizedShift * 5 * Mathf.PI / 2);
        } else if (normalizedShift < 0.6){
            green = 1;
        } else if (normalizedShift < 0.9){
            green = Mathf.Cos((normalizedShift - 0.6f) * NORMALIZED_THIRD * Mathf.PI / 2);
        } else {
            green = 0;
        }

        // calculate blue
        if (normalizedShift < 0.3){
            blue = 0;
        } else if (normalizedShift < 0.6){
            blue = 1 - Mathf.Cos((normalizedShift - 0.3f) * NORMALIZED_THIRD * Mathf.PI / 2);
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

    private void SetupUVs()
    {
        Vector2[] uv = new Vector2[bufferSize];
        float stepSize = 1f / (bufferSize * 0.5f);

        int step = 0;
        for (int i = 0; i < bufferSize; i += 2)
        {
            uv[i] = new Vector2(0f, step * stepSize);
            uv[i+1] = new Vector2(1, step * stepSize);
            step++;
        }

        mesh.uv = uv;
    }

    private IEnumerator Coroutine_Wobble()
    {
        float direction = 1f;
        float t = 0;
        while (true)
        {
            Vector3[] v = vertices.Content;

            for (int i = 0; i < v.Length; i += 2)
            {
                v[i] += Vector3.right * Time.deltaTime * wobbleStrength * direction;
                v[i + 1] += Vector3.left * Time.deltaTime * wobbleStrength * direction;
            }

            t += Time.deltaTime * wobbleSpeed;
            if (t > 1)
            {
                direction = -direction;
                t -= 1f;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Coroutine_CreateColliders()
    {
        while (true)
        {
            yield return new WaitForSeconds(colliderSpawnInterval);
            SpawnCollider(EnemyColor.GREEN);
            /*
            if (currentLaserColor == Color.black)
            {
                continue;
            }
            else if (currentLaserColor == Color.green)
            {
                SpawnCollider(EnemyColor.GREEN);
            }
            else if (currentLaserColor == Color.blue)
            {
                SpawnCollider(EnemyColor.BLUE);
            }
            else if (currentLaserColor == Color.red)
            {
                SpawnCollider(EnemyColor.RED);
            }*/
        }
    }

    private void SpawnCollider(EnemyColor color)
    {
        GameObject coll = GameObject.Instantiate(colliderPrefab, origin.position, Quaternion.identity);
        switch (color)
        {
            case EnemyColor.BLUE:
                coll.layer = blueLayer;
                break;
            case EnemyColor.RED:
                coll.layer = redLayer;
                break;
            case EnemyColor.GREEN:
                coll.layer = greenLayer;
                break;
        }

        coll.GetComponent<LaserCollider>().Init(speed);
    }
}
