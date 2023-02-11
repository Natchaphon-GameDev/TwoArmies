using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : NetworkBehaviour
{
    [SerializeField] private GameObject redTeamButtonLocal;
    [SerializeField] private GameObject blueTeamButtonLocal;
    [SerializeField] private GameObject greenTeamButtonLocal;
    [SerializeField] private GameObject purpleTeamButtonLocal;
    [SerializeField] private GameObject redTeamNameLocal;
    [SerializeField] private GameObject blueTeamNameLocal;
    [SerializeField] private GameObject greenTeamNameLocal;
    [SerializeField] private GameObject purpleTeamNameLocal;
    
    [SyncVar] public GameObject redTeamButton;
    [SyncVar] public GameObject blueTeamButton;
    [SyncVar] public GameObject greenTeamButton;
    [SyncVar] public GameObject purpleTeamButton;
    [SyncVar] public GameObject redTeamName;
    [SyncVar] public GameObject blueTeamName;
    [SyncVar] public GameObject greenTeamName;
    [SyncVar] public GameObject purpleTeamName;

    public GameObject LocalPlayerObject;
    public PlayerObjectController playerObjectController;
    public LobbyController lobbyController;

    public override void OnStartClient()
    {
        redTeamButtonLocal = redTeamButton;
        blueTeamButtonLocal = blueTeamButton;
        greenTeamButtonLocal = greenTeamButton;
        purpleTeamButtonLocal = purpleTeamButton;
        redTeamNameLocal = redTeamName;
        blueTeamNameLocal = blueTeamName;
        greenTeamNameLocal = greenTeamName;
        purpleTeamNameLocal = purpleTeamName;
    }

    private void Start()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        playerObjectController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    // public void SelectedTeamColorRed()
    // {
    //     if (playerObjectController.isSelectedColor)
    //     {
    //         return;
    //     }
    //     var player = LocalPlayerObject.GetComponent<RTSPlayer>();
    //     player.SetTeamColor(Color.red);
    //     redTeamButton.GetComponent<Button>().interactable = false;
    //     redTeamName.GetComponent<TextMeshProUGUI>().text = playerObjectController.PlayerName;
    //     playerObjectController.isSelectedColor = true;
    // }
    //
    // public void SelectedTeamColorBlue()
    // {
    //     if (playerObjectController.isSelectedColor)
    //     {
    //         return;
    //     }
    //     var player = LocalPlayerObject.GetComponent<RTSPlayer>();
    //     player.SetTeamColor(Color.blue);
    //     blueTeamButton.GetComponent<Button>().interactable = false;
    //     blueTeamName.GetComponent<TextMeshProUGUI>().text = playerObjectController.PlayerName;
    //     playerObjectController.isSelectedColor = true;
    // }
    //
    // public void SelectedTeamColorGreen()
    // {
    //     if (playerObjectController.isSelectedColor)
    //     {
    //         return;
    //     }
    //     var player = LocalPlayerObject.GetComponent<RTSPlayer>();
    //     player.SetTeamColor(Color.green);
    //     greenTeamButton.GetComponent<Button>().interactable = false;
    //     greenTeamName.GetComponent<TextMeshProUGUI>().text = playerObjectController.PlayerName;
    //     playerObjectController.isSelectedColor = true;
    // }
    //
    // public void SelectedTeamColorOrange()
    // {
    //     if (playerObjectController.isSelectedColor)
    //     {
    //         return;
    //     }
    //     var player = LocalPlayerObject.GetComponent<RTSPlayer>();
    //     player.SetTeamColor(Color.magenta);
    //     purpleTeamButton.GetComponent<Button>().interactable = false;
    //     purpleTeamName.GetComponent<TextMeshProUGUI>().text = playerObjectController.PlayerName;
    //     playerObjectController.isSelectedColor = true;
    // }
}