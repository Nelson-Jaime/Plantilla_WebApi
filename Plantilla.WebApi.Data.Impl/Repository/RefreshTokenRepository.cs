using Microsoft.EntityFrameworkCore;
using Plantilla.WebApi.Data.Contracts;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Impl.Repository
{
    public class RefreshTokenRepository : BaseRepository<UserDbContext, RefreshToken, Guid>, IRefreshTokenRepository
    {
        private const string sourceName = "Database";
        private readonly DbSet<RefreshToken> _dbSet;

        public RefreshTokenRepository(UserDbContext context) : base(context)
        {
            _dbSet = context.Set<RefreshToken>();
        }

        public string Source => sourceName;

        public async Task DeleteByJTIAsync(Guid JTI)
        {
            _dbSet.Remove(await _dbSet.Where(rt => rt.JTI == JTI).FirstOrDefaultAsync());
        }

        public async override Task<RefreshToken> GetByIdAsync(Guid jti)
        {
            return await _dbSet.Where(ds => ds.JTI == jti).FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _dbSet.Where(ds => ds.Token == token).FirstOrDefaultAsync();
        }

        public async Task<string> GetTokenByJti(Guid jti)
        {
            var refreshToken = await _dbSet.Where(ds => ds.JTI == jti).FirstOrDefaultAsync();
            return refreshToken.Token;
        }
    }
}
