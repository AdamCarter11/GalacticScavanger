using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;

public class EnemyAI2 : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotateSpeed, moveSpeed;
    [SerializeField] LayerMask whatToAvoid;
    Vector3 dir;
    Quaternion rotGoal;

    //[SerializeField] float pitchThreshold, fineSteeringAngle, rollFactor, steeringSpeed, moveSpeed;
    //[SerializeField] Vector3 errorDir;
    //Vector3 targetInput, lastInput;

    private void Update()
    {
        //CalculateMove();


        // fix rotation if the player will be moving into a wall on their current trajectory
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, transform.localScale.y, transform.TransformDirection(Vector3.forward), out hit, 5, whatToAvoid))
        {
            // SphereCast can shoot out a specific direction (parameter goes after the radius, i.e., third para)
            Vector3 tempDir = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
            //dir = (tempDir).normalized;
            //transform.Rotate(transform.rotation.x, transform.rotation.y, transform.rotation.z);
            //figure out how to rotate properly here
            transform.RotateAround(hit.transform.position, Vector3.up, 20 * Time.deltaTime);
        }
        else
        {
            // calculate base rotation
            dir = (target.position - transform.position).normalized;
            rotGoal = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, rotateSpeed);

        }
        

        // move AI
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }


    //test 1
    /*
    void CalculateMove()
    {
        //calculate the error
        Vector3 error = target.position - transform.position;
        error = Quaternion.Inverse(transform.rotation) * error;

        //Steering based on pitch
        Vector3 pitchError = new Vector3(0, error.y, error.z).normalized;
        float pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);
        if (-pitch < pitchThreshold) //15 could be a pitch threshold
        {
            pitch += 360f;
        }
        targetInput.x = pitch;

        Vector3 rollError = new Vector3(error.x, error.y, 0).normalized;
        if (Vector3.Angle(Vector3.forward, errorDir) < fineSteeringAngle)
        {
            targetInput.y = error.x;
        }
        else
        {
            float roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            targetInput.z = roll * rollFactor;
        }

        targetInput.x = Mathf.Clamp(targetInput.x, -1, 1);
        targetInput.y = Mathf.Clamp(targetInput.y, -1, 1);
        targetInput.z = Mathf.Clamp(targetInput.z, -1, 1);

        var input = Vector3.MoveTowards(lastInput, targetInput, steeringSpeed * Time.deltaTime);
        lastInput = input;

        //apply input to position
        print(input);
        transform.rotation = Quaternion.Euler(input.x, input.y, input.z);
        transform.position += Vector3.forward * Time.deltaTime * moveSpeed;
    }
    */
}
