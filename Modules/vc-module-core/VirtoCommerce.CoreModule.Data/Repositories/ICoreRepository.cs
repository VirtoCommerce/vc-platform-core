using System.Linq;
using VirtoCommerce.CoreModule.Data.Currency;
using VirtoCommerce.CoreModule.Data.Model;
using VirtoCommerce.CoreModule.Data.Package;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public interface ICoreRepository : IRepository
    {
        IQueryable<SequenceEntity> Sequences { get; }
        IQueryable<CurrencyEntity> Currencies { get; }
        IQueryable<PackageTypeEntity> PackageTypes { get; }
    }
}
