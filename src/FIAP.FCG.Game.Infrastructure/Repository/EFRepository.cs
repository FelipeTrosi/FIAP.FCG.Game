using FIAP.FCG.Game.Domain.Entity;
using FIAP.FCG.Game.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Game.Infrastructure.Repository
{
    public class EFRepository<T>(ApplicationDbContext context) : IRepository<T> where T : EntityBase
    {
        protected ApplicationDbContext _context = context;
        protected DbSet<T> _dbSet = context.Set<T>();

        public T Create(T entity)
        {
            var entityCreated = _dbSet.Add(entity);
            _context.SaveChanges();
            return entityCreated.Entity;
        }

        public IList<T> GetAll() 
            => _dbSet.ToList();

        public T? GetById(long id)
            => _dbSet.AsNoTracking().FirstOrDefault(q => q.Id == id);

        public T Update(T entity)
        {
            var entityUpdated = _dbSet.Update(entity);
            _context.SaveChanges();
            return entityUpdated.Entity;
        }

        public void DeleteById(long id)
        {
            _dbSet.Remove(GetById(id)!);
            _context.SaveChanges();
        }
    }
}
