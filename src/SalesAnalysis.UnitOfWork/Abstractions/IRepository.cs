using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SalesAnalysis.UnitOfWork.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);

        Task<TEntity> GetAsync(int id);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression);

        void Add(TEntity entity);

        Task AddSync(TEntity entity);
        
        void AddRange(IEnumerable<TEntity> entities);

        Task AddRangAsync(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);
        
    }
}