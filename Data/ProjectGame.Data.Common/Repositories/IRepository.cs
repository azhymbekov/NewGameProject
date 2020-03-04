using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGame.Data.Common.Repositories
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> All();

        IQueryable<TEntity> AllAsNoTracking();

        Task<TEntity> GetByIdAsync(Guid id);

        TEntity GetBy(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> predicate);

        Task AddAsync(TEntity entity);

        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> FindByAsNoTracking(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync();

        Task LoadRelatedCollections(TEntity entity, params Expression<Func<TEntity, IEnumerable<object>>>[] properties);

        Task LoadRelatedReferences(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
    }
}
