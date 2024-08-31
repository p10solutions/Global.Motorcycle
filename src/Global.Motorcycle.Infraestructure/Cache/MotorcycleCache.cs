using Global.Motorcycle.Domain.Contracts.Cache;
using Global.Motorcycle.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Global.Motorcycle.Infraestructure.Cache
{
    public class MotorcycleCache : IMotorcycleCache
    {
        readonly IDistributedCache _distributedCache;
        readonly int _minutesExpiration;
        readonly string _cacheKey;

        public MotorcycleCache(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _minutesExpiration = Convert.ToInt32(configuration.GetSection("Cache:MinutesExpiration").Value);
            _cacheKey = configuration.GetSection("Cache:Key").Value;
        }

        public async Task AddAsync(MotorcycleEntity Motorcycle)
        {
            var serializedMotorcycle = JsonSerializer.Serialize(Motorcycle);

            var cacheMotorcycle = Encoding.UTF8.GetBytes(serializedMotorcycle);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(_minutesExpiration));

            var key = $"{_cacheKey}:{Motorcycle.Id}";

            await _distributedCache.SetAsync(key, cacheMotorcycle, options);
        }

        public async Task<MotorcycleEntity?> GetAsync(Guid id)
        {
            var key = $"{_cacheKey}:{id}";

            var MotorcycleCache = await _distributedCache.GetAsync(key);

            if (MotorcycleCache == null)
                return null;    

            var MotorcycleSerialized = Encoding.UTF8.GetString(MotorcycleCache);

            var Motorcycle = JsonSerializer.Deserialize<MotorcycleEntity>(MotorcycleSerialized);

            return Motorcycle;
        }
    }
}
