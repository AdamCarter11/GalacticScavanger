using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI5 : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Transform myTransform;

    Vector3 storeTarget;
    Vector3 newTargetPos;
    bool savePos;
    bool overrideTarget;
    Transform obstacle;
    private List<Vector3> EscapeDirections = new List<Vector3>();

    //AI Checks
    private bool chasing = false;
    private bool evading = false;

    //AI stuff
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxDist;
    [SerializeField] private float minDist;
    [SerializeField] private float fireDist;

    //AI Speeds
    public float moveSpeed;

    void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Ship").transform;
        }
        myTransform = this.transform;
    }

    void FixedUpdate()
    {
        //Find Distance to target
        //var distance = (target.position - myTransform.position).magnitude;
        var distance = Vector3.Distance(target.position, myTransform.position);

        if (chasing)
        {
            //Rotate to look at player
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
            //Move towards the player
            myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;

            //Moves too far away from player
            if (distance > maxDist)
            {
                chasing = false;
            }

            //Attack if close enough
            if (distance < fireDist)
            {
                //insert attack commands here when ready
            }

            if (distance < minDist)
            {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(myTransform.position - target.position), rotationSpeed * Time.deltaTime);
                myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (distance < maxDist)
            {
                chasing = true;
            }
        }

        ObstacleAvoidance(transform.forward, 0, target);

    }

    void ObstacleAvoidance(Vector3 direction, float offsetX, Transform currTarget)
    {
        RaycastHit[] hit = Rays(direction, offsetX);

        for (int i = 0; i < hit.Length - 1; i++)
        {
            //So we dont detect ourself as a hit collision
            if (hit[i].transform.root.gameObject != this.gameObject)
            {
                if (!savePos)
                {
                    storeTarget = currTarget.position;
                    obstacle = hit[i].transform;
                    savePos = true;
                }

                FindEscapeDirections(hit[i].collider);
            }
        }

        if (EscapeDirections.Count > 0)
        {
            if (!overrideTarget)
            {
                newTargetPos = getClosests();
                overrideTarget = true;
            }
        }
        float distance = Vector3.Distance(transform.position, currTarget.position);

        if (distance < 5)
        {
            if (savePos)
            {
                //if we reach the target
                currTarget.position = storeTarget;
                savePos = false;
            }
            else
            {
                //if we had waypoints
                //index++
            }
            overrideTarget = false;
            EscapeDirections.Clear();
        }
    }

    Vector3 getClosests()
    {
        Vector3 clos = EscapeDirections[0];
        float distance = Vector3.Distance(transform.position, EscapeDirections[0]);

        foreach (Vector3 dir in EscapeDirections)
        {
            float tempDistance = Vector3.Distance(transform.position, dir);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                clos = dir;
            }
        }
        return clos;

    }

    void FindEscapeDirections(Collider col)
    {
        //Check for obstacles above
        RaycastHit hitUp;
        if (Physics.Raycast(col.transform.position, col.transform.up, out hitUp, col.bounds.extents.y * 2 + 5))
        {

        }
        else
        {
            //if there is something above
            Vector3 dir = col.transform.position + new Vector3(0, col.bounds.extents.y * 2 + 5, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        //Check for obstacles below
        RaycastHit hitDown;
        if (Physics.Raycast(col.transform.position, -col.transform.up, out hitDown, col.bounds.extents.y * 2 + 5))
        {

        }
        else
        {
            //if there is something below
            Vector3 dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2 - 5, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }
        //Check for obstacles to the Right
        RaycastHit hitRight;
        if (Physics.Raycast(col.transform.position, col.transform.right, out hitRight, col.bounds.extents.x * 2 + 5))
        {

        }
        else
        {
            //if there is something to the right
            Vector3 dir = col.transform.position + new Vector3(col.bounds.extents.x * 2 + 5, 0, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }
        //Check for obstacles to the Left
        RaycastHit hitLeft;
        if (Physics.Raycast(col.transform.position, -col.transform.right, out hitLeft, col.bounds.extents.x * 2 + 5))
        {

        }
        else
        {
            //if there is something to the right
            Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x * 2 - 5, 0, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }
    }

    RaycastHit[] Rays(Vector3 direction, float offsetX)
    {
        Ray ray = new Ray(transform.position + new Vector3(offsetX, 0, 0), direction);
        Debug.DrawRay(transform.position + new Vector3(offsetX, 0, 0), direction * 10 * moveSpeed, Color.red);

        float distanceToLookAhead = moveSpeed * 5;
        //Adjust 5 to proper radius around object to pick up raycast hits
        RaycastHit[] hits = Physics.SphereCastAll(ray, 5, distanceToLookAhead);

        return hits;
    }


}
