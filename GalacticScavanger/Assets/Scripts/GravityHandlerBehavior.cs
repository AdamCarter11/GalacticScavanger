using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// code source: https://www.youtube.com/watch?v=o3rt2FaMwBs
public class GravityHandlerBehavior : MonoBehaviour
{
    [SerializeField] private float gravity = 1f;
    private static float G;
    // in physical universe every body would be both attractor and attractee
    [SerializeField] public static List<Rigidbody> attractors = new List<Rigidbody>();
    [SerializeField] public static List<Rigidbody> attractees = new List<Rigidbody>();
    [SerializeField] private static bool isSimulatingLive = true;

    private void FixedUpdate()
    {
        G = gravity;
        if (isSimulatingLive)
        {
            SimulateGravities();
        }
    }

    public static void SimulateGravities()
    {
        foreach (Rigidbody attractor in attractors)
        {
            foreach (Rigidbody attractee in attractees)
            {
                if (attractor != attractee)
                {
                    AddGravityForce(attractor, attractee);
                }
            }
        }
    }

    public static void AddGravityForce(Rigidbody attractor, Rigidbody target)
    {
        float massProduct = attractor.mass * target.mass * G;

        Vector3 difference = attractor.position - target.position;
        float distance = difference.magnitude;

        float unScaledforceMagnitude = massProduct / Mathf.Pow(distance, 2);
        float forceMagnitude = G * unScaledforceMagnitude;

        Vector3 forceDirection = difference.normalized;

        Vector3 forceVector = forceDirection * forceMagnitude;
        target.AddForce(forceVector);
    }
}
