using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public abstract class DbContextWithTriggersAndQueryFiltersBase : DbContextWithTriggers
    {
        protected DbContextWithTriggersAndQueryFiltersBase(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISupportSoftDeletion).IsAssignableFrom(type.ClrType) && (type.BaseType == null
                    || !typeof(ISupportSoftDeletion).IsAssignableFrom(type.BaseType.ClrType)))
                {
                    modelBuilder.SetSoftDeleteFilter(type.ClrType);
                }
                    
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
