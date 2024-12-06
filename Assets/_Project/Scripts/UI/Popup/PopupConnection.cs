using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupConnection : UIBase
{
    [SerializeField] private TMP_InputField gameServerIp;
    [SerializeField] private TMP_InputField gameServerPort;
    [SerializeField] private TMP_InputField chattingServerIp;
    [SerializeField] private TMP_InputField chattingServerPort;

    public override void Opened(object[] param)
    {
        gameServerIp.text = PlayerPrefs.GetString("gameServerIp", "");
        gameServerPort.text = PlayerPrefs.GetString("gameServerPort", "");

        chattingServerIp.text = PlayerPrefs.GetString("chattingServerIp", "");
        chattingServerPort.text = PlayerPrefs.GetString("chattingServerPort", "");
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupConnection>();
    }

    public void OnClickIPPortInput()
    {
        if (string.IsNullOrEmpty(gameServerIp.text)) gameServerIp.text = "127.0.0.1";
        if (string.IsNullOrEmpty(gameServerPort.text)) gameServerPort.text = "5555";
        if (string.IsNullOrEmpty(chattingServerIp.text)) chattingServerIp.text = "127.0.0.1";
        if (string.IsNullOrEmpty(chattingServerPort.text)) chattingServerPort.text = "5556";

        PlayerPrefs.SetString("gameServerIp", gameServerIp.text);
        PlayerPrefs.SetString("gameServerPort", gameServerPort.text);
        PlayerPrefs.SetString("chattingServerIp", chattingServerIp.text);
        PlayerPrefs.SetString("chattingServerPort", chattingServerPort.text);

        HideDirect();
    }    
}