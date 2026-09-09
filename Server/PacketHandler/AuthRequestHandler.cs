using Microsoft.Extensions.Logging;
using TTT.Server;
using TTT.Server.Data;
using TTT.Server.Game;

[HandlerRegister(PacketType.AuthRequest)]
public class AuthRequestHandler : IPacketHandler
{

    private readonly ILogger<AuthRequestHandler> _logger;
    private readonly UsersManager _usersManager;
    private readonly NetworkServer _server;
    private readonly IUserRepository _userRepository;

    public AuthRequestHandler(
        ILogger<AuthRequestHandler> logger,
        UsersManager usersManager,
        NetworkServer server,
        IUserRepository userRepository)
    {
        _logger = logger;
        _usersManager = usersManager;
        _server = server;
        _userRepository = userRepository;
    }

    public void Handle(INetPacket packet, int connectionId)
    {
        var msg = (Net_AuthRequest)packet;

        _logger.LogInformation("Login message receive");

        bool loginSuccess = _usersManager.LoginOrRegister(connectionId, msg.Username, msg.Password);

        _logger.LogInformation("Login result for {Username}: {Result}", msg.Username, loginSuccess);

        if (!loginSuccess)
        {
            _server.SendClient(connectionId, new Net_OnAuthFailed());
            return;
        }

        var user = _usersManager.GetUser(connectionId);

        _server.SendClient(connectionId, new Net_OnAuth
        {
            Username = user.Username,
            Score = user.Score,
        });

        NotifyOtherPlayers(connectionId);
    }

    private void NotifyOtherPlayers(int excludedConnectionId)
    {
        var rmsg = new Net_OnServerStatus
        {
            PlayersCount = _userRepository.GetTotalCount(),
            Players = _usersManager.GetTopPlayers()
        };

        var otherIds = _usersManager.GetOtherConnectionIds(excludedConnectionId);

        foreach (var connectionId in otherIds)
        {
            _server.SendClient(connectionId, rmsg);
        }
    }
}