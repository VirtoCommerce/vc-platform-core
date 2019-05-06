using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.OrdersModule.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Repositories
{
    public class OrderDbContext : DbContextWithTriggers
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        protected OrderDbContext(DbContextOptions options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region CustomerOrder
            modelBuilder.Entity<CustomerOrderEntity>().ToTable("CustomerOrder").HasKey(x => x.Id);
            modelBuilder.Entity<CustomerOrderEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<CustomerOrderEntity>().ToTable("CustomerOrder");
            modelBuilder.Entity<CustomerOrderEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");

            #endregion

            #region LineItem

            modelBuilder.Entity<LineItemEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<LineItemEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<LineItemEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<LineItemEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.Items)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LineItemEntity>().ToTable("OrderLineItem");
            #endregion

            #region ShipmentItemEntity

            modelBuilder.Entity<ShipmentItemEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentItemEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<ShipmentItemEntity>().HasOne(x => x.LineItem).WithMany()
                        .HasForeignKey(x => x.LineItemId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            modelBuilder.Entity<ShipmentItemEntity>().HasOne(x => x.Shipment).WithMany(x => x.Items)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            modelBuilder.Entity<ShipmentItemEntity>().HasOne(x => x.ShipmentPackage).WithMany(x => x.Items)
                        .HasForeignKey(x => x.ShipmentPackageId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShipmentItemEntity>().ToTable("OrderShipmentItem");
            #endregion

            #region ShipmentPackageEntity

            modelBuilder.Entity<ShipmentPackageEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentPackageEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<ShipmentPackageEntity>().HasOne(x => x.Shipment).WithMany(x => x.Packages)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Cascade).IsRequired();


            modelBuilder.Entity<ShipmentPackageEntity>().ToTable("OrderShipmentPackage");
            #endregion

            #region Shipment

            modelBuilder.Entity<ShipmentEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<ShipmentEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<ShipmentEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.Shipments)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            modelBuilder.Entity<ShipmentEntity>().ToTable("OrderShipment");
            #endregion

            #region Address

            modelBuilder.Entity<AddressEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AddressEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<AddressEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AddressEntity>().HasOne(x => x.Shipment).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddressEntity>().HasOne(x => x.PaymentIn).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.PaymentInId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddressEntity>().ToTable("OrderAddress");
            #endregion

            #region PaymentIn

            modelBuilder.Entity<PaymentInEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PaymentInEntity>().Property(x => x.Id).HasMaxLength(128);

            modelBuilder.Entity<PaymentInEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<PaymentInEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.InPayments)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentInEntity>().HasOne(x => x.Shipment).WithMany(x => x.InPayments)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentInEntity>().ToTable("OrderPaymentIn");
            #endregion

            #region Discount

            modelBuilder.Entity<DiscountEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<DiscountEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.Shipment).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.LineItem).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.LineItemId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.PaymentIn).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.PaymentInId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiscountEntity>().ToTable("OrderDiscount");
            #endregion

            #region TaxDetail

            modelBuilder.Entity<TaxDetailEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<TaxDetailEntity>().Property(x => x.Id).HasMaxLength(128);

            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.CustomerOrder).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.Shipment).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.LineItem).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.LineItemId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.PaymentIn).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.PaymentInId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaxDetailEntity>().ToTable("OrderTaxDetail");
            #endregion

            #region PaymentGatewayTransactionEntity

            modelBuilder.Entity<PaymentGatewayTransactionEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PaymentGatewayTransactionEntity>().Property(x => x.Id).HasMaxLength(128); ;

            modelBuilder.Entity<PaymentGatewayTransactionEntity>().HasOne(x => x.PaymentIn).WithMany(x => x.Transactions)
                        .HasForeignKey(x => x.PaymentInId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<PaymentGatewayTransactionEntity>().ToTable("OrderPaymentGatewayTransaction");
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
