using FIAP.FCG.Game.Domain.Entity;
using FIAP.FCG.Game.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Game.Infrastructure.Repository
{
    public class EFRepository<T>(ApplicationDbContext context) : IRepository<T> where T : EntityBase
    {
        protected ApplicationDbContext _context = context;
        protected DbSet<T> _dbSet = context.Set<T>();

        public void Create(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public IList<T> GetAll() 
            => _dbSet.ToList();

        public T? GetById(long id)
            => _dbSet.FirstOrDefault(q => q.Id == id);

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void DeleteById(long id)
        {
            _dbSet.Remove(GetById(id)!);
            _context.SaveChanges();
        }
    }
}
