using System.Linq;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public class CustomerRepository : MemberRepositoryBase, ICustomerRepository
    {
        public CustomerRepository(CustomerDbContext dbContext) : base(dbContext)
        {
        }

        #region ICustomerRepository Members
        public IQueryable<OrganizationEntity> Organizations => DbContext.Set<OrganizationEntity>();
        public IQueryable<ContactEntity> Contacts => DbContext.Set<ContactEntity>();
        public IQueryable<EmployeeEntity> Employees => DbContext.Set<EmployeeEntity>();
        public IQueryable<VendorEntity> Vendors => DbContext.Set<VendorEntity>();
        #endregion
    }
}
