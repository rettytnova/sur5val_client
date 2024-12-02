using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;

public class PopupRoomCreate : UIBase
{
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_Dropdown count;

    public override void Opened(object[] param)
    {
        var roomNameSample = new List<string>() { "�ʸ� ���� ��!", "�����ִ� �����", "��� �Ѻ�?", "��ſ� ���� ���� �Ͻ�?", "����! ����!" };
        roomName.text = roomNameSample.RandomValue();
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupRoomCreate>();
    }

    public void OnClickCreate()
    {
        if (Managers.networkManager.GameServerIsConnected())
        {
            GamePacket packet = new GamePacket();
            packet.CreateRoomRequest = new C2SCreateRoomRequest() { MaxUserNum = count.value + 4, Name = roomName.text };
            Managers.networkManager.GameServerSend(packet);            
        }
        else
        {
            OnRoomCreateResult(true, new RoomData() { Id = 1, MaxUserNum = count.value + 4, Name = roomName.text, OwnerId = UserInfo.myInfo.id, State = 0 });
        }

        // ä�� ���� �� ����
        if (Managers.networkManager.ChattingServerIsConnected())
        {            
            ChattingPacket chattingPacket = new ChattingPacket();
            chattingPacket.ChattingServerCreateRoomRequest = new C2SChattingServerCreateRoomRequest();
            Managers.networkManager.ChattingServerSend(chattingPacket);
        }
    }

    public void OnRoomCreateResult(bool isSuccess, RoomData roomData)
    {
        if(isSuccess)
        {
            UIManager.Show<UIRoom>(roomData);
            HideDirect();
        }
    }
}