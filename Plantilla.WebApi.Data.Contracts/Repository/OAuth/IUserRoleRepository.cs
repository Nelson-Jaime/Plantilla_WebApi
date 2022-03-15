using Plantilla.WebApi.Data.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts.Repository.OAuth
{
    public interface IUserRoleRepository : IRepository<UserRole, Guid>
    {
        Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
    }
}