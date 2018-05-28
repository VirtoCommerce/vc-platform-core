using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public abstract class ServiceBase
    {

        protected virtual void CommitChanges(IRepository repository)
        {
            repository.UnitOfWork.Commit();
        }

    }
}
