using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    Mesh mesh;
    public MeshCollider meshCollider; 
    public BoidSettings settings;

    

    float prevWidth; 
    float prevHeight;


    // Start is called before the first frame update
    void Start()
    {
        setUpMesh();
        prevWidth = settings.width;
        prevHeight = settings.height;
    }


    private void Update()
    {
        if (prevHeight != settings.height || prevWidth != settings.width)
        {
            setUpMesh();
            prevWidth = settings.width;
            prevHeight = settings.height;
        }
    }

    void setUpMesh()
    {
        Vector3[] vertices = new Vector3[] {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 0, settings.width),
            new Vector3 (settings.width, 0, settings.width),
            new Vector3 (settings.width, 0, 0),

            //new Vector3 (0, settings.height, settings.width),
            //new Vector3 (0, settings.height, 0),

            //new Vector3 (settings.width, settings.height, settings.width),

            //new Vector3 (settings.width, settings.height, 0),
        };

        int[] triangles = new int[] {
            0, 1, 2, 
            0, 2, 3,

            //0, 4, 1, 
            //0, 5, 4,

            //1, 6, 2, 
            //1, 4, 6, 

            //2, 7, 3, 
            //2, 6, 7, 

            //0, 3, 7,
            //0, 7, 5,

            //5, 7, 6,
            //5, 6, 4
        };

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        if (meshCollider == null)
            meshCollider = this.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false;
    }
}
