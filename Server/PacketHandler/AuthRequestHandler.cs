using Microsoft.Extensions.Logging;
using TTT.Server.Game;

[HandlerRegister(PacketType.AuthRequest)]
public class AuthRequestHandler : IPacketHandler
{

    private readonly ILogger<AuthRequestHandler> _logger;
    private readonly UsersManager _usersManager;

    public AuthRequestHandler(ILogger<AuthRequestHandler> logger, UsersManager usersManager)
    {
        _logger = logger;
        _usersManager = usersManager;
    }

    public void Handle(INetPacket packet, int connectionId)
    {
        var msg = (Net_AuthRequest)packet;

        _logger.LogInformation("Login message receive");

        var loginSuccess = _usersManager.LoginOrRegister(connectionId, msg.Username, msg.Password);

        _logger.LogInformation("Login result for {Username}: {Result}", msg.Username, loginSuccess);
    }
}