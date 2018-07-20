using System.Linq;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public interface ICustomerRepository : IMemberRepository
    {
        IQueryable<OrganizationEntity> Organizations { get; }
        IQueryable<ContactEntity> Contacts { get; }
        IQueryable<VendorEntity> Vendors { get; }
        IQueryable<EmployeeEntity> Employees { get; }
    }
}
