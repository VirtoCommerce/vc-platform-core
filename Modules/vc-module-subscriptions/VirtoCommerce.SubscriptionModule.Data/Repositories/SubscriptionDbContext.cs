using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.SubscriptionModule.Data.Model;

namespace VirtoCommerce.SubscriptionModule.Data.Repositories
{
    public class SubscriptionDbContext : DbContextWithTriggers
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }

        protected SubscriptionDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Subscription        
            modelBuilder.Entity<SubscriptionEntity>().ToTable("Subscription").HasKey(x => x.Id);
            modelBuilder.Entity<SubscriptionEntity>().Property(x => x.Id).HasMaxLength(128);
            #endregion

            #region PaymentPlan
            modelBuilder.Entity<PaymentPlanEntity>().ToTable("PaymentPlan").HasKey(x => x.Id);
            modelBuilder.Entity<PaymentPlanEntity>().Property(x => x.Id).HasMaxLength(128);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
