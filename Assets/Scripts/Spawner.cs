using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject boidPrefab;
    public BoidSettings settings;
    public GameObject target;
    public bool DrawGizmos = true;

    [HideInInspector] public List<GameObject> boids = new();
    [HideInInspector] public BoundaryController boundaryController; 

    float widthMin = 0f;
    float widthMax = 0f;
    float heightMin = 0f;
    float heightMax = 0f;
    int prevBoidAmount;
    Vector3 lastPos; 

    

    // Start is called before the first frame update
    void Start()
    {  
        widthMax = settings.width;
        heightMax = settings.height;
        prevBoidAmount = settings.boidAmount;
        boundaryController = new BoundaryController(settings);
        boundaryController.spawnerPosition = transform.position;
        lastPos = transform.position;
        SpawnBoids();
    }

    private void Update()
    {
        if (lastPos != transform.position)
        {
            updateMiddlePositions();
            lastPos = transform.position;   
            boundaryController.spawnerPosition = transform.position;
        }
        
        if (settings.boidAmount > prevBoidAmount)
        {
            AddBoids();
            prevBoidAmount = settings.boidAmount;
        }
        else if (settings.boidAmount < prevBoidAmount)
        {
            RemoveBoids();
            prevBoidAmount = settings.boidAmount;
        }
    }

    void updateMiddlePositions()
    {
        foreach (var boid in boids)
        {
            boid.GetComponent<Boid>().middle = transform.position + new Vector3(settings.width, settings.height, settings.width) / 2f;
        }
    }

    private void RemoveBoids()
    {
        List<GameObject> birdsToBeRemoved = new(); 
        for (int i = settings.boidAmount;  i < boids.Count; i++)
        {
            birdsToBeRemoved.Add(boids[i]);
        }

        foreach (GameObject bird in birdsToBeRemoved)
        {
            boids.Remove(bird);
            Destroy(bird);
        }
    }

    private void AddBoids()
    {
        for (int i = 0; i < settings.boidAmount - prevBoidAmount; i++)
        {
            SpawnBoid();
        }

    }
    
    void SpawnBoids()
    {
        for (int i = 0; i < settings.boidAmount; i++)
        {
            SpawnBoid(); 
        }
    }

    void SpawnBoid()
    {
        GameObject boid = Instantiate(boidPrefab);
        boid.transform.SetParent(transform, false);

        float x = Random.Range(widthMin + 10f, widthMax - 10f);
        float z = Random.Range(widthMin + 10f, widthMax - 10f);
        float y = Random.Range(heightMin + 10f, heightMax - 10f);

        boid.transform.position = new Vector3(x, y, z) + transform.position;
        boid.transform.rotation = Random.rotation;
        boid.GetComponent<Boid>().middle = transform.position + new Vector3(settings.width, settings.height, settings.width) / 2f;
        boid.GetComponent<Boid>().speed = Mathf.Max(0.5f, settings.boidSpeed + 2f * Random.value - 1f);

        boids.Add(boid);
    }


    private void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;

        Vector3 pos = transform.position; 
        Vector3 forward = Vector3.forward * settings.width;
        Vector3 right = Vector3.right * settings.width;
        Vector3 up = Vector3.up * settings.height;

        Gizmos.DrawLine(pos, pos + forward);
        Gizmos.DrawLine(pos, pos + right);
        Gizmos.DrawLine(pos + forward, pos + forward + right);
        Gizmos.DrawLine(pos + right, pos + forward + right);

        Gizmos.DrawLine(pos + up, pos + forward + up);
        Gizmos.DrawLine(pos + up, pos + right + up);
        Gizmos.DrawLine(pos + forward + up, pos + forward + right + up);
        Gizmos.DrawLine(pos + right + up, pos + forward + right + up);


        Gizmos.DrawLine(pos, pos + up);
        Gizmos.DrawLine(pos + forward, pos + forward + up);
        Gizmos.DrawLine(pos + right, pos + right + up);
        Gizmos.DrawLine(pos + forward + right, pos + forward + right + up);
    }
}
