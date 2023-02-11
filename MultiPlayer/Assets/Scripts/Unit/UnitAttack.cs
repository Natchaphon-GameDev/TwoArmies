using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitAttack : NetworkBehaviour
{
    [SerializeField] private Targeter target = default;
    [SerializeField] private GameObject bulletPrefab = default;
    [SerializeField] private Transform bulletSpawnPoint = default;
    [SerializeField] private float fireRange = default;
    [SerializeField] private float fireRate= default;
    [SerializeField] private float rotationSpeed = default;
    
    private float lastFireTime;

    #region Server

    [ServerCallback]
    private void Update()
    {
        var target = this.target.GetTarget();
        
        if (target == null) {return;}
        
        if (!CanFireTarget()) {return;}

        var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            var bulletRotation =
                Quaternion.LookRotation(target.GetAimAtPoint().position - bulletSpawnPoint.position);
            
            var bulletInstance = Instantiate(bulletPrefab, bulletSpawnPoint.position , bulletRotation);
            
            NetworkServer.Spawn(bulletInstance, connectionToClient);
            
            lastFireTime = Time.time;
        }

    }

    [Server]
    private bool CanFireTarget()
    {
        //Optimize Vector3.Distance
        return (target.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

    #endregion
    
}
