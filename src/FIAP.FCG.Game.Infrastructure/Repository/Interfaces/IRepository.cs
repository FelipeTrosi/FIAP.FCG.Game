using FIAP.FCG.Game.Domain.Entity;

namespace FIAP.FCG.Game.Infrastructure.Repository.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    IList<T> GetAll();
    T? GetById(long id);
    T Create(T entity);
    T Update(T entity);
    void DeleteById(long id);

}
