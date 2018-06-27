using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public class LicenseRepository : DbContextRepositoryBase<LicenseDbContext>, ILicenseRepository
    {
        public LicenseRepository(LicenseDbContext dbContext) : base(dbContext)
        {
        }

        #region ILicenseRepository
        public IQueryable<LicenseEntity> Licenses => DbContext.Set<LicenseEntity>();

        public async Task<LicenseEntity[]> GetByIdsAsync(string[] ids)
        {
            return await Licenses.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public async Task RemoveByIdsAsync(string[] ids)
        {
            var entries = await GetByIdsAsync(ids);
            foreach (var entry in entries)
            {
                Remove(entry);
            }
        }
        #endregion
    }
}
