using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;


    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool isReady;

    [SyncVar] public bool isSelectedColor;

    [SyncVar(hook = nameof(SelectedRedTeam))]
    public bool isSelectedColorRed;

    [SyncVar(hook = nameof(SelectedBlueTeam))]
    public bool isSelectedColorBlue;

    [SyncVar(hook = nameof(SelectedGreenTeam))]
    public bool isSelectedColorGreen;

    [SyncVar(hook = nameof(SelectedPurpleTeam))]
    public bool isSelectedColorPurple;

    private RTSNetworkManager manager;

    private RTSNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = RTSNetworkManager.singleton as RTSNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SelectedRedTeam(bool oldValue, bool newValuer)
    {
        LobbyController.Instance.UpdateTeamColor(1,PlayerName);
    }

    public void SelectedBlueTeam(bool oldValue, bool newValuer)
    {
        LobbyController.Instance.UpdateTeamColor(2,PlayerName);
    }

    public void SelectedGreenTeam(bool oldValue, bool newValuer)
    {
        LobbyController.Instance.UpdateTeamColor(3,PlayerName);
    }

    public void SelectedPurpleTeam(bool oldValue, bool newValuer)
    {
        LobbyController.Instance.UpdateTeamColor(4,PlayerName);
    }

    public void PlayerReadyUpdate(bool oldValue, bool newValuer)
    {
        if (isServer)
        {
            this.isReady = newValuer;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        if (!isSelectedColor)
        {
            if (isLocalPlayer)
            {
                NotificationMessages.instance.NotificationTeamColor();
            }
            else if (isServer)
            {
                NotificationMessages.instance.NotificationTeamColor();
            }
            return;
        }
        this.PlayerReadyUpdate(this.isReady, !this.isReady);
    }
    
    [Command]
    private void CmdSetTeamColor(int team)
    {
        if (team == 1)
        {
            isSelectedColorRed = true;
            isSelectedColor = true;
        }
        else if (team == 2)
        {
            isSelectedColorBlue = true;
            isSelectedColor = true;
        }
        else if (team == 3)
        {
            isSelectedColorGreen = true;
            isSelectedColor = true;
        }
        else if (team == 4)
        {
            isSelectedColorPurple = true;
            isSelectedColor = true;
        }
    }

    public void SelectTeam(int team)
    {
        if (hasAuthority)
        {
            CmdSetTeamColor(team);
        }
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
        Debug.Log("Client Exit");
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer) //Host
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    //Start Game
    public void CanStartGame(string sceneName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(sceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        Manager.StartGame(sceneName);
    }
}