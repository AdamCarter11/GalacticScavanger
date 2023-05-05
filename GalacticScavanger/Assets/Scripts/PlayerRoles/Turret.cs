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
        }
        RotationUpdate();
    }

    private void RotationUpdate()
    {
        float x = pitchYaw.x;
        float y = pitchYaw.y;
        Vector3 rotate = new Vector3(x * rotationSpeed, y * rotationSpeed);
        this.gameObject.transform.eulerAngles = transform.eulerAngles - rotate;
        //Debug.Log("in turret rotation update. new vector:" + rotate);
    }

    private void PositionUpdate()
    {
        this.gameObject.transform.position = turretLocation.position;
    }

    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }
    #endregion
}
