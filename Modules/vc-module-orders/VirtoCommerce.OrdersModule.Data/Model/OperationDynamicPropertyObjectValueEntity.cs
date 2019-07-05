using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class OperationDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual OperationEntity Operation { get; set; }
    }
}
