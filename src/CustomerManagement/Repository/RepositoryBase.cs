using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<TEntity> dbSetEntity;

        public RepositoryBase(ApplicationDbContext context)
        {
            _dbContext = context;
            dbSetEntity = _dbContext.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            dbSetEntity.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete(int id)
        {
            dbSetEntity.Remove(dbSetEntity.Find(id)!);
            _dbContext.SaveChanges();
        }

        public IEnumerable<TEntity> GetAll(PaginationFilter validFilter)
        {
            var pagedData = dbSetEntity
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToArray();
            
            return pagedData;
        }

        public TEntity GetById(int id)
        {
            return dbSetEntity.Find(id)!;
        }

        public TEntity Update(int id, TEntity entity)
        {
            dbSetEntity.Update(entity);
            _dbContext.SaveChanges();

            return dbSetEntity.Find(id)!;
        }
    }
}