using Microsoft.Extensions.Logging;
using TTT.Server.Game;

public class Matchmaker
{
    private List<MMRequest> _pool = new List<MMRequest>();
    private readonly ILogger<Matchmaker> _logger;
    public Matchmaker(ILogger<Matchmaker> logger)
    {
        _logger = logger;
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

        // DoMatchmaking();
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

            var xUser = request.Connection.User.Id;
            var oUser = match.Connection.User.Id;
            //_gameManager.RegisterGame(xUser, oUser)
            //send Net_OnStartGame();
            _logger.LogInformation(
                "Match found: {XUsername} (X, {XUserId}) vs {OUsername} (O, {OUserId})",
                request.Connection.User.Username, xUser,
                match.Connection.User.Username, oUser);
        }
    }
}