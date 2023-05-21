using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float emissionRate = 10f;
    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // Calculate the distance between the start and end points
        float distance = Vector3.Distance(startPoint.position, endPoint.position);

        // Calculate the emission rate based on the distance
        float calculatedRate = emissionRate * distance;

        // Set the particle emission rate
        var emission = particleSystem.emission;
        emission.rateOverTime = calculatedRate;

        // Set the particle system position to the start point
        transform.position = startPoint.position;

        // Calculate the direction from start to end
        Vector3 direction = (endPoint.position - startPoint.position).normalized;

        // Set the particle system rotation to face the direction
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
