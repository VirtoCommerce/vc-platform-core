using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public abstract class ServiceBase
	{

        protected virtual void CommitChanges(IRepository repository)
        {
            repository.UnitOfWork.Commit();
        }

        protected virtual ObservableChangeTracker GetChangeTracker(IRepository repository)
		{
			var retVal = new ObservableChangeTracker
			{
                RemoveAction = x => repository.Remove(x),
                AddAction = x => repository.Add(x)
			};

			return retVal;
		}
	}
}
