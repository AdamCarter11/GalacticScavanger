using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI3 : MonoBehaviour
{
    public float maxSpeed = 100f;
    public float maxForce = 10f;
    public float mass = 1f;
    public float slowingDistance = 10f;
    public float turnSpeed = 5f;
    public float pathUpdateInterval = 1f;
    public Transform player;

    private List<Vector3> path = new List<Vector3>();
    private int currentWaypoint = 0;
    private float lastPathUpdateTime = 0f;

    void Start()
    {
        // Set player to be the main camera's transform by default
        if (player == null)
        {
            player = Camera.main.transform;
        }
    }

    void Update()
    {
        // Update path periodically
        if (Time.time - lastPathUpdateTime > pathUpdateInterval)
        {
            path = GeneratePath(transform.position, player.position);
            currentWaypoint = 0;
            lastPathUpdateTime = Time.time;
        }

        if (path.Count > 0)
        {
            // Check if we need to turn towards the next waypoint
            Vector3 waypointDirection = path[currentWaypoint] - transform.position;
            if (waypointDirection.magnitude < 10f)
            {
                currentWaypoint++;
                if (currentWaypoint >= path.Count)
                {
                    currentWaypoint = 0;
                }
            }
            else
            {
                if(Vector3.Distance(transform.position, player.position) > 5)
                {
                    // Use steering behaviors to turn towards the waypoint
                    Vector3 desiredVelocity = waypointDirection.normalized * maxSpeed;
                    Vector3 steeringForce = desiredVelocity - GetComponent<Rigidbody>().velocity;
                    steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
                    Vector3 steering = steeringForce / mass;

                    // Slow down as we approach the waypoint
                    float distance = waypointDirection.magnitude;
                    if (distance < slowingDistance)
                    {
                        steering *= distance / slowingDistance;
                    }

                    // Rotate towards the direction of movement
                    if (GetComponent<Rigidbody>().velocity.magnitude > 0)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetComponent<Rigidbody>().velocity), turnSpeed * Time.deltaTime);
                    }

                    // Apply steering force to the airplane's rigidbody
                    GetComponent<Rigidbody>().AddForce(steering);

                    // Clamp velocity to maximum speed
                    if (GetComponent<Rigidbody>().velocity.magnitude > maxSpeed)
                    {
                        GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
                    }
                }
                else
                {
                    //you are near the player, probably shoot here
                }
            }
        }
    }

    List<Vector3> GeneratePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();

        // Start with the current position
        Vector3 current = start;

        // Raycast towards the player's position and follow the path until we reach the player
        while (current != end)
        {
            RaycastHit hit;
            if (Physics.SphereCast(current, transform.localScale.y, (end - current).normalized, out hit, Mathf.Infinity))
            {
                if (hit.collider.transform == player)
                {
                    path.Add(end);
                    break;
                }
                else
                {
                    path.Add(hit.point);
                    current = hit.point;
                }
            }
            else
            {
                break;
            }
        }

        return path;
    }
}
