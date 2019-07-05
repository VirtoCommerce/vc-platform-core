using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class PaymentDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual PaymentEntity Payment { get; set; }
    }
}
