using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class CartDbContext : DbContextWithTriggers
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ShoppingCart

            modelBuilder.Entity<ShoppingCartEntity>().ToTable("Cart").HasKey(x => x.Id);
            modelBuilder.Entity<ShoppingCartEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ShoppingCartEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            #endregion

            #region LineItem
            modelBuilder.Entity<LineItemEntity>().ToTable("CartLineItem").HasKey(x => x.Id);
            modelBuilder.Entity<LineItemEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<LineItemEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<LineItemEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Items)
                        .HasForeignKey(x => x.ShoppingCartId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Shipment

            modelBuilder.Entity<ShipmentEntity>().ToTable("CartShipment").HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ShipmentEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<ShipmentEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Shipments)
                        .HasForeignKey(x => x.ShoppingCartId).IsRequired().OnDelete(DeleteBehavior.Cascade);


            #endregion

            #region ShipmentItemEntity
            modelBuilder.Entity<ShipmentItemEntity>().ToTable("CartShipmentItem").HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentItemEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ShipmentItemEntity>().HasOne(x => x.LineItem).WithMany()
                        .HasForeignKey(x => x.LineItemId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ShipmentItemEntity>().HasOne(x => x.Shipment).WithMany(x => x.Items)
                        .HasForeignKey(x => x.ShipmentId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Address
            modelBuilder.Entity<AddressEntity>().ToTable("CartAddress").HasKey(x => x.Id);
            modelBuilder.Entity<AddressEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<AddressEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AddressEntity>().HasOne(x => x.Shipment).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AddressEntity>().HasOne(x => x.Payment).WithMany(x => x.Addresses)
                        .HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AddressEntity>().ToTable("CartAddress");
            #endregion

            #region Payment

            modelBuilder.Entity<PaymentEntity>().ToTable("CartPayment").HasKey(x => x.Id);
            modelBuilder.Entity<PaymentEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<PaymentEntity>().Property(x => x.TaxPercentRate).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<PaymentEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Payments)
                        .HasForeignKey(x => x.ShoppingCartId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentEntity>().ToTable("CartPayment");
            #endregion

            #region TaxDetail
            modelBuilder.Entity<TaxDetailEntity>().ToTable("CartTaxDetail").HasKey(x => x.Id);
            modelBuilder.Entity<TaxDetailEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.Shipment).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.LineItem).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.LineItemId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDetailEntity>().HasOne(x => x.Payment).WithMany(x => x.TaxDetails)
                        .HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Discount
            modelBuilder.Entity<DiscountEntity>().ToTable("CartDiscount").HasKey(x => x.Id);
            modelBuilder.Entity<DiscountEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.Shipment).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.LineItem).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.LineItemId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DiscountEntity>().HasOne(x => x.Payment).WithMany(x => x.Discounts)
                        .HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Coupons
            modelBuilder.Entity<CouponEntity>().ToTable("CartCoupon").HasKey(x => x.Id);
            modelBuilder.Entity<CouponEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<CouponEntity>().HasOne(x => x.ShoppingCart).WithMany(x => x.Coupons).IsRequired()
                        .HasForeignKey(x => x.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region DynamicProperty

            #region LineItem

            modelBuilder.Entity<LineItemDynamicPropertyObjectValueEntity>().ToTable("CartLineItemDynamicPropertyObjectValue").HasKey(x => x.Id);
            modelBuilder.Entity<LineItemDynamicPropertyObjectValueEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<LineItemDynamicPropertyObjectValueEntity>().Property(x => x.DecimalValue).HasColumnType("decimal(18,5)");
            modelBuilder.Entity<LineItemDynamicPropertyObjectValueEntity>().HasOne(p => p.LineItem)
                .WithMany(s => s.DynamicPropertyObjectValues).HasForeignKey(k => k.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LineItemDynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                .IsUnique(false)
                .HasName("IX_ObjectType_ObjectId");

            #endregion

            #region Payment

            modelBuilder.Entity<PaymentDynamicPropertyObjectValueEntity>().ToTable("CartPaymentDynamicPropertyObjectValue").HasKey(x => x.Id);
            modelBuilder.Entity<PaymentDynamicPropertyObjectValueEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<PaymentDynamicPropertyObjectValueEntity>().Property(x => x.DecimalValue).HasColumnType("decimal(18,5)");
            modelBuilder.Entity<PaymentDynamicPropertyObjectValueEntity>().HasOne(p => p.Payment)
                .WithMany(s => s.DynamicPropertyObjectValues).HasForeignKey(k => k.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PaymentDynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                .IsUnique(false)
                .HasName("IX_ObjectType_ObjectId");

            #endregion

            #region Shipment

            modelBuilder.Entity<ShipmentDynamicPropertyObjectValueEntity>().ToTable("CartShipmentDynamicPropertyObjectValue").HasKey(x => x.Id);
            modelBuilder.Entity<ShipmentDynamicPropertyObjectValueEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ShipmentDynamicPropertyObjectValueEntity>().Property(x => x.DecimalValue).HasColumnType("decimal(18,5)");
            modelBuilder.Entity<ShipmentDynamicPropertyObjectValueEntity>().HasOne(p => p.Shipment)
                .WithMany(s => s.DynamicPropertyObjectValues).HasForeignKey(k => k.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ShipmentDynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                .IsUnique(false)
                .HasName("IX_ObjectType_ObjectId");

            #endregion

            #region ShoppingCart

            modelBuilder.Entity<ShoppingCartDynamicPropertyObjectValueEntity>().ToTable("CartDynamicPropertyObjectValue").HasKey(x => x.Id);
            modelBuilder.Entity<ShoppingCartDynamicPropertyObjectValueEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ShoppingCartDynamicPropertyObjectValueEntity>().Property(x => x.DecimalValue).HasColumnType("decimal(18,5)");
            modelBuilder.Entity<ShoppingCartDynamicPropertyObjectValueEntity>().HasOne(p => p.ShoppingCart)
                .WithMany(s => s.DynamicPropertyObjectValues).HasForeignKey(k => k.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ShoppingCartDynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                .IsUnique(false)
                .HasName("IX_ObjectType_ObjectId");

            #endregion

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
