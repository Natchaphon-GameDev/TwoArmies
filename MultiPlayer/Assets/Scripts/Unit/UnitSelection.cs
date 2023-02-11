using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UnitSelection : MonoBehaviour
{
    [Header("Multi Selection Unit")] 
    [SerializeField] private RectTransform unitSelectArea = default;
    
    private Vector2 startPosition;
    private RTSPlayer rtsPlayer;
    
    [Header("LayerMask")]
    [SerializeField] private LayerMask layerMask = default;
    
    private Camera mainCamera;

    public List<Unit> selectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;

        Unit.authorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandle.ClientOnGameOver += HandleClientGameOver;
    }

    private void OnDestroy()
    {
        Unit.authorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandle.ClientOnGameOver -= HandleClientGameOver;
    }

    private void Update()
    {
        //Debug Loop code
        if (rtsPlayer == null)
        {
            rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (rtsPlayer.isNotSelection)
        {
            return;
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void HandleClientGameOver(string winnerName)
    {
        enabled = false;
    }

    private void UpdateSelectionArea()
    {
        var MousePos = Mouse.current.position.ReadValue();

        var areaWidth = MousePos.x - startPosition.x;
        var areaHeight = MousePos.y - startPosition.y;

        unitSelectArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            selectedUnits.Clear();

        }
      
        unitSelectArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();
        
        UpdateSelectionArea();
    }

    private void ClearSelectionArea()
    {
        unitSelectArea.gameObject.SetActive(false);

        if (unitSelectArea.sizeDelta.magnitude == 0)
        {
            var raycast = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(raycast,out var hit,Mathf.Infinity, layerMask)) {return;}
        

            if (!hit.collider.TryGetComponent(out Unit unit)) {return;}

            if (!unit.hasAuthority) {return;}
        
            selectedUnits.Add(unit);

            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }
            
            return;
        }

        var min = unitSelectArea.anchoredPosition - (unitSelectArea.sizeDelta / 2);
        var max = unitSelectArea.anchoredPosition + (unitSelectArea.sizeDelta / 2);

        foreach (var myUnit in rtsPlayer.GetMyUnits())
        {
            if (selectedUnits.Contains(myUnit)) {continue;}
            
            var screenPosition = mainCamera.WorldToScreenPoint(myUnit.transform.position);

            if (screenPosition.x > min.x 
                && screenPosition.x < max.x 
                && screenPosition.y > min.y 
                && screenPosition.y < max.y)
            {
                selectedUnits.Add(myUnit);
                myUnit.Select();
            }
        }
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        selectedUnits.Remove(unit);
    }
}
