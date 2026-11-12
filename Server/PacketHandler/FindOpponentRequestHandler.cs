using TTT.Server.Game;

[HandlerRegister(PacketType.FindOpponentRequest)]
public class FindOpponentRequestHandler : IPacketHandler
{
    private readonly UsersManager _usersManager;
    private readonly Matchmaker _matchmaker;
    public FindOpponentRequestHandler(UsersManager usersManager, Matchmaker matchmaker)
    {
        _usersManager = usersManager;
        _matchmaker = matchmaker;
    }
    public void Handle(INetPacket packet, int connectionId)
    {
        Console.WriteLine("Receive find opponent request!");
        var connection = _usersManager.GetConnection(connectionId);
        _matchmaker.RegisterPlayer(connection);

    }
}