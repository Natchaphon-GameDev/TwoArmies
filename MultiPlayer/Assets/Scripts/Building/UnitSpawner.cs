using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = default;
    [SerializeField] private Unit unitPrefab = default;
    [SerializeField] private Transform unitSpawnerPoint = default;
    [SerializeField] private TMP_Text remainingText = default;
    [SerializeField] private Image unitProcessImage = default;
    [SerializeField] private int maxUnitQueue = default;
    [SerializeField] private float spawnMoveRange = default;
    [SerializeField] private float unitSpawnDuration = default;

    [SyncVar(hook = nameof(ClientHandleQueuedUnits))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server
    
    public override void OnStartServer()
    {
        health.ServerOnDie += HandleServerOnDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleServerOnDie;
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) {return;}

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration) {return;}
        
        var unityInstance = Instantiate(unitPrefab.gameObject, unitSpawnerPoint.position, unitSpawnerPoint.rotation);
        
        NetworkServer.Spawn(unityInstance, connectionToClient);

        var spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnerPoint.position.y;
        
        var unitMovement = unityInstance.GetComponent<UnitMovement>();
        
        //TODO : Make spawn point and unit Y axis as same as 
        unitMovement.ServerMove(unitSpawnerPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;
    }

    [Server]
    private void HandleServerOnDie()
    {
        //TODO: PARTICLE EXPLODE Here
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQueue) {return;}
        
        var player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < unitPrefab.GetResourceCost()) {return;}

        queuedUnits++;
        
        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        var newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProcessImage.fillAmount)
        {
            unitProcessImage.fillAmount = newProgress;
        }
        else
        {
            unitProcessImage.fillAmount = Mathf.SmoothDamp(
                unitProcessImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f);
        }
    }
    
    
    private void ClientHandleQueuedUnits(int oldUnit, int newUnit)
    {
        remainingText.text = newUnit.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) {return;}

        if (!hasAuthority) {return;}

        CmdSpawnUnit();
    }

    #endregion

   
}
