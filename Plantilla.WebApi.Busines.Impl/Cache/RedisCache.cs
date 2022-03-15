using ATC.RedisClient.Contracts.ServiceLibrary;
using Plantilla.WebApi.Busines.Contracts.Cache;
using Plantilla.WebApi.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Impl.Cache
{
    public class RedisCache : IRedisCache
    {
        private readonly IRedisCacheClient _redisCacheClient;

        public RedisCache(IRedisCacheClient redisCacheClient)
        {
            _redisCacheClient = redisCacheClient;
        }

        public async Task<UserDto> GetAsync()
        {
            var cachedData = await _redisCacheClient.GetAsync<UserDto>("UserInfo");

            if (cachedData.IsSuccessfulOperation && cachedData.CacheValue != null)
            {
                return cachedData.CacheValue;
            }
            return null;
        }

        public async Task SetAsync(IEnumerable<UserDto> dto)
        {
            await _redisCacheClient.SetAsync("UserInfo", dto);
        }
    }
}
