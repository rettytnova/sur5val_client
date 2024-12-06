using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using Unity.Multiplayer.Playmode;

public class PopupLogin : UIBase
{
    [SerializeField] private GameObject touch;
    [SerializeField] public GameObject buttonSet;
    [SerializeField] public GameObject register;
    [SerializeField] public GameObject login;
    [SerializeField] private TMP_InputField loginId;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_InputField regId;
    [SerializeField] private TMP_InputField regNickname;
    [SerializeField] private TMP_InputField regPassword;
    [SerializeField] private TMP_InputField regPasswordRe;
    public override void Opened(object[] param)
    {
        register.SetActive(false);
        login.SetActive(false);

        var tags = CurrentPlayer.ReadOnlyTags();
        if (tags.Length == 0)
        {
            tags = new string[1] { "player1" };
        }
        loginId.text = PlayerPrefs.GetString("id" + tags[0], "");
        loginPassword.text = PlayerPrefs.GetString("password" + tags[0], "");
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupLogin>();
    }

    public void OnClickLogin()
    {
        if (!Managers.networkManager.GameServerIsConnected())
        {
            var gameServerIp = PlayerPrefs.GetString("gameServerIp", "127.0.0.1");
            var gameServerPort = PlayerPrefs.GetString("gameServerPort", "5555");

            var chattingServerIp = PlayerPrefs.GetString("chattingServerIp", "127.0.0.1");
            var chattingServerPort = PlayerPrefs.GetString("chattingServerPort", "5556");

            //Debug.Log($"gameServerIp {gameServerIp} gameServerPort {gameServerPort}");
            //Debug.Log($"chattingServerIp {chattingServerIp} chattingServerPort {chattingServerPort}");

            Managers.networkManager.GameServerConnect(gameServerIp,int.Parse(gameServerPort), "login");
            Managers.networkManager.ChattingServerConnect(chattingServerIp,int.Parse(chattingServerPort));
        }
        else
        {
            buttonSet.SetActive(false);
            login.SetActive(true);
        }
    }

    public void OnClickRegister()
    {
        if (!Managers.networkManager.GameServerIsConnected())
        {
            var gameServerIp = PlayerPrefs.GetString("gameServerIp", "127.0.0.1");
            var gameServerPort = PlayerPrefs.GetString("gameServerPort", "5555");

            var chattingServerIp = PlayerPrefs.GetString("chattingServerIp", "127.0.0.1");
            var chattingServerPort = PlayerPrefs.GetString("chattingServerPort", "5556");           

            Managers.networkManager.GameServerConnect(gameServerIp, int.Parse(gameServerPort), "register");
            Managers.networkManager.ChattingServerConnect(chattingServerIp, int.Parse(chattingServerPort));
        }
        else
        {
            buttonSet.SetActive(false);
            register.SetActive(true);
        }
    }

    public void OnClickSendLogin()
    {
        GamePacket packet = new GamePacket();
        packet.LoginRequest = new C2SLoginRequest() { Email = loginId.text, Password = loginPassword.text };
        var tags = CurrentPlayer.ReadOnlyTags();
        if(tags.Length == 0)
        {
            tags = new string[1] { "player1" };
        }
        PlayerPrefs.SetString("id" + tags[0], loginId.text);
        PlayerPrefs.SetString("password" + tags[0], loginPassword.text);
        Managers.networkManager.GameServerSend(packet);  
        
        ChattingPacket chattingPacket = new ChattingPacket();
        chattingPacket.ChattingServerLoginRequest = new C2SChattingServerLoginRequest() { Email = loginId.text };
        Managers.networkManager.ChattingServerSend(chattingPacket);        
        //OnLoginEnd(true);
    }

    public void OnClickSendRegister()
    {
        if(regPassword.text != regPasswordRe.text)
        {
            UIManager.ShowAlert("��й�ȣ�� �ٸ��ϴ�.");
            return;
        }
        GamePacket packet = new GamePacket();
        packet.RegisterRequest = new C2SRegisterRequest() { Nickname = regNickname.text, Email = regId.text, Password = regPassword.text };
        var tags = CurrentPlayer.ReadOnlyTags();
        if (tags.Length == 0)
        {
            tags = new string[1] { "player1" };
        }
        PlayerPrefs.SetString("id" + tags[0], regId.text);
        PlayerPrefs.SetString("password" + tags[0], regPassword.text);
        Managers.networkManager.GameServerSend(packet);
    }

    public void OnClickCancelRegister()
    {
        buttonSet.SetActive(true);
        register.SetActive(false);
    }

    public void OnClickCancelLogin()
    {
        buttonSet.SetActive(true);
        login.SetActive(false);
    }

    public void OnTouchScreen()
    {
        touch.SetActive(false);
        buttonSet.SetActive(false);
    }

    public async void OnLoginEnd(bool isSuccess)
    {
        if (isSuccess)
        {
            await UIManager.Show<UIMain>();
            UIManager.Get<UIMain>().OnRefreshRoomList();
            HideDirect();
            await UIManager.Show<UITopBar>();
            await UIManager.Show<UIGnb>();
        }
        else
        {
            UIManager.ShowAlert("�α��� ����");
        }
    }

    public void OnRegisterEnd(bool isSuccess)
    {
        if (isSuccess)
        {
            register.SetActive(false);
            login.SetActive(true);
            var tags = CurrentPlayer.ReadOnlyTags();
            if (tags.Length == 0)
            {
                tags = new string[1] { "player1" };
            }
            loginId.text = PlayerPrefs.GetString("id" + tags[0]);
            loginPassword.text = PlayerPrefs.GetString("password" + tags[0]);
        }
        else
        {
            UIManager.ShowAlert("ȸ������ ����");
        }

    }

    public void OnClickChangeServer()
    {
        UIManager.Show<PopupConnection>();
    }
}