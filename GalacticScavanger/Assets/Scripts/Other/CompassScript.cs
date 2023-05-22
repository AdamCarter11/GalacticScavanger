using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassScript : MonoBehaviour
{
    [SerializeField] string targetTag;
    void LateUpdate()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(currentPosition, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            // Point the GameObject towards the closest target
            transform.LookAt(closestTarget.transform);
        }
        else
        {
            transform.LookAt(new Vector3(0, 0, 0));
        }
    }
}
