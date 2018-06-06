using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Data
{
    public class PlatformRepository2 : PlatformRepository
    {
        public PlatformRepository2(PlatformDbContext2 dbContext)
            : base(dbContext)
        {
        }
    }
}
