using UnityEngine;
using System.Collections;
public class EnemyAI6 : MonoBehaviour
{
    // Fix a range how early u want your enemy detect the obstacle.
    [SerializeField] private int range = 160;
    [SerializeField]  private float speed = 10.0f;
    private bool isThereAnyThing = false;
    // Specify the target for the enemy.
    [SerializeField] private GameObject target;
    [SerializeField] private float rotationSpeed = 45.0f;
    private RaycastHit hit;

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        //Look At smoothly Towards the Target if there is nothing in front.
        if (!isThereAnyThing)
        {
            Vector3 relativePos = target.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        }
        // Enemy translate in forward direction.
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //Checking for any Obstacle in front.
        // Two rays left and right to the object to detect the obstacle.
        Transform leftRay = transform;
        Transform rightRay = transform;
        //Use Phyics.RayCast to detect the obstacle
        if (Physics.Raycast(leftRay.position + (transform.right * 7), transform.forward, out hit, range) || Physics.Raycast(rightRay.position - (transform.right * 7), transform.forward, out hit, range))
        {
            if (hit.collider.gameObject.CompareTag("Obstacle"))
            {
                print("OBSTACLE");
                isThereAnyThing = true;
                transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            }
        }
        // Now Two More RayCast At The End of Object to detect that object has already pass the obsatacle.
        // Just making this boolean variable false it means there is nothing in front of object.
        if (Physics.Raycast(transform.position - (transform.forward * 4), transform.right, out hit, 10) ||
         Physics.Raycast(transform.position - (transform.forward * 4), -transform.right, out hit, 10))
        {
            if (hit.collider.gameObject.CompareTag("Obstacle"))
            {
                isThereAnyThing = false;
            }
        }
        // Use to debug the Physics.RayCast.
        Debug.DrawRay(transform.position + (transform.right * 7), transform.forward * 20, Color.red);
        Debug.DrawRay(transform.position - (transform.right * 7), transform.forward * 20, Color.red);
        Debug.DrawRay(transform.position - (transform.forward * 4), -transform.right * 20, Color.yellow);
        Debug.DrawRay(transform.position - (transform.forward * 4), transform.right * 20, Color.yellow);
    }
}