using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LiteNetLib;
using UnityEngine;


public class Server : INetEventListener
{
    private NetManager _netManager;
    private Dictionary<int, NetPeer> _connections;
    public void Start()
    {
        _connections = new Dictionary<int, NetPeer>();
        _netManager = new NetManager(this)
        {
            DisconnectTimeout = 100000
        };

        _netManager.Start(9050);

        Debug.Log("[Server] listening on 9050");

    }
    public void PollEvents()
    {
        _netManager.PollEvents();
    }

    public void Stop()
    {
        _netManager.Stop();
    }
    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log($"[Server] incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"[Server] client connected with Id: {peer.Id}");
        _connections.Add(peer.Id, peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"[Server] client disconnected with Id: {peer.Id}");
        _connections.Remove(peer.Id);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var data = Encoding.UTF8.GetString(reader.GetRemainingBytes());
        reader.Recycle();
        Debug.Log($"[Server] data received from client: '{data}'");

        // reply to client
        var reply = "General Kenobi";
        var bytes = Encoding.UTF8.GetBytes(reply);
        peer.Send(bytes, DeliveryMethod.ReliableOrdered);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {

    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }


    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {

    }

}

