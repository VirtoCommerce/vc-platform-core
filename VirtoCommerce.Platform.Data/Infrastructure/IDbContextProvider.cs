using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    /// <summary>
    /// The interface that allows to retrieve the <see cref="DbContext"/> from some other object, e.g. from
    /// some <see cref="IRepository"/> implementation. This interface is currently used solely for the
    /// <see cref="RepositoryExtension.SetCommandTimeout"/> method, so that it could extract the <see cref="DbContext"/>
    /// without dealing with generic types.
    /// </summary>
    public interface IDbContextProvider
    {
        /// <summary>
        /// Retrieves the <see cref="DbContext"/> instance associated with this object.
        /// </summary>
        /// <returns>The instance of <see cref="DbContext"/>.</returns>
        DbContext GetDbContext();
    }
}
