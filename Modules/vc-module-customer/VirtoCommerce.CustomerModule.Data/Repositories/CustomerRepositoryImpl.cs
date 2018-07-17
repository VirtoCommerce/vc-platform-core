using System.Linq;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public class CustomerRepositoryImpl : MemberRepositoryBase, ICustomerRepository
    {
        public CustomerRepositoryImpl(CustomerDbContext dbContext) : base(dbContext)
        {
        }


        #region ICustomerRepository Members
        public IQueryable<OrganizationDataEntity> Organizations => DbContext.Set<OrganizationDataEntity>();
        public IQueryable<ContactDataEntity> Contacts => DbContext.Set<ContactDataEntity>();
        public IQueryable<EmployeeDataEntity> Employees => DbContext.Set<EmployeeDataEntity>();
        public IQueryable<VendorDataEntity> Vendors => DbContext.Set<VendorDataEntity>();
        #endregion

        
    }

}
