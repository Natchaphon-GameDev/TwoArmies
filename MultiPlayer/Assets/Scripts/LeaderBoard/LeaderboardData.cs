using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardData : MonoBehaviour
{
    public RawImage steamImage;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textUnit;
    public TextMeshProUGUI textBase;
    public Image teamColor;

    public Texture SteamImage
    {
        set
        {
            steamImage.texture = value;
        }
    }
    
    public Color TeamColor
    {
        set
        {
            teamColor.color = value;
        }
    }

    public string TextName
    {
        set
        {
            textName.text = value;
        }
    }
    
    public string TextUnit
    {
        set
        {
            textUnit.text = value;
        }
    }
    
    public string TextBase
    {
        set
        {
            textBase.text = value;
        }
    }
}