using Microsoft.Extensions.Logging;
using TTT.Server;
using TTT.Server.Game;

public class Matchmaker
{
    private List<MMRequest> _pool = new List<MMRequest>();
    private readonly ILogger<Matchmaker> _logger;
    private readonly GamesManager _gamesManager;
    private readonly NetworkServer _server;

    public Matchmaker(ILogger<Matchmaker> logger, GamesManager gamesManager, NetworkServer server)
    {
        _logger = logger;
        _gamesManager = gamesManager;
        _server = server;
    }

    public void RegisterPlayer(ServerConnection Connection)
    {
        if (_pool.Any(x => x.Connection.User.Id == Connection.User.Id))
        {
            _logger.LogWarning(
                "Player {Username} (connection {ConnectionId}) is already in the matchmaking pool",
                Connection.User.Username, Connection.ConnectionId);
            return;
        }

        var request = new MMRequest
        {
            Connection = Connection,
            SearchStart = DateTime.UtcNow,

        };

        _pool.Add(request);
        _logger.LogInformation(
            "Player {Username} (connection {ConnectionId}) joined matchmaking. Pool size: {PoolSize}",
            Connection.User.Username, Connection.ConnectionId, _pool.Count);

        DoMatchmaking();
    }

    public void TryUnregisterPlayer(string name)
    {
        var request = _pool.FirstOrDefault(x => x.Connection.User.Username == name);
        if (request != null)
        {
            _logger.LogInformation(
                "Player {Username} left matchmaking after {WaitSeconds:F1}s in queue",
                name, (DateTime.UtcNow - request.SearchStart).TotalSeconds);
            _pool.Remove(request);
        }
    }

    private void DoMatchmaking()
    {
        var matchedRequested = new List<MMRequest>();

        foreach (var request in _pool)
        {
            var match = _pool.FirstOrDefault(
                x => !x.MatchFound && x.Connection.ConnectionId != request.Connection.ConnectionId);

            if (match == null)
            {
                continue;
            }

            request.MatchFound = true;
            match.MatchFound = true;

            matchedRequested.Add(request);
            matchedRequested.Add(match);

            var xUser = request.Connection.User.Username;
            var oUser = match.Connection.User.Username;
            var gameId = _gamesManager.RegisterGame(xUser, oUser);

            request.Connection.GameId = gameId;
            match.Connection.GameId = gameId;

            var msg = new Net_OnStartGame
            {
                GameId = gameId,
                XUser = xUser,
                OUser = oUser,

            };

            var playerOne = request.Connection.ConnectionId;
            var playerTwo = match.Connection.ConnectionId;

            _server.SendClient(playerOne, msg);
            _server.SendClient(playerTwo, msg);

            _logger.LogInformation(
                "Match found: {XUsername} (X) vs {OUsername} (O). Game {GameId}",
                xUser, oUser, gameId);
        }

        foreach (var request in matchedRequested)
        {
            _pool.Remove(request);
        }
    }
}