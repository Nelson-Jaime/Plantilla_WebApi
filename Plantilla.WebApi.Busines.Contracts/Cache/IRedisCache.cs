using Plantilla.WebApi.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Contracts.Cache
{
    public interface IRedisCache
    {
        Task<UserDto> GetAsync();
        Task SetAsync(IEnumerable<UserDto> dto);
    }
}