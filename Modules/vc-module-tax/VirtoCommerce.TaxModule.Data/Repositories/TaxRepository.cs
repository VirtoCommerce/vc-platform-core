using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.TaxModule.Data.Model;

namespace VirtoCommerce.TaxModule.Data.Repositories
{
    public class TaxRepository : DbContextRepositoryBase<TaxDbContext>, ITaxRepository
    {
        public TaxRepository(TaxDbContext dbContext) : base(dbContext)
        {
        }

        #region IStoreRepository Members

        public IQueryable<StoreTaxProviderEntity> StoreTaxProviders => DbContext.Set<StoreTaxProviderEntity>();

        public async Task<StoreTaxProviderEntity[]> GetStoreTaxProviderByIdsAsync(string[] ids, string responseGroup = null)
        {
            return await StoreTaxProviders.Where(x => ids.Contains(x.Id))
                                          .ToArrayAsync();
        }

        #endregion
    }
}
