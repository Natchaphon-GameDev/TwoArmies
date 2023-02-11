using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview = default;
    [SerializeField] private Sprite icon = default;
    [SerializeField] private int price = default;
    [SerializeField] private int id = default;
    
    public static event Action<Building> severOnBuildSpawned;
    public static event Action<Building> severOnBuildDespawned;
        
    public static event Action<Building> authorityOnBuildingSpawned;
    public static event Action<Building> authorityOnBuildingDespawned;

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetPrice()
    {
        return price;
    }
    
    public int GetId()
    {
        return id;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    #region Server

    public override void OnStartServer()
    {
        severOnBuildSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        severOnBuildDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        authorityOnBuildingSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) {return;}
        
        authorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion
}
