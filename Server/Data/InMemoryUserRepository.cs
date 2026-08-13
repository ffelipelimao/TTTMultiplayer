namespace TTT.Server.Data;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        _users =
        [
            new() { Username = "player1", Password = "123456", IsOnline = true, Score = 10 },
            new() { Username = "player2", Password = "123456", IsOnline = true, Score = 25 },
            new() { Username = "player3", Password = "123456", IsOnline = true, Score = 5 },
        ];
    }

    public void Add(User entity)
    {
        _users.Add(entity);
    }

    public void Update(User entity)
    {
        var index = _users.FindIndex(u => u.Id == entity.Id);
        if (index >= 0)
            _users[index] = entity;
    }

    public User Get(string id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public IQueryable<User> GetQuery()
    {
        return _users.AsQueryable();
    }

    public void Delete(string id)
    {
        _users.RemoveAll(u => u.Id == id);
    }

    public void SetOnline(string id)
    {
        SetOnlineStatus(id, true);
    }

    public void SetOffline(string id)
    {
        SetOnlineStatus(id, false);
    }

    private void SetOnlineStatus(string id, bool isOnline)
    {
        var user = Get(id);
        if (user != null)
            user.IsOnline = isOnline;
    }

    public void SetScore(string id, int score)
    {
        var user = Get(id);
        if (user != null)
            user.Score = score;
    }
}
