using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public class DbContextUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;

        public DbContextUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region IUnitOfWork Members

        public virtual int Commit()
        {
            _dbContext.ChangeTracker.DetectChanges();
            return _dbContext.SaveChanges();
        }

        public virtual async Task<int> CommitAsync()
        {
            _dbContext.ChangeTracker.DetectChanges();
            return await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
