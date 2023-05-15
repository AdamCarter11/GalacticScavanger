using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Turret Parts")]
    [SerializeField] private GameObject turretCamera;
    [SerializeField] private GameObject turretBarrelBase;

    // ship information
    private GameObject ship;
    private Transform turretLocation;
    private Vector3 turretToShip;
    
    // input variable
    private Vector2 pitchYaw;
    private bool firePressed;

    [Header("Limit Rotation Angles")] 
    [SerializeField] private float minAngle = -89.999f;
    [SerializeField] private float maxAngle = 5;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Ship");
        if (ship)
        {
            turretLocation = ship.GetComponent<Ship>().turretLocation;
            this.transform.parent = turretLocation;
            this.transform.position = turretLocation.position;
        }
        else
        {
            Debug.Log("turret did not find ship");
        }
    }

    private void Update()
    {
        if (ship)
        {
            RotationUpdate();
            ShootingUpdate();
            //DebugViewingRays();
        }
        else
        {
            Debug.Log("no ship detected, not updating turret");
        }
    }

    private void RotationUpdate()
    {
        float x = pitchYaw.y;
        float y = pitchYaw.x;

        // rotate base on y axis
        Quaternion localRotation = this.transform.localRotation;
        this.transform.RotateAround(this.transform.position, this.transform.up, y);
        
        // rotate barrel on the x axis
        Vector3 localEuler = turretBarrelBase.transform.localRotation.eulerAngles;

        //clamp the roll
        float rollAngle = turretBarrelBase.transform.eulerAngles.x - x;
        rollAngle = (rollAngle > 180) ? rollAngle - 360 : rollAngle;
        rollAngle = Mathf.Clamp(rollAngle, minAngle, maxAngle);
        turretBarrelBase.transform.eulerAngles = new Vector3(rollAngle, turretBarrelBase.transform.eulerAngles.y, turretBarrelBase.transform.eulerAngles.z);
    }
    
    private void DebugViewingRays()
    {
        // debug stuff
        Debug.DrawRay(this.transform.position, this.transform.up*10, Color.yellow);
        Debug.DrawRay(this.transform.position, this.transform.forward*10, Color.magenta);
        Debug.DrawRay(turretLocation.position, turretToShip, Color.red);
        Debug.DrawRay(ship.transform.position, ship.transform.up*10, Color.green);
        Debug.DrawRay(ship.transform.position, ship.transform.forward*10, Color.blue);
    }

    private void ShootingUpdate()
    {
        
    }
    
    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        //Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        firePressed = context.performed;
        Debug.Log("in OnFire:" + firePressed);
    }
    #endregion
}
