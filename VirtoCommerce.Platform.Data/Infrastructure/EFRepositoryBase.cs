using System.Linq;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    /// <summary>
    /// Base class for repository implementations that are based on the Entity Framework.
    /// </summary>
    public abstract class EFRepositoryBase : IRepository
    {
        public EFRepositoryBase(DbContext dbContext)
        {
            DbContext = dbContext;
            UnitOfWork = new DbContextUnitOfWork(dbContext);
        }

        public DbContext DbContext { get; private set; }
        #region IRepository Members

        /// <summary>
        /// Gets the unit of work. This class actually saves the data into underlying storage.
        /// </summary>
        /// <value>
        /// The unit of work.
        /// </value>
        public IUnitOfWork UnitOfWork { get; protected set; }

        /// <summary>
        /// Attaches the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Attach<T>(T item) where T : class
        {
            DbContext.Attach(item);
        }  
       
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Add<T>(T item) where T : class
        {
            DbContext.Add(item);
        }      

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Update<T>(T item) where T : class
        {
            DbContext.Update(item);
        }
       
        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Remove<T>(T item) where T : class
        {
            DbContext.Remove(item);
        }

        /// <summary>
        /// Gets as queryable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> GetAsQueryable<T>() where T : class
        {
            return DbContext.Set<T>();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
        #endregion
    }
}
