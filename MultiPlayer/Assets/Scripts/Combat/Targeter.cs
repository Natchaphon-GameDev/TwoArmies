using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target = default;
    
    public Targetable GetTarget()
    {
        return target;
    }
    
    #region Server

    public override void OnStartServer()
    {
        GameOverHandle.ServerOnGameOver += HandleServerGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandle.ServerOnGameOver -= HandleServerGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent(out Targetable newTarget)) {return;}
        
        target = newTarget;
    }
    
    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    [Server]
    private void HandleServerGameOver()
    {
        ClearTarget();
    }

    #endregion

    #region Client

    

    #endregion
}
