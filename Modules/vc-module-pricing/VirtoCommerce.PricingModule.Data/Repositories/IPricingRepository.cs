using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Data.Model;


namespace VirtoCommerce.PricingModule.Data.Repositories
{
	public interface IPricingRepository : IRepository
	{
		IQueryable<PricelistEntity> Pricelists { get; }
		IQueryable<PriceEntity> Prices { get; }
		IQueryable<PricelistAssignmentEntity> PricelistAssignments { get; }

		Task<PriceEntity[]> GetPricesByIdsAsync(string[] priceIds);
		Task<PricelistEntity[]> GetPricelistByIdsAsync(string[] pricelistIds);
		Task<PricelistAssignmentEntity[]> GetPricelistAssignmentsByIdAsync(string[] assignmentsId);

        Task DeletePricesAsync(string[] ids);
        Task DeletePricelistsAsync(string[] ids);
        Task DeletePricelistAssignmentsAsync(string[] ids);
    }
}
