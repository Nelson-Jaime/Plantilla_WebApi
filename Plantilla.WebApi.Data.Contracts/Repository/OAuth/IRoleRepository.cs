using Plantilla.WebApi.Data.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts.Repository.OAuth
{
    public interface IRoleRepository : IRepository<Role, Guid>
    {
        Task<IEnumerable<Role>> GetAllRolesAsync(IEnumerable<Guid> Ids);
    }
}