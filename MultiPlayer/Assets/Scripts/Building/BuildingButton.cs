using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = default;
    [SerializeField] private Image iconImg = default;
    [SerializeField] private TMP_Text priceText = default;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Start()
    {
        mainCamera = Camera.main;

        iconImg.sprite = building.GetIcon();
        priceText.text = $"{building.GetPrice().ToString()} $";

        buildingCollider = building.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (player == null)
        {
             player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (buildingPreviewInstance == null) {return;}
        
        UpdateBuildingPreview();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) {return;}

        if (player.GetResources() < building.GetPrice()) {return;}

        player.isNotSelection = true;

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
        
        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) {return;}

        var raycast = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(raycast, out var hit, Mathf.Infinity,floorMask))
        {
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        
        player.isNotSelection = false;
        
        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        var raycast = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(raycast, out var hit, Mathf.Infinity,floorMask)) {return;}

        buildingPreviewInstance.transform.position = hit.point;

        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }

        var color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;
        
        buildingRendererInstance.material.SetColor("_BaseColor", color);
    }
}