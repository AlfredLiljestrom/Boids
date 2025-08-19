using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject birdPrefab;
    public BoidSettings settings;

    public List<GameObject> birds = new(); 
    public GameObject meshCreator;

    public GameObject target; 

    

    float widthMin = 0f;
    float widthMax = 0f;
    float heightMin = 0f;
    float heightMax = 0f;
    int prevBirdAmount;

    

    // Start is called before the first frame update
    void Start()
    {
        widthMax = settings.width;
        heightMax = settings.height;
        prevBirdAmount = settings.birdAmount;

        SpawnBirds();
    }

    private void Update()
    {
        if (settings.birdAmount > prevBirdAmount)
        {
            AddBirds();
            prevBirdAmount = settings.birdAmount;
        }
        else if (settings.birdAmount < prevBirdAmount)
        {
            RemoveBirds();
            prevBirdAmount = settings.birdAmount;
        }
    }

    private void RemoveBirds()
    {
        List<GameObject> birdsToBeRemoved = new(); 
        for (int i = settings.birdAmount;  i < birds.Count; i++)
        {
            birdsToBeRemoved.Add(birds[i]);
        }

        foreach (GameObject bird in birdsToBeRemoved)
        {
            birds.Remove(bird);
            Destroy(bird);
        }
    }

    private void AddBirds()
    {
        List<GameObject> birdsToAdded = new();
        for (int i = 0; i < settings.birdAmount - prevBirdAmount; i++)
        {
            SpawnBird();
        }

    }

    int getIndex(Vector3Int bc)
    {
        return bc.x * settings.widthRows + bc.y * settings.heightRows + bc.z * settings.widthRows;
    }
    

    void SpawnBirds()
    {
        for (int i = 0; i < settings.birdAmount; i++)
        {
            SpawnBird(); 
        }
    }

    void SpawnBird()
    {
        GameObject bird = Instantiate(birdPrefab);
        bird.transform.SetParent(transform, false);

        float x = Random.Range(widthMin + 10f, widthMax - 10f);
        float z = Random.Range(widthMin + 10f, widthMax - 10f);
        float y = Random.Range(heightMin + 10f, heightMax - 10f);

        bird.transform.position = new Vector3(x, y, z);
        bird.transform.rotation = Random.rotation;

        birds.Add(bird);
    }

    

    
   



}
