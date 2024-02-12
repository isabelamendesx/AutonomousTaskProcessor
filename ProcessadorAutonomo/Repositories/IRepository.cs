using AutonomousTaskProcessor.Entities;

namespace AutonomousTaskProcessor.Repositories;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetById(int id);
    void Add();
    void Update(T entity);
}
