using Microsoft.EntityFrameworkCore;
using Plantilla.WebApi.Data.Contracts;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Data.Contracts.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Impl.Repository
{
    public class UserRepository : BaseRepository<UserDbContext, User, Guid>, IUserRepository
    {
        private readonly DbSet<User> _dbSet;
        public UserRepository(UserDbContext context) : base(context)
        {
            _dbSet = context.Set<User>();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet.Where(u => u.UserName == username).FirstOrDefaultAsync();
        }
    }
}
