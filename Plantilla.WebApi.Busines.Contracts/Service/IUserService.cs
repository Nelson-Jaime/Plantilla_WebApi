using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Contracts.Service
{
    public interface IUserService
    {

        Task<OperationResult<UserDto>> AddUserAsync(UserDto dto);
        Task<OperationResult<UserDto>> AddUserByName(string userName);
        Task<OperationResult> SaveUserWithRolesAsync(string username, string password, string roleName);
        Task<OperationResult<IEnumerable<UserDto>>> GetAllUsers();
        Task<OperationResult<UserDto>> GetUser(Guid userId);
        Task<OperationResult> RemoveUser(Guid userId);
    }
}
