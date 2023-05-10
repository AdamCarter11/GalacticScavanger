using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float velocity = 1f;
    [SerializeField] private float lifetimeSeconds = 3f;
    private float runningTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (runningTime > lifetimeSeconds)
        {
            Destroy(this.gameObject);
        }

        this.transform.position += this.transform.forward * velocity * Time.deltaTime;

        runningTime += Time.deltaTime;
    }
}
