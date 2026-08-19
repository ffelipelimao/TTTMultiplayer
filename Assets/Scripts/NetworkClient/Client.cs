using System;
using System.Collections.Generic;
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

    private HandleRegistry _handlerRegistry;
    private PacketRegistry _packetRegistry;

    // Handlers sao stateless, entao vale reaproveitar a instancia. O registry
    // guarda o Type; este dicionario guarda o objeto ja construido.
    private readonly Dictionary<PacketType, IPacketHandler> _handlers = new Dictionary<PacketType, IPacketHandler>();

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
        _handlerRegistry = new HandleRegistry();
        _packetRegistry = new PacketRegistry();
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
        try
        {
            var packetType = (PacketType)reader.GetByte();
            var packet = ResolvePacket(packetType, reader);
            var handler = ResolveHandler(packetType);

            if (handler == null)
            {
                Debug.LogWarning($"[Client] no handler registered for {packetType}, packet dropped");
                return;
            }

            handler.Handle(packet, peer.Id);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            reader.Recycle();
        }
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
    public IPacketHandler ResolveHandler(PacketType packetType)
    {
        if (_handlers.TryGetValue(packetType, out var cached))
        {
            return cached;
        }

        if (!_handlerRegistry.Handlers.TryGetValue(packetType, out var type))
        {
            return null;
        }

        var handler = (IPacketHandler)Activator.CreateInstance(type);
        _handlers[packetType] = handler;

        return handler;
    }

    private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader)
    {
        var type = _packetRegistry.PacketType[packetType];
        var packet = (INetPacket)Activator.CreateInstance(type);
        packet.Deserialize(reader);

        return packet;
    }
}
