using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public class ShippingRepository : DbContextRepositoryBase<ShippingDbContext>, IShippingRepository
    {
        public ShippingRepository(ShippingDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<StoreShippingMethodEntity> StoreShippingMethods => DbContext.Set<StoreShippingMethodEntity>();

        public async Task<StoreShippingMethodEntity[]> GetStoreShippingMethodsByIdsAsync(string[] ids, string responseGroup = null)
        {
            return await StoreShippingMethods.Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }
    }
}
