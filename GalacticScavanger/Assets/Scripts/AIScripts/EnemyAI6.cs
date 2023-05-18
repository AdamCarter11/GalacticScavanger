using UnityEngine;
using System.Collections;
public class EnemyAI6 : MonoBehaviour
{
    // Fix a range how early u want your enemy detect the obstacle.
    [SerializeField] private int playerAttackRange = 150;
    [SerializeField] private int scrapSearchRadius = 500;
    [SerializeField] private int range = 160;
    [SerializeField] private string targetTag;
    [SerializeField] private LayerMask scrapLayer;
    [SerializeField] private int scrapDepositThreshold = 30;
    [SerializeField]  private float speed = 10.0f;
    private bool isThereAnyThing = false;
    // Specify the target for the enemy.
    [SerializeField] private GameObject target, player;
    [SerializeField] private float rotationSpeed = 45.0f;
    private RaycastHit hit;

    GameObject nearestObject = null;

    //private GameObject nearestObject;

    [HideInInspector] public int collectedScrap;
    int depositedScrap;

    private Vector3 respawnPoint;
    [SerializeField] int actualEnemyHealth;
    [HideInInspector] public int enemyHealth;
    private int startingHealth;
    void Start()
    {
        enemyHealth = actualEnemyHealth;
        startingHealth = enemyHealth;
        respawnPoint = transform.position;
    }
    private void OnEnable()
    {
        enemyHealth = startingHealth;
        transform.position = respawnPoint;
    }
    // Update is called once per frame
    void Update()
    {
        //Look At smoothly Towards the Target if there is nothing in front.
        if (!isThereAnyThing)
        {
            if(collectedScrap < scrapDepositThreshold)
            {
                if (Vector3.Distance(player.transform.position, transform.position) > playerAttackRange)
                {
                    //target = nearest scrap unless there is no nearby scrap in search radius
                    FindNearestObject();
                }
                else
                {
                    target = player;
                }
            }
            else
            {
                //find nearest docking station
                GameObject[] dockingStations = GameObject.FindGameObjectsWithTag("DockingStation");
                float shortestDistance = Mathf.Infinity;
                Vector3 currPos = transform.position;
                foreach(GameObject obj in dockingStations)
                {
                    float distance = Vector3.Distance(currPos, obj.transform.position);
                    if(distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestObject = obj;
                    }
                }
                target = nearestObject;
            }
            
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DockingStation"))
        {
            depositedScrap = collectedScrap;
            collectedScrap = 0;
            if(depositedScrap > 100)
            {   
                //lose condition for player maybe?

            }
        }
    }

    void FindNearestObject()
    {

        // Find all objects with the target tag within the search radius
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, scrapSearchRadius, scrapLayer);
        print(objectsInRange.Length);
        if (objectsInRange.Length <= 1)
        {
            target = player.gameObject;
            print("player is nearest");
        }
        else
        {
            GameObject nearestObject = null;
            float nearestDistance = float.MaxValue;

            // Iterate through each object and check if it is closer than the current nearest object
            foreach (Collider col in objectsInRange)
            {
                if (col.gameObject.CompareTag(targetTag))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestObject = col.gameObject;
                        nearestDistance = distance;
                    }
                }
            }

            // Store the nearest object found
            target = nearestObject;
        }

    }
    
}
