using System.Linq;
using VirtoCommerce.CoreModule.Data.Currency;
using VirtoCommerce.CoreModule.Data.Model;
using VirtoCommerce.CoreModule.Data.Package;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public class CoreRepositoryImpl : DbContextRepositoryBase<CoreDbContext>, ICoreRepository
    {
        public CoreRepositoryImpl(CoreDbContext dbContext, IUnitOfWork unitOfWork = null) : base(dbContext, unitOfWork)
        {
        }

        #region IСommerceRepository Members


        public IQueryable<SequenceEntity> Sequences => DbContext.Set<SequenceEntity>();
        public IQueryable<CurrencyEntity> Currencies => DbContext.Set<CurrencyEntity>();
        public IQueryable<PackageTypeEntity> PackageTypes => DbContext.Set<PackageTypeEntity>();



        #endregion
    }

}
