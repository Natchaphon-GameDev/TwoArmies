using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Mirror;
using TMPro;
using UnityEngine;

public struct NotificationSend : NetworkMessage
{
    public string title;
    public string description;
}

public class NotificationMessages : MonoBehaviour
{
    public static NotificationMessages instance;
    
    [SerializeField] private NotificationManager notificationUI;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        notificationUI.timer = 3f;
        notificationUI.enableTimer = true;
        NetworkClient.RegisterHandler<NotificationSend>(OnNotification);
    }

    private void OnNotification(NotificationSend message)
    {
        notificationUI.title = message.title; // Change title
        notificationUI.description = message.description; // Change desc
        notificationUI.UpdateUI(); // Update UI
        notificationUI.OpenNotification(); // Open notification
    }
    
    public void NotificationTeamColor()
    {
        notificationUI.title = "Notification Alert"; 
        notificationUI.description = "Please Select Your Team Color"; 
        notificationUI.UpdateUI();
        notificationUI.OpenNotification();
    }
}
