using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Client : MonoBehaviour, INetEventListener
{
    private NetManager _netManager;
    private NetPeer _netServer;
    private NetDataWriter _netDataWriter;

    public event Action OnServerConnected;

    public static Client _instance;

    public static Client Instance
    {
        get
        {
            return _instance;
        }
    }


    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _netManager.PollEvents();
    }

    void Init()
    {

        _netDataWriter = new NetDataWriter();
        _netManager = new NetManager(this)
        {
            DisconnectTimeout = 100000
        };
        _netManager.Start();
    }

    public void Connect()
    {
        _netManager.Connect("localhost", 9050, "");
    }

    public void SendServer<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable
    {
        if (_netServer == null)
        {
            Debug.LogWarning("[Client] not connected yet, ignoring send");
            return;
        }
        _netDataWriter.Reset();
        packet.Serialize(_netDataWriter);
        _netServer.Send(_netDataWriter, deliveryMethod);

    }

    //OnNetworkReceive the callback from server that we send in SendServer
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var data = Encoding.UTF8.GetString(reader.GetRemainingBytes());
        reader.Recycle();
        Debug.Log($"[Client] data received from server: '{data}'");
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {

    }

    // OnPeerConnected and OnPeerDisconnected are the callbacks from server that we send in _netManager.Connect
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"[Client] connected to server at {peer}");
        _netServer = peer;
        OnServerConnected?.Invoke();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[Client] lost connection to server!");
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {

    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }
}
