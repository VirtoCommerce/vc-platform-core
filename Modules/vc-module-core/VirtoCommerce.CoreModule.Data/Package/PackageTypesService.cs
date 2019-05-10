using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Package;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Package
{
    public class PackageTypesService : IPackageTypesService
    {

        private readonly Func<ICoreRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PackageTypesService(Func<ICoreRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<IEnumerable<PackageType>> GetAllPackageTypesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllPackageTypesAsync");
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PackageTypeCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var packageTypes = await repository.PackageTypes.OrderBy(x => x.Name).ToArrayAsync();
                    var result = packageTypes.Select(x => x.ToModel(AbstractTypeFactory<PackageType>.TryCreateInstance()));
                    return result;
                }
            });
        }

        public async Task SaveChangesAsync(PackageType[] packageTypes)
        {
            if (packageTypes == null)
            {
                throw new ArgumentNullException(nameof(packageTypes));
            }

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                foreach (var packageType in packageTypes)
                {
                    var sourceEntry = AbstractTypeFactory<PackageTypeEntity>.TryCreateInstance().FromModel(packageType, pkMap);
                    var targetEntry = await repository.PackageTypes.FirstOrDefaultAsync(x => x.Id == packageType.Id);
                    if (targetEntry == null)
                    {
                        repository.Add(sourceEntry);
                    }
                    else
                    {
                        sourceEntry.Patch(targetEntry);
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                PackageTypeCacheRegion.ExpireRegion();
            }
        }

        public async Task DeletePackageTypesAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var packageTypes = await repository.PackageTypes.Where(x => ids.Contains(x.Id)).ToArrayAsync();
                foreach (var packageType in packageTypes)
                {
                    repository.Remove(packageType);
                }
                await repository.UnitOfWork.CommitAsync();
                PackageTypeCacheRegion.ExpireRegion();
            }
        }
    }
}
