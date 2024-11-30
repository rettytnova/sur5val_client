using static GamePacket;
using static ChattingPacket;
using System.Collections.Generic;
using Ironcow.WebSocketPacket;
using System;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Net;
using UnityEngine;
using System.IO;
using Google.Protobuf;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class Session : MonoBehaviour
{
    public bool useDNS = false;

    #region GamePacket Recv Send
    // GamePacket 전용
    public Dictionary<PayloadOneofCase, Action<GamePacket>> _onGamePacketRecv = new Dictionary<PayloadOneofCase, Action<GamePacket>>();

    public Queue<GamePayload> sendGameQueue = new Queue<GamePayload>();
    public Queue<GamePayload> receiveGameQueue = new Queue<GamePayload>();

    #endregion

    // ChattingPacket 전용
    #region ChattingPacket Recv Send
    public Dictionary<ChattingPayloadOneofCase, Action<ChattingPacket>> _onChattingPacketRecv = new Dictionary<ChattingPayloadOneofCase, Action<ChattingPacket>>();

    public Queue<ChattingPayload> sendChattingQueue = new Queue<ChattingPayload>();
    public Queue<ChattingPayload> receiveChattingQueue = new Queue<ChattingPayload>();
    #endregion

    // recv 버프
    byte[] recvBuff = new byte[1024];
    private byte[] remainBuffer = Array.Empty<byte>();

    public string ip;
    public int port;

    public string version = "1.0.0";
    public int sequenceNumber = 1;

    public Socket socket;

    public bool isConnected;

    bool isInit = false;

    // 서버 타입
    en_ServerPacketType serverPacketType = en_ServerPacketType.SERVER_PACKET_TYPE_NONE;
    
    public void SessionInit(string ip, int port, en_ServerPacketType serverPacketType)
    {
        this.ip = ip;
        this.port = port;

        if (isInit) return;

        this.serverPacketType = serverPacketType;

        string[] payloads;

        switch (this.serverPacketType)
        {
            case en_ServerPacketType.SERVER_PACKET_TYPE_GAME:
                payloads = Enum.GetNames(typeof(PayloadOneofCase));
                foreach (var payload in payloads)
                {
                    var val = (PayloadOneofCase)Enum.Parse(typeof(PayloadOneofCase), payload);
                    var method = GetType().GetMethod(payload);                    
                    if (method != null)
                    {
                        var action = (Action<GamePacket>)Delegate.CreateDelegate(typeof(Action<GamePacket>), this, method);
                        _onGamePacketRecv.Add(val, action);
                    }
                }
                break;
            case en_ServerPacketType.SERVER_PACKET_TYPE_CHATTING:
                payloads = Enum.GetNames(typeof(ChattingPayloadOneofCase));
                foreach (var payload in payloads)
                {
                    var val = (ChattingPayloadOneofCase)Enum.Parse(typeof(ChattingPayloadOneofCase), payload);
                    var method = GetType().GetMethod(payload);
                    if (method != null)
                    {
                        var action = (Action<ChattingPacket>)Delegate.CreateDelegate(typeof(Action<ChattingPacket>), this, method);
                        _onChattingPacketRecv.Add(val, action);
                    }
                }
                break;
            default:
                break;
        }

        isInit = true;
    }

    // 서버와 연결
    public async void Connect(UnityAction callback = null)
    {       
        IPEndPoint endPoint;
        if (IPAddress.TryParse(ip, out IPAddress ipAddress))
        {
            endPoint = new IPEndPoint(ipAddress, port);
        }
        else
        {
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }

        if (useDNS)
        {
            IPHostEntry ipHOST = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHOST.AddressList[0];

            endPoint = new IPEndPoint(ipAddress, port);
        }

        Debug.Log("Tcp Ip : " + ipAddress.MapToIPv4().ToString() + ", Port : " + port);
        socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await socket.ConnectAsync(endPoint);
            isConnected = socket.Connected;
            OnReceive();

            switch(serverPacketType)
            {
                case en_ServerPacketType.SERVER_PACKET_TYPE_GAME:
                    StartCoroutine(OnSendGameQueue());
                    StartCoroutine(OnReceiveGameQueue());                    
                    break;
                case en_ServerPacketType.SERVER_PACKET_TYPE_CHATTING:
                    StartCoroutine(OnSendChattingQueue());
                    StartCoroutine(OnReceiveChattingQueue());
                    break;
            }

            callback?.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // 서버로부터 데이터 수신
    private async void OnReceive()
    {
        if (socket != null)
        {
            while (socket.Connected && isConnected)
            {
                try
                {
                    var recvByteLength = await socket.ReceiveAsync(recvBuff, SocketFlags.None); //socket.ReceiveAsync는 await로 대기 시 새로운 데이터를 받기 전까지 대기한다.
                    if (!isConnected)
                    {
                        Debug.Log("Socket is disconnect");
                        break;
                    }
                    if (recvByteLength <= 0)
                    {
                        continue;
                    }

                    var newBuffer = new byte[remainBuffer.Length + recvByteLength];
                    Array.Copy(remainBuffer, 0, newBuffer, 0, remainBuffer.Length);
                    Array.Copy(recvBuff, 0, newBuffer, remainBuffer.Length, recvByteLength);

                    var processedLength = 0;
                    while (processedLength < newBuffer.Length)
                    {
                        if (newBuffer.Length - processedLength < 11)
                        {
                            break;
                        }

                        using var stream = new MemoryStream(newBuffer, processedLength, newBuffer.Length - processedLength);
                        using var reader = new BinaryReader(stream);

                        var typeBytes = reader.ReadBytes(2);
                        Array.Reverse(typeBytes);

                        var type = BitConverter.ToInt16(typeBytes);
                        //Debug.Log($"PacketType:{type}");

                        var versionLength = reader.ReadByte();
                        if (newBuffer.Length - processedLength < 11 + versionLength)
                        {
                            break;
                        }
                        var versionBytes = reader.ReadBytes(versionLength);
                        var version = BitConverter.ToString(versionBytes);

                        var sequenceBytes = reader.ReadBytes(4);
                        Array.Reverse(sequenceBytes);
                        var sequence = BitConverter.ToInt32(sequenceBytes);

                        var payloadLengthBytes = reader.ReadBytes(4);
                        Array.Reverse(payloadLengthBytes);
                        var payloadLength = BitConverter.ToInt32(payloadLengthBytes);

                        if (newBuffer.Length - processedLength < 11 + versionLength + payloadLength)
                        {
                            break;
                        }
                        var payloadBytes = reader.ReadBytes(payloadLength);

                        var totalLength = 11 + versionLength + payloadLength;

                        switch (serverPacketType)
                        {
                            case en_ServerPacketType.SERVER_PACKET_TYPE_GAME:
                                var gameType = (PayloadOneofCase)type;                               

                                var gamePacket = new GamePayload(gameType, version, sequence, payloadBytes);
                                receiveGameQueue.Enqueue(gamePacket);
                                break;
                            case en_ServerPacketType.SERVER_PACKET_TYPE_CHATTING:
                                var chattingType = (ChattingPayloadOneofCase)type;

                                var chattingPacket = new ChattingPayload(chattingType, version, sequence, payloadBytes);
                                receiveChattingQueue.Enqueue(chattingPacket);
                                break;
                        }

                        processedLength += totalLength;
                    }

                    var remainLength = newBuffer.Length - processedLength;
                    if (remainLength > 0)
                    {
                        remainBuffer = new byte[remainLength];
                        Array.Copy(newBuffer, processedLength, remainBuffer, 0, remainLength);
                        break;
                    }

                    remainBuffer = Array.Empty<byte>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.StackTrace}");
                }
            }

            if (socket != null && socket.Connected)
            {
                Debug.Log("소켓 리시브 멈춤 다시 시작");
                OnReceive();
            }
        }
    }

    // 게임서버로 패킷 전송
    public void SendGamePacket(GamePacket gamePacket)
    {
        if (socket == null) return;
        var byteArray = gamePacket.ToByteArray();
        var packet = new GamePayload(gamePacket.PayloadCase, version, sequenceNumber++, byteArray);
        sendGameQueue.Enqueue(packet);
        //Debug.Log($"sendGameQueue.Count: {sendGameQueue.Count} packetType {packet.type}");
    }

    // 채팅서버로 패킷 전송
    public void SendChattingPacket(ChattingPacket chattingPacket)
    {
        if (socket == null) return;
        var byteArray = chattingPacket.ToByteArray();
        var packet = new ChattingPayload(chattingPacket.ChattingPayloadCase, version, sequenceNumber++, byteArray);
        sendChattingQueue.Enqueue(packet);
    }

    IEnumerator OnSendGameQueue()
    {
        while (true)
        {
            yield return new WaitUntil(() => sendGameQueue.Count > 0);            

            var gamePacket = sendGameQueue.Dequeue();

            var bytes = gamePacket.ToByteArray();
            var sendByte = socket.Send(bytes, SocketFlags.None);
            //Debug.Log($"Send Packet: {gamePacket.type}, Sent bytes: {sendByte}");

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator OnReceiveGameQueue()
    {
        while (true)
        {
            yield return new WaitUntil(() => receiveGameQueue.Count > 0);

            try
            {
                var gamePacket = receiveGameQueue.Dequeue();
                _onGamePacketRecv[gamePacket.type].Invoke(gamePacket.gamePacket);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator OnSendChattingQueue()
    {
        while (true)
        {
            yield return new WaitUntil(() => sendChattingQueue.Count > 0);
            var chattingPacket = sendChattingQueue.Dequeue();

            var bytes = chattingPacket.ToByteArray();
            var sendByte = socket.Send(bytes, SocketFlags.None);

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator OnReceiveChattingQueue()
    {
        while (true) 
        {
            yield return new WaitUntil(() => receiveChattingQueue.Count > 0);

            try
            {
                var chattingPacket = receiveChattingQueue.Dequeue();
                _onChattingPacketRecv[chattingPacket.type].Invoke(chattingPacket.chattingPacket);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnDestory()
    {
        Disconnect();
    }

    public async void Disconnect(bool isReconnect = false)
    {
        StopAllCoroutines();
        if (isConnected)
        {
            this.isConnected = false;           
            socket.Disconnect(isReconnect);
            if (isReconnect)
            {
                Connect();
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "Main")
                {
                    await SceneManager.LoadSceneAsync("Main");
                }
                else
                {
                    UIManager.Hide<UITopBar>();
                    UIManager.Hide<UIGnb>();
                    await UIManager.Show<PopupLogin>();
                }
            }
        }
    }
}