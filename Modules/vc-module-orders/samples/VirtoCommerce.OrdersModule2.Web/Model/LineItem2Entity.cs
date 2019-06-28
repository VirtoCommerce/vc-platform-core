using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule2.Web.Model
{
    public class LineItem2Entity : LineItemEntity
    {
        public override LineItem ToModel(LineItem lineItem)
        {
            return base.ToModel(lineItem);
        }

        public override LineItemEntity FromModel(LineItem lineItem, PrimaryKeyResolvingMap pkMap)
        {
            return base.FromModel(lineItem, pkMap);
        }

        public override void Patch(LineItemEntity target)
        {
            base.Patch(target);
            var lineItem2 = target as LineItem2Entity;
            lineItem2.OuterId = OuterId;

        }
    }
}
