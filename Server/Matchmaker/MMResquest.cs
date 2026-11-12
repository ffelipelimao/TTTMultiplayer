using TTT.Server.Game;

public class MMRequest
{
    public ServerConnection Connection { get; set; }
    public DateTime SearchStart { get; set; }
    public bool MatchFound { get; set; }
}