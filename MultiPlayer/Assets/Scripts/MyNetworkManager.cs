using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("[]Connect To Sever");
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        Debug.Log("[]Player Changed Scene");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        var networkPlayer = conn.identity.GetComponent<MyNetworkPlayer>();
        
        networkPlayer.SetPlayerName($"Player {numPlayers}");
        networkPlayer.SetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        Debug.Log($"[]Player{conn.connectionId} Connect from Sever");
        Debug.Log($"[]Now We have {numPlayers} player");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("[]Client Disconnect from sever");
    }
}


