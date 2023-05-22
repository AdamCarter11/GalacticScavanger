using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAsteroids : MonoBehaviour
{
    public float minSpeed = 10f;
    public float maxSpeed = 50f;

    private Vector3 randomAxis;
    private float randomSpeed;

    void Start()
    {
        randomAxis = Random.onUnitSphere;
        randomSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        transform.Rotate(randomAxis, randomSpeed * Time.deltaTime);
    }
}
