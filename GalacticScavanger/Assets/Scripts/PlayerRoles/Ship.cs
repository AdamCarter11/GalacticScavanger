using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// referenced from: https://www.youtube.com/watch?v=fZvJvZA4nhY&ab_channel=DanPos

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [Header("Ship Movement Vars")]
    //controls how fast you can rotate
    [SerializeField] float yawTorque = 500f;    // y axis
    [SerializeField] float pitchTorque = 1000f; // x axis
    [SerializeField] float rollTorque = 1000f;  // z axis
    //controls how fast you can actually move
    [SerializeField] float thrust = 100f;       // forward speed
    [SerializeField] float upThrust = 50f;      // up/down speed
    [SerializeField] float strafeThrust = 50f;  // left/right speed
    //controls accel/deccel (also what makes the ship eventually stop after they release the key
    [SerializeField, Range(0.001f, 0.999f)] float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)] float upDownGlideReduction = .111f;
    [SerializeField, Range(0.001f, 0.999f)] float leftRightGlideReduction = .111f;
    float glide, verticalGlide, horizontalGlide = 0f;
    [SerializeField] float rollClampVal;
    [SerializeField] float pitchClamp;
    [SerializeField] bool canDrift = false;

    [Header("Boosting Vars")]
    [SerializeField] float maxBoostAmount = 2f;
    [SerializeField] float boostLossRate = .25f;
    [SerializeField] float boostRechargeRate = .5f;
    [SerializeField] float boostMultiplier = 5f;
    public bool boosting = false;
    public float currBoostAmount;

    [Header("Health and Collectable Vars")]
    public int health = 10;
    public int collectableCount = 0;
    [SerializeField] private GameObject healthUIText;

    [Header("Gunner Vars")] 
    [SerializeField] public Transform turretLocation;
    [SerializeField] public GameObject turretPrefab;
    
    Rigidbody rb;
    // input variables
    float thrust1D;
    float upDown1D;
    float strafe1D;
    float roll1D;
    Vector2 pitchYaw;

    [Header("Pause Vars")]
    // Pause variables
    [SerializeField] GameObject pausePanel;
    bool isPaused = false;
    bool inputPaused;

    [HideInInspector] public static bool isDead;
    private Vector3 respawnPos;

    [Header("Class Ability Vars")]
    [SerializeField] float teleportCooldownOrScanCooldown;
    int whichClass; // 1 is navigator, 2 is pilot
    bool canTeleport = true;
    
    //scan vars
    bool scanOut = false;
    [SerializeField] SphereCollider scanCol;
    [SerializeField] float scanSpeed;
    [SerializeField] int howManyToScan;
    [SerializeField] float maxScanRadius;
    [SerializeField] LayerMask scanLayers;
    private int currentObjects;

    Quaternion startingRotation;
    float timeCount = 0.0f;
    bool resetSpeed = false;
    bool shielding = false;
    bool shieldingOnCool = false;
    [SerializeField] float shieldTime = 5.0f;
    [SerializeField] float shieldCoolDown = 10.0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currBoostAmount = maxBoostAmount;
        Cursor.visible = false;
        respawnPos = transform.position;
        whichClass = PlayerPrefs.GetInt("Player1Character");
        startingRotation = transform.rotation;
        if (!healthUIText)
        {
            healthUIText = GameObject.Find("HealthPointsText");
        }
    }

    private void Awake()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");
        if (ships.Length > 1)
        {
            Debug.Log("ship already exists, spawning turret");
            Instantiate(turretPrefab);
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        if (isDead)
        {
            isDead = false;
            transform.position = respawnPos;
            rb.velocity = Vector3.zero;
        }
        if (scanOut)
        {
            scanLogic();
        }
        healthUIText.GetComponent<TextMeshProUGUI>().text = "Health: " + health;
    }
    void FixedUpdate()
    {
        HandleBoost();
        HandleMovement();
        PauseGameFunc();
    }
    void HandleBoost()
    {
        if(whichClass == 2)
        {
            if (boosting && currBoostAmount > 0f)
            {
                currBoostAmount -= boostLossRate;
                if (currBoostAmount <= 0f)
                {
                    boosting = false;
                }
            }
            else
            {
                if (currBoostAmount < maxBoostAmount)
                {
                    currBoostAmount += boostRechargeRate;
                }
            }
        }
        //teleporting ability if we wanted it
        /*
        if(whichClass == 1)
        {
            if(boosting && canTeleport)
            {
                StartCoroutine(TeleportCooldown());
                teleportLogic();
            }
        }
        */
        //if(whichClass == 1)
        {
            //I'm just using canTeleport so I don't have to make a new ability
            if(boosting && canTeleport)
            {
                StartCoroutine(TeleportCooldown());
                //scanLogic();
                scanOut = true;
            }
        }
        if(whichClass == 2)
        {
            if (!shielding && boosting && !shieldingOnCool)
            {
                StartCoroutine(shieldLogic());
            }
        }
    }
    IEnumerator shieldLogic()
    {
        shielding = true;
        yield return new WaitForSeconds(shieldTime);
        shielding = false;
        StartCoroutine(shieldCoolDownFunc());
    }
    IEnumerator shieldCoolDownFunc()
    {
        shieldingOnCool = true;
        yield return new WaitForSeconds(shieldCoolDown);
        shieldingOnCool = false;
    }

    void scanLogic()
    {
        //print("Start scan");
        //scanCol.transform.localScale += Vector3.one * scanSpeed * Time.deltaTime;
        if(scanCol.radius <= maxScanRadius)
        {
            scanCol.radius += 1f * scanSpeed * Time.deltaTime;
        }
        else
        {
            print("Finished scan");
            scanOut = false;
        }
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, scanCol.radius, scanLayers, QueryTriggerInteraction.Collide);
        foreach (Collider objectHit in colliders){
            GameObject tempObj = objectHit.gameObject;
            tempObj.GetComponent<Outline>().enabled = true;
            print(tempObj.name);
        }
        currentObjects = colliders.Length;
        //print(currentObjects);

        // Stop growing if we've hit the maximum number of objects
        if (currentObjects >= howManyToScan)
        {
            print("Finished scan");
            scanOut = false;
        }
    }

    void teleportLogic()
    {
        //Vector3 randomPos = new Vector3(0,0,0);
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 100f;
        Collider[] colliders = Physics.OverlapSphere(randomPos, 0.1f);

        if (colliders.Length > 1)
        {
            // The random position is inside a collider, generate a new position
            teleportLogic();
        }
        else
        {
            // Teleport the player to the random position
            transform.position = randomPos;
        }

    }
    IEnumerator TeleportCooldown()
    {
        canTeleport = false;
        yield return new WaitForSeconds(teleportCooldownOrScanCooldown);
        canTeleport = true;
    }
    void HandleMovement()
    {
        
        // pitch, if we want to invert to feel realistic, remove the - in front of pitchYaw
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);

        // Yaw
        float pitchAngle = transform.eulerAngles.x;
        pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;
        pitchAngle = Mathf.Clamp(pitchAngle, -pitchClamp, pitchClamp);
        transform.eulerAngles = new Vector3(pitchAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        Vector3 pitchVal = (Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);
        if ((pitchAngle <= -pitchClamp && pitchVal.x < 0) || (pitchAngle >= pitchClamp && pitchVal.x > 0))
        {
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            rb.AddRelativeTorque(pitchVal);
        }
        

        //clamp the roll
        float rollAngle = transform.eulerAngles.z;
        rollAngle = (rollAngle > 180) ? rollAngle - 360 : rollAngle;
        rollAngle = Mathf.Clamp(rollAngle, -rollClampVal, rollClampVal);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rollAngle);

        // roll
        Vector3 rollVal = (Vector3.back * roll1D * rollTorque * Time.deltaTime);
        //print(rollVal);
        
        if ((rollAngle <= -rollClampVal && rollVal.z < 0) || (rollAngle >= rollClampVal && rollVal.z > 0))
        {
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            rb.AddRelativeTorque(rollVal);

        }
        /*
        print("angular velocity: " + rb.angularVelocity);
        //  resets players rotation if their speed is less than .1
        
        if((rb.angularVelocity.x <= .1f && rb.angularVelocity.x >= -.1f) && (rb.angularVelocity.y <= .1f && rb.angularVelocity.y >= -.1f) && (rb.angularVelocity.z <= .1f && rb.angularVelocity.z >= -.1f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, startingRotation, timeCount * .001f);
            timeCount += Time.deltaTime;
            if (resetSpeed)
            {
                // need to figure out how to reset the speed properly
                //rb.angularVelocity = Vector3.zero;
                resetSpeed = false;
            }
        }
        */
        /*
        if (rollAngle > 85 && rollVal.z > 0)
        {
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            rb.AddRelativeTorque(rollVal);
        }
        */

        // Thrust (if statement is to prevent controller drift
        if (thrust1D > .1f || thrust1D < -.1f)
        {
            float currentThrust;


            if (boosting)
            {
                currentThrust = thrust * boostMultiplier;
            }
            else
            {
                currentThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;

        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;

            resetSpeed = true;
        }
        //  THIS IS A BAD FIX TO PREVENT DRIFTING (I'm leaving it in for now)
        if(canDrift && (thrust1D < 1f || glide < 1f) && (upThrust < 1f || verticalGlide < 1f) && (strafe1D < 1f || horizontalGlide < 1f))
        {
            rb.velocity = Vector3.zero;
        }

        // Up/Down
        if (upDown1D > .1f || upDown1D < -.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D * upThrust;
            if(verticalGlide <= .5f)
            {
                verticalGlide = 0;
            }
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
            if (verticalGlide <= .5f)
            {
                verticalGlide = 0;
            }
        }

        // strafing
        if(strafe1D > .1f || strafe1D < -.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * strafeThrust;
            if(horizontalGlide <= .5f)
            {
                horizontalGlide = 0;
            }
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
            if (horizontalGlide <= .5f)
            {
                horizontalGlide = 0;
            }
        }

        if(thrust1D > .1f && strafe1D > .1f && upDown1D > .1f && roll1D > .1f && pitchYaw.x > .1f && pitchYaw.y > .1f)
        {
            resetSpeed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CollectionArea"))
        {
            print(other.gameObject.name);
            GameManager.instance.DockingScrap(other.gameObject.transform.parent.gameObject, 10);
        }
        if (other.gameObject.CompareTag("DockingStation"))
        {
            GameManager.instance.ChangeScrapVal(0);
        }
        if (other.gameObject.CompareTag("EnemyProj"))
        {
            Destroy(other.gameObject);

            if (!shielding)
            {
                health--;
                if (health <= 0)
                {
                    // game over
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CollectionArea")){
            GameManager.instance.StopDock();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        /*
        Vector3 averageNormal = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            averageNormal += contact.normal; 
        }
        averageNormal /= collision.contacts.Length;
        //print("Opposing force: " + averageNormal);
        // Calculate the opposing force vector
        float magnitude = 1; // Adjust this value as needed
        Vector3 opposingForce = -averageNormal * magnitude;
        print("Opposing force: " + opposingForce);
        print("Player force: " + rb.velocity);

        // Apply the opposing force to the spaceship
        GetComponent<Rigidbody>().AddForce(opposingForce, ForceMode.Impulse);
        */
    }

    void PauseGameFunc()
    {
        /*
        if (!isPaused && inputPaused)
        {
            isPaused = true;
            print("Paused");
        }
        else if (isPaused && inputPaused)
        {
            isPaused = false;
            print("NOT paused");
        }
        */
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            //print("Paused");
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            //print("NOT paused");
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            Cursor.visible = false;
        }
        //inputPaused = context.performed;
    }
    #endregion
}
