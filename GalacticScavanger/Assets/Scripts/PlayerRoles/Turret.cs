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


    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Ship");
        if (ship)
        {
            turretLocation = ship.GetComponent<Ship>().turretLocation;
            this.transform.parent = turretLocation;
            this.transform.position = turretLocation.position;
            runningTime = fireRate;
        }
        else
        {
            Debug.Log("turret did not find ship");
        }
        startingFireRate = fireRate;
        gunnertCooldownImage = GameObject.FindGameObjectWithTag("GunnerCooldownImage").GetComponent<Image>();
        barrelToRotate.GetComponent<turretBarrleRot>().enabled = false;
    }

    private void Update()
    {
        if (ship)
        {
            RotationUpdate();
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
        rollAngle = (rollAngle > 180) ? rollAngle - 360 : rollAngle;
        rollAngle = Mathf.Clamp(rollAngle, minAngle, maxAngle);
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
