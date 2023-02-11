using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = default;
    [SerializeField] private TMP_Text winnerNameText = default;
    
    private void Start()
    {
        GameOverHandle.ClientOnGameOver += HandleClientOnGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandle.ClientOnGameOver -= HandleClientOnGameOver;
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            //Stop Host
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //Stop Client
            NetworkManager.singleton.StopClient();
        }
    }

    private void HandleClientOnGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won!";
        
        gameOverDisplayParent.SetActive(true);
    }
}
