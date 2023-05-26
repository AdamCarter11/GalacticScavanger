using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [Header("Turret Parts")]
    [SerializeField] private GameObject turretCamera;
    [SerializeField] private GameObject turretBarrelBase;
    [SerializeField] private GameObject turretBarrel;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private GameObject projSpawnPoint;
    
    [Header("Shooting Information")]
    [SerializeField] public float fireRate = 0.1f;
    [SerializeField] private float doubleFireRateTime = 5.0f; //in seconds
    [SerializeField] private float doubleFireCooldown = 10f;
    private float startingFireRate;
    private float barrelSpinningRate = 3f;
    [SerializeField] public int projectileDamage = 1;
    [SerializeField] private float sphereCastRadius = 0.5f;
    [SerializeField] private float sphereCastDistance = 100f;
    private float runningTime = 0f;
    [SerializeField] private LayerMask layerMaskToIgnore;
    [SerializeField] private GameObject projectile;

    // ship information
    private GameObject ship;
    private Transform turretLocation;
    private Vector3 turretToShip;
    
    // input variable
    private Vector2 pitchYaw;
    private bool firePressed;
    private bool abilityPressed;

    [Header("Limit Rotation Angles")] 
    [SerializeField] private float minAngle = -89.999f;
    [SerializeField] private float maxAngle = 5;

    bool doubleFireRate = false;
    int whichClass;
    bool canDouble = true;
    bool turretAbility = false;
    [SerializeField] GameObject muzzleFlashPS;
    [SerializeField] GameObject shieldObj;
    public static bool shielding = false;
    bool shieldingOnCool = false;
    [SerializeField] float shieldTime = 5.0f;
    [SerializeField] float shieldCoolDown = 10.0f;
    private Image gunnertCooldownImage;
    [SerializeField] GameObject barrelToRotate;
    float timeFiringDur = 1f;
    float lastFirePress;

    // Audio Clips
    AudioSource audioSource;
    [SerializeField] AudioClip baseLaserClip;
    [SerializeField] AudioClip amplifyClip;
    [SerializeField] AudioClip shieldClip;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Ship");
        if (ship)
        {
            turretLocation = ship.GetComponent<Ship>().turretLocation;
            //this.transform.parent = turretLocation;
            this.transform.position = turretLocation.position;
            runningTime = fireRate;
        }
        else
        {
            Debug.Log("turret did not find ship");
        }
        startingFireRate = fireRate;
        //gunnertCooldownImage = GameObject.FindGameObjectWithTag("GunnerCooldownImage").GetComponent<Image>();
        barrelToRotate.GetComponent<turretBarrleRot>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (ship)
        {
            // update turret location
            this.transform.position = turretLocation.position;
            
            RotateTurret(pitchYaw);

            //RotationUpdate();
            FireUpdate();
            //DebugViewingRays();
        }
        else
        {
            Debug.Log("no ship detected, not updating turret");
        }

        // turret ability
        if (PlayerPrefs.GetInt("Player2Character") == 1 && !doubleFireRate && canDouble && turretAbility)
        {
            StartCoroutine(fireRateDouble());
        }
        if (PlayerPrefs.GetInt("Player2Character") == 2)
        {
            if (!shielding && turretAbility && !shieldingOnCool)
            {
                StartCoroutine(shieldLogic());
            }
        }
        if (Time.time - lastFirePress >= timeFiringDur)
        {
            barrelToRotate.GetComponent<turretBarrleRot>().enabled = false;
        }
    }
    IEnumerator shieldLogic()
    {
        // Play audio
        audioSource.Stop();
        audioSource.clip = shieldClip;
        audioSource.Play();

        shielding = true;
        shieldObj.SetActive(true);
        yield return new WaitForSeconds(shieldTime);
        shieldObj.SetActive(false);
        shielding = false;
        gunnertCooldownImage.color = Color.red;
        StartCoroutine(shieldCoolDownFunc());
    }
    IEnumerator shieldCoolDownFunc()
    {
        gunnertCooldownImage.color = Color.red;
        shieldingOnCool = true;
        yield return new WaitForSeconds(shieldCoolDown);
        gunnertCooldownImage.color = Color.green;
        shieldingOnCool = false;
    }
    IEnumerator fireRateDouble()
    {
        // Play audio
        audioSource.Stop();
        audioSource.clip = amplifyClip;
        audioSource.Play();

        doubleFireRate = true;
        fireRate /= 2;
        gunnertCooldownImage.color = Color.red;
        yield return new WaitForSeconds(doubleFireRateTime);
        fireRate = startingFireRate;
        doubleFireRate = false;
        StartCoroutine(rateCooldown());
    }
    IEnumerator rateCooldown()
    {
        canDouble = false;
        gunnertCooldownImage.color = Color.red;
        yield return new WaitForSeconds(doubleFireCooldown);
        gunnertCooldownImage.color = Color.green;
        canDouble = true;
    }

    private void RotationUpdate()
    {
        float x = pitchYaw.y;
        float y = pitchYaw.x;

        // rotate base on y axis
        Quaternion localRotation = this.transform.localRotation;
        this.transform.RotateAround(this.transform.position, this.transform.up, y);
        
        // rotate barrel on the x axis
        Vector3 localEuler = turretBarrelBase.transform.localRotation.eulerAngles;

        //clamp the roll
        float rollAngle = turretBarrelBase.transform.localEulerAngles.x - x;
        float clampedRollAngle = turretBarrelBase.transform.eulerAngles.x - x;
        rollAngle = (rollAngle > 180) ? rollAngle - 360 : rollAngle;
        rollAngle = Mathf.Clamp(clampedRollAngle, minAngle, maxAngle);
        turretBarrelBase.transform.localRotation = Quaternion.Euler(new Vector3(rollAngle, turretBarrelBase.transform.localRotation.y, turretBarrelBase.transform.localRotation.z));
    }

    private void FireUpdate()
    {
        if (firePressed)
        {
            runningTime -= Time.deltaTime;
        }
        else
        {
            runningTime = 0;
        }
        
        if (firePressed && runningTime <= 0)
        {
            runningTime = fireRate;
            FiringHelper();
        }
    }

    private void FiringHelper()
    {
        audioSource.PlayOneShot(baseLaserClip);
        lastFirePress = Time.time;
        if(barrelToRotate.GetComponent<turretBarrleRot>().isActiveAndEnabled == false)
        {
            barrelToRotate.GetComponent<turretBarrleRot>().enabled = true;
            print("Barrel started spinning");
        }
        // UNCOMMENT IF WE WANT MUZZLEFLASH
        //Instantiate(muzzleFlashPS, projSpawnPoint.transform.position, Quaternion.identity);
        float tempSphereCastRadius = sphereCastRadius;
        if (doubleFireRate)
        {
            tempSphereCastRadius *= 8;
        }
        RaycastHit[] hits = Physics.SphereCastAll(turretBarrel.transform.position, tempSphereCastRadius, turretBarrel.transform.forward, sphereCastDistance, ~layerMaskToIgnore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Hit an enemy!");
                hit.transform.gameObject.GetComponent<EnemyHealth>().DecrementHealth();
            }
            else
            {
                //Debug.Log("Hit object: " + hit.transform.name);
            }
            // Do something with the hit object here
        }
        // Draw a debug line showing the direction and distance of the spherecast
        Debug.DrawRay(turretBarrel.transform.position, turretBarrel.transform.forward * sphereCastDistance, Color.yellow, 1f);
        //barrelModel.transform.localRotation = new Quaternion(barrelModel.transform.localRotation.x + barrelSpinningRate, barrelModel.transform.localRotation.y, barrelModel.transform.localRotation.z, barrelModel.transform.localRotation.w);
        GameObject tempBullet = Instantiate(projectile, projSpawnPoint.transform.position, Quaternion.Euler(turretBarrel.transform.eulerAngles));
        
    }
    
    private void DebugViewingRays()
    {
        // debug stuff
        Debug.DrawRay(this.transform.position, this.transform.up*10, Color.yellow);
        Debug.DrawRay(this.transform.position, this.transform.forward*10, Color.magenta);
        Debug.DrawRay(turretLocation.position, turretToShip, Color.red);
        Debug.DrawRay(ship.transform.position, ship.transform.up*10, Color.green);
        Debug.DrawRay(ship.transform.position, ship.transform.forward*10, Color.blue);
    }

    
    private const float maxRightRotation = 90f;

    public void RotateTurret(Vector2 inputVector)
    {
        Vector3 turretForward = this.transform.forward;
        Vector3 turretUp = this.transform.up;

        Quaternion shipRotation = ship.transform.rotation;
        Vector3 shipUp = ship.transform.up;

        Vector3 turretToShip = ship.transform.position - this.transform.position;

        // Calculate the dot product between turret forward and ship up vectors
        float dotProduct = Vector3.Dot(turretForward, shipUp);

        // Calculate the maximum allowed angle between turret forward and ship up vectors
        float maxAngle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        // Clamp the rotation on the right vector axis within [-90, 90] degrees based on the maximum angle
        float clampedRightRotation = Mathf.Clamp(inputVector.x * maxRightRotation, -maxAngle, maxAngle);

        // Calculate the rotation axis for the turret's up vector in ship's local space
        Vector3 rotationAxis = Vector3.Cross(turretToShip, turretForward).normalized;

        // Rotate the turret around its up vector axis using the clamped right rotation angle in ship's local space
        Quaternion upRotation = Quaternion.AngleAxis(clampedRightRotation, rotationAxis);

        // Apply the rotation to the turret in ship's global space
        this.transform.rotation = shipRotation * Quaternion.LookRotation(turretForward, upRotation * shipUp);
    }

    #region Input Methods
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
        //Debug.Log("in OnPitchYaw:" + this.gameObject.name);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        firePressed = context.performed;
        //Debug.Log("in OnFire:" + firePressed);
    }
    public void OnTurretAbility(InputAction.CallbackContext context)
    {
        turretAbility = context.performed;
        //Debug.Log("in OnFire:" + firePressed);
    }
    #endregion
}
