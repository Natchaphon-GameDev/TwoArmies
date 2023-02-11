using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerVoiceChat : NetworkBehaviour
{
    public AudioSource audioSource;
    public KeyCode keyTalk = KeyCode.V;
    public KeyCode keyMute = KeyCode.M;
    
    public bool isMute;

    private DisplayUiControl displayUIControl;
    [SerializeField] private bool isHearYourself;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        displayUIControl = GetComponent<DisplayUiControl>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(keyTalk) && !isMute)
            {
                SteamUser.StartVoiceRecording();
                Debug.Log("Start Record");
                CmdChangeSpeakerStatus(Speaker.talk);
            }
            else if (Input.GetKeyUp(keyTalk) && ! isMute)
            {
                SteamUser.StopVoiceRecording();
                Debug.Log("Stop Record");
                CmdChangeSpeakerStatus(Speaker.idle);
            }

            if (Input.GetKeyDown(keyMute))
            {
                isMute = !isMute;
                CmdChangeSpeakerStatus(isMute ? Speaker.mute : Speaker.idle);
                if (isMute)
                {
                    SteamUser.StopVoiceRecording();
                }
            }
            
            GetVoice();
        }
    }

    private void GetVoice()
    {
        uint compressed;
        EVoiceResult result = SteamUser.GetAvailableVoice(out compressed);

        if (result == EVoiceResult.k_EVoiceResultOK && compressed > 1024)
        {
            Debug.Log(compressed);
            byte[] destBuffer = new byte[1024];
            uint bytesWritten;
            result = SteamUser.GetVoice(true, destBuffer, 1024, out bytesWritten);
            if (result == EVoiceResult.k_EVoiceResultOK && bytesWritten > 0)
            {
                CmdSendData(destBuffer,bytesWritten);
            }
        }
    }

    [Command(channel = 2)]
    private void CmdSendData(byte[] data, uint size)
    {
        Debug.Log("Command Activate");
        PlayerVoiceChat[] players = FindObjectsOfType<PlayerVoiceChat>();

        for (int i = 0; i < players.LongLength; i++)
        {
            TargetPlayerSound(players[i].GetComponent<NetworkIdentity>().connectionToClient,data,size);
        }
    }
    
    [TargetRpc(channel = 2)]
    private void TargetPlayerSound(NetworkConnection conn, byte[] destBuffer, uint bytesWritten)
    {
        if (!isHearYourself)
        {
            if (isLocalPlayer)
            {
                return;
            }
        }
        
        Debug.Log("Target");
        byte[] destBuffer2 = new byte[22050 * 2];
        uint bytesWritten2;
        EVoiceResult result = SteamUser.DecompressVoice(destBuffer, bytesWritten, destBuffer2, (uint) destBuffer2.Length, out bytesWritten2, 22050);

        if (result == EVoiceResult.k_EVoiceResultOK && bytesWritten2 > 0)
        {
            audioSource.clip = AudioClip.Create(Random.Range(100, 1000000).ToString(), 22050, 1, 22050, false);

            float[] sample = new float[22050];
            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = (short) (destBuffer2[i * 2] | destBuffer2[i * 2 + 1] << 8) / 32768.0f;
            }

            audioSource.clip.SetData(sample, 0);
            audioSource.Play();
        }
    }

    [Command]
    private void CmdChangeSpeakerStatus(Speaker status)
    {
        displayUIControl.GetDisplayData.StatusSpeaker = status;
        RpcChangeSpeakerStatus(status);
    }

    [ClientRpc]
    private void RpcChangeSpeakerStatus(Speaker status)
    {
        displayUIControl.GetDisplayData.StatusSpeaker = status;
    }
}
