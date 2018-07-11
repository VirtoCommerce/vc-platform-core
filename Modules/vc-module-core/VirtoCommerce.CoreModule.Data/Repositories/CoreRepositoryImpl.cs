using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Data.Model;
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
        
        public IQueryable<SeoUrlKeywordEntity> SeoUrlKeywords => DbContext.Set<SeoUrlKeywordEntity>();
        public IQueryable<SequenceEntity> Sequences => DbContext.Set<SequenceEntity>(); 
        public IQueryable<CurrencyEntity> Currencies => DbContext.Set<CurrencyEntity>();
        public IQueryable<PackageTypeEntity> PackageTypes => DbContext.Set<PackageTypeEntity>();

        public Task<SeoUrlKeywordEntity[]> GetSeoByIdsAsync(string[] ids)
        {
            return SeoUrlKeywords.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Keyword).ToArrayAsync();
        }
        public Task<SeoUrlKeywordEntity[]> GetObjectSeoUrlKeywordsAsync(string objectType, string objectId)
        {
            return SeoUrlKeywords.Where(x => x.ObjectId == objectId && x.ObjectType == objectType).OrderBy(x => x.Language).ToArrayAsync();
        }

        #endregion
    }

}
