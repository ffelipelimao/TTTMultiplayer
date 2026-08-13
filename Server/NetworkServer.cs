using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.Server.Game;

namespace TTT.Server;

public class NetworkServer : INetEventListener
{
    private readonly ILogger<NetworkServer> _logger;
    private readonly IServiceProvider _provider;
    private UsersManager _usersManager;
    private NetManager _netManager;

    public NetworkServer(ILogger<NetworkServer> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }


    public void Start()
    {

        _netManager = new NetManager(this)
        {
            DisconnectTimeout = 100000
        };

        _netManager.Start(9050);
        _usersManager = _provider.GetRequiredService<UsersManager>();

        _logger.LogInformation("Server listening on {Port}", 9050);
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
        _logger.LogInformation("Incoming connection from {EndPoint}", request.RemoteEndPoint);
        request.Accept();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _logger.LogInformation("Client connected to server: {Peer}. Id: {Id}", peer, peer.Id);
        _usersManager.AddConnection(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _netManager.DisconnectPeer(peer);
        _usersManager.RemoveConnection(peer.Id);
        _logger.LogInformation("Connection removed from server with Id: {Id}", peer.Id);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        using (var scope = _provider.CreateScope())
        {
            try
            {
                var packetType = (PacketType)reader.GetByte();
                var packet = ResolvePacket(packetType, reader);
                var handler = ResolveHandler(packetType);

                handler.Handle(packet, peer.Id);

                reader.Recycle();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error processing message");
            }
        }
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


    public IPacketHandler ResolveHandler(PacketType packetType)
    {
        var registry = _provider.GetRequiredService<HandleRegistry>();
        var type = registry.Handlers[packetType];
        return (IPacketHandler)_provider.GetRequiredService(type);
    }

    private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader)
    {
        var registry = _provider.GetRequiredService<PacketRegistry>();
        var type = registry.PacketType[packetType];
        var packet = (INetPacket)Activator.CreateInstance(type);
        packet.Deserialize(reader);

        return packet;
    }
}
