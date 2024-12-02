using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIMain : UIListBase<ItemRoom>
{
    float time = 0;
    List<RoomData> rooms;
    public override void Opened(object[] param)
    {

    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 0.5)
        {
            time = 0;
            OnRefreshRoomList();
        }
    }

    public void SetRoomList(List<RoomData> rooms)
    {
        this.rooms = rooms;
        SetList();
    }

    public void OnRefreshRoomList()
    {        
        if (Managers.networkManager.GameServerIsConnected())
        {
            GamePacket packet = new GamePacket();
            packet.GetRoomListRequest = new C2SGetRoomListRequest();
            Managers.networkManager.GameServerSend(packet);            
        }
    }

    public override void HideDirect()
    {
        UIManager.Hide<UIMain>();
    }

    public override void SetList()
    {
        ClearList();
        for (int i = 0; i < rooms.Count; i++)
        {
            var item = AddItem();
            item.SetItem(rooms[i], OnJoinRoom);
        }
    }

    public void OnClickRandomMatch()
    {
        if (Managers.networkManager.GameServerIsConnected())
        {
            GamePacket packet = new GamePacket();
            packet.JoinRandomRoomRequest = new C2SJoinRandomRoomRequest();
            Managers.networkManager.GameServerSend(packet);
        }
    }

    public void OnClickCreateRoom()
    {
        UIManager.Show<PopupRoomCreate>();
    }

    public void OnJoinRoom(int idx, string email)
    {
        if (Managers.networkManager.GameServerIsConnected())
        {
            GamePacket packet = new GamePacket();
            packet.JoinRoomRequest = new C2SJoinRoomRequest() { RoomId = idx };
            Managers.networkManager.GameServerSend(packet);
        }

        if (Managers.networkManager.ChattingServerIsConnected())
        {            
            ChattingPacket chattingJoinRoomPacket = new ChattingPacket();
            chattingJoinRoomPacket.ChattingServerJoinRoomRequest =
                new C2SChattingServerJoinRoomRequest() { OwnerEmail = email };
            Managers.networkManager.ChattingServerSend(chattingJoinRoomPacket);
        }
    }
}