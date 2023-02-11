using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health;

    public static event Action<int> SeverOnPlayerDie; 
    public static event Action<UnitBase> serverOnBaseSpawned;
    public static event Action<UnitBase> serverOnBaseDespawned;
     
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += HandleServerOnDie;
        serverOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleServerOnDie;
        serverOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void HandleServerOnDie()
    {
        SeverOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    

    #endregion
}
