using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();
    
    [SyncVar(hook = nameof(UpdateLeaderBoardUnit))] 
    private int UnitScore = 0;
    [SyncVar(hook = nameof(UpdateLeaderBoardBuilding))] 
    private int BuildingScore = 0;
    
    [SyncVar(hook = nameof(HandleClientResources))]
    [SerializeField]private int resources = 500;

    public event Action<int> clientOnResourcesUpdated;
    
    public event Action<int> updateLeaderBoardUnit;
    public event Action<int> updateLeaderBoardBuilding;

    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private Building[] buildings = default;
    [SerializeField] private float buildingRangeLimit = default;
    [SerializeField] private LayerMask buildingBlockLayer = default;

    public bool isNotSelection = default;

    [SyncVar]
    public Color teamColor = new Color();

    public Color GetTeamColor()
    {
        return teamColor;
    }
    
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }


    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }
    
    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }
    
    public int GetResources()
    {
        return resources;
    }
    
       
    public void UpdateLeaderBoardBuilding(int oldScore , int newScore)
    {
        GetComponent<DisplayUiControl>().leaderBoardData.TextBase = newScore.ToString();
        
        if (newScore > oldScore)
        {
            updateLeaderBoardBuilding?.Invoke(BuildingScore);
        }
    } 
    public void UpdateLeaderBoardUnit(int oldScore , int newScore)
    {
        GetComponent<DisplayUiControl>().leaderBoardData.TextUnit = newScore.ToString();

        if (newScore > oldScore)
        {
            updateLeaderBoardUnit?.Invoke(UnitScore);
        }
    }

    #region Sever

    public override void OnStartServer()
    {
        Unit.severOnUnitSpawned += SeverHandleUnitSpawned;
        Unit.severOnUnitDespawned += SeverHandleUnitDespawned;
        Building.severOnBuildSpawned += SeverHandleBuildingSpawned;
        Building.severOnBuildDespawned += SeverHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.severOnUnitSpawned -= SeverHandleUnitSpawned;
        Unit.severOnUnitDespawned -= SeverHandleUnitDespawned;
        Building.severOnBuildSpawned -= SeverHandleBuildingSpawned;
        Building.severOnBuildDespawned -= SeverHandleBuildingDespawned;
    }
    
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }
    
    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 spawnPoint)
    {
        Building buildingToPlace = null;

        foreach (var building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) {return;}

        if (resources < buildingToPlace.GetPrice()) {return;}

        var buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, spawnPoint)) {return;}

        var buildingInstance = Instantiate(buildingToPlace.gameObject, spawnPoint, buildingToPlace.transform.rotation);
        
        NetworkServer.Spawn(buildingInstance,connectionToClient);
        
        SetResources(resources - buildingToPlace.GetPrice());
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center , buildingCollider.size / 2, Quaternion.identity , buildingBlockLayer))
        {
            return false;
        }

        foreach (var building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }
    
    private void SeverHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) {return;}
        
        myUnits.Add(unit);
        UnitScore++;
    }
    
    private void SeverHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) {return;}
        
        myUnits.Remove(unit);
        UnitScore--;
    }
    
    private void SeverHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) {return;}
        
        myBuildings.Add(building);
        BuildingScore++;
    }
    
    private void SeverHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) {return;}
        
        myBuildings.Remove(building);
        BuildingScore--;
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return;}
        
        Unit.authorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.authorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.authorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.authorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }
    
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return;}
        
        Unit.authorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.authorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.authorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.authorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void HandleClientResources(int oldResources, int newResources)
    {
        clientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }
    
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }
    
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }
    
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    #endregion
    
}
