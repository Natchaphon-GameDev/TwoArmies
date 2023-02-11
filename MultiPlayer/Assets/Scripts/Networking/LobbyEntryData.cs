using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntryData : MonoBehaviour
{
    [Header("Data")] 
    public CSteamID lobbySteamID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;
    public Button joinButton;

    public void SetLobbyName()
    {
        if (lobbyName == String.Empty)
        {
            lobbyNameText.text = "Empty";
            lobbyNameText.color = Color.gray;
            joinButton.interactable = false;
        }
        else
        {
            lobbyNameText.text = lobbyName;

            if (lobbyName.Contains("'s Lobby"))
            {
                lobbyNameText.color = Color.green;
            }
            else
            {
                lobbyNameText.color = Color.gray;
                joinButton.interactable = false;
            }
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbySteamID);
    }
}