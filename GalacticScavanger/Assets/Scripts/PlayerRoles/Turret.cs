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
   
    // input variables
    //float xRotate1D;
    //float yRotate1D;
    Vector2 pitchYaw;


    // Start is called before the first frame update
    void Start()
    {
        //Vector3 newRotation = Vector3.RotateTowards();
        //this.gameObject.transform.rotation =
    }

    private void Update()
    {
        RotationUpdate();
    }

    private void RotationUpdate()
    {
        float x = pitchYaw.x;
        float y = pitchYaw.y;
        Vector3 rotate = new Vector3(x * rotationSpeed, y * rotationSpeed);
        this.gameObject.transform.eulerAngles = transform.eulerAngles - rotate;
        Debug.Log("in turret rotation update");
    }

    #region Input Methods
    public void OnXYRotation(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }
    #endregion
}
