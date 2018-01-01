using System;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public abstract class ServiceBase
	{
      
        protected virtual void CommitChanges(IRepository repository)
		{
			try
			{
				repository.UnitOfWork.Commit();
			}
			catch (Exception ex)
			{
                throw new PlatformException(ex.ExpandExceptionMessage());
			}
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
