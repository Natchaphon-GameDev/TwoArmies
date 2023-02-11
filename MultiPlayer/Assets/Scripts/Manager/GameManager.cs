using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraController[] cameraControllers;
    public RTSNetworkManager rtsNetworkManager;
    [SerializeField] private KeyCode keyScoreBoard = KeyCode.Tab;
    [SerializeField] private GameObject myScoreboard;

    private void Start()
    {
        cameraControllers = FindObjectsOfType<CameraController>();
        foreach (var camera in cameraControllers)
        {
            camera.enabled = true;
            var displayUiControl = camera.transform.GetComponent<DisplayUiControl>();

            displayUiControl.SetPlayerValues();

            displayUiControl.SetLeaderBoard();
        }

        rtsNetworkManager = FindObjectOfType<RTSNetworkManager>();
    }

    private void Update()
    {
        if (Input.GetKey(keyScoreBoard))
        {
            myScoreboard.SetActive(true);
        }
        else
        {
            myScoreboard.SetActive(false);
        }
    }

    public void Logout()
    {
        rtsNetworkManager.Logout();
    }
}
