using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class PaymentInDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual PaymentInEntity PaymentIn { get; set; }
    }
}
