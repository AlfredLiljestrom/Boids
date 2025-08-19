using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bird : MonoBehaviour
{
    public static Vector3 coherence = Vector3.zero; 
    Vector3 prevCoherenceChange = Vector3.zero;
    // Sensor
    Vector3[] sensoryPoints;
    public BoundaryController boundaryController;

    public BoidSettings settings; 
    Spawner spawner; 

    // Bird physics
    public float stayInBoxSpeed = 3f; 

    // Boundary
    public int inBoundary = -1;
    public Vector3Int boundaryCord;

    private void Start()
    {
        boundaryController = FindObjectOfType<BoundaryController>();
        
        spawner = GetComponentInParent<Spawner>();
        prevCoherenceChange = transform.position / spawner.GetComponent<Spawner>().birds.Count;
        coherence += prevCoherenceChange;
        boundaryController.boundaryCheck(ref inBoundary, transform.position, gameObject);
    }

    private void OnDestroy()
    {
        coherence -= prevCoherenceChange;
        boundaryController.removeBirdFromBoundary(inBoundary, gameObject); 
    }

    private void FixedUpdate()
    {
        // Get the current sensory points for each bird. 
        var points = FibonacciSpread();
        sensoryPoints = FilterSpherePoints(points);
        
        // Get the rotation from boids algorithm.
        var otherBirds = spawner.GetComponent<Spawner>().birds;
        BoidsAlgorithm(otherBirds);

        // Move the bird forward.
        MoveBird();
        


        // Update boundary if necessary. 
        boundaryController.boundaryCheck(ref inBoundary, transform.position, gameObject);
    }

    void MoveBird()
    {
        int inverse = 1; 
        if (transform.position.x >= settings.width ||
            transform.position.x <= 0f ||
            transform.position.z >= settings.width ||
            transform.position.z <= 0f ||
            transform.position.y >= settings.height ||
            transform.position.y <= 0f) 
            inverse = -1;
        transform.position += inverse * transform.forward * settings.birdSpeed * Time.deltaTime;
    }

    void BoidsAlgorithm(List<GameObject> otherBirds)
    {
        if (otherBirds.Count == 0) { return; }

        Vector3 averagePosition = Vector3.zero;

        // Seperation. Maybe slow?
        Sens();

        // Coherence. Really Fast.
        Coherence(otherBirds); 

        // Alignment. Quite fast.
        Align();

        flyTowardsTarget();
    }

    void Coherence(List<GameObject> otherBirds)
    {

        coherence += transform.position / otherBirds.Count - prevCoherenceChange;
        prevCoherenceChange = transform.position / otherBirds.Count;  

        ChangeDirection((coherence - transform.position).normalized, settings.coherenceSpeed);
    }

    void flyTowardsTarget()
    {
        Vector3 direction = (spawner.target.transform.position - transform.position).normalized;

        transform.position += direction * settings.toTargetSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, settings.toTargetSpeed * Time.deltaTime);
    }

    void Align()
    {
        Vector3 averageForward = Vector3.zero;
        int count = 0;

        List<GameObject> otherBirds = boundaryController.closeBirds(inBoundary);

        foreach (var bird in otherBirds)
        {
            float dist = Vector3.Distance(transform.position, bird.transform.position);
            if (dist < settings.alignmentDistance)
            {
                averageForward += bird.transform.forward / ++count;
            }
        }

        if (count == 0) { return ; }

        ChangeDirection(averageForward.normalized, settings.alignmentSpeed);
    }

    void Sens()
    {
        Vector3 rayOrigin = transform.position;    
        RaycastHit hit;
        List<Vector3> possibleDirections = new();
        bool hasHitBox = false;
        bool hasHitBird = false;

        foreach (var point in sensoryPoints)
        {
            Vector3 rayDir = (point - rayOrigin).normalized;
            Ray ray = new Ray(rayOrigin, rayDir);
            float rayDistance = Vector3.Distance(point, rayOrigin); 

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.tag == "Box" || hit.collider.tag == "Bird")
                {
                    if (hit.collider.tag == "Box")
                        hasHitBox = true;
                    if (hit.collider.tag == "Bird")
                        hasHitBird = true;
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    continue;
                }
            }

            possibleDirections.Add(point); 
        }

        // If nothing is obstructing, continue forward as normal. 
        if (!hasHitBird && !hasHitBox) return;

        float speedOfSeperation = settings.seperationSpeed;
        if (hasHitBox) { speedOfSeperation = stayInBoxSpeed + settings.birdSpeed; }
            
        

        Vector3 dir = Vector3.zero;
        if (possibleDirections.Count != 0)
        {
            // Steer towards the average of all directions that is not obstructed. 
            foreach (var direction in possibleDirections)
            {
                dir += (direction - transform.position).normalized / possibleDirections.Count; ;
            }
        }
        else
        {
            // If nothing works then turn around. 
            dir = -transform.forward;
        }

        ChangeDirection(dir, speedOfSeperation);
    }

    public void ChangeDirection(Vector3 dir, float speedOfRotation)
    {
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedOfRotation);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(coherence, Vector3.one * 1.0f);
        if (sensoryPoints == null || sensoryPoints.Length == 0) return;
        
        foreach(var point in sensoryPoints) 
        {
            //Gizmos.DrawSphere(point, settings.radiusSphere); 
            //Gizmos.DrawLine(transform.position, point);
            
        }
    }

    Vector3[] FibonacciSpread()
    {
        List<Vector3> points = new();
        float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5));

        for (int i = 0; i < settings.sampleAmount; i++)
        {

            float y = 1f - (2f * i) / (settings.sampleAmount - 1f);
            float r = Mathf.Sqrt(1f - y * y);
            float theta = goldenAngle * i;

            Vector3 point = new Vector3(r * Mathf.Cos(theta), y, r * Mathf.Sin(theta));

            // put points at a chosen distance from the object
            Vector3 pointAroundObject = this.transform.position + point * settings.radiusSpread;

            points.Add(pointAroundObject);
        }

        return points.ToArray();
    }

    Vector3[] FilterSpherePoints(Vector3[] samples)
    {
        List<Vector3> points = new();

        foreach (Vector3 point in samples)
        {
            var dir = (point - this.transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle <= settings.filterAngle * 0.5f)
            {
                points.Add(point);
            }


        }

        return points.ToArray(); 
    }
}
