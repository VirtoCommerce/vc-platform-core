using System.Linq;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Data
{
    public class PlatformRepository2 : PlatformRepository
    {
        public PlatformRepository2(PlatformDbContext dbContext)
            : base(dbContext)
        {
        }

        public override IQueryable<SettingEntity> Settings
        {
            get { return DbContext.Set<SettingEntity2>(); }
        }
    }
}
