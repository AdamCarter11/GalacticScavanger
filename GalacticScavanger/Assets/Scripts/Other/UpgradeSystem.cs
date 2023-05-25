using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Ship Engine Upgrade Percentages")]
    //controls how fast you can rotate
    [SerializeField] float yawTorqueBonus = 1.1f;    // y axis
    [SerializeField] float pitchTorqueBonus = 1.1f; // x axis
    [SerializeField] float rollTorqueBonus = 1.1f;  // z axis
    //controls how fast you can actually move
    [SerializeField] float thrustBonus = 1.1f;       // forward speed
    [SerializeField] float upThrustBonus = 1.1f;      // up/down speed
    [SerializeField] float strafeThrustBonus = 1.1f;  // left/right speed
    //controls accel/deccel (also what makes the ship eventually stop after they release the key
    //[SerializeField, Range(0.001f, 0.999f)] float thrustGlideReductionBonus = 1.1f;
    //[SerializeField, Range(0.001f, 0.999f)] float upDownGlideReductionBonus = 1.1f;
    //[SerializeField, Range(0.001f, 0.999f)] float leftRightGlideReductionBonus = 1.1f;
    [Header("Ship Turret Upgrade Percentages")]
    [SerializeField] public float fireRateBonus = 0.9f;
    [SerializeField] public int projectileDamageBonus = 1;
    [SerializeField] public int healthBonus = 1;

    private bool canBeUpgraded = false;
    private int upgradeLevel = 1;

    int whichClass1;
    int whichClass2;

    public void Upgrade()
    {
        whichClass1 = PlayerPrefs.GetInt("Player1Character");
        whichClass2 = PlayerPrefs.GetInt("Player2Character");

        //engine upgrades
        GetComponent<Ship>().yawTorque *= yawTorqueBonus;
        GetComponent<Ship>().pitchTorque *= pitchTorqueBonus;
        GetComponent<Ship>().rollTorque *= rollTorqueBonus;

        GetComponent<Ship>().thrust *= thrustBonus;
        GetComponent<Ship>().upThrust *= upThrustBonus;
        GetComponent<Ship>().strafeThrust *= strafeThrustBonus;
        //GetComponent<Ship>().thrustGlideReduction *= thrustGlideReductionBonus;
        //GetComponent<Ship>().upDownGlideReduction *= upDownGlideReductionBonus;
        //GetComponent<Ship>().leftRightGlideReduction *= leftRightGlideReductionBonus;

        //gun upgrades
        GetComponent<Ship>().health += healthBonus;
        GetComponent<Turret>().fireRate *= fireRateBonus;
        GetComponent<Turret>().projectileDamage += projectileDamageBonus;
    }
}
