using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.PricingModule.Data.Model;

namespace VirtoCommerce.PricingModule.Data.Repositories
{
    public class PricingRepositoryImpl : DbContextRepositoryBase<PricingDbContext>, IPricingRepository
    {
        public PricingRepositoryImpl(PricingDbContext dbContext, IUnitOfWork unitOfWork = null)
            : base(dbContext, unitOfWork)
        {
        }

        public IQueryable<PricelistEntity> Pricelists => DbContext.Set<PricelistEntity>();
        public IQueryable<PriceEntity> Prices => DbContext.Set<PriceEntity>();
        public IQueryable<PricelistAssignmentEntity> PricelistAssignments => DbContext.Set<PricelistAssignmentEntity>();

        public virtual async Task<PriceEntity[]> GetPricesByIdsAsync(string[] priceIds)
        {
            // TODO: replace Include with separate query
            var retVal = await Prices.Include(x => x.Pricelist).Where(x => priceIds.Contains(x.Id)).ToArrayAsync();
            return retVal;
        }

        public virtual async Task<PricelistEntity[]> GetPricelistByIdsAsync(string[] priceListIds)
        {
            // TODO: replace Include with separate query
            var retVal = await Pricelists.Include(x => x.Assignments)
                                         .Where(x => priceListIds.Contains(x.Id))
                                         .ToArrayAsync();
            return retVal;
        }

        public virtual async Task<PricelistAssignmentEntity[]> GetPricelistAssignmentsByIdAsync(string[] assignmentsIds)
        {
            // TODO: replace Include with separate query
            var retVal = await PricelistAssignments.Include(x => x.Pricelist).Where(x => assignmentsIds.Contains(x.Id)).ToArrayAsync();
            return retVal;
        }

        public async Task DeletePricesAsync(string[] ids)
        {
            await ExecuteStoreCommand("DELETE FROM Price WHERE Id IN ({0})", ids);
        }

        public async Task DeletePricelistsAsync(string[] ids)
        {
            await ExecuteStoreCommand("DELETE FROM Pricelist WHERE Id IN ({0})", ids);
        }

        public async Task DeletePricelistAssignmentsAsync(string[] ids)
        {
            await ExecuteStoreCommand("DELETE FROM PricelistAssignment WHERE Id IN ({0})", ids);
        }


        protected virtual async Task ExecuteStoreCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            await DbContext.Database.ExecuteSqlCommandAsync(command.Text, command.Parameters);
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToArray(),
            };
        }

        protected class Command
        {
            public string Text { get; set; }
            public object[] Parameters { get; set; }
        }
    }
}
