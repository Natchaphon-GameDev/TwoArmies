using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommand : MonoBehaviour
{
    [SerializeField] private UnitSelection unitSelection = default;
    [SerializeField] private LayerMask layerMask = default;
    [SerializeField] private GameObject arrowPrefab = default;

    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandle.ClientOnGameOver += HandleClientGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandle.ClientOnGameOver -= HandleClientGameOver;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) {return;}

        var raycast = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(raycast, out var hit,Mathf.Infinity,layerMask)) {return;}

        if (hit.collider.TryGetComponent(out Targetable target))
        {
            if (target.hasAuthority)
            {
                Debug.Log("My Team");
                TryMove(hit.point);
                return;
            }
            
            Debug.Log("attack");
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
        var instanceTemp = Instantiate(arrowPrefab, hit.point, arrowPrefab.transform.rotation);
        Destroy(instanceTemp,.5f);
    }

    private void TryMove(Vector3 point)
    {
        foreach (var unit in unitSelection.selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
    
    private void TryTarget(Targetable target)
    {
        foreach (var unit in unitSelection.selectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void HandleClientGameOver(string winnerName)
    {
        enabled = false;
    }

}
