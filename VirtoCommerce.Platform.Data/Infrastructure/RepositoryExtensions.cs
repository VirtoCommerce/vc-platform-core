using System;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public static class RepositoryExtension
    {
        public static void DisableChangesTracking(this IRepository repository)
        {
            //http://stackoverflow.com/questions/29106477/nullreferenceexception-in-entity-framework-from-trygetcachedrelatedend
            if (repository.UnitOfWork is DbContextUnitOfWork dbContextUoW)
            {
                dbContextUoW.DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }

        /// <summary>
        /// Sets the command timeout for the repository.
        /// </summary>
        /// <param name="repository">Repository to set command timeout for. This method will  set 
        /// the command timeout successfully only if this repository implements the <see cref="IDbContextProvider"/> 
        /// interface (e.g. if it derives from the <see cref="DbContextRepositoryBase{TContext}"/>), 
        /// otherwise calling this method will do nothing.</param>
        /// <param name="commandTimeout">Command timeout, null to default on underlying provider settings.</param>
        public static void SetCommandTimeout(this IRepository repository, TimeSpan? commandTimeout)
        {
            if (repository is IDbContextProvider dbContextProvider)
            {
                var dbContext = dbContextProvider.GetDbContext();
                dbContext.Database.SetCommandTimeout((int?)commandTimeout?.TotalSeconds);
            }
        }
    }
}
