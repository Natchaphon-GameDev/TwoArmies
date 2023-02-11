using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = default;
    [SerializeField] private Targeter targeter = default;
    [SerializeField] private Health health = default;
    [SerializeField] private UnitMovement unitMovement = default;
    [SerializeField] private UnityEvent onSelected = default;
    [SerializeField] private UnityEvent onDeselected = default;


    public static event Action<Unit> severOnUnitSpawned;
    public static event Action<Unit> severOnUnitDespawned;

    public static event Action<Unit> authorityOnUnitSpawned;
    public static event Action<Unit> authorityOnUnitDespawned;

    public int GetResourceCost()
    {
        return resourceCost;
    }
    
    public Targeter GetTargeter()
    {
        return targeter;
    }
    
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    #region Sever

    public override void OnStartServer()
    {
        health.ServerOnDie += HandleServerOnDie;
        severOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleServerOnDie;
        severOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void HandleServerOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
    
    #region Client

    public override void OnStartAuthority()
    {
        authorityOnUnitSpawned?.Invoke(this);

    }
    public override void OnStopClient()
    {
        if (!hasAuthority) {return;}
        
        authorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if(!hasAuthority) {return;}
        
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if(!hasAuthority) {return;}
        
        onDeselected?.Invoke();
    }

    #endregion
    
}
