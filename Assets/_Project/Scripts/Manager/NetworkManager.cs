using UnityEngine;
using UnityEngine.TextCore.Text;

public class NetworkManager
{
    string gameServerIp = "127.0.0.1";
    int gameServerPort = 5555;

    string chattingServerIp = "127.0.0.1";
    int chattingServerPort = 5556;

    ServerSession gameServerSession = null;
    ServerSession chattingServerSession = null;

    public void GameServerSend(GamePacket gamePacket)
    {
        gameServerSession.SendGamePacket(gamePacket);
    }

    public bool GameServerIsConnected()
    {
        if (gameServerSession != null)
        {
            return gameServerSession.isConnected;
        }

        return false;
    }

    public bool ChattingServerIsConnected()
    {
        if (chattingServerSession != null)
        {
            return chattingServerSession.isConnected;
        }

        return false;
    }

    public void ChattingServerSend(ChattingPacket chattingPacket)
    {
        chattingServerSession.SendChattingPacket(chattingPacket);
    }

    public void GameServerConnect(string gameServerIp, int gameServerPort)
    {
        if (gameServerSession == null)
        {
            this.gameServerIp = gameServerIp;
            this.gameServerPort = gameServerPort;

            GameObject GOManagers = GameObject.Find("@Managers");
            if (GOManagers != null)
            {
                GameObject gameServerSessionGO = new GameObject { name = "@gameServerSession" };
                gameServerSessionGO.transform.SetParent(GOManagers.transform);
                gameServerSessionGO.AddComponent<ServerSession>();
                gameServerSession = gameServerSessionGO.GetComponent<ServerSession>();

                // 게임 서버 접속
                gameServerSession.SessionInit(this.gameServerIp, this.gameServerPort, en_ServerPacketType.SERVER_PACKET_TYPE_GAME);
                gameServerSession.Connect(() =>
                {
                    //UIManager.Get<PopupLogin>().buttonSet.SetActive(false);
                    //UIManager.Get<PopupLogin>().login.SetActive(true);
                });
            }
        }
    }

    public void GameServerDisconnect(bool isReconnect = false)
    {
        if(gameServerSession != null)
        {
            gameServerSession.Disconnect(isReconnect);
        }
    }

    public void ChattingServerConnect(string chattingServerIp, int chattingServerPort)
    {
        if (chattingServerSession == null)
        {
            this.chattingServerIp = chattingServerIp;
            this.chattingServerPort = chattingServerPort;

            GameObject GOManagers = GameObject.Find("@Managers");
            if (GOManagers != null)
            {
                GameObject chattingServerSessionGO = new GameObject { name = "@chattingServerSession" };
                chattingServerSessionGO.transform.SetParent(GOManagers.transform);
                chattingServerSessionGO.AddComponent<ServerSession>();

                chattingServerSession = chattingServerSessionGO.GetComponent<ServerSession>();

                // 채팅 서버 접속
                chattingServerSession.SessionInit(this.chattingServerIp, this.chattingServerPort, en_ServerPacketType.SERVER_PACKET_TYPE_CHATTING);
                chattingServerSession.Connect();
            }
        }
    }

    public void ChattingServerDisconnect(bool isReconnect = false)
    {
        if(chattingServerSession != null)
        {
            chattingServerSession.Disconnect(isReconnect);
        }
    }
}
