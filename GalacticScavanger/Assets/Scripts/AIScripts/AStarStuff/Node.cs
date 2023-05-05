using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 position;
    public Node[] neighbors;
    public float neighborDistance = 10f;

    public float gCost;
    public float hCost;
    public float fCost { get { return gCost + hCost; } }

    public Node parent;

    public void Initialize()
    {
        position = transform.position;
        neighbors = FindNeighbors();
    }

    public Node[] FindNeighbors()
    {
        List<Node> neighborList = new List<Node>();
        Collider[] hitColliders = Physics.OverlapSphere(position, neighborDistance);

        foreach (Collider col in hitColliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null && node != this)
            {
                neighborList.Add(node);
            }
        }

        return neighborList.ToArray();
    }

}

