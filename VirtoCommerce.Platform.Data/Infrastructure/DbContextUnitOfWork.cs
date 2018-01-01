using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public class DbContextUnitOfWork : IUnitOfWork
    {
        private readonly EFRepositoryBase _observableContext;
        private readonly IInterceptor[] _interceptors;

        public DbContextUnitOfWork(EFRepositoryBase observableContext, IInterceptor[] interceptors)
        {
            _observableContext = observableContext;
            _interceptors = interceptors;
        }

        #region IUnitOfWork Members

        public virtual int Commit()
        {
            try
            {
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new PlatformException(ex.ExpandExceptionMessage());
            }
        }
    
        public Task<int> CommitAsync()
        {
            throw new NotImplementedException();
        }
        #endregion


        protected virtual int SaveChanges()
        {
            _observableContext.ChangeTracker.DetectChanges();

            InterceptionContext interceptionContext = null;

            if (_interceptors != null)
            {
                var entries = _observableContext.ChangeTracker.Entries().ToList();

                interceptionContext = new InterceptionContext(_interceptors)
                {
                    DatabaseContext = _observableContext,
                    ObjectContext = ObjectContext,
                    ObjectStateManager = ObjectStateManager,
                    ChangeTracker = _observableContext.ChangeTracker,
                    Entries = entries,
                    EntriesByState = entries.ToLookup(entry => entry.State),
                };
            }

            interceptionContext?.Before();

            var result = _observableContext.SaveChangesInternal();

            interceptionContext?.After();

            return result;
        }

        protected ObjectContext ObjectContext => ((IObjectContextAdapter)_observableContext).ObjectContext;
        protected ObjectStateManager ObjectStateManager => ObjectContext.ObjectStateManager;
       
    }
}
