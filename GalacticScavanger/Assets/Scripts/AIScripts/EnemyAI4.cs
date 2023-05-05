using UnityEngine;

public class EnemyAI4 : MonoBehaviour
{
    public GameObject player;
    public float speed = 10f;
    public float avoidDistance = 5f;
    public float avoidForce = 10f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 desiredVelocity = (player.transform.position - transform.position).normalized * speed;

        // Check for obstacles
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, avoidDistance, transform.forward, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Scrap"))
            {
                Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal);
                desiredVelocity += avoidDirection.normalized * avoidForce;
            }
        }

        rb.AddForce(desiredVelocity - rb.velocity, ForceMode.VelocityChange);
    }
}
