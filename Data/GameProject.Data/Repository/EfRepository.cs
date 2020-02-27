using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Data.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity>
      where TEntity : class
    {
        public EfRepository(AppDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.DbSet = this.Context.Set<TEntity>();
        }

        protected DbSet<TEntity> DbSet { get; set; }

        protected AppDbContext Context { get; set; }

        public virtual IQueryable<TEntity> All() => this.DbSet;

        public virtual IQueryable<TEntity> AllAsNoTracking() => this.DbSet.AsNoTracking();

        public virtual Task<TEntity> GetByIdAsync(Guid id) => this.DbSet.FindAsync(id);

        public virtual TEntity GetBy(Expression<Func<TEntity, bool>> predicate) => this.DbSet.SingleOrDefault(predicate);

        public virtual Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> predicate) => this.DbSet.SingleOrDefaultAsync(predicate);

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate) => this.DbSet.Where(predicate);

        public virtual IQueryable<TEntity> FindByAsNoTracking(Expression<Func<TEntity, bool>> predicate) => this.DbSet.Where(predicate).AsNoTracking();

        public void Add(TEntity entity) => this.DbSet.Add(entity);

        public virtual Task AddAsync(TEntity entity) => this.DbSet.AddAsync(entity);

        public virtual void Update(TEntity entity)
        {
            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity) => this.DbSet.Remove(entity);

        public Task<int> SaveChangesAsync() => this.Context.SaveChangesAsync();

        public void Dispose() => this.Context.Dispose();

        public async Task LoadRelatedCollections(TEntity entity, params Expression<Func<TEntity, IEnumerable<object>>>[] properties)
        {
            foreach (var property in properties)
            {
                if (!this.Context.Entry(entity).Collection(property).IsLoaded)
                {
                    await this.Context.Entry(entity).Collection(property).LoadAsync();
                }
            }
        }

        public async Task LoadRelatedReferences(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            foreach (var property in properties)
            {
                if (!this.Context.Entry(entity).Reference(property).IsLoaded)
                {
                    await this.Context.Entry(entity).Reference(property).LoadAsync();
                }
            }
        }

        public Task<int> SaveChangesAsync(Guid currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
