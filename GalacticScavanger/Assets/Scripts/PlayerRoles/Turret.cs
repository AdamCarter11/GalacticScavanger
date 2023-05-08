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

    private GameObject ship;

    private Transform turretLocation;
    private Vector3 maxUpVector;
    private Vector3 maxFlatVector;
    
    // input variables
    //float xRotate1D;
    //float yRotate1D;
    Vector2 pitchYaw;


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
        float x = pitchYaw.y;
        float y = pitchYaw.x;
        Vector3 rotate = new Vector3(x * rotationSpeed, -y * rotationSpeed);
        this.gameObject.transform.eulerAngles = transform.eulerAngles - rotate;
        
        Debug.DrawRay(this.transform.position, this.transform.up*10, Color.cyan);
        Debug.DrawRay(this.transform.position, this.transform.forward*10, Color.yellow);
        //Debug.Log("in turret rotation update. new vector:" + rotate);
    }

    private void UpdateLegalViewingAngle()
    {
        maxUpVector = turretLocation.right;
        maxFlatVector = turretLocation.forward;
        Debug.DrawRay(turretLocation.position, maxUpVector*10, Color.green);
        Debug.DrawRay(turretLocation.position, maxFlatVector*10, Color.red);
    }
    
    private void CorrectOutOfRangeViewing()
    {
        Transform myT = this.transform;
        if (myT.right.x > maxUpVector.x - 90)
        {
            myT.forward = maxUpVector;
        }
        /*if (myT.forward.x < maxFlatVector.x)
        {
            myT.forward = maxFlatVector;
        }*/
    }

    private void PositionUpdate()
    {
        this.gameObject.transform.position = turretLocation.position;
    }

    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        //Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }
    #endregion
}
