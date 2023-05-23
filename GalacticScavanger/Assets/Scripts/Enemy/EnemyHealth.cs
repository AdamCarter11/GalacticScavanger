using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int health = 3;
    private int startingHealth;
    [SerializeField] ParticleSystem explosionPS;
    private Vector3 respawnPoint;
    

    private void Start()
    {
        startingHealth = health;
        respawnPoint = transform.position;
    }
    public void DecrementHealth()
    {
        health--;
    }
    private void OnEnable()
    {
        if (gameObject.activeSelf && respawnPoint.x != 0 && respawnPoint.y != 0 && respawnPoint.z != 0)
        {
            health = startingHealth;
            transform.position = respawnPoint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(explosionPS, transform.position, Quaternion.identity);
            
            //Destroy(this.gameObject);
        }
    }
}
