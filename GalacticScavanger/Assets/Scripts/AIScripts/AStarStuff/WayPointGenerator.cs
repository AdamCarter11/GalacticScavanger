using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointGenerator : MonoBehaviour
{
    public GameObject target; // the object to follow
    public float distanceThreshold = 10f; // the distance threshold to create a new waypoint
    public float obstacleAvoidanceDistance = 5f; // the distance to check for obstacles
    public LayerMask obstacleMask; // the layers to consider as obstacles

    private List<Vector3> waypoints; // the list of generated waypoints

    private void Start()
    {
        waypoints = new List<Vector3>();
    }

    private void Update()
    {
        // check if we need to create a new waypoint
        if (waypoints.Count == 0 || Vector3.Distance(target.transform.position, waypoints[waypoints.Count - 1]) > distanceThreshold)
        {
            // create a new waypoint
            Vector3 newWaypoint = GetNewWaypoint(target.transform.position);

            // add it to the list
            waypoints.Add(newWaypoint);
        }
    }

    private Vector3 GetNewWaypoint(Vector3 currentPosition)
    {
        Vector3 newWaypoint = Vector3.zero;

        // calculate a random direction
        Vector3 randomDirection = Random.insideUnitSphere * distanceThreshold;
        randomDirection += currentPosition;
        randomDirection.y = 0f;

        // check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(currentPosition, randomDirection - currentPosition, out hit, obstacleAvoidanceDistance, obstacleMask))
        {
            // if an obstacle is detected, avoid it
            Vector3 avoidanceDirection = Vector3.Reflect(randomDirection - currentPosition, hit.normal);
            newWaypoint = currentPosition + avoidanceDirection.normalized * distanceThreshold;
        }
        else
        {
            // no obstacles, set the new waypoint to the random direction
            newWaypoint = randomDirection;
        }

        return newWaypoint;
    }

    // for debugging purposes only, to visualize the generated waypoints
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 1f);
        }
    }
    */
}
