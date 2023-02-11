using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Notification = UnityEngine.Playables.Notification;
using Random = UnityEngine.Random;

public class RTSNetworkManager : NetworkManager
{
   //Add
   public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
   //Add
   [SerializeField] private PlayerObjectController gamePlayerPrefab;

   [SerializeField] private GameObject unitSpawnerPrefab = default;
   [SerializeField] private GameOverHandle gameOverHandlePrefab = default;
   
   [ContextMenu("Test Notification")]
   public void SendNotification(string title, string description)
   {
      NetworkServer.SendToAll(new NotificationSend{title = title , description = description});
   }

   public override void OnServerAddPlayer(NetworkConnectionToClient conn)
   {
      //Add
      if (SceneManager.GetActiveScene().name == "RTS Multiplayer Lobby")
      {
         var gamePlayerInstance = Instantiate(gamePlayerPrefab);
         
         gamePlayerInstance.ConnectionID = conn.connectionId;
         gamePlayerInstance.PlayerIDNumber = GamePlayers.Count + 1;
         gamePlayerInstance.PlayerSteamID = (ulong) SteamMatchmaking
            .GetLobbyMemberByIndex((CSteamID) SteamLobby.Instance.currentLobbyID, GamePlayers.Count);
         
         NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
         
         var player = conn.identity.GetComponent<RTSPlayer>();

         player.SetTeamColor(new Color(
            Random.Range(0f,1f),
            Random.Range(0f,1f),
            Random.Range(0f,1f)
         ));

         // SendNotification("Player has Joined!",gamePlayerInstance.PlayerName);
        
      }
   }

   public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
   {
      base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
      if (newSceneName != SceneManager.GetSceneByBuildIndex(0).name)
      {
         // foreach (var player in GamePlayers)
         // {
         // }
      }
   }

   public override void OnServerSceneChanged(string newSceneName)
   {
      if (SceneManager.GetActiveScene().name.StartsWith("RTS Multiplayer Map"))
      {
         var gameOverHandleInstance = Instantiate(gameOverHandlePrefab);
         NetworkServer.Spawn(gameOverHandleInstance.gameObject);

         foreach(var player in GamePlayers)
         {
            var baseInstance = Instantiate(
               unitSpawnerPrefab,
               GetStartPosition().position,
               Quaternion.identity);
            
            NetworkServer.Spawn(baseInstance, player.connectionToClient);
            
            player.transform.position = (baseInstance.transform.position);
         }
      }
   }

   //Add
   public void StartGame(string sceneName)
   {
      ServerChangeScene(sceneName);
   }
   
   //Add
   public void Logout()
   {
      StopHost();
      StopClient();
   }
   
}
