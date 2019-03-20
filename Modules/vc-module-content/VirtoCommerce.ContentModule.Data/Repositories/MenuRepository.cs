using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class MenuRepository : DbContextRepositoryBase<MenuDbContext>, IMenuRepository
    {
        public MenuRepository(MenuDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<MenuLinkListEntity> MenuLinkLists => DbContext.Set<MenuLinkListEntity>();

        public IQueryable<MenuLinkEntity> MenuLinks => DbContext.Set<MenuLinkEntity>();

        public async Task<IEnumerable<MenuLinkListEntity>> GetAllLinkListsAsync()
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).ToArrayAsync();
        }

        public async Task<MenuLinkListEntity> GetListByIdAsync(string listId)
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).Where(m => m.Id == listId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MenuLinkListEntity>> GetListsByStoreIdAsync(string storeId)
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).Where(m => m.StoreId == storeId).ToArrayAsync();
        }
    }
}
