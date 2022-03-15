using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts.Repository
{
    public interface IRepository<TEntity, in TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate = null);

        Task AddAsync(TEntity entity);

        Task AddAllAsync(IEnumerable<TEntity> entities);
        Task RemoveAsync(TEntity entity);

        Task RemoveAllAsync(IEnumerable<TEntity> entities);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
