using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = default;
    [SerializeField] private float liftTime = default;
    [SerializeField] private float launchForce = default;
    
    [SerializeField] private int damageToDeal = default;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    #region Server

    public override void OnStartServer()
    {
        //Create LiftTime
        Invoke(nameof(DestroySelf), liftTime);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) {return;}
        }

        if (other.TryGetComponent(out Health health))
        {
            health.DealDamage(damageToDeal);
        }
        
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
