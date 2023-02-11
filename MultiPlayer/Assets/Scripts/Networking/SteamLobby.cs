using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    //Callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;
    
    //Variables
    public ulong currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private RTSNetworkManager manager;
    
    //Lobby List Callback
    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyListUpdate;
    
    private List<CSteamID> lobbyIds = new List<CSteamID>();

    //GameObjects
    // public GameObject hostButton;
    // public TextMeshProUGUI lobbyNameText;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        
        if (Instance == null)
        {
            Instance = this;
        }
        
        manager = GetComponent<RTSNetworkManager>();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        lobbyListUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        //Friend Only
        //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        
        //Public
        //Show every lobbies that steam can see 
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }

    private void OnDestroy()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.LeaveLobby((CSteamID)currentLobbyID);
        }
        lobbyCreated.Unregister();
        joinRequest.Unregister();
        lobbyEntered.Unregister();
        lobbyList.Unregister();
        lobbyListUpdate.Unregister();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }
        Debug.Log("Lobby has been created");

        //For Test Debug
        // if (!this)
        // {
        //     return;
        // }
        
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        // hostButton.SetActive(false);
        currentLobbyID = callback.m_ulSteamIDLobby;
        // lobbyNameText.gameObject.SetActive(true);
        // lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        
        //client
        if (NetworkServer.active)
        {
            return;
        }
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        //For Test Debug
        if (!this)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(manager.networkAddress))
        {
            manager.StartClient();
        }
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetListOfLobbies()
    {
        if (lobbyIds.Count > 0)
        {
            lobbyIds.Clear();
        }
        //Limit show lobbies 100 room
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(50);
        
        //For Implement next time
        SteamAPICall_t tempList = SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbyListManager.instance.DisplayLobbies(lobbyIds,result);
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbyListManager.instance.listOfLobbies.Count > 0)
        {
            LobbyListManager.instance.DestroyLobbies();
        }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            var lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIds.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }
}
