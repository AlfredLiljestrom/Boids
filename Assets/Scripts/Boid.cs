using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Boid : MonoBehaviour
{
    [HideInInspector] public static Vector3 coherence = Vector3.zero; 
    Vector3 prevCoherenceChange = Vector3.zero;

    public BoidSettings settings; 
    Spawner spawner; 

    public Vector3 middle;
    public float speed = 0f; 

    // Boundary
    public BoundaryController boundaryController;
    public int inBoundary = -1;
    public Vector3Int boundaryCord;



    private void Start()
    {
        spawner = GetComponentInParent<Spawner>();
        boundaryController = spawner.boundaryController;


        // For coherence 
        prevCoherenceChange = transform.position / spawner.boids.Count;
        coherence += prevCoherenceChange;

        // Set initial boundary
        boundaryController.boundaryCheck(ref inBoundary, transform.position, gameObject);
    }

    private void OnDestroy()
    {
        coherence -= prevCoherenceChange;
        boundaryController.removeBoidFromBoundary(inBoundary, gameObject); 
    }

    private void FixedUpdate()
    {  
        // Get the rotation from boids algorithm.
        BoidsAlgorithm(spawner.boids);

        // Move the boid forward.
        MoveBoid(); 

        // Make sure the boid is in the correct boundary. 
        boundaryController.boundaryCheck(ref inBoundary, transform.position, gameObject);
    }

    void MoveBoid()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void BoidsAlgorithm(List<GameObject> otherBoids)
    {
        if (otherBoids.Count == 0) { return; }
        List<GameObject> closeBoids = boundaryController.closeBirds(inBoundary);

        // These variables are here to make sure we don't take to many birds into consideration. 
        float increment = 1 - settings.maxBoidCalculations / (float)closeBoids.Count;
        float tracker = 0f; 

        Vector3 averageForward = Vector3.zero, hitDirection = Vector3.zero;
        float averageSpeed = 0f;
        int numSpeeds = 0; 
        foreach (var boid in closeBoids)
        {
            if (tracker > 1 )
            {
                tracker -= 1; 
                continue; 
            }

            // Data from boid position
            Vector3 dirToBoid = boid.transform.position - transform.position;
            float angle = Vector3.Dot(dirToBoid.normalized, transform.forward);
            float distance = dirToBoid.magnitude;

            // Seperation Calculation
            if (angle > settings.seperationAngle && distance < settings.seperationDistance)
                hitDirection += dirToBoid;

            // Alignment Calculation 
            if (distance < settings.alignmentDistance)
            {
                averageForward += boid.transform.forward;
                averageSpeed += boid.GetComponent<Boid>().speed; 
                numSpeeds++;
            }
                

            tracker += increment; 
        }

        if (averageForward.magnitude > 0f)
            ChangeDirection(averageForward.normalized, settings.alignmentSpeed);
        if (numSpeeds > 0f)
        {
            speed = Mathf.Lerp(speed, averageSpeed / (float)numSpeeds, Time.deltaTime * settings.alignmentSpeed) ;   
        }
        if (hitDirection.magnitude > 0f)
            ChangeDirection(-hitDirection.normalized, settings.seperationSpeed);

        // Coherence. Really Fast.
        coherence += transform.position / otherBoids.Count - prevCoherenceChange;
        prevCoherenceChange = transform.position / otherBoids.Count;
        ChangeDirection((coherence - transform.position).normalized, settings.coherenceSpeed);


        SteerFromBorder();
        flyTowardsTarget();
        
    }


    void flyTowardsTarget()
    {
        if (spawner.target == null)
            return; 
        Vector3 direction = (spawner.target.transform.position - transform.position).normalized;

        transform.position += direction * settings.toTargetSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, settings.toTargetSpeed * Time.deltaTime);
    }

    void SteerFromBorder()
    {
        Vector3 parentPos = spawner.transform.position;

        Vector3 projectedPosition = transform.position + transform.forward * settings.steerAway; 



        if (projectedPosition.x >= settings.width + parentPos.x  || projectedPosition.x <= parentPos.x ||
            projectedPosition.z >= settings.width + parentPos.z  || projectedPosition.z <= parentPos.z ||
            projectedPosition.y >= settings.height + parentPos.y || projectedPosition.y <= parentPos.y)
        {
            Vector3 dir = (middle - transform.position).normalized; //+ RandomDirection() / 10f;
            ChangeDirection(dir, speed / 1 + settings.steerAway);
        }
        else
        {
            SteerToRandomDirection();
        }
    }

    public void ChangeDirection(Vector3 dir, float speedOfRotation)
    {
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedOfRotation);
    }

    void SteerToRandomDirection()
    {
        float jitter = Mathf.Pow(Random.value, 2f); 
        float x = (2 * Random.value - 1) * jitter;
        float y = (2 * Random.value - 1) * jitter;
        float z = Random.value * 0.5f + 0.5f;

        Vector3 dir = transform.rotation * new Vector3(x, y, z).normalized;
        ChangeDirection(dir, settings.randomSteering); 
    }


    //Vector3[] FibonacciSpread()
    //{
    //    List<Vector3> points = new();
    //    float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5));

    //    for (int i = 0; i < settings.sampleAmount; i++)
    //    {

    //        float y = 1f - (2f * i) / (settings.sampleAmount - 1f);
    //        float r = Mathf.Sqrt(1f - y * y);
    //        float theta = goldenAngle * i;

    //        Vector3 point = new Vector3(r * Mathf.Cos(theta), y, r * Mathf.Sin(theta));

    //        // put points at a chosen distance from the object
    //        Vector3 pointAroundObject = this.transform.position + point * settings.radiusSpread;

    //        points.Add(pointAroundObject);
    //    }

    //    return points.ToArray();
    //}
}
