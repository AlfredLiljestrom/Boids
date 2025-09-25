using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController
{
    public List<Boundary> boundaries; 
    BoidSettings settings;

    public Vector3 spawnerPosition; 
    
    public BoundaryController(BoidSettings settings)
    {
        boundaries = new();
        this.settings = settings;
        createBoundaries();
    }

    public void boundaryCheck(ref int boundaryIndex, Vector3 position, GameObject boid)
    {
        position -= spawnerPosition;
        var index = getCurrentBoundaryIndex(position);
        if (index == boundaryIndex)
            return;

        if (index >= boundaries.Count)
        {
            Debug.Log("Out1"); 
            index = boundaries.Count - 1;
        }
        else if (index < 0)
        {
            Debug.Log("Out2");
            index = 0;
        }
            

        boundaries[index].AddBoid(boid);
        if (boundaryIndex != -1)
            boundaries[boundaryIndex].RemoveBoid(boid); 
        boundaryIndex = index;
    }

    public List<GameObject> closeBirds(int index)
    {
        List<GameObject> birds = new();
        List<Boundary> neighboringBoundries = GetAllNeighbors(index);

        foreach (Boundary neighbor in neighboringBoundries)
        {
            birds.AddRange(neighbor.boidsInBoundary);
        }

        birds.AddRange(boundaries[index].boidsInBoundary);

        return birds; 
    }

    int getCurrentBoundaryIndex(Vector3 position)
    {
        Vector3Int boundaryCoord = getBoundaryCoord(position);
        int index = getIndexFromBoundaryCoord(boundaryCoord);
        return index; 
    }

    int getIndexFromBoundaryCoord(Vector3Int boundaryCoord)
    {
        return boundaryCoord.x + boundaryCoord.z * settings.widthRows + boundaryCoord.y * settings.widthRows * settings.widthRows;
    }

    Vector3Int getBoundaryCoord(Vector3 position)
    {
        float xSize = settings.width / settings.widthRows;
        float ySize = settings.height / settings.heightRows;
        float zSize = settings.width / settings.widthRows;

        return new Vector3Int(Mathf.FloorToInt((position.x) / xSize), (int)Mathf.FloorToInt(position.y / ySize), (int)Mathf.FloorToInt((position.z) / zSize));
    }

    public void removeBoidFromBoundary(int index, GameObject boid)
    {
        boundaries[index].RemoveBoid(boid); 
    }

    void createBoundaries()
    {
        boundaries.Clear();
        float xSize = settings.width / settings.widthRows;
        float ySize = settings.height / settings.heightRows;
        float zSize = settings.width / settings.widthRows;

        for (int i = 0; i < settings.heightRows; i++)
        {
            for (int j = 0; j < settings.widthRows; j++)
            {
                for (int k = 0; k < settings.widthRows; k++)
                {
                    Vector3 offset = new Vector3(k * xSize, i * ySize, j * zSize);
                    Boundary boundary = new Boundary(offset, xSize, ySize, new Vector3Int(k, j, i));
                    boundaries.Add(boundary);
                }
            }
        }
    }

    List<Boundary> GetAllNeighbors(int index)
    {
        List<Boundary> neighbors = new();
        Vector3Int coord = boundaries[index].boundaryCord;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    // Skip the cell itself
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    int nx = coord.x + dx;
                    int ny = coord.y + dy;
                    int nz = coord.z + dz;

                    // Check bounds (edge cases)
                    if (nx < 0 || nx >= settings.widthRows) continue;
                    if (ny < 0 || ny >= settings.heightRows) continue;
                    if (nz < 0 || nz >= settings.widthRows) continue;

                    int neighborIndex = getIndexFromBoundaryCoord(new Vector3Int(nx, ny, nz));
                    neighbors.Add(boundaries[neighborIndex]);
                }
            }
        }
        return neighbors;
    }
}
