namespace TTT.Server.Game;

public class GamesManager
{
    private readonly List<Games> _games;

    public GamesManager()
    {
        _games = new List<Games>();
    }

    public Guid RegisterGame(string xUser, string oUser)
    {
        var game = new Games(xUser, oUser);
        _games.Add(game);
        return game.Id;
    }

    public Games FindGame(string username)
    {
        return _games.FirstOrDefault(g => g.XUser == username || g.OUser == username);
    }

    public Games CloseGame(string username)
    {
        var game = FindGame(username);
        if (game != null)
        {
            _games.Remove(game);
        }
        return game;
    }

    public bool GameExists(string username)
    {
        return _games.Any(g => g.XUser == username || g.OUser == username);
    }

    public int GetGameCount()
    {
        return _games.Count;
    }
}
