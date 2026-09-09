using Microsoft.Extensions.Logging;
using TTT.Server;
using TTT.Server.Data;
using TTT.Server.Game;

[HandlerRegister(PacketType.ServerStatusRequest)]
public class ServerStatusRequestHandler : IPacketHandler
{
    private readonly NetworkServer _server;
    private readonly IUserRepository _userRepository;
    private readonly UsersManager _usersManager;

    public ServerStatusRequestHandler(NetworkServer server, IUserRepository userRepository, UsersManager usersManager)
    {
        _server = server;
        _userRepository = userRepository;
        _usersManager = usersManager;
    }

    public void Handle(INetPacket packet, int connectionId)
    {
        var msg = new Net_OnServerStatus
        {
            PlayersCount = _userRepository.GetTotalCount(),
            Players = _usersManager.GetTopPlayers()
        };
        _server.SendClient(connectionId, msg);
    }
}
