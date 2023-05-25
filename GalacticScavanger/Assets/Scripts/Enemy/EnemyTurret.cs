using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float distanceToFire = 40f;
    [SerializeField] private float fireDelaySeconds = 0.5f;
    private float runningTime = 0;
    private GameObject playerShip;
    private AudioSource laserAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        playerShip = GameObject.FindWithTag("Ship");
        laserAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerShip)
        {
            Vector3 enemyToPlayer = playerShip.transform.position - this.transform.position;
            //this.transform.rotation = Quaternion.Euler(enemyToPlayer);
            // Calculate the rotation needed to look in that direction
            Quaternion targetRotation = Quaternion.LookRotation(enemyToPlayer);

            // Apply the rotation gradually over time
            float rotationSpeed = 100f; // Adjust as needed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            
            if (enemyToPlayer.magnitude <= distanceToFire && runningTime >= fireDelaySeconds)
            {
                FireWeapon();
                runningTime = 0 - Time.deltaTime;
            }
            runningTime += Time.deltaTime;
        }
        else
        {
            playerShip = GameObject.FindWithTag("Ship");
        }
    }

    private void FireWeapon()
    {
        Debug.Log("Firing Weapon");
        laserAudio.Play();
        Instantiate(projectile, this.transform.position, this.transform.rotation);
    }
}
