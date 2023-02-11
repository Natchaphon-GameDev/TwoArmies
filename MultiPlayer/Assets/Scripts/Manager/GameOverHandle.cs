using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;

public class GameOverHandle : NetworkBehaviour
{
    public List<UnitBase> bases = new List<UnitBase>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        UnitBase.serverOnBaseSpawned += HandleServerBaseSpawned;
        UnitBase.serverOnBaseDespawned += HandleServerBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.serverOnBaseSpawned -= HandleServerBaseSpawned;
        UnitBase.serverOnBaseDespawned -= HandleServerBaseDespawned;
    }

    [Server]
    private void HandleServerBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void HandleServerBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1) {return;}
        //if can rebuild base change to < 1

        var playerId = bases[0].connectionToClient.identity.GetComponent<PlayerObjectController>().PlayerName;
        
        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
        
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
