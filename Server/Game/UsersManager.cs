using LiteNetLib;
using TTT.Server.Data;

namespace TTT.Server.Game;

public class UsersManager(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly Dictionary<int, ServerConnection> _connections = new();

    public bool LoginOrRegister(int connectionId, string username, string password)
    {
        var user = _userRepository.GetQuery()
            .FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            user = new User
            {
                Username = username,
                Password = password,
            };

            _userRepository.Add(user);
        }
        else if (user.Password != password)
        {
            return false;
        }

        _userRepository.SetOnline(user.Id);
        _userRepository.SetScore(user.Id, 0);

        var connection = GetConnection(connectionId);
        if (connection != null)
            connection.User = user;

        return true;
    }

    public void AddConnection(NetPeer peer)
    {
        _connections.Add(peer.Id, new ServerConnection
        {
            ConnectionId = peer.Id,
            Peer = peer,
        });
    }

    public ServerConnection GetConnection(int connectionId)
    {
        return _connections.TryGetValue(connectionId, out var connection) ? connection : null;
    }

    public User GetUser(int connectionId)
    {
        return GetConnection(connectionId)?.User;
    }

    public void RemoveConnection(int connectionId)
    {
        if (!_connections.Remove(connectionId, out var connection))
            return;

        if (connection.User != null)
            _userRepository.SetOffline(connection.User.Id);
    }
}
