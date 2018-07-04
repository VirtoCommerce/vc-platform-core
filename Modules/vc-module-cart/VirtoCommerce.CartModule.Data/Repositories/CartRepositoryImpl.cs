using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class CartRepositoryImpl : DbContextRepositoryBase<CartDbContext>, ICartRepository
    {
        public CartRepositoryImpl(CartDbContext dbContext) : base(dbContext)
        {
        }

        #region ICartRepository Members

        public IQueryable<ShoppingCartEntity> ShoppingCarts => DbContext.Set<ShoppingCartEntity>();
        public IQueryable<AddressEntity> Addresses => DbContext.Set<AddressEntity>();
        public IQueryable<PaymentEntity> Payments => DbContext.Set<PaymentEntity>();
        public IQueryable<LineItemEntity> LineItems => DbContext.Set<LineItemEntity>();
        public IQueryable<ShipmentEntity> Shipments => DbContext.Set<ShipmentEntity>();
        protected IQueryable<DiscountEntity> Discounts => DbContext.Set<DiscountEntity>();
        protected IQueryable<TaxDetailEntity> TaxDetails => DbContext.Set<TaxDetailEntity>();

        public virtual async Task<ShoppingCartEntity[]> GetShoppingCartsByIdsAsync(string[] ids, string responseGroup = null)
        {
            var carts = await ShoppingCarts.Where(x => ids.Contains(x.Id)).ToArrayAsync();

            var cartResponseGroup = EnumUtility.SafeParse(responseGroup, CartResponseGroup.Full);

            var cartTaxDetails = TaxDetails.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
            var cartDiscounts = Discounts.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
            var cartAddresses = Addresses.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
            await Task.WhenAll(cartTaxDetails, cartDiscounts, cartAddresses);

            if ((cartResponseGroup & CartResponseGroup.WithPayments) == CartResponseGroup.WithPayments)
            {
                var payments = await Payments.Include(x => x.Addresses).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var paymentIds = payments.Select(x => x.Id).ToArray();

                var paymentTaxDetails = TaxDetails.Where(x => paymentIds.Contains(x.PaymentId)).ToArrayAsync();
                var paymentDiscounts = Discounts.Where(x => paymentIds.Contains(x.PaymentId)).ToArrayAsync();
                await Task.WhenAll(paymentTaxDetails, paymentDiscounts);
            }

            if ((cartResponseGroup & CartResponseGroup.WithLineItems) == CartResponseGroup.WithLineItems)
            {
                var lineItems = await LineItems.Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var lineItemIds = lineItems.Select(x => x.Id).ToArray();
                var lineItemsTaxDetails = TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).ToArrayAsync();
                var lineItemsDiscounts = Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).ToArrayAsync();
                await Task.WhenAll(lineItemsTaxDetails, lineItemsDiscounts);
            }

            if ((cartResponseGroup & CartResponseGroup.WithShipments) == CartResponseGroup.WithShipments)
            {
                var shipments = await Shipments.Include(x => x.Items).Where(x => ids.Contains(x.ShoppingCartId)).ToArrayAsync();
                var shipmentIds = shipments.Select(x => x.Id).ToArray();
                var shipmentTaxDetails = TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var shipmentDiscounts = Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var shipmentAddresses = Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                await Task.WhenAll(shipmentTaxDetails, shipmentDiscounts, shipmentAddresses);
            }

            return carts;
        }

        public virtual async Task RemoveCartsAsync(string[] ids)
        {

            var carts = await GetShoppingCartsByIdsAsync(ids);
            if (!carts.IsNullOrEmpty())
            {
                foreach (var cart in carts)
                {
                    Remove(cart);
                }
            }
        }

        #endregion
    }
}
