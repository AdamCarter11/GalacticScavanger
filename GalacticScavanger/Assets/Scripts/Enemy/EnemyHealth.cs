using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] ParticleSystem explosionPS;

    public void DecrementHealth()
    {
        health--;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(explosionPS, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
