using Microsoft.EntityFrameworkCore;
using Plantilla.WebApi.Data.Contracts;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Impl.Repository
{
    public class UserRoleRepository : BaseRepository<UserDbContext, UserRole, Guid>, IUserRoleRepository
    {
        private readonly DbSet<UserRole> _dbSet;
        public UserRoleRepository(UserDbContext context) : base(context)
        {
            _dbSet = context.Set<UserRole>();
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
