using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.Platform.Data.Infrastructure
{
    public abstract class DbContextWithTriggersAndQueryFilters : DbContextWithTriggers
    {
        protected DbContextWithTriggersAndQueryFilters(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISupportSoftDeletion).IsAssignableFrom(type.ClrType))
                {
                    //modelBuilder.SetSoftDeleteFilter(type.ClrType);
                    modelBuilder.Entity(type.ClrType).AddQueryFilter<ISupportSoftDeletion>(x => !x.IsDeleted);
                }
                    
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
