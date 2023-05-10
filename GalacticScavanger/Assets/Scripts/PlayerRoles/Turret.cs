using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject projectile;
    [SerializeField] public float rotationSpeed = 1f;

    // ship information
    private GameObject ship;
    private Transform turretLocation;
    private Vector3 turretToShip;
    
    // input variable
    Vector2 pitchYaw;

    [Header("Limit Rotation Angles")] 
    [SerializeField] private float minAngle = 90;
    //[SerializeField] private float maxAngle = 180;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Ship");
        if (ship)
        {
            turretLocation = ship.GetComponent<Ship>().turretLocation;    
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
            PositionUpdate();
            UpdateLegalViewingAngle();
            RotationUpdate();
            CorrectOutOfRangeViewing();
        }
        else
        {
            Debug.Log("no ship detected, not updating turret");
        }
    }

    private void RotationUpdate()
    {
        float tempAngle = ship.transform.rotation.z;
        //this.gameObject.transform.Rotate(this.gameObject.transform.forward, tempAngle);
        
        this.transform.rotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, tempAngle, transform.rotation.w);
        
        
        float x = pitchYaw.y;
        float y = pitchYaw.x;
        Vector3 rotate = new Vector3(x * rotationSpeed, -y * rotationSpeed, ship.transform.rotation.z);
        this.gameObject.transform.eulerAngles = transform.eulerAngles - rotate;
        
        Debug.DrawRay(this.transform.position, this.transform.up*10, Color.yellow);
        Debug.DrawRay(this.transform.position, this.transform.forward*10, Color.magenta);
        //Debug.Log("in turret rotation update. new vector:" + rotate);
    }

    private void UpdateLegalViewingAngle()
    {
        turretToShip = this.transform.position - turretLocation.position;
        Debug.DrawRay(turretLocation.position, turretToShip, Color.red);
        
        Debug.DrawRay(ship.transform.position, ship.transform.up*10, Color.green);
        Debug.DrawRay(ship.transform.position, ship.transform.forward*10, Color.blue);
    }
    
    private void CorrectOutOfRangeViewing()
    {
        Transform myT = this.transform;
        float currentAngle = Vector3.Angle(turretToShip, -myT.forward);
        float facingForwardDotProduct = Vector3.Dot(turretLocation.forward, myT.forward);
        Debug.Log("Current Angle:"+currentAngle + " | Current Dot Product:"+facingForwardDotProduct);
        
        if (currentAngle < minAngle)
        {
            Debug.Log("less than 90 degrees");
        }
        if (facingForwardDotProduct < 0)
        {
            Debug.Log("greater than 180 degrees");
        }
    }

    private void PositionUpdate()
    {
        float arbitraryLength = 2f;
        
        //this.gameObject.transform.position = new Vector3(turretLocation.position.x, turretLocation.position.y + (turretLocation.up.y * 2), turretLocation.position.z);
        this.gameObject.transform.position = turretLocation.position + (turretLocation.up*arbitraryLength);
        //Vector3 projectedVector = Vector3.Project(this.gameObject.transform.position, turretLocation.up);
        //this.gameObject.transform.position = projectedVector;
    }

    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        //Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }
    #endregion
}
