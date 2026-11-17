namespace TTT.Server.Game;

public class Games
{
    public Games(string xUser, string oUser)
    {
        Id = Guid.NewGuid();
        StartTime = DateTime.Now;
        CurrentStartTime = DateTime.Now;
        XUser = xUser;
        OUser = oUser;
        Round = 1;
        CurrentUser = xUser;
    }
    public Guid Id { get; set; }
    public string CurrentUser { get; set; }
    public ushort Round { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CurrentStartTime { get; set; }
    public string OUser { get; set; }
    public ushort OWins { get; set; }
    public ushort OWantsRematch { get; set; }
    public string XUser { get; set; }
    public ushort XWins { get; set; }
    public ushort XWantsRematch { get; set; }
}