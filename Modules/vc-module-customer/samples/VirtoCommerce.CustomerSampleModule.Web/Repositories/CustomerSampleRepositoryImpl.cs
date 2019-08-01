using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerSampleModule.Web.Model;

namespace VirtoCommerce.CustomerSampleModule.Web.Repositories
{
    public class CustomerSampleRepositoryImpl : CustomerRepository
    {
        public CustomerSampleRepositoryImpl(CustomerSampleDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<SupplierEntity> Suppliers => DbContext.Set<SupplierEntity>();
        public IQueryable<Contact2Entity> Contact2 => DbContext.Set<Contact2Entity>();
        public override async Task<MemberEntity[]> GetMembersByIdsAsync(string[] ids, string responseGroup = null, string[] memberTypes = null)
        {
            var retVal = await base.GetMembersByIdsAsync(ids, responseGroup, memberTypes);
            return retVal;
        }
    }
}
