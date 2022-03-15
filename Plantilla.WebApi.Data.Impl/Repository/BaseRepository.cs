using Microsoft.EntityFrameworkCore;
using Plantilla.WebApi.Data.Contracts.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Impl.Repository
{
    public class BaseRepository<TContext, TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
                                                                                      where TContext : DbContext
    {
        protected TContext Context { get; set; }
        protected DbSet<TEntity> DbSet { get; set; }
        private bool _disposed;

        public BaseRepository(TContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task AddAllAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return await DbSet.Where(predicate).ToListAsync();
            }

            return await DbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            await Task.Run(() => DbSet.Remove(entity));
        }

        public virtual async Task RemoveAllAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => DbSet.RemoveRange(entities));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
