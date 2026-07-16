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

    public void SendServer(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        _netServer.Send(bytes, DeliveryMethod.ReliableOrdered);
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

    //OnNetworkReceive the callback from server that we send in SendServer
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var data = Encoding.UTF8.GetString(reader.RawData).Replace("\0", "");
        Debug.Log($"Data receive from server: '{data}'");
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {

    }

    // OnPeerConnected and OnPeerDisconnected are the callbacks from server that we send in _netManager.Connect
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Connected to server at " + peer);
        _netServer = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("Lost connection to server!");
    }
}
