using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    public List<Boundary> boundaries = new();
    public BoidSettings settings;

    // Start is called before the first frame update
    void Start()
    {
        createBoundaries();
    }

    public void boundaryCheck(ref int boundaryIndex, Vector3 position, GameObject bird)
    {
        var index = getCurrentBoundaryIndex(position);
        if (index == boundaryIndex)
            return;


        boundaries[index].AddBird(bird);
        if (boundaryIndex != -1)
            boundaries[boundaryIndex].RemoveBird(bird); 
        boundaryIndex = index;
    }

    public List<GameObject> closeBirds(int index)
    {
        List<GameObject> birds = new();
        List<Boundary> neighboringBoundries = GetAllNeighbors(index);

        foreach (Boundary neighbor in neighboringBoundries)
        {
            birds.AddRange(neighbor.birdsInBoundary);
        }

        birds.AddRange(boundaries[index].birdsInBoundary);

        return birds; 
    }

    public int getCurrentBoundaryIndex(Vector3 position)
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

    public void removeBirdFromBoundary(int index, GameObject bird)
    {
        boundaries[index].RemoveBird(bird); 
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

    private void OnDrawGizmos()
    {
        if (boundaries.Count == 0) return;

        foreach (Boundary boundary in boundaries)
        {
            Gizmos.color = Color.white;
            if (boundary.birdsInBoundary.Count != 0)
            {
                Gizmos.color = Color.red;
            }
            if (boundary.lightUp)
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawLine(boundary.position, boundary.position + Vector3.forward * boundary.sizeWidth);
            Gizmos.DrawLine(boundary.position, boundary.position + Vector3.right * boundary.sizeWidth);
            Gizmos.DrawLine(boundary.position, boundary.position + Vector3.up * boundary.sizeHeight);
        }

    }
}
