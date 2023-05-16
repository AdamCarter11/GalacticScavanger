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
    [SerializeField] private GameObject turretBarrel;
    
    [Header("Shooting Information")]
    [SerializeField] public float fireRate = 0.1f;
    [SerializeField] public int projectileDamage = 1;
    [SerializeField] private float sphereCastRadius = 0.5f;
    [SerializeField] private float sphereCastDistance = 100f;
    private float runningTime = 0f;
    [SerializeField] private LayerMask layerMaskToIgnore;
    [SerializeField] private GameObject projectile;

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
            runningTime = fireRate;
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
            FireUpdate();
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

    private void FireUpdate()
    {
        if (firePressed)
        {
            runningTime -= Time.deltaTime;
        }
        else
        {
            runningTime = 0;
        }
        
        if (firePressed && runningTime <= 0)
        {
            runningTime = fireRate;
            FiringHelper();
        }
    }

    private void FiringHelper()
    {
        //Debug.Log("FireHelper");
        //myRay = new Ray(turretCamera.transform.position, turretCamera.transform.forward);

        /*Rect viewportRect = turretCamera.GetComponent<Camera>().rect;
        Vector3 viewportCenter = new Vector3(viewportRect.x + viewportRect.width * 0.5f,
            viewportRect.y + viewportRect.height * 0.5f, 0f);
        //Ray ray = turretCamera.GetComponent<Camera>().ViewportPointToRay(viewportCenter);
        myRay = turretCamera.GetComponent<Camera>().ViewportPointToRay(viewportCenter);
        
        RaycastHit hit;
        //layerMask = LayerMask.GetMask("Default");
        
        if (Physics.Raycast(myRay, out hit, Mathf.Infinity, ~layerMaskToIgnore))
        {
            Debug.Log("Hit object: " + hit.transform.name);
            //Instantiate(projectile, myRay.origin, Quaternion.Euler(myRay.direction));
            // Do something with the hit object here
        }*/
        
        
        RaycastHit[] hits = Physics.SphereCastAll(turretBarrel.transform.position, sphereCastRadius, turretBarrel.transform.forward, sphereCastDistance, ~layerMaskToIgnore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Hit an enemy!");
            }
            else
            {
                Debug.Log("Hit object: " + hit.transform.name);
            }
            // Do something with the hit object here
        }
        // Draw a debug line showing the direction and distance of the spherecast
        Debug.DrawRay(turretBarrel.transform.position, turretBarrel.transform.forward * sphereCastDistance, Color.yellow, 1f);
        
        Instantiate(projectile, turretBarrel.transform.position, Quaternion.Euler(turretBarrel.transform.eulerAngles));
        
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

    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        //Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        firePressed = context.performed;
        //Debug.Log("in OnFire:" + firePressed);
    }
    #endregion
}
