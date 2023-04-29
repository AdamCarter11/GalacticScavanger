using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class Graviton : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public bool IsAttractee {
        get
        {
            return isAttractee;
        }
        set
        {
            if(value==true)
            {
                if(!GravityHandlerBehavior.attractees.Contains(this.GetComponent<Rigidbody>()))
                {
                    GravityHandlerBehavior.attractees.Add(_rigidbody);
                }
                
            }
            else if(value==false)
            {
                GravityHandlerBehavior.attractees.Remove(_rigidbody);
            }
            isAttractee = value;
        }
    }
    public bool IsAttractor
    {
        get
        {
            return isAttractor;
        }
        set
        {
            if(value==true)
            {
                if(!GravityHandlerBehavior.attractors.Contains(this.GetComponent<Rigidbody>()))
                    GravityHandlerBehavior.attractors.Add(_rigidbody);
            }
            else if(value==false)
            {
                GravityHandlerBehavior.attractors.Remove(_rigidbody);
            }
            isAttractor = value;
        }
    }
    [SerializeField] private bool isAttractor;
    [SerializeField] private bool isAttractee;

    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private bool applyInitialVelocityOnStart;

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        IsAttractor = isAttractor;
        IsAttractee = isAttractee;
    }

    void Start()
    {
        if (applyInitialVelocityOnStart)
        {
            ApplyVelocity(initialVelocity);
        }
    }

    private void OnDisable()
    {
        GravityHandlerBehavior.attractors.Remove(_rigidbody);
        GravityHandlerBehavior.attractees.Remove(_rigidbody);
    }

    void ApplyVelocity(Vector3 velocity)
    {
        _rigidbody.AddForce(initialVelocity, ForceMode.Impulse);
    }

   
}
