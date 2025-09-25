using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary
{
    public Vector3 position;
    public Vector3Int boundaryCord;
    public float sizeWidth;
    public float sizeHeight;
    public List<GameObject> boidsInBoundary = new();
    public bool lightUp = false; 

    public Boundary(Vector3 position, float sizeWidth, float sizeHeight, Vector3Int boundaryCord)
    {
        this.position = position;
        this.sizeWidth = sizeWidth;
        this.sizeHeight = sizeHeight;
        this.boundaryCord = boundaryCord;
    }

    public void AddBoid(GameObject boid)
    {
        boidsInBoundary.Add(boid);
    }

    public void RemoveBoid(GameObject boid)
    {
        boidsInBoundary.Remove(boid);
    }
}
