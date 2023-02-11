using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = default;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    [SerializeField] private GameObject destroyParticle;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdate;

    #region Server

    [ClientRpc]
    private void RpcPlayParticle()
    {
        if (isServer)
        {
            return;
        }
        Instantiate(destroyParticle, transform);
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.SeverOnPlayerDie += HandleServerOnPlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.SeverOnPlayerDie -= HandleServerOnPlayerDie;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0) {return;}

        //use Build in method
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        
        //not use
        // currentHealth -= damageAmount;
        //
        // if (currentHealth < 0)
        // {
        //     currentHealth = 0;
        // }

        
        if (currentHealth != 0) {return;}
        
        var particleInstance = Instantiate(destroyParticle, transform);
        NetworkServer.Spawn(particleInstance);
        RpcPlayParticle();

        ServerOnDie?.Invoke();
    }

    [Server]
    private void HandleServerOnPlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) {return;}
        
        DealDamage(currentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdate?.Invoke(newHealth,maxHealth);
    }

    #endregion

}
