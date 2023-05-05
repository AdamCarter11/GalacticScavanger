using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public static List<WayPointNode> FindPath(WayPointNode startNode, WayPointNode endNode)
    {
        List<WayPointNode> openSet = new List<WayPointNode>();
        HashSet<WayPointNode> closedSet = new HashSet<WayPointNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            WayPointNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (WayPointNode neighborNode in currentNode.neighbors)
            {
                if (closedSet.Contains(neighborNode))
                {
                    continue;
                }

                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighborNode);
                if (newMovementCostToNeighbor < neighborNode.gCost || !openSet.Contains(neighborNode))
                {
                    neighborNode.gCost = newMovementCostToNeighbor;
                    neighborNode.hCost = GetDistance(neighborNode, endNode);
                    neighborNode.parent = currentNode;

                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }

        // Path not found
        return null;
    }

    private static List<WayPointNode> RetracePath(WayPointNode startNode, WayPointNode endNode)
    {
        List<WayPointNode> path = new List<WayPointNode>();
        WayPointNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        List<WayPointNode> waypoints = new List<WayPointNode>();
        foreach (WayPointNode waypointNode in path)
        {
            waypoints.Add(waypointNode);
        }

        return waypoints;
    }

    private static float GetDistance(WayPointNode nodeA, WayPointNode nodeB)
    {
        float dstX = Mathf.Abs(nodeA.transform.position.x - nodeB.transform.position.x);
        float dstY = Mathf.Abs(nodeA.transform.position.y - nodeB.transform.position.y);
        float dstZ = Mathf.Abs(nodeA.transform.position.z - nodeB.transform.position.z);

        if (dstX > dstZ)
            return 1.4f * dstZ + 1f * (dstX - dstZ) + 1.4f * dstY;
        return 1.4f * dstX + 1f * (dstZ - dstX) + 1.4f * dstY;
    }
}