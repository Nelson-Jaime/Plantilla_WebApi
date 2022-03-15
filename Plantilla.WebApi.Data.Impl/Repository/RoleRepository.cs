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
    public class RoleRepository : BaseRepository<UserDbContext, Role, Guid>, IRoleRepository
    {
        private readonly DbSet<Role> _dbSet;
        public RoleRepository(UserDbContext context) : base(context)
        {
            _dbSet = context.Set<Role>();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(IEnumerable<Guid> Ids)
        {
            return await _dbSet.Where(r => Ids.Contains(r.Id)).ToListAsync();
        }
    }
}
