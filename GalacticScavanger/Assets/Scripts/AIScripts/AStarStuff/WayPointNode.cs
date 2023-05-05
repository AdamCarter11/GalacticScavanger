using System;
using UnityEngine;

public class WayPointNode : MonoBehaviour
{
    public WayPointNode[] neighbors;
    public float[] neighborsDistance;
    public float fCost; // total cost of the node
    public float gCost; // cost to move from start node to current node
    public float hCost;
    public WayPointNode parent;

    public float GetDistance(WayPointNode otherNode)
    {
        Vector3 diff = otherNode.transform.position - transform.position;
        return diff.magnitude;
    }

    public void FindNeighbors(float maxDistance)
    {
        // find all other WayPointNodes within maxDistance
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            WayPointNode otherNode = hitColliders[i].GetComponent<WayPointNode>();
            if (otherNode != null && otherNode != this)
            {
                float distance = GetDistance(otherNode);
                // add neighbor and distance to arrays
                if (neighbors == null)
                {
                    neighbors = new WayPointNode[] { otherNode };
                    neighborsDistance = new float[] { distance };
                }
                else
                {
                    Array.Resize(ref neighbors, neighbors.Length + 1);
                    neighbors[neighbors.Length - 1] = otherNode;
                    Array.Resize(ref neighborsDistance, neighborsDistance.Length + 1);
                    neighborsDistance[neighborsDistance.Length - 1] = distance;
                }
            }
        }
    }
    public float CalculateFCost()
    {
        return gCost + hCost;
    }
}
