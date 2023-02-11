using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayName))] [SerializeField] private string playerName = default;

    [SyncVar(hook = nameof(HandleDisplayColor))] [SerializeField] private Color playerColor = default;

    [SerializeField] private string reName;
    [SerializeField] private TMP_Text displayNameText = default;
    [SerializeField] private Renderer playerRenderer = default;

    #region Server

    [Server]
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    [Server]
    public void SetColor(Color color)
    {
        playerColor = color;
    }

    [Command]
    private void CmdSetDisplayName(string newName)
    {
        
        if (newName.Length < 2 || newName.Length > 20)
        {
            return;
        }
        
        RpcLogNewName(newName);
        SetPlayerName(newName);
    }

    #endregion

    #region Client

    private void HandleDisplayColor(Color oldColor, Color newColor)
    {
        playerRenderer.material.color = newColor;
    }

    private void HandleDisplayName(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    [ContextMenu("Rename")]
    private void Rename()
    {
        CmdSetDisplayName("Shiro");
    }

    [TargetRpc]
    private void RpcLogNewName(string newName)
    {
        Debug.Log($"Your name has been changed to {newName}");
    }

    #endregion
}
