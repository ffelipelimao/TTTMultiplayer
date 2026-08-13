using System.Linq;

namespace TTT.Server.Data;

public interface IRepository<T> where T : class
{
    void Add(T entity);

    void Update(T entity);

    T Get(string id);

    IQueryable<T> GetQuery();

    void Delete(string id);
}
