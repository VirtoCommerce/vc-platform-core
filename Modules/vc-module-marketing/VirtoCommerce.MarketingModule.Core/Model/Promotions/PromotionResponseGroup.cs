using System;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    [Flags]
    public enum PromotionResponseGroup
    {
        None = 0,
        WithDynamicExpression = 1,
        Full = None | WithDynamicExpression
    }
}
