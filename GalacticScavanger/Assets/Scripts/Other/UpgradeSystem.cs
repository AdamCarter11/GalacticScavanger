using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Ship Engine Upgrade Percentages")]
    //controls how fast you can rotate
    [SerializeField] float yawTorqueBonus = 1.15f;    // y axis
    [SerializeField] float pitchTorqueBonus = 1.15f; // x axis
    [SerializeField] float rollTorqueBonus = 1.15f;  // z axis
    //controls how fast you can actually move
    [SerializeField] float thrustBonus = 1.1f;       // forward speed
    [SerializeField] float upThrustBonus = 1.15f;      // up/down speed
    [SerializeField] float strafeThrustBonus = 1.15f;  // left/right speed
    //controls accel/deccel (also what makes the ship eventually stop after they release the key
    //[SerializeField, Range(0.001f, 0.999f)] float thrustGlideReductionBonus = 1.1f;
    //[SerializeField, Range(0.001f, 0.999f)] float upDownGlideReductionBonus = 1.1f;
    //[SerializeField, Range(0.001f, 0.999f)] float leftRightGlideReductionBonus = 1.1f;
    [Header("Ship Turret Upgrade Percentages")]
    [SerializeField] public float fireRateBonus = 0.85f;
    [SerializeField] public int projectileDamageBonus = 1;
    //[SerializeField] public int healthBonus = 2;
    GameObject playerShipRef;
    GameObject turretRef;

    private bool canBeUpgraded = false;
    private int upgradeLevel = 1;

    int whichClass1;
    int whichClass2;

    private void Update()
    {
        if(playerShipRef == null)
        {
            playerShipRef = GameObject.FindGameObjectWithTag("Ship");
        }
        if (turretRef == null)
        {
            turretRef = GameObject.FindGameObjectWithTag("Turret");
        }
    }

    public void Upgrade()
    {
        whichClass1 = PlayerPrefs.GetInt("Player1Character");
        whichClass2 = PlayerPrefs.GetInt("Player2Character");

        //engine upgrades
        if(playerShipRef != null)
        {
            if (whichClass1 == 1)
            {
                playerShipRef.GetComponent<Ship>().yawTorque *= yawTorqueBonus;
                playerShipRef.GetComponent<Ship>().pitchTorque *= pitchTorqueBonus;
                playerShipRef.GetComponent<Ship>().rollTorque *= rollTorqueBonus;
            }
            if (whichClass1 == 2)
            {
                playerShipRef.GetComponent<Ship>().upThrust *= upThrustBonus;
                playerShipRef.GetComponent<Ship>().strafeThrust *= strafeThrustBonus;
            }
            playerShipRef.GetComponent<Ship>().thrust *= thrustBonus;
        }
        
        //gun upgrades
        if(turretRef != null)
        {
            if (whichClass2 == 1)
            {
                turretRef.GetComponent<Turret>().projectileDamage += projectileDamageBonus;
            }
            if (whichClass2 == 2)
            {
                turretRef.GetComponent<Turret>().fireRate *= fireRateBonus;
            }
        }
       
        //non class upgrades
        
        //playerShipRef.GetComponent<Ship>().health += healthBonus;
        
        //GetComponent<Ship>().thrustGlideReduction *= thrustGlideReductionBonus;
        //GetComponent<Ship>().upDownGlideReduction *= upDownGlideReductionBonus;
        //GetComponent<Ship>().leftRightGlideReduction *= leftRightGlideReductionBonus;
    }
}
