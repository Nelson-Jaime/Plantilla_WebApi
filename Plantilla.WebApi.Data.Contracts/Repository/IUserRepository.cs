using Plantilla.WebApi.Data.Contracts.Model;
using System;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts.Repository
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<User> GetByUsernameAsync(string username);
    }
}
