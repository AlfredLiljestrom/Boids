using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Settings/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Boid Behaviour")]
    [Range(0, 10)] public float alignmentDistance = 5f;
    [Range(0, 10)] public float alignmentSpeed = 1.0f;
    [Range(0, 10)] public float coherenceSpeed = 0.2f;
    [Range(0, 10)] public float seperationSpeed = 5f;
    [Range(-1, 1)] public float seperationAngle = -0.5f;
    [Range(0, 10)] public float seperationDistance = 2.0f;
    [Range(0, 10)] public float randomSteering = 0.5f;

    [Header("Boid Settings")]
    [Min(0)] public int boidAmount = 50;
    [Min(0)]public float boidSpeed = 0.5f;
    public float toTargetSpeed = 4.0f;

    [Header("Environment Settings")]
    [Min(0)] public float width = 50;
    [Min(0)] public float height = 50;
    [Range(0, 10)] public float steerAway = 1.0f; 

    [Header("Boundary Settings")]
    [Min(0)] public int widthRows = 8;
    [Min(0)] public int heightRows = 5;

    [Header("Performance Settings")]
    public int maxBoidCalculations = 50; 

    public void ResetValues()
    {
        width = 50; height = 50;
    }
}
