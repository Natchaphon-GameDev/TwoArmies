using System;
using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using Mirror;
using TMPro;
using UnityEngine;


public class LeaderboardController : MonoBehaviour
{
    public string playerName;
    public int IDUnit;
    public int IDBuilding;

    private GameObject playerObj;

    private int maxScore = 10;
    [SerializeField] private LeaderboardEnter leaderboardEnterData;
    [SerializeField] private Transform contentUnit;
    [SerializeField] private Transform contentBuilding;
    [SerializeField] private GameObject leaderboardTable;
    [SerializeField] private List<GameObject> listTemp;
    private bool toggle;
    
    private RTSPlayer player = default;

    private void Start()
    {
        LootLockerSDKManager.StartGuestSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Failed");
            }
        });

        playerObj = GameObject.Find("LocalGamePlayer");
        playerName = playerObj.GetComponent<PlayerObjectController>().PlayerName;
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (player != null)
            {
                player.updateLeaderBoardBuilding += SubmitScoreBuilding;
                player.updateLeaderBoardUnit += SubmitScoreUnit;
            }
        }
    }
    
    private void OnDestroy()
    {
        player.updateLeaderBoardBuilding -= SubmitScoreBuilding;
        player.updateLeaderBoardUnit -= SubmitScoreUnit;
    }

    private void SubmitScoreUnit(int playerScore)
    {
        LootLockerSDKManager.SubmitScore(playerName, playerScore, IDUnit, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }

    private void SubmitScoreBuilding(int playerScore)
    {
        if (playerScore == 1) {return;}

        LootLockerSDKManager.SubmitScore(playerName, playerScore, IDBuilding, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }

    public void ShowScore()
    {
        toggle = !toggle;

        if (toggle)
        {
            ShowScoreBuilding();
            ShowScoreUnit();
            leaderboardTable.SetActive(true);
        }
        else
        {
            leaderboardTable.SetActive(false);
            foreach (var list in listTemp)
            {
                Destroy(list);
            }
            listTemp.Clear();
        }
    }

    //Make it can show both of unit and building
    private void ShowScoreUnit()
    {
        LootLockerSDKManager.GetScoreList(IDUnit, maxScore, (response) =>
        {
            if (response.success)
            {
                var scores = response.items;

                for (var i = 0; i < scores.Length; i++)
                {
                    var leaderboard = Instantiate(leaderboardEnterData, contentUnit);
                    leaderboard.nameText.text = ($"#{scores[i].rank} {scores[i].member_id}");
                    leaderboard.scoreText.text = ($"{scores[i].score}");
                    listTemp.Add(leaderboard.gameObject);
                }

                if (scores.Length < maxScore)
                {
                    for (var i = scores.Length; i < maxScore; i++)
                    {
                        var leaderboard = Instantiate(leaderboardEnterData, contentUnit);
                        leaderboard.nameText.text = $"#{i + 1} ......none";
                        listTemp.Add(leaderboard.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }
    
    private void ShowScoreBuilding()
    {
        LootLockerSDKManager.GetScoreList(IDBuilding, maxScore, (response) =>
        {
            if (response.success)
            {
                var scores = response.items;

                for (var i = 0; i < scores.Length; i++)
                {
                    var leaderboard = Instantiate(leaderboardEnterData, contentBuilding);
                    leaderboard.nameText.text = ($"#{scores[i].rank} {scores[i].member_id}");
                    leaderboard.scoreText.text = ($"{scores[i].score}");
                    listTemp.Add(leaderboard.gameObject);
                }

                if (scores.Length < maxScore)
                {
                    for (var i = scores.Length; i < maxScore; i++)
                    {
                        var leaderboard = Instantiate(leaderboardEnterData, contentBuilding);
                        leaderboard.nameText.text = $"#{i + 1} ......none";
                        listTemp.Add(leaderboard.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }
}