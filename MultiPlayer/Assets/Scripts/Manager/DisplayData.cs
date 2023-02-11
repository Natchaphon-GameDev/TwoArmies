using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum  Speaker{idle,talk,mute}

public class DisplayData : MonoBehaviour
{
    public RawImage steamImage;
    public TextMeshProUGUI textName;
    public CanvasGroup canvasGroup;

    [Header("Speaker Setup")] [SerializeField]
    private Image speakerImage;

    public Sprite idle, talk, mute;

    public Speaker statusSpeaker;

    public Speaker StatusSpeaker
    {
        set
        {
            statusSpeaker = value;
            switch (statusSpeaker)
            {
                case Speaker.idle:
                    speakerImage.color = Color.white;
                    speakerImage.sprite = idle;
                    break;
                case Speaker.mute:
                    speakerImage.color = Color.red;
                    speakerImage.sprite = mute;
                    break;
                case Speaker.talk:
                    speakerImage.color = Color.green;
                    speakerImage.sprite = talk;
                    break;
            }
        }
        get
        {
            return statusSpeaker;
        }
    }

    public Texture SteamImage
    {
        set
        {
            steamImage.texture = value;
        }
    }

    public string TextName
    {
        set
        {
            textName.text = value;
        }
    }
}