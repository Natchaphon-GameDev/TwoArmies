using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = default;

    private RTSPlayer player = default;

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (player != null)
            {
                ClientHandleResourcesUpdate(player.GetResources());
                
                player.clientOnResourcesUpdated += ClientHandleResourcesUpdate;
            }
        }
    }

    private void OnDestroy()
    {
        player.clientOnResourcesUpdated -= ClientHandleResourcesUpdate;
    }

    private void ClientHandleResourcesUpdate(int resources)
    {
        resourcesText.text = $"Money : {resources} $";
    }
}
