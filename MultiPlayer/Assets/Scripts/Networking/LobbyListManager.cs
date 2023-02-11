using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using System.Linq;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;

    [Header("Lobbies")] public GameObject lobbyListMenu;
    public GameObject lobbyEntryPrefab;
    public GameObject scrollViewContent;

    public GameObject homeMenu;

    public List<LobbyEntryData> listOfLobbies = new List<LobbyEntryData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        homeMenu.SetActive(false);
        lobbyListMenu.SetActive(true);
        
        SteamLobby.Instance.GetListOfLobbies();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID == result.m_ulSteamIDLobby && listOfLobbies.All(x => x.lobbySteamID != (CSteamID) lobbyIds[i].m_SteamID))
            {
                var createdLobbyItem = Instantiate(lobbyEntryPrefab);
                var createdLobbyDataEntry = createdLobbyItem.GetComponent<LobbyEntryData>();
                createdLobbyDataEntry.lobbySteamID = (CSteamID) lobbyIds[i].m_SteamID;
                createdLobbyDataEntry.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID) lobbyIds[i].m_SteamID, "name");
                createdLobbyDataEntry.SetLobbyName();
                
                createdLobbyItem.transform.SetParent(scrollViewContent.transform);
                createdLobbyItem.transform.localScale = Vector3.one;
                
                listOfLobbies.Add(createdLobbyDataEntry);

                if (createdLobbyDataEntry.lobbyName.Contains("'s Lobby"))
                {
                    createdLobbyItem.transform.SetAsFirstSibling();
                }
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void DestroyLobbies()
    {
        foreach (var lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem.gameObject);
        }

        listOfLobbies.Clear();
    }
}