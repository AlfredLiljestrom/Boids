using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Settings/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Boid Behaviour")]
    public float alignmentDistance = 5f;
    public float alignmentSpeed = 1.0f;
    public float coherenceSpeed = 0.2f;
    public float seperationSpeed = 5f;

    [Header("Sensor Settings")]
    public float radiusSphere = 3f;
    public float radiusSpread = 20f;
    public int sampleAmount = 20;
    public float filterAngle = 90.0f;

    [Header("Bird Settings")]
    public int birdAmount = 50;
    public float birdSpeed = 0.5f;
    public float toTargetSpeed = 4.0f;

    [Header("Environment Settings")]
    public float width = 50;
    public float height = 50;

    [Header("Boundary Settings")]
    public int widthRows = 8;
    public int heightRows = 5;

    public void ResetValues()
    {
        width = 50; height = 50;
    }
}
