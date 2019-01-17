using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Redis
{
    public class RedisCacheHostedService : IHostedService
    {
        private readonly ICacheBackplane _cacheBackplane;

        public RedisCacheHostedService(ICacheBackplane cacheBackplane)
        {
            _cacheBackplane = cacheBackplane;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cacheBackplane.SubscribeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
