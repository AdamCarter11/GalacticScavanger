using UnityEngine;
using System.Collections.Generic;

public class AIAstarMove : MonoBehaviour
{
    private AStar aStar;

    private List<WayPointNode> path;
    private int currentWaypointIndex;
    private bool followingPath;

    public Transform target;

    public float moveSpeed = 5f;
    public float closeEnoughDistance = 0.2f;
    public float nextWaypointDistance = 3f;

    private void Start()
    {
        aStar = GetComponent<AStar>();
        //path = new List<Vector3>();
        currentWaypointIndex = 0;
        followingPath = false;
    }

    private void Update()
    {
        // Find the closest waypoint nodes to the start and end positions
        WayPointNode startNode = GetClosestWaypointNode(transform.position);
        WayPointNode endNode = GetClosestWaypointNode(target.position);

        // Find the path using A* algorithm
         path = AStar.FindPath(startNode, endNode);

        // Move along the path
        if (path != null && path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentWaypointIndex].transform.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, path[currentWaypointIndex].transform.position) < nextWaypointDistance)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= path.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
        }
    }

    // Find the closest waypoint node to a given position
    private WayPointNode GetClosestWaypointNode(Vector3 position)
    {
        WayPointNode closestNode = null;
        float closestDistance = Mathf.Infinity;
        foreach (WayPointNode waypoint in path)
        {
            WayPointNode waypointNode = waypoint.GetComponent<WayPointNode>();
            float distance = Vector3.Distance(position, waypointNode.transform.position);
            if (distance < closestDistance)
            {
                closestNode = waypointNode;
                closestDistance = distance;
            }
        }
        return closestNode;
    }
}
