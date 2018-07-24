using Microsoft.EntityFrameworkCore;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule2.Web.Model;

namespace VirtoCommerce.OrdersModule2.Web.Repositories
{
    public class Order2DbContext : OrderDbContext
    {
        public Order2DbContext(DbContextOptions<Order2DbContext> builderOptions) : base(builderOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region CustomerOrder2
            modelBuilder.Entity<CustomerOrder2Entity>().ToTable("CustomerOrder");
            #endregion

            #region OrderInvoice
            modelBuilder.Entity<InvoiceEntity>().ToTable("OrderInvoice");

            modelBuilder.Entity<InvoiceEntity>().HasOne(m => m.CustomerOrder2).WithMany(m => m.Invoices)
                .HasForeignKey(m => m.CustomerOrder2Id).OnDelete(DeleteBehavior.Cascade);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
