using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Redis;

namespace VirtoCommerce.Platform.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddOptions<RedisCachingOptions>().Bind(configuration.GetSection("Caching:Redis")).ValidateDataAnnotations();

                var redis = ConnectionMultiplexer.Connect(redisConnectionString);
                services.AddSingleton(redis.GetSubscriber());
                services.AddSingleton<IPlatformMemoryCache, RedisPlatformMemoryCache>();
                services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            }
            else
            {
                //Use MemoryCache decorator to use global platform cache settings
                services.AddSingleton<IPlatformMemoryCache, PlatformMemoryCache>();
            }



            return services;
        }
    }
}
