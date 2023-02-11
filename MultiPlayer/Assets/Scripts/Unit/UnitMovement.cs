using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = default;
    [SerializeField] private Targeter targeter = default;
    [SerializeField] private float chaseRange = default;
    

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
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        //Clear target when Unit Move
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
        
        Debug.Log("Walk");

    }

    [Server]
    private void HandleServerGameOver()
    {
        agent.ResetPath();
    }
    
    [ServerCallback]
    private void Update()
    {
        var target = targeter.GetTarget();
        
        //Chasing Target
        if (target != null)
        {
            //How to optimize Vector3.Distance
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }
        
        if (!agent.hasPath) {return;}
        
        if (agent.remainingDistance > agent.stoppingDistance) {return;}
        
        agent.ResetPath();
    }

    #endregion

}
