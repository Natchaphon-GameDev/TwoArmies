using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    public TextMeshProUGUI LobbyNameText;

    //Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    [SerializeField] private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalplayerController;

    //Ready
    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;
    [SerializeField] private Button greenTeamButton;
    [SerializeField] private Button purpleTeamButton;
    [SerializeField] private TextMeshProUGUI redTeamName;
    [SerializeField] private TextMeshProUGUI blueTeamName;
    [SerializeField] private TextMeshProUGUI greenTeamName;
    [SerializeField] private TextMeshProUGUI purpleTeamName;


    //Manager
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

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalplayerController.isReady)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (var player in Manager.GamePlayers)
        {
            if (player.isReady)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (LocalplayerController.PlayerIDNumber == 1) //Hosting
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        } //Host

        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (PlayerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void UpdateTeamColor(int team, string playerName)
    {
        switch (team)
        {
            case 1:
                redTeamButton.interactable = false;
                redTeamName.text = playerName;
                break;
            case 2:
                blueTeamButton.interactable = false;
                blueTeamName.text = playerName;
                break;
            case 3:
                greenTeamButton.interactable = false;
                greenTeamName.text = playerName;
                break;
            case 4:
                purpleTeamButton.interactable = false;
                purpleTeamName.text = playerName;
                break;
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }


    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.isReady = player.isReady;
            NewPlayerItemScript.SetPlayerValues();


            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }

        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.isReady = player.isReady;
                NewPlayerItemScript.SetPlayerValues();


                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.isReady = player.isReady;
                    PlayerListItemScript.SetPlayerValues();
                    if (player == LocalplayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }

        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                if (playerlistItemToRemove)
                {
                    GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                    PlayerListItems.Remove(playerlistItemToRemove);
                    ObjectToRemove = null;
                    Destroy(ObjectToRemove);
                }
            }
        }
    }

    public void SelectedTeamColorRed()
    {
        if (LocalplayerController.isSelectedColor)
        {
            return;
        }

        var player = LocalPlayerObject.GetComponent<RTSPlayer>();
        player.SetTeamColor(Color.red);
        redTeamButton.interactable = false;
        redTeamName.text = LocalplayerController.PlayerName;
        LocalplayerController.SelectTeam(1);
        LocalplayerController.isSelectedColor = true;
    }

    public void SelectedTeamColorBlue()
    {
        if (LocalplayerController.isSelectedColor)
        {
            return;
        }

        var player = LocalPlayerObject.GetComponent<RTSPlayer>();
        player.SetTeamColor(Color.blue);
        blueTeamButton.interactable = false;
        blueTeamName.text = LocalplayerController.PlayerName;
        LocalplayerController.SelectTeam(2);
        LocalplayerController.isSelectedColor = true;
    }

    public void SelectedTeamColorGreen()
    {
        if (LocalplayerController.isSelectedColor)
        {
            return;
        }

        var player = LocalPlayerObject.GetComponent<RTSPlayer>();
        player.SetTeamColor(Color.green);
        greenTeamButton.interactable = false;
        greenTeamName.text = LocalplayerController.PlayerName;
        LocalplayerController.SelectTeam(3);
        LocalplayerController.isSelectedColor = true;
    }

    public void SelectedTeamColorPurple()
    {
        if (LocalplayerController.isSelectedColor)
        {
            return;
        }

        var player = LocalPlayerObject.GetComponent<RTSPlayer>();
        player.SetTeamColor(Color.magenta);
        purpleTeamButton.interactable = false;
        purpleTeamName.text = LocalplayerController.PlayerName;
        LocalplayerController.SelectTeam(4);
        LocalplayerController.isSelectedColor = true;
    }

    public void StartGame(string sceneName)
    {
        LocalplayerController.CanStartGame(sceneName);
    }

    public void Logout()
    {
        manager.StopHost();
        manager.StopClient();
    }
}