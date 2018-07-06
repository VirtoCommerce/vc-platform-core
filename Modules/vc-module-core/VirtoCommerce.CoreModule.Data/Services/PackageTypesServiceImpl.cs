using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Services
{
    public class PackageTypesServiceImpl : IPackageTypesService
    {

        private readonly Func<ICommerceRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public PackageTypesServiceImpl(Func<ICommerceRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        public async Task<IEnumerable<PackageType>> GetAllPackageTypesAsync()
        {
            using (var repository = _repositoryFactory())
            {
                var packageTypes = await repository.PackageTypes.OrderBy(x => x.Name).ToArrayAsync();
                var result = packageTypes.Select(x => x.ToModel(AbstractTypeFactory<PackageType>.TryCreateInstance()));
                return result;
            }
        }

        public async Task UpsertPackageTypesAsync(PackageType[] packageTypes)
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
                    var sourceEntry = AbstractTypeFactory<Model.PackageTypeEntity>.TryCreateInstance().FromModel(packageType, pkMap);
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
            }
        }
    }
}
