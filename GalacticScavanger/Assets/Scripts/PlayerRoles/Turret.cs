using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code source: https://www.youtube.com/watch?v=bCz7awDbl58
public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject projectile;
    [SerializeField] public float rotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 newRotation = Vector3.RotateTowards();
        //this.gameObject.transform.rotation =
    }
/*
    private void Update() 
    {
        Aim();
        //weapon.fire();
    }

    private void Aim()
    {
        // TURN
        float targetPlaneAngle = vector3AngleOnPlane(target.position, transform.position, -transform.up, transform.forward);
        Vector3 newRotation = new Vector3(0, targetPlaneAngle, 0);
        transform.Rotate(newRotation, Space.Self);
        
        // UP/DOWN
        float upAngle = Vector3.Angle(target.position, barrel.transform.up);
        Vector3 upRotation = new Vector3(-upAngle + 90, 0, 0);
        barrel.transform.Rotate(upRotation, Space.Self);
    }

    float vector3AngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal, Vector3 toZeroAngle)
    {
        Vector3 projectedVector = Vector3.ProjectOnPlane(from - to, planeNormal);
        float projectedVectorAngle = Vector3.SignedAngle(projectedVector, toZeroAngle, planeNormal);

        return projectedVectorAngle;
    } 
    */
}
