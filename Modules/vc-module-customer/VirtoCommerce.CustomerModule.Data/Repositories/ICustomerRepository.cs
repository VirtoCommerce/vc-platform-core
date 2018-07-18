using System.Linq;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public interface ICustomerRepository : IMemberRepository
    {
        IQueryable<OrganizationDataEntity> Organizations { get; }
        IQueryable<ContactDataEntity> Contacts { get; }
        IQueryable<VendorDataEntity> Vendors { get; }
        IQueryable<EmployeeDataEntity> Employees { get; }
    }
}
