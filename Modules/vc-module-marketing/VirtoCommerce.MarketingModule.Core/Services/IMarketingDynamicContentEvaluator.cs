using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingDynamicContentEvaluator
    {
        DynamicContentItem[] EvaluateItems(IEvaluationContext context);
    }
}
